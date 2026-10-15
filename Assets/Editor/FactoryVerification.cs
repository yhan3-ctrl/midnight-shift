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
        File.WriteAllText("Work/playcheck.txt","RUNNING / current verification not yet complete");
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
        Render(camera,"Work/Previews/02-packaging.png",new Vector3(1,2.1f,19),new Vector3(-1,1.2f,24));
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
        Application.runInBackground=true;Time.captureDeltaTime=1f/60;
        g=FactoryGame.I;p=g.player;
        try{Check(g&&p&&g.inspector,"scene references survive serialization");Check(FindObjectsByType<Machine>(FindObjectsSortMode.None).Length==5,"five machines present");Check(FindObjectsByType<StoryPoint>(FindObjectsSortMode.None).Length==4,"story interaction components survive serialization");}
        catch(Exception e){Fail(e);yield break;}
        g.Begin();CloseDialogue();
        try{
            var graph=CircuitGrid.Create("mould");graph.shapes=new int[9];graph.turns=new int[9];for(int i=3;i<=5;i++)graph.shapes[i]=CircuitGrid.W|CircuitGrid.E;graph.sourceRow=1;graph.targets=new[]{5};graph.beacons=new[]{1};graph.Refresh();
            Check(graph.LitOutlets==1&&!graph.Solved,"an outlet alone cannot bypass required contacts");
            graph.shapes[4]=CircuitGrid.N|CircuitGrid.S;graph.Refresh();Check(graph.LitOutlets==0,"adjacent tiles require matching reciprocal edges");
            graph.shapes[4]=CircuitGrid.W|CircuitGrid.E;graph.Rotate(3);Check(!graph.Powered(3)&&graph.LitOutlets==0,"source must face the incoming supply");
        }catch(Exception e){Fail(e);yield break;}

        var gates=FindObjectsByType<FactoryGate>(FindObjectsSortMode.None);
        var crates=FindObjectsByType<Pickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(x=>x.kind==Pickup.Kind.Crate).ToArray();
        var core=FindObjectsByType<Pickup>(FindObjectsSortMode.None).Single(x=>x.kind==Pickup.Kind.Core);
        var machines=FindObjectsByType<Machine>(FindObjectsSortMode.None);
        var foyer=gates.Single(x=>x.name=="Foyer gate");
        p.Teleport(new Vector3(0,0,10),0);Physics.SyncTransforms();
        for(int n=0;n<60;n++){p.GetComponent<CharacterController>().Move(Vector3.forward*.15f);yield return null;}
        try{Check(p.transform.position.z<12&&Mathf.Abs(foyer.panel.position.y-foyer.closedPosition.y)<.1f,"closed unpowered gate resists approach and cannot be bypassed by anti-crush logic");}
        catch(Exception e){Fail(e);yield break;}

        try{
            p.Teleport(new Vector3(0,0,-6),0);Physics.SyncTransforms();
            Check(!foyer.Evaluate(),"foyer initially blocks progression");
            Check(g.Objective.Contains("OPEN THE FIRST DOOR"),"foyer objective teaches the immediate task before power mechanics");
            var powerGate=gates.Single(x=>x.name=="Workshop power gate");
            var overrideCrate=crates.Single(x=>x.name=="Workshop override crate");overrideCrate.PlaceAt(powerGate.plate.position+Vector3.up*.885f);Physics.SyncTransforms();
            Check(powerGate.relay.gate==powerGate&&powerGate.relay.OverrideHeld,"relay identifies crate-supported access independently of core power");
            Check(powerGate.Evaluate(),"crate override keeps workshop gate open without a core");
            p.Teleport(new Vector3(0,0,15),0);Check(g.Objective.Contains("Moulding"),"objective advances from gate preparation to manufacturing once crate holds access");p.Teleport(new Vector3(0,0,-6),0);overrideCrate.ResetHome();Physics.SyncTransforms();

            var tutorialCrate=crates.Single(x=>x.name=="Foyer crate");p.Pick(tutorialCrate);
            p.Teleport(new Vector3(-4.8f,0,7.3f),0);Physics.SyncTransforms();p.TryPlace();Check(p.held==tutorialCrate,"plate assist refuses to place a crate over the player");
            var worktop=GameObject.CreatePrimitive(PrimitiveType.Cube);worktop.transform.position=new Vector3(0,1,1.5f);worktop.transform.localScale=new Vector3(2,.2f,2);p.Teleport(Vector3.zero,0);Physics.SyncTransforms();p.TryPlace();Check(p.held==tutorialCrate,"placement refuses elevated worktops");DestroyImmediate(worktop);Physics.SyncTransforms();
            p.Teleport(new Vector3(-4.8f,0,5.3f),0);Physics.SyncTransforms();p.TryPlace();
            Check(p.held==null&&Vector3.Distance(tutorialCrate.transform.position,foyer.plate.position+Vector3.up*.885f)<.05f&&foyer.Evaluate(),"near-edge crate placement snaps onto plate and opens gate");
            p.Teleport(new Vector3(0,0,-6),0);

            crates.Single(x=>x.name=="Foyer crate").transform.position=foyer.plate.position+Vector3.up*.885f;Physics.SyncTransforms();Check(foyer.Evaluate(),"placed crate opens pressure gate");
            p.Pick(core);g.ToggleJournal();Check(g.Frozen&&Time.timeScale==0&&p.held==core,"journal freezes world and retains held item");g.ToggleJournal();Check(!g.Frozen&&Time.timeScale==1,"journal returns to active play");g.TogglePause();Check(g.Frozen&&Time.timeScale==0,"pause freezes world");g.TogglePause();Check(!g.Frozen&&p.held==core,"resume preserves held item");var blocked=machines.Single(x=>x.machineId=="press");blocked.Use(p);Check(p.held==core&&blocked.core==null,"out-of-order machine never traps the only core");var mould=machines.Single(x=>x.machineId=="mould");mould.Use(p);Check(mould.core==core&&p.held==null,"core inserted into machine");mould.Use(p);SolveRouting(mould);
        }catch(Exception e){Fail(e);yield break;}
        yield return new WaitForSecondsRealtime(.1f);Time.timeScale=16;
        yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        var ribbon=machines.Single(x=>x.machineId=="ribbon");
        try{
            Check(g.shell,"mould completes using game update");
            var shellDisplay=FindObjectsByType<LanternDisplay>(FindObjectsSortMode.None).Single(x=>x.station&&x.station.machineId=="mould");shellDisplay.Refresh();
            Check(shellDisplay.Visible&&shellDisplay.Stage==1&&!shellDisplay.ribbon.gameObject.activeSelf,"mould produces a visible persistent plain shell");
            Check(g.buttonHelper&&g.buttonHelper.Celebrations>0,"Button reacts to manufacturing completion");
            g.TalkToButton();Check(g.Reading&&g.line.Contains("shell"),"optional Button dialogue responds to the current craft stage");
            Check(g.buttonHelper.Celebrating&&g.Frozen,"optional helper conversation retains a celebration while world is paused");CloseDialogue();
            machines.Single(x=>x.machineId=="mould").Use(p);Check(p.held==core,"completed machine returns same core");
            ribbon.Use(p);ribbon.Use(p);SolveRouting(ribbon);Check(g.shell,"processed shell survives power transfer");
            shellDisplay.Refresh();Check(shellDisplay.Visible,"visible shell survives removing and reusing its core");
        }catch(Exception e){Fail(e);yield break;}
        Time.timeScale=16;yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        try{
            var ribbonDisplay=FindObjectsByType<LanternDisplay>(FindObjectsSortMode.None).Single(x=>x.station&&x.station.machineId=="ribbon");ribbonDisplay.Refresh();
            Check(ribbonDisplay.Visible&&ribbonDisplay.Stage==2&&ribbonDisplay.ribbon.gameObject.activeSelf&&!ribbonDisplay.inscription.gameObject.activeSelf,"ribbon output adds its bow without revealing engraving");
            Check(g.transferred&&gates.Single(x=>x.name=="Packaging gate").Evaluate(),"packaging door unlocks only after both jobs");
            var archive=gates.Single(x=>x.name=="Drawing cabinet");Check(!archive.Evaluate(),"archive drawing protected by physical gate");
            var crate=crates.Single(x=>x.name=="Archive crate");crate.transform.position=archive.plate.position+Vector3.up*.885f;Physics.SyncTransforms();Check(!archive.Evaluate(),"archive requires taught chime interaction as well as crate");
            ribbon.Use(p);var chime=machines.Single(x=>x.machineId=="chime");chime.Use(p);chime.Use(p);
            Check(g.inspectorLesson&&archive.Evaluate(),"chime practice and crate open drawing route");
            g.ReadDrawing();CloseDialogue();Check(g.blueprint&&gates.Single(x=>x.name=="Assembly entrance").Evaluate(),"story reveal unlocks assembly");
            machines.Single(x=>x.machineId=="chime").Use(p);p.Teleport(new Vector3(0,0,37),0);g.inspector.ResetPatrol();g.inspector.Hear(new Vector3(-6,0,35));Check(g.inspector.state==Inspector.State.Investigate,"machine noise changes inspector state");
            g.ResetSection();Check(g.resets==1&&g.shell&&g.transferred&&g.blueprint,"safety reset preserves completed progress");Check(p.held==null&&core.transform.parent==null,"reset releases core to recoverable home");
            Check(g.Reading&&g.Frozen,"retry explains recovery while safely pausing the world");CloseDialogue();
            g.inspector.enabled=false;g.inspector.ResetPatrol();g.inspector.transform.position=new Vector3(0,0,36);g.inspector.transform.rotation=Quaternion.identity;p.Teleport(new Vector3(0,0,40),0);Physics.SyncTransforms();
            Check(g.inspector.CanSeePlayer(),"inspector vision sees player in range and field of view");
            var cover=crates.First();cover.transform.position=new Vector3(0,.835f,38);Physics.SyncTransforms();
            Check(!g.inspector.CanSeePlayer(),"placed crate physically blocks inspector sight");
            cover.SetColliders(false);Physics.SyncTransforms();Check(g.inspector.CanSeePlayer(),"disabled carried-item collision does not create invisible cover");cover.SetColliders(true);cover.ResetHome();
            p.Teleport(new Vector3(0,0,47.2f),0);g.inspector.assemblyDuty=true;g.inspector.route=new[]{new Vector3(4,0,52),new Vector3(-1,0,50)};g.inspector.ResetPatrol();
            p.Pick(core);g.ResetSection();Check(p.transform.position.z>46&&core.transform.position.z>46,"final-area retry recovers player and held core locally");Check(!g.inspector.CanSeePlayer(),"safety reset grants a brief detection grace period");CloseDialogue();
            g.inspector.enabled=true;g.inspector.assemblyDuty=false;g.inspector.route=new[]{new Vector3(-2.3f,0,35.5f),new Vector3(2.3f,0,40.5f)};g.inspector.ResetPatrol();p.Teleport(new Vector3(0,0,15),0);
            var etch=machines.Single(x=>x.machineId=="etch");p.Pick(core);etch.Use(p);etch.Use(p);SolveRouting(etch);
        }catch(Exception e){Fail(e);yield break;}
        Time.timeScale=16;yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        try{
            Check(g.etched,"engraving completes");
            var engraving=FindObjectsByType<LanternDisplay>(FindObjectsSortMode.None).Single(x=>x.station&&x.station.machineId=="etch");engraving.Refresh();
            Check(engraving.Visible&&engraving.Stage==3&&engraving.inscription.gameObject.activeSelf&&!engraving.glow.gameObject.activeSelf,"engraving adds the promise tag before the light is finished");machines.Single(x=>x.machineId=="etch").Use(p);
            var press=machines.Single(x=>x.machineId=="press");press.Use(p);press.Use(p);SolveRouting(press);
        }catch(Exception e){Fail(e);yield break;}
        Time.timeScale=16;yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        try{
            Check(g.assembled&&g.lantern.gameObject.activeInHierarchy,"press creates physical lantern");
            var finalDisplay=g.lantern.GetComponentInChildren<LanternDisplay>();finalDisplay.Refresh();
            Check(finalDisplay.portable&&finalDisplay.Stage==4&&finalDisplay.glow.gameObject.activeSelf&&finalDisplay.warmth.enabled,"finished pickup carries ribbon, inscription and an actual warm light");
            var pressDisplay=FindObjectsByType<LanternDisplay>(FindObjectsSortMode.None).Single(x=>x.station&&x.station.machineId=="press");pressDisplay.Refresh();Check(!pressDisplay.Visible,"press clears its workpiece when the one deliverable lantern is ready");p.Pick(g.lantern);g.Deliver();Check(g.ended&&p.held==null,"physical lantern delivery reaches ending");
            Check(FindObjectsByType<Pickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).Count(x=>x.kind==Pickup.Kind.Core)==1,"one core throughout run; no duplication");
            File.WriteAllText("Work/playcheck.txt",$"PASS: {checks} engine integration checks.\nChecks use actual scene components and Update processing.\nThis is not an unassisted human playthrough or a timing validation.\n");Debug.Log("PLAYCHECK_PASS / "+checks);
        }catch(Exception e){Fail(e);yield break;}
        float endStarted=Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup-endStarted<15){yield return null;}
        try{var ending=FindFirstObjectByType<EndOfShift>();Check(ending&&ending.pip.position.z>70&&ending.exitDoor.position.y>5,"ending opens exit and takes the helpers into the village");File.WriteAllText("Work/playcheck.txt",$"PASS: {checks} engine integration checks, including the animated ending.\nActual scene components, physics visibility, progression, retries and delivery exercised.\nNot an unassisted human playthrough or timing validation.\n");}
        catch(Exception e){Fail(e);yield break;}
        Application.logMessageReceived-=OnLog;EditorApplication.Exit(0);
    }
    void SolveRouting(Machine machine){
        Check(g.activeCircuit==machine&&g.Frozen&&Time.timeScale==0,"routing mode freezes workshop: "+machine.machineId);
        g.PowerCircuit();Check(g.activeCircuit==machine&&!machine.running,"unconnected routing cannot start the machine: "+machine.machineId);
        var grid=machine.puzzle;Check(!grid.Solved,"authored circuit begins unsolved: "+machine.machineId);
        for(int i=0;i<grid.turns.Length;i++)while(grid.turns[i]!=0)grid.Rotate(i);
        Check(grid.Solved,"authored circuit has a valid connected solution: "+machine.machineId);
        g.PowerCircuit();Check(g.activeCircuit==null&&machine.running,"connected circuit starts machine: "+machine.machineId);
        var output=FindObjectsByType<LanternDisplay>(FindObjectsSortMode.None).Single(x=>x.station==machine);output.Refresh();Check(output.caption&&!output.caption.text.Contains("READY"),"running output label does not promise completion: "+machine.machineId);
        if(machine.machineId=="mould"){machine.progress=machine.workSeconds*.5f;output.Refresh();Check(Mathf.Abs(output.model.localPosition.y-output.halfHeight*output.model.localScale.y+output.halfHeight)<.001f,"growing shell stays anchored to its tray");machine.progress=0;var relay=g.workshopRelay;var crate=FindObjectsByType<Pickup>(FindObjectsSortMode.None).First(x=>x.kind==Pickup.Kind.Crate&&!x.carried);var position=crate.transform.position;crate.PlaceAt(relay.gate.plate.position+Vector3.up*.885f);p.Teleport(new Vector3(0,0,19),0);Physics.SyncTransforms();Check(g.Objective.Contains("TAKING SHAPE"),"running machine objective does not instruct the player to remove its core or reopen routing");crate.PlaceAt(position);}

    }
    void CloseDialogue(){for(int i=0;i<20&&g.Reading;i++)g.Advance();Time.timeScale=1;}
    void Check(bool ok,string what){if(!ok)throw new Exception(what);checks++;Debug.Log("CHECK_PASS / "+what);}
    void OnLog(string message,string trace,LogType type){if(type==LogType.Exception)Fail(new Exception(message+"\n"+trace));}
    void Fail(Exception e){File.WriteAllText("Work/playcheck.txt","FAIL: "+e);Debug.LogError("PLAYCHECK_FAIL / "+e);EditorApplication.Exit(1);}
}
