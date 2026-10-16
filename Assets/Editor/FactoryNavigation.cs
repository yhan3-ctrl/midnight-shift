using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using LastLantern;
public static class FactoryNavigation {
    [InitializeOnLoadMethod]static void Resume(){if(SessionState.GetBool("WillowWalkthrough",false))EditorApplication.playModeStateChanged+=OnState;}
    public static void Run(){File.WriteAllText("Work/navigation.txt","RUNNING");EditorSceneManager.OpenScene("Assets/Scenes/WillowWorks.unity");SessionState.SetBool("WillowWalkthrough",true);EditorApplication.playModeStateChanged+=OnState;EditorApplication.EnterPlaymode();}
    static void OnState(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredPlayMode)return;SessionState.SetBool("WillowWalkthrough",false);EditorApplication.playModeStateChanged-=OnState;new GameObject("Physical walkthrough check").AddComponent<FactoryWalker>();}
}
public class FactoryWalker : MonoBehaviour {
    FactoryGame g;PlayerMotor p;CharacterController cc;int interactions,placements,routingSolved;float began, heartbeat;bool failed;
    void Update(){if(!g)return;if(Time.realtimeSinceStartup-heartbeat>8){heartbeat=Time.realtimeSinceStartup;Debug.Log("NAV_HEARTBEAT dt="+Time.deltaTime+" scale="+Time.timeScale+" paused="+g.paused+" reading="+g.Reading+" journal="+g.journal+" ended="+g.ended);if(Time.realtimeSinceStartup-began>240)Fail("Overall watchdog timeout");}}
    IEnumerator Start(){
        yield return null;g=FactoryGame.I;p=g.player;cc=p.GetComponent<CharacterController>();p.enabled=false;g.inspector.enabled=false;Application.runInBackground=true;QualitySettings.vSyncCount=0;Application.targetFrameRate=120;Time.captureDeltaTime=1f/60;
        began=Time.realtimeSinceStartup;g.Begin();Close();Time.timeScale=5;
        yield return Go(-4.8f,2.2f);Use(new Vector3(-4.8f,.61f,3.8f),"Foyer crate");
        yield return Go(-4.8f,6.5f);Place(0,"Foyer gate");yield return Wait(.9f);
        yield return Go(0,10);yield return Go(0,15.3f);Use(new Vector3(0,.6f,16),"Shared power core");
        yield return Go(-5.4f,13.5f);Use(new Vector3(-5.4f,.8f,14.8f),"WORKSHOP RELAY");yield return Wait(1);yield return Go(0,16.5f);yield return Go(0,19.8f);yield return Go(5,18.6f);Use(new Vector3(5,.61f,20),"Workshop override crate");yield return Go(0,19.8f);yield return Go(0,16);yield return Go(4.8f,14.5f);Place(0,"Workshop power gate");yield return Go(-5.4f,13.5f);Use(new Vector3(-5.4f,.8f,14.8f),"WORKSHOP RELAY");yield return Go(0,16.5f);yield return Go(0,19.8f);
        yield return Go(-5.4f,18.6f);Use(new Vector3(-5.4f,.8f,19.3f),"MOULDING TABLE");yield return Job("mould");Use(new Vector3(-5.4f,.8f,19.3f),"MOULDING TABLE");
        yield return Go(0,20);yield return Go(5.4f,20);yield return Go(5.4f,23.1f);Use(new Vector3(5.4f,.8f,24.3f),"RIBBON CONVEYOR");yield return Job("ribbon");Use(new Vector3(5.4f,.8f,24.3f),"RIBBON CONVEYOR");
        yield return Go(7.3f,23.1f);yield return Go(7.3f,27.5f);yield return Go(0,28.8f);yield return Wait(.9f);yield return Go(0,31.7f);
        yield return Go(-6.3f,31.7f);yield return Go(-6.3f,33.3f);Use(new Vector3(-6.3f,.8f,34.3f),"BREAK-TIME CHIME");Use(new Vector3(-6.3f,.8f,34.3f),"BREAK-TIME CHIME");
        yield return Go(-4,31.3f);Use(new Vector3(-4,.61f,32.8f),"Archive crate");
        yield return Go(8,31.5f);yield return Go(8,41.3f);yield return Go(7,41.3f);Place(270,"Drawing cabinet");yield return Wait(.9f);
        yield return Go(7,42.4f);yield return Go(2.3f,42.4f);yield return Go(0,42.5f);yield return Go(0,44.1f);Use(new Vector3(2.6f,1.5f,44.83f),"Notice / AN OLD DRAWING");Close();Time.timeScale=5;
        yield return Go(0,42);yield return Go(-3.2f,42);yield return Go(-3.2f,37);yield return Go(-6.3f,37);Use(new Vector3(-6.3f,.8f,35.7f),"BREAK-TIME CHIME");
        yield return Go(-3.2f,37);yield return Go(-3.2f,42);yield return Go(0,42);yield return Go(0,44.1f);yield return Go(0,45.4f);yield return Wait(.9f);yield return Go(0,48);
        yield return Go(-5.4f,49.1f);Use(new Vector3(-5.4f,.8f,50.1f),"ENGRAVING TABLE");yield return Job("etch");
        yield return Go(0,48);yield return Go(4.8f,47.4f);Use(new Vector3(4.8f,.61f,49),"Assembly crate");yield return Go(0,48);yield return Go(-2,50);Place(0,"Assembly work gate");yield return Wait(.9f);
        yield return Go(-5.4f,49.1f);Use(new Vector3(-5.4f,.8f,50.1f),"ENGRAVING TABLE");yield return Go(0,49);yield return Go(0,52);yield return Go(0,55);yield return Go(5.4f,55.8f);
        Use(new Vector3(5.4f,.8f,56.8f),"LANTERN PRESS");yield return Job("press");Close();Time.timeScale=5;
        yield return Go(3.5f,55.8f);yield return Go(3.5f,56.8f);Use(new Vector3(3.6f,1.1f,58.5f),"The last lantern");yield return Go(0,58.2f);Use(new Vector3(0,1.4f,60.3f),"Pip");
        if(!g.ended)Fail("Delivery did not reach ending");
        if(!failed){string report=$"PASS: physical route to ending. {interactions} raycast interactions, {placements} validated Q placements, {routingSolved} authored circuits executed.\nAll traversal after initial spawn used CharacterController.Move, with live wall/gate/collision geometry.\nInspector locomotion disabled for deterministic navigation isolation; inspector behaviour has separate integration checks.\nRouting solved using the known authored solution; does not measure puzzle skill.\nTime accelerated; this is NOT an unassisted human playtest or a valid duration measurement.\n";File.WriteAllText("Work/navigation.txt",report);Debug.Log("NAVIGATION_PASS / "+report);EditorApplication.Exit(0);}
    }
    void Close(){for(int i=0;i<30&&g.Reading;i++)g.Advance();}
    IEnumerator Wait(float gameSeconds){float t=0;while(t<gameSeconds&&!failed){t+=Time.deltaTime;yield return null;}}
    IEnumerator Job(string id){var m=FindObjectsByType<Machine>(FindObjectsSortMode.None).Single(x=>x.machineId==id);float start=Time.realtimeSinceStartup;while(!m.done&&!failed){if(Time.realtimeSinceStartup-start>8){Fail("Job stalled: "+id);yield break;}yield return null;}Close();Time.timeScale=5;}
    IEnumerator Go(float x,float z){
        Vector3 target=new Vector3(x,0,z);float start=Time.realtimeSinceStartup;
        while(!failed){
            if(Time.realtimeSinceStartup-began>240){Fail("Walkthrough overall timeout");yield break;}
            Close();if(Time.timeScale!=5)Time.timeScale=5;
            Vector3 delta=target-p.transform.position;delta.y=0;if(delta.magnitude<.09f)break;
            if(Time.realtimeSinceStartup-start>20){Vector3 dir=(target-p.transform.position).normalized;Physics.SphereCast(p.transform.position+Vector3.up*.9f,.3f,dir,out RaycastHit obstacle,2);Fail("Blocked en route to "+target+" at "+p.transform.position+" delta="+Time.deltaTime+" scale="+Time.timeScale+" obstacle="+(obstacle.collider?obstacle.collider.name:"none"));yield break;}
            Vector3 horizontal=Vector3.ClampMagnitude(delta,4.1f*Time.deltaTime);cc.Move(horizontal+Vector3.down*4*Time.deltaTime);yield return null;
        }
    }
    void Use(Vector3 target,string expected){
        if(failed)return;p.view.transform.LookAt(target);Physics.SyncTransforms();p.ScanFocus();
        var owner=p.focus==g.sharedCore?(Component)g.sharedCore.socket:null;if(!owner&&p.focus==g.sharedCore)owner=g.sharedCore.GetComponentInParent<GridRelay>();bool coreAlias=owner&&owner.name.StartsWith(expected);
        if(!p.focus||(!p.focus.name.StartsWith(expected)&&!coreAlias)){Physics.Raycast(p.view.transform.position,p.view.transform.forward,out RaycastHit raw,3.1f,~0,QueryTriggerInteraction.Ignore);Fail("Raycast expected "+expected+", found "+(p.focus?p.focus.name:"nothing")+" collider="+(raw.collider?raw.collider.name:"none")+" view="+p.view.transform.position+" dir="+p.view.transform.forward+" from "+p.transform.position);return;}
        p.TryInteract();interactions++;
        if(g.activeCircuit){var circuit=g.activeCircuit.puzzle;for(int i=0;i<circuit.turns.Length;i++)while(circuit.turns[i]!=0)circuit.Rotate(i);if(!circuit.Solved){Fail("Authored routing solution invalid");return;}g.PowerCircuit();routingSolved++;}Debug.Log("NAV_USE / "+expected);
    }
    void Place(float yaw,string gate){
        if(failed)return;p.transform.rotation=Quaternion.Euler(0,yaw,0);p.TryPlace();placements++;Physics.SyncTransforms();
        if(p.held){Fail("Q placement failed near "+gate+": "+g.toast);return;}
        var door=FindObjectsByType<FactoryGate>(FindObjectsSortMode.None).Single(x=>x.name==gate);if(!door.Evaluate())Fail("Placement missed plate for "+gate);else Debug.Log("NAV_PLACED / "+gate);
    }
    void Fail(string message){if(failed)return;failed=true;File.WriteAllText("Work/navigation.txt","FAIL: "+message);Debug.LogError("NAVIGATION_FAIL / "+message);EditorApplication.Exit(1);}
}
