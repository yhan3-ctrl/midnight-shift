using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using LastLantern;

public static class FactoryVerification {
    [InitializeOnLoadMethod]
    static void ResumeAfterReload(){if(SessionState.GetBool("WillowVerification",false))EditorApplication.playModeStateChanged+=OnState;}
    public static void Run(){
        EditorSceneManager.OpenScene("Assets/Scenes/WillowWorks.unity");
        SessionState.SetBool("WillowVerification",true);
        EditorApplication.playModeStateChanged+=OnState;
        EditorApplication.EnterPlaymode();
    }
    static void OnState(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredPlayMode){SessionState.SetBool("WillowVerification",false);EditorApplication.playModeStateChanged-=OnState;var r=new GameObject("Verification runner");r.AddComponent<FactoryPlaycheck>();}}
    public static void Capture(){
        EditorSceneManager.OpenScene("Assets/Scenes/WillowWorks.unity");Directory.CreateDirectory("Work/Previews");
        var camera=UnityEngine.Object.FindFirstObjectByType<Camera>();
        Render(camera,"Work/Previews/01-foyer.png",new Vector3(0,1.8f,-6),new Vector3(0,1.7f,9));
        Render(camera,"Work/Previews/02-packaging.png",new Vector3(1,2.5f,15),new Vector3(-1,1.2f,24));
        Render(camera,"Work/Previews/03-archive.png",new Vector3(0,2.1f,32),new Vector3(0,1.3f,40));
        Render(camera,"Work/Previews/04-assembly.png",new Vector3(0,2.2f,55),new Vector3(1,1.4f,61));
        Debug.Log("PREVIEWS_CAPTURED");
    }
    static void Render(Camera c,string path,Vector3 position,Vector3 target){
        c.transform.position=position;c.transform.LookAt(target);var rt=new RenderTexture(1440,900,24);c.targetTexture=rt;c.Render();RenderTexture.active=rt;var tex=new Texture2D(1440,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1440,900),0,0);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());c.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);
    }
}

public class FactoryPlaycheck : MonoBehaviour {
    int checks; FactoryGame g; PlayerMotor p;
    IEnumerator Start(){
        Application.logMessageReceived+=OnLog;
        yield return null;
        g=FactoryGame.I;p=g.player;
        try{Check(g&&p&&g.inspector,"scene references survive serialization");Check(FindObjectsByType<Machine>(FindObjectsSortMode.None).Length==5,"five machines present");Check(FindObjectsByType<StoryPoint>(FindObjectsSortMode.None).Length==3,"story interaction components survive serialization");}
        catch(Exception e){Fail(e);yield break;}
        g.Begin();CloseDialogue();
        var gates=FindObjectsByType<FactoryGate>(FindObjectsSortMode.None);
        var crates=FindObjectsByType<Pickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(x=>x.kind==Pickup.Kind.Crate).ToArray();
        var core=FindObjectsByType<Pickup>(FindObjectsSortMode.None).Single(x=>x.kind==Pickup.Kind.Core);
        var machines=FindObjectsByType<Machine>(FindObjectsSortMode.None);
        var foyer=gates.Single(x=>x.name=="Foyer gate");
        try{
            p.Teleport(new Vector3(0,0,-6),0);Physics.SyncTransforms();
            Check(!foyer.Evaluate(),"foyer initially blocks progression");
            crates.Single(x=>x.name=="Foyer crate").transform.position=foyer.plate.position+Vector3.up*.66f;Physics.SyncTransforms();Check(foyer.Evaluate(),"placed crate opens pressure gate");
            p.Pick(core);var mould=machines.Single(x=>x.machineId=="mould");mould.Use(p);Check(mould.core==core&&p.held==null,"core inserted into machine");mould.Use(p);
        }catch(Exception e){Fail(e);yield break;}
        yield return new WaitForSecondsRealtime(.1f);Time.timeScale=16;
        yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        var ribbon=machines.Single(x=>x.machineId=="ribbon");
        try{
            Check(g.shell,"mould completes using game update");
            machines.Single(x=>x.machineId=="mould").Use(p);Check(p.held==core,"completed machine returns same core");
            ribbon.Use(p);ribbon.Use(p);Check(g.shell,"processed shell survives power transfer");
        }catch(Exception e){Fail(e);yield break;}
        Time.timeScale=16;yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        try{
            Check(g.transferred&&gates.Single(x=>x.name=="Packaging gate").Evaluate(),"packaging door unlocks only after both jobs");
            var archive=gates.Single(x=>x.name=="Drawing cabinet");Check(!archive.Evaluate(),"archive drawing protected by physical gate");
            var crate=crates.Single(x=>x.name=="Archive crate");crate.transform.position=archive.plate.position+Vector3.up*.66f;Physics.SyncTransforms();Check(archive.Evaluate(),"archive crate opens drawing route");
            g.ReadDrawing();CloseDialogue();Check(g.blueprint&&gates.Single(x=>x.name=="Assembly entrance").Evaluate(),"story reveal unlocks assembly");
            ribbon.Use(p);p.Teleport(new Vector3(0,0,37),0);g.inspector.ResetPatrol();g.inspector.Hear(new Vector3(-6,0,35));Check(g.inspector.state==Inspector.State.Investigate,"machine noise changes inspector state");
            g.ResetSection();Check(g.resets==1&&g.shell&&g.transferred&&g.blueprint,"safety reset preserves completed progress");Check(p.held==null&&core.transform.parent==null,"reset releases core to recoverable home");
            var etch=machines.Single(x=>x.machineId=="etch");p.Pick(core);etch.Use(p);etch.Use(p);
        }catch(Exception e){Fail(e);yield break;}
        Time.timeScale=16;yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        try{
            Check(g.etched,"engraving completes");machines.Single(x=>x.machineId=="etch").Use(p);
            var press=machines.Single(x=>x.machineId=="press");press.Use(p);press.Use(p);
        }catch(Exception e){Fail(e);yield break;}
        Time.timeScale=16;yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        try{
            Check(g.assembled&&g.lantern.gameObject.activeInHierarchy,"press creates physical lantern");p.Pick(g.lantern);g.Deliver();Check(g.ended&&p.held==null,"physical lantern delivery reaches ending");
            Check(FindObjectsByType<Pickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).Count(x=>x.kind==Pickup.Kind.Core)==1,"one core throughout run; no duplication");
            File.WriteAllText("Work/playcheck.txt",$"PASS: {checks} engine integration checks.\nChecks use actual scene components and Update processing.\nThis is not an unassisted human playthrough or a timing validation.\n");Debug.Log("PLAYCHECK_PASS / "+checks);
        }catch(Exception e){Fail(e);yield break;}
        Application.logMessageReceived-=OnLog;EditorApplication.Exit(0);
    }
    void CloseDialogue(){for(int i=0;i<20&&g.Reading;i++)g.Advance();Time.timeScale=1;}
    void Check(bool ok,string what){if(!ok)throw new Exception(what);checks++;Debug.Log("CHECK_PASS / "+what);}
    void OnLog(string message,string trace,LogType type){if(type==LogType.Exception)Fail(new Exception(message+"\n"+trace));}
    void Fail(Exception e){File.WriteAllText("Work/playcheck.txt","FAIL: "+e);Debug.LogError("PLAYCHECK_FAIL / "+e);EditorApplication.Exit(1);}
}
