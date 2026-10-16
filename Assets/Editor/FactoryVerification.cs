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
        g=FactoryGame.I;p=g.player;p.SetLookMode(1,false);
        try{Check(g&&p&&g.inspector,"scene references survive serialization");Check(FindObjectsByType<Machine>(FindObjectsSortMode.None).Length==5,"five machines present");Check(FindObjectsByType<StoryPoint>(FindObjectsSortMode.None).Length==4,"story interaction components survive serialization");}
        catch(Exception e){Fail(e);yield break;}
        int initialLanguage=UiCopy.Selected;UiCopy.Language(0,false);
        g.Begin();CloseDialogue();
        try{
            Check(UiCopy.Entries.Length>=260&&UiCopy.Entries.All(x=>x.values.Length==7&&x.values.All(v=>!string.IsNullOrWhiteSpace(v))),"all catalog entries contain seven complete language values");
            var englishAction=g.guide.Current.action;string greeting="Welcome to Willow Works. I'm Pip. Let me show you around, one little step at a time.";
            for(int language=1;language<UiCopy.Codes.Length;language++){
                UiCopy.Language(language,false);
                Check(UiCopy.Translate(greeting)!=greeting&&g.guide.Current.action!=englishAction&&!g.shell&&!g.player.held,"language switch localizes actual guide/story without advancing work: "+UiCopy.Codes[language]);
                var face=UiCopy.BodyFont(language);var glyphs=new System.Collections.Generic.HashSet<char>();foreach(var entry in UiCopy.Entries)foreach(char c in entry.values[language])if(!char.IsWhiteSpace(c))glyphs.Add(c);
                Check(face&&glyphs.All(c=>face.HasCharacter(c)),"bundled font covers every catalog glyph: "+UiCopy.Codes[language]);
            }
            UiCopy.Language(-1,false);Check(UiCopy.Selected==0,"unsupported language preference safely falls back to English");
        }catch(Exception e){Fail(e);yield break;}

        try{
            Check(g.guide&&g.guide.Current.key=="RIGHT-DRAG","optional pointer mode teaches right-drag without implying the default view is fixed");
            p.SetLookMode(0,false);Check(g.AwaitingLook&&g.Frozen&&Time.timeScale==0&&!p.LookCaptured&&Cursor.visible,"default FPS mode waits for explicit click before capturing the pointer or running hazards");
            Check(g.guide.Current.key=="MOUSE","default opening guide explicitly teaches mouse turning");
            Check(g.guide.Navigate(g.guide.Current).key=="MOUSE","distance navigation must not replace the initial mouse-look instruction with a walking task");
            g.TogglePause();Check(!p.CaptureMouseLook()&&!p.LookCaptured&&Cursor.visible,"pause menu rejects mouse capture");g.TogglePause();Check(g.AwaitingLook&&!p.LookCaptured,"resuming pause never silently captures the pointer");
            g.ToggleMap();Check(!p.CaptureMouseLook()&&g.mapOpen,"map rejects capture");g.ToggleMap();Check(g.AwaitingLook&&!p.LookCaptured,"map Back returns to explicit look entry");
            g.TogglePhoto();Check(!p.CaptureMouseLook()&&Cursor.visible,"screenshot view rejects capture");g.TogglePhoto();Check(g.AwaitingLook&&!p.LookCaptured,"closing screenshot view never silently captures");
            g.Say("Control regression","One last line.");Check(!p.CaptureMouseLook(),"dialogue rejects capture");CloseDialogue();Check(g.AwaitingLook&&!p.LookCaptured,"finishing dialogue never silently captures");
            g.HoldForDesktop();Check(g.desktopHold&&g.Frozen,"desktop hold also works while awaiting look entry");g.ResumeDesktop();Check(g.AwaitingLook&&!p.LookCaptured,"desktop resume requires a fresh click");
            g.OpenComfort();Check(g.Modal&&!p.CaptureMouseLook()&&Cursor.visible,"comfort settings are paused and reject capture");g.CloseComfort(false);Check(g.AwaitingLook,"closing comfort restores underlying look entry");
            g.sensitivity=1.7f;g.verticalSensitivity=1.5f;p.Teleport(new Vector3(0,0,-7),0);p.ApplyLook(new Vector2(20,10),0,0,0);
            Check(Mathf.Abs(Mathf.DeltaAngle(p.transform.eulerAngles.y,34))<.01f&&Mathf.Abs(p.Pitch+15)<.01f,"actual look transform applies raw mouse motion on independent horizontal and vertical axes");
            p.ApplyLook(new Vector2(-40,-20),0,0,0);Check(Mathf.Abs(Mathf.DeltaAngle(p.transform.eulerAngles.y,-34))<.01f&&Mathf.Abs(p.Pitch-15)<.01f,"mouse look turns in both directions rather than only strafing");
            p.ApplyLook(new Vector2(720,1000),0,0,0);Check(Mathf.Abs(p.Pitch+72)<.01f,"large mouse motion clamps pitch without rolling the view");
            p.ApplyLook(Vector2.zero,0,0,0);Check(Mathf.Abs(p.Pitch+72)<.01f,"stationary pointer causes no auto-turn or recentering");
            p.ComfortDefaults(false);Check(!p.handMotion&&p.speed==3.3f&&p.view.fieldOfView==72&&p.LookMode==0,"comfort defaults use static hands, adjustable FOV and default mouse look");
            p.SetLookMode(2,false);Check(!g.AwaitingLook&&!p.NeedsLookStart&&g.guide.Current.key=="ARROWS","explicit arrow-only alternative runs without capturing the pointer");
            p.SetLookMode(1,false);p.Teleport(new Vector3(0,0,-7),0);

            Check(Cursor.lockState==CursorLockMode.None&&Cursor.visible,"active exploration keeps the operating-system pointer free");
            g.TogglePhoto();Check(g.photoMode&&g.Frozen&&Time.timeScale==0&&Cursor.lockState==CursorLockMode.None,"screenshot view freezes hazards without capturing the pointer");g.TogglePhoto();Check(!g.Frozen,"screenshot view returns to the previous active state");
            g.HoldForDesktop();Check(g.desktopHold&&g.Frozen&&!g.paused,"desktop focus loss freezes without covering the game with the pause menu");g.ResumeDesktop();Check(!g.Frozen&&Cursor.visible,"desktop resume restores play with a free pointer");

            g.ToggleMap();Check(g.mapOpen&&g.Frozen&&Time.timeScale==0&&Cursor.visible,"expandable factory map freezes safely with a free pointer");g.ToggleMap();Check(!g.Frozen,"closing map preserves active gameplay");
            g.Say("Map test","Keep this line.");g.ToggleMap();g.ToggleMap();Check(g.Reading&&g.line=="Keep this line.","map preserves the underlying dialogue");CloseDialogue();
            Check(Mathf.Abs(FactoryPath.Heading(new Vector3(0,.9f,1),Vector3.zero,new Vector3(0,-5,10)))<.01f,"route compass ignores height and camera pitch instead of asking the player to look down");
            Check(FactoryPath.Heading(Vector3.forward,Vector3.zero,new Vector3(-5,0,0)) < -80&&FactoryPath.Heading(Vector3.forward,Vector3.zero,new Vector3(5,0,0)) > 80,"heading compass distinguishes left and right on the walking plane");
            var closeMachine=FindObjectsByType<Machine>(FindObjectsSortMode.None).Single(x=>x.machineId=="mould");p.Pick(g.sharedCore);p.Teleport(new Vector3(-5.4f,0,18.9f),0);Physics.SyncTransforms();
            Check(p.CanReach(closeMachine)&&p.NearbyCoreSocket()==closeMachine,"close horizontal view reaches the whole machine without hunting for a socket or looking down");
            p.ScanFocus();Check(p.focus==closeMachine,"carrying core prioritizes the visible nearby machine over decorative pickups");
            Check(!g.guide.IsCurrentCoreSocket(closeMachine),"nearby but out-of-sequence machine cannot replace the actual gate objective");var hintCrate=FindObjectsByType<Pickup>(FindObjectsSortMode.None).Single(x=>x.name=="Workshop override crate");Vector3 hintCrateHome=hintCrate.transform.position;hintCrate.PlaceAt(g.workshopRelay.gate.plate.position+Vector3.up*.885f);Physics.SyncTransforms();Check(g.guide.IsCurrentCoreSocket(closeMachine),"E/Q insertion coach appears when the nearby machine is the actual current task");

            Check(g.guide.Navigate(g.guide.Current).key=="E"&&g.guide.Navigate(g.guide.Current).routeDistance==0,"reachable socket retains its E action rather than routing behind a worktop");
            p.TryPlace();Check(!p.held&&closeMachine.core==g.sharedCore&&g.activeCircuit==closeMachine,"Q near the console performs actual core insertion and opens the circuit instead of rejecting the worktop");
            g.CloseCircuit();closeMachine.Remove(p);p.PutBackHeld();hintCrate.PlaceAt(hintCrateHome);Physics.SyncTransforms();
            var machineWall=GameObject.CreatePrimitive(PrimitiveType.Cube);machineWall.transform.position=new Vector3(-5.4f,1.4f,19.1f);machineWall.transform.localScale=new Vector3(3,3,.12f);Physics.SyncTransforms();p.Pick(g.sharedCore);Check(!p.CanReach(closeMachine)&&p.NearbyCoreSocket()!=closeMachine,"broad machine assist still cannot insert a core through a wall");p.PutBackHeld();DestroyImmediate(machineWall);
            p.ApplyLook(new Vector2(10,20),0,0,0);float beforeLevel=p.transform.eulerAngles.y;p.LevelView();Check(p.Pitch==0&&Quaternion.Angle(p.view.transform.localRotation,Quaternion.identity)<.01f&&Mathf.Abs(Mathf.DeltaAngle(p.transform.eulerAngles.y,beforeLevel))<.01f,"explicit level-view recovery changes pitch without silently changing the player's heading");
            p.Teleport(new Vector3(0,0,-7),0);g.navigation.RefreshVisuals();var raised=g.navigation.GetComponentsInChildren<LineRenderer>().Where(x=>x.name=="Raised gold route marker").ToArray();Check(raised.Any(x=>x.enabled&&x.GetPosition(3).y>.9f),"actual route rendering includes stationary raised lights visible above the floor");g.ToggleMap();g.navigation.RefreshVisuals();Check(raised.All(x=>!x.enabled),"raised navigation lights are hidden while reading the map");g.ToggleMap();
            p.Teleport(new Vector3(.8f,0,14.2f),0);Physics.SyncTransforms();p.ScanFocus();Check(p.focus==g.sharedCore,"nearby core can be picked up off-centre without pointing at its tiny collider");
            Check(p.TryInteract()&&p.held==g.sharedCore,"forgiving core focus performs the actual pickup");p.PutBackHeld();
            var chimeFocus=FindObjectsByType<Machine>(FindObjectsSortMode.None).Single(x=>x.machineId=="chime");var focusCrate=FindObjectsByType<Pickup>(FindObjectsSortMode.None).Single(x=>x.name=="Archive crate");p.Pick(g.sharedCore);chimeFocus.Use(p);p.Teleport(new Vector3(-2.85f,0,35.09f),220);p.view.transform.LookAt(focusCrate.transform.position);Physics.SyncTransforms();p.ScanFocus();
            Check(p.focus==focusCrate,"direct crate aim wins over a nearby installed core instead of silently retrieving the core");Check(p.TryInteract()&&p.held==focusCrate,"the intended crate is actually picked up with E beside the powered chime");p.PutBackHeld();chimeFocus.Remove(p);p.PutBackHeld();
            var drawingFocus=FindObjectsByType<StoryPoint>(FindObjectsSortMode.None).Single(x=>x.kind==StoryPoint.StoryKind.Drawing);p.Teleport(new Vector3(1.06f,0,44.61f),90);Physics.SyncTransforms();p.ScanFocus();Check(p.focus==drawingFocus,"nearby drawing uses forgiving visible-object focus rather than requiring a thin exact ray hit");

            var blockingWall=GameObject.CreatePrimitive(PrimitiveType.Cube);blockingWall.transform.position=new Vector3(.4f,1,15.1f);blockingWall.transform.localScale=new Vector3(3,2,.2f);Physics.SyncTransforms();Check(!p.CanReach(g.sharedCore),"forgiving core pickup cannot pass through a solid wall");DestroyImmediate(blockingWall);
            p.Teleport(new Vector3(0,0,-7),0);Physics.SyncTransforms();
            var packDoor=FindObjectsByType<FactoryGate>(FindObjectsSortMode.None).Single(x=>x.name=="Packaging gate");var savedPanel=packDoor.panel.position;packDoor.panel.position=packDoor.closedPosition+Vector3.up*3.7f;Physics.SyncTransforms();
            Check(g.navigation.Build(new Vector3(5.4f,0,23.1f),new Vector3(0,0,31.4f)),"floor route reaches archive entrance around the conveyor");
            bool clearRoute=true;for(int i=1;i<g.navigation.Points.Count;i++){Vector3 a=g.navigation.Points[i-1]+Vector3.up*.8f,b=g.navigation.Points[i]+Vector3.up*.8f;foreach(var hit in Physics.SphereCastAll(a,.28f,(b-a).normalized,Vector3.Distance(a,b),~0,QueryTriggerInteraction.Ignore))if(!hit.transform.IsChildOf(p.transform)&&!hit.collider.GetComponentInParent<GhostIdle>())clearRoute=false;}
            Check(clearRoute,"drawn archive route has no wall, shelf or conveyor crossing");
            Check(g.navigation.Build(new Vector3(0,0,31.4f),new Vector3(5.5f,0,41.3f)),"floor route reaches archive plate around solid shelves");
            packDoor.panel.position=savedPanel;Physics.SyncTransforms();Check(!g.navigation.Build(new Vector3(0,0,28),new Vector3(0,0,32)),"route cannot cross a closed packaging gate");

            g.guide.looked=true;Check(g.guide.Current.key=="W A S D","guide advances to walking only after observing look input");
            g.guide.walked=true;Check(g.guide.Current.key=="E","guide then asks for the crate interaction");
            g.Say("Menu regression","This line must survive opening the menu.");g.TogglePause();
            Check(g.paused&&g.Reading&&g.Frozen&&Time.timeScale==0,"menu can open during dialogue without discarding it");
            g.TogglePause();Check(!g.paused&&g.Reading&&g.line.Contains("survive"),"resume returns to the same dialogue line");CloseDialogue();
            g.audioScore.RestoreAudio();var music=g.audioScore.GetComponents<AudioSource>().Single(x=>x.loop);music.Stop();
            g.audioScore.RestoreAudio();Check(g.audioScore.MusicPlaying,"audio recovery starts a stopped source rather than only unpausing it");
            var previousClip=music.clip;g.audioScore.SendMessage("ConfigurationChanged",true);g.audioScore.RestoreAudio();
            Check(g.audioScore.MusicPlaying&&music.clip!=previousClip,"audio device recovery recreates the generated music clips");
            Check(g.audioScore.GetComponents<AudioSource>().All(x=>x.mute),"automated checks stay silent even after restoring audio");
            var font=Resources.Load<Font>("Fonts/NotoSansSC");Check(font&&font.HasCharacter('我')&&Resources.Load<Font>("Fonts/Fraunces"),"redistributable fonts include Chinese guide glyphs and a display face");
        }catch(Exception e){Fail(e);yield break;}
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

            p.Pick(overrideCrate);p.Teleport(new Vector3(4.8f,0,18.6f),180);Physics.SyncTransforms();Check(!p.Placement(out _,out _)&&!p.ReadyOnPlate(powerGate),"plate assistance never places a crate through a wall");p.PutBackHeld();
            var tutorialCrate=crates.Single(x=>x.name=="Foyer crate");p.Pick(tutorialCrate);
            p.Teleport(new Vector3(-4.8f,0,7.3f),0);Physics.SyncTransforms();p.TryPlace();Check(p.held==tutorialCrate,"plate assist refuses to place a crate over the player");
            var worktop=GameObject.CreatePrimitive(PrimitiveType.Cube);worktop.transform.position=new Vector3(0,1,1.5f);worktop.transform.localScale=new Vector3(2,.2f,2);p.Teleport(Vector3.zero,0);Physics.SyncTransforms();p.TryPlace();Check(p.held==tutorialCrate,"placement refuses elevated worktops");DestroyImmediate(worktop);Physics.SyncTransforms();
            p.Teleport(new Vector3(-4.8f,0,5.3f),0);Physics.SyncTransforms();Check(p.ReadyOnPlate(foyer)&&g.guide.Current.action.Contains("Press Q"),"green plate-ready guidance refers to the same validated Q landing point");p.TryPlace();
            Check(p.held==null&&Vector3.Distance(tutorialCrate.transform.position,foyer.plate.position+Vector3.up*.885f)<.05f&&foyer.Evaluate(),"near-edge crate placement snaps onto plate and opens gate");
            p.Teleport(new Vector3(0,0,-6),0);

            crates.Single(x=>x.name=="Foyer crate").transform.position=foyer.plate.position+Vector3.up*.885f;Physics.SyncTransforms();Check(foyer.Evaluate(),"placed crate opens pressure gate");
            p.Pick(core);g.ToggleJournal();Check(g.Frozen&&Time.timeScale==0&&p.held==core,"journal freezes world and retains held item");g.ToggleJournal();Check(!g.Frozen&&Time.timeScale==1,"journal returns to active play");g.TogglePause();Check(g.Frozen&&Time.timeScale==0,"pause freezes world");g.TogglePause();Check(!g.Frozen&&p.held==core,"resume preserves held item");var blocked=machines.Single(x=>x.machineId=="press");blocked.Use(p);Check(p.held==core&&blocked.core==null,"out-of-order machine never traps the only core");var mould=machines.Single(x=>x.machineId=="mould");mould.Use(p);Check(mould.core==core&&p.held==null&&g.activeCircuit==mould,"one console interaction inserts core and opens the routing board");mould.Use(p);SolveRouting(mould);
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
            var completedMould=machines.Single(x=>x.machineId=="mould");p.Teleport(new Vector3(-4.3f,0,18.1f),0);p.view.transform.LookAt(completedMould.corePoint.position+Vector3.left*.7f);Physics.SyncTransforms();p.ScanFocus();Check(p.focus==core,"completed socket core is reachable despite its disabled collider");p.TryInteract();Check(p.held==core&&completedMould.core==null&&!completedMould.running,"completed machine returns same core with tolerant focus");p.Teleport(new Vector3(0,0,-6),0);
            ribbon.Use(p);ribbon.Use(p);SolveRouting(ribbon);Check(g.shell,"processed shell survives power transfer");
            shellDisplay.Refresh();Check(shellDisplay.Visible,"visible shell survives removing and reusing its core");
        }catch(Exception e){Fail(e);yield break;}
        Time.timeScale=16;yield return new WaitForSecondsRealtime(.5f);CloseDialogue();
        try{
            var ribbonDisplay=FindObjectsByType<LanternDisplay>(FindObjectsSortMode.None).Single(x=>x.station&&x.station.machineId=="ribbon");ribbonDisplay.Refresh();
            Check(ribbonDisplay.Visible&&ribbonDisplay.Stage==2&&ribbonDisplay.ribbon.gameObject.activeSelf&&!ribbonDisplay.inscription.gameObject.activeSelf,"ribbon output adds its bow without revealing engraving");
            p.Teleport(new Vector3(0,0,29),0);Check(g.guide.Current.target==g.sharedCore.transform.position,"archive guidance retrieves the core before sending the player into the next room");p.Teleport(new Vector3(0,0,-6),0);
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
            g.inspectorLesson=false;Check(!g.inspector.CanSeePlayer(),"archive first practice remains safe before the chime lesson");g.inspectorLesson=true;
            Check(g.inspector.CanSeePlayer(),"inspector vision sees player in range and field of view");
            var cover=crates.First();cover.transform.position=new Vector3(0,.835f,38);Physics.SyncTransforms();
            Check(!g.inspector.CanSeePlayer(),"placed crate physically blocks inspector sight");
            cover.SetColliders(false);Physics.SyncTransforms();Check(g.inspector.CanSeePlayer(),"disabled carried-item collision does not create invisible cover");cover.SetColliders(true);cover.ResetHome();
            p.Teleport(new Vector3(0,0,47.2f),0);g.inspector.assemblyDuty=true;g.inspector.route=new[]{new Vector3(4,0,52),new Vector3(-1,0,50)};g.inspector.ResetPatrol();
            var finalAccess=gates.Single(x=>x.name=="Assembly work gate");var finalCrate=crates.Single(x=>x.name=="Assembly crate");finalCrate.PlaceAt(finalAccess.plate.position+Vector3.up*.885f);Physics.SyncTransforms();
            p.Pick(core);g.ResetSection();Check(finalAccess.CrateHoldsPlate,"safety retry preserves a successfully placed gate crate instead of repeating solved work");finalCrate.ResetHome();Physics.SyncTransforms();Check(p.transform.position.z>46&&core.transform.position.z>46,"final-area retry recovers player and held core locally");Check(!g.inspector.CanSeePlayer(),"safety reset grants a brief detection grace period");CloseDialogue();
            g.inspector.enabled=true;g.inspector.assemblyDuty=false;g.inspector.route=new[]{new Vector3(-2.3f,0,35.5f),new Vector3(2.3f,0,40.5f)};g.inspector.ResetPatrol();p.Teleport(new Vector3(0,0,15),0);
            var etch=machines.Single(x=>x.machineId=="etch");p.Pick(core);etch.Use(p);etch.Use(p);SolveRouting(etch);
            p.Teleport(new Vector3(-5.4f,0,49.1f),0);var preparation=g.guide.Current;Check(preparation.targetName=="Crate"&&preparation.reason.Contains("while it works"),"engraver gives the player gate preparation rather than exposed waiting");
            g.Hint();Check(g.toast.Contains(g.guide.Navigate(g.guide.Current).action),"requested hints stay consistent with the current formal-game action");p.Teleport(new Vector3(0,0,15),0);
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
            var pressDisplay=FindObjectsByType<LanternDisplay>(FindObjectsSortMode.None).Single(x=>x.station&&x.station.machineId=="press");pressDisplay.Refresh();Check(!pressDisplay.Visible,"press clears its workpiece when the one deliverable lantern is ready");p.Pick(g.lantern);p.Teleport(new Vector3(0,0,58.2f),0);Physics.SyncTransforms();p.ScanFocus();Check(p.focus is StoryPoint deliveryPoint&&deliveryPoint.kind==StoryPoint.StoryKind.Pip&&g.guide.Navigate(g.guide.Current).key=="E","reachable delivery target retains its E action rather than routing around the dispatch desk");p.TryInteract();Check(g.ended&&p.held==null,"physical lantern delivery reaches ending");
            Check(FindObjectsByType<Pickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).Count(x=>x.kind==Pickup.Kind.Core)==1,"one core throughout run; no duplication");
            File.WriteAllText("Work/playcheck.txt",$"PASS: {checks} engine integration checks.\nChecks use actual scene components and Update processing.\nThis is not an unassisted human playthrough or a timing validation.\n");Debug.Log("PLAYCHECK_PASS / "+checks);
        }catch(Exception e){Fail(e);yield break;}
        float endStarted=Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup-endStarted<15){yield return null;}
        try{var ending=FindFirstObjectByType<EndOfShift>();Check(ending&&ending.pip.position.z>70&&ending.exitDoor.position.y>5,"ending opens exit and takes the helpers into the village");File.WriteAllText("Work/playcheck.txt",$"PASS: {checks} engine integration checks, including the animated ending.\nActual scene components, physics visibility, progression, retries and delivery exercised.\nNot an unassisted human playthrough or timing validation.\n");}
        catch(Exception e){Fail(e);yield break;}
        UiCopy.Language(initialLanguage,false);Application.logMessageReceived-=OnLog;EditorApplication.Exit(0);
    }
    void SolveRouting(Machine machine){
        Check(g.activeCircuit==machine&&g.Frozen&&Time.timeScale==0,"routing mode freezes workshop: "+machine.machineId);
        g.PowerCircuit();Check(g.activeCircuit==machine&&!machine.running,"unconnected routing cannot start the machine: "+machine.machineId);
        var grid=machine.puzzle;Check(!grid.Solved,"authored circuit begins unsolved: "+machine.machineId);
        for(int i=0;i<grid.turns.Length;i++)while(grid.turns[i]!=0)grid.Rotate(i);
        Check(grid.Solved,"authored circuit has a valid connected solution: "+machine.machineId);
        g.PowerCircuit();Check(g.activeCircuit==null&&machine.running,"connected circuit starts machine: "+machine.machineId);
        var output=FindObjectsByType<LanternDisplay>(FindObjectsSortMode.None).Single(x=>x.station==machine);output.Refresh();Check(output.caption&&!output.caption.text.Contains("READY"),"running output label does not promise completion: "+machine.machineId);
        if(machine.machineId=="mould"){machine.progress=machine.workSeconds*.5f;output.Refresh();Check(Mathf.Abs(output.model.localPosition.y-output.halfHeight*output.model.localScale.y+output.halfHeight)<.001f,"growing shell stays anchored to its tray");machine.progress=0;var relay=g.workshopRelay;var crate=FindObjectsByType<Pickup>(FindObjectsSortMode.None).First(x=>x.kind==Pickup.Kind.Crate&&!x.carried);var position=crate.transform.position;crate.PlaceAt(relay.gate.plate.position+Vector3.up*.885f);p.Teleport(new Vector3(0,0,19),0);Physics.SyncTransforms();Check(string.IsNullOrEmpty(g.guide.Navigate(g.guide.Current).key),"watching a running mould does not turn into an unnecessary walking instruction");Check(g.Objective.Contains("TAKING SHAPE"),"running machine objective does not instruct the player to remove its core or reopen routing");crate.PlaceAt(position);}

    }
    void CloseDialogue(){for(int i=0;i<20&&g.Reading;i++)g.Advance();Time.timeScale=1;}
    void Check(bool ok,string what){if(!ok)throw new Exception(what);checks++;Debug.Log("CHECK_PASS / "+what);}
    void OnLog(string message,string trace,LogType type){if(type==LogType.Exception)Fail(new Exception(message+"\n"+trace));}
    void Fail(Exception e){File.WriteAllText("Work/playcheck.txt","FAIL: "+e);Debug.LogError("PLAYCHECK_FAIL / "+e);EditorApplication.Exit(1);}
}
