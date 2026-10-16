using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastLantern {
public class FactoryGame : MonoBehaviour {
    public static FactoryGame I;
    public PlayerMotor player;
    public Inspector inspector;
    public Pickup lantern, sharedCore;
    public AudioScore audioScore;
    public GridRelay workshopRelay; public GhostIdle buttonHelper,pipHelper;
    int circuitOpenedFrame;
    public Machine activeCircuit; CircuitView circuitView=new CircuitView();
    public bool started, paused, ended, journal, photoMode, desktopHold, mapOpen;
    public bool shell, transferred, blueprint, etched, assembled, inspectorLesson;
    public int resets, hintsRequested;
    public float playSeconds;
    public string toast="", speaker="", line="";
    public bool Reading => dialogue.Count>0;
    public bool Modal => !started || paused || Reading || ended || journal || activeCircuit!=null || photoMode || desktopHold || mapOpen || screen.ComfortOpen;
    public bool ComfortOpen=>screen.ComfortOpen;
    public bool AwaitingLook=>!Modal&&player&&player.NeedsLookStart;
    public bool Frozen=>Modal||AwaitingLook;
    readonly Queue<string> dialogue=new Queue<string>();
    float toastUntil;
    public float musicVolume=.38f, effectsVolume=.65f, sensitivity=1.7f,verticalSensitivity=1.5f,fieldOfView=72;
    public FactoryPath navigation; public FactoryMap factoryMap=new FactoryMap();
    public ShiftGuide guide; ShiftInterface screen=new ShiftInterface();
    int dialogueOpenedFrame;bool archiveWelcome,assemblyWelcome,foyerSuccess;
    public bool ToastVisible=>Time.unscaledTime<toastUntil;
    public string CoreLocation {
        get {
            if(!sharedCore)return "Not located";
            if(player&&player.held==sharedCore)return "In your hands";
            if(sharedCore.socket)return sharedCore.socket.displayName;
            var relay=sharedCore.GetComponentInParent<GridRelay>();if(relay)return "Workshop Relay";
            float z=sharedCore.transform.position.z;return z<18?"Packaging entrance":z<30?"Packaging floor":z<46?"Archive floor":"Assembly floor";
        }
    }
    public string Objective {
        get {
            if(!shell&&player&&player.transform.position.z<12) return "00  /  OPEN THE FIRST DOOR\nCarry the foyer crate toward the striped plate. Q places it a step ahead; leave it on the plate, then walk through the centre door.";
            if(!shell){
                if(workshopRelay){
                    if(workshopRelay.OverrideHeld){
                        if(workshopRelay.core)return "01  /  REUSE THE CORE\nThe crate keeps the workshop open. Take the core out of the Workshop Relay, then bring it to the Moulding Table on the left.";
                        if(sharedCore&&sharedCore.socket&&sharedCore.socket.machineId=="mould"&&sharedCore.socket.running)return "01  /  THE SHELL IS TAKING SHAPE\nWatch the shell form on the worktop. Leave the core in until the machine finishes; then it can power the next job.";
                        if(sharedCore&&sharedCore.socket&&sharedCore.socket.machineId=="mould")return "01  /  MAKE THE LANTERN SHELL\nAt the Moulding Table: E opens the routing board. Connect the contact and outlet, then send power. Your shell stays finished without power.";
                        return "01  /  MAKE THE LANTERN SHELL\nThe gate is held by the crate. Bring the core to the Moulding Table on the left and use E to insert it.";
                    }
                    if(workshopRelay.core)return "01  /  KEEP THE WAY OPEN\nFetch the crate beyond the powered gate. Bring it back to the striped override plate before reusing the core.";
                }
                return "01  /  POWER THE WORKSHOP\nPick up the gold core and insert it into the purple Workshop Relay on the left. The gate opens while it has power.";
            }
            if(!transferred) return "02  /  SHARE THE POWER\nThe shell stays finished. Move the SAME core to the Ribbon Conveyor.";
            if(!blueprint)return inspectorLesson?"03  /  FIND THE OLD DRAWING\nYou have tried the Chime. Watch Hush's route, use crates as cover and leave a crate on the archive plate to reach the drawing.":"03  /  FIND THE OLD DRAWING\nPower the Break-time Chime to call Hush, then hold the archive plate with a crate.";
            if(!etched) return "04  /  FINISH YOUR OLD PROMISE\nBring the power core to the Engraving Table in Final Assembly.";
            if(!assembled) return "05  /  LIGHT THE WAY HOME\nKeep the assembly gate open with a crate. Move the core to the Lantern Press.";
            return "06  /  ONE LAST DELIVERY\nPick up the finished lantern and give it to Pip at the dispatch desk.";
        }
    }
    void Awake(){ I=this; Time.timeScale=1;guide=GetComponent<ShiftGuide>();if(!guide)guide=gameObject.AddComponent<ShiftGuide>();navigation=GetComponent<FactoryPath>();if(!navigation)navigation=gameObject.AddComponent<FactoryPath>();if(!GetComponent<PlacementPreview>())gameObject.AddComponent<PlacementPreview>(); }
    void Start(){ player.LoadComfort();ReleaseCursor();if(!Application.isBatchMode&&!PlayerPrefs.HasKey("shift-v08-window")){Screen.SetResolution(1280,800,FullScreenMode.Windowed);PlayerPrefs.SetInt("shift-v08-window",1);} }
    void Update(){
        if(PlayerMotor.SystemShortcutHeld){if(player.LookCaptured)HoldForDesktop();return;}
        if(Modal&&player.LookCaptured){ReleaseCursor();RefreshFreeze();}
        if((!started||paused||AwaitingLook)&&!screen.ComfortOpen&&Input.GetKeyDown(KeyCode.C)){OpenComfort();return;}
        if(screen.ComfortOpen){if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Backspace))CloseComfort();return;}
        if(Input.GetKeyDown(KeyCode.F2)&&started&&!ended){TogglePhoto();return;}
        if(photoMode){if(Input.GetKeyDown(KeyCode.Escape))TogglePhoto();return;}
        if(desktopHold){if(Input.GetKeyDown(KeyCode.Return))ResumeDesktop();else if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Tab)){desktopHold=false;TogglePause();}return;}
        if(started&&!ended&&Input.GetKeyDown(KeyCode.M)){ToggleMap();return;}
        if(mapOpen){if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Backspace))ToggleMap();return;}
        if(screen.LanguageOpen){screen.LanguageKeys();return;}
        if((paused||!started)&&Input.GetKeyDown(KeyCode.L)){screen.OpenLanguages();return;}
        bool menu=Input.GetKeyDown(KeyCode.Tab)||Input.GetKeyDown(KeyCode.F1);
        bool escape=Input.GetKeyDown(KeyCode.Escape);
        if(paused){if(Input.GetKeyDown(KeyCode.X)){Quit();return;}if(menu||escape)TogglePause();return;}
        if(menu&&started&&!ended){TogglePause();return;}
        if(!started&&(Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.Space))){Begin();return;}
        if(started&&!paused&&!Reading&&!ended&&!journal&&!photoMode&&!desktopHold&&!mapOpen&&!AwaitingLook)playSeconds+=Time.unscaledDeltaTime;
        if(escape&&started&&!ended){TogglePause();return;}
        if(activeCircuit){
            if(Time.frameCount==circuitOpenedFrame)return;
            if(Input.GetKeyDown(KeyCode.Backspace)){CloseCircuit();return;}
            var grid=activeCircuit.puzzle;int x=activeCircuit.selectedTile%grid.size,y=activeCircuit.selectedTile/grid.size;
            if(Input.GetKeyDown(KeyCode.LeftArrow))x=Mathf.Max(0,x-1);if(Input.GetKeyDown(KeyCode.RightArrow))x=Mathf.Min(grid.size-1,x+1);
            if(Input.GetKeyDown(KeyCode.UpArrow))y=Mathf.Max(0,y-1);if(Input.GetKeyDown(KeyCode.DownArrow))y=Mathf.Min(grid.size-1,y+1);
            activeCircuit.selectedTile=grid.Index(x,y);
            if(Input.GetKeyDown(KeyCode.Space)){grid.Rotate(activeCircuit.selectedTile);audioScore.Tick(550);}
            if(Input.GetKeyDown(KeyCode.Return)&&grid.Solved)PowerCircuit();return;
        }
        if(journal&&Input.GetKeyDown(KeyCode.Backspace)){ToggleJournal();return;}
        if(Input.GetKeyDown(KeyCode.J)&&!Reading)ToggleJournal();
        if(Reading&&Time.frameCount!=dialogueOpenedFrame&&(Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.Return))){Advance();return;}
        if(started&&!Modal&&Input.GetKeyDown(KeyCode.R)){player.LevelView();return;}
        if(AwaitingLook&&Input.GetKeyDown(KeyCode.Return)){StartMouseLook();return;}
        if(started&&!Frozen&&Input.GetKeyDown(KeyCode.H))Hint();
        if(started&&!Frozen){
            if(!foyerSuccess&&player.transform.position.z<12){foreach(var gate in FindObjectsByType<FactoryGate>(FindObjectsSortMode.None))if(gate.name=="Foyer gate"&&gate.CrateHoldsPlate){foyerSuccess=true;Notify("Nice work! The crate keeps the door open. Leave it there and follow the marker.",5);if(buttonHelper)buttonHelper.Celebrate();audioScore.Chord();break;}}
            if(transferred&&!blueprint&&!archiveWelcome&&player.transform.position.z>30.4f){archiveWelcome=true;Say("PIP / ARCHIVE TOUR","This is Hush, our safety inspector. He follows a route and checks noisy machines. He cannot hurt you.","Try the chime first. Then use the crate to open the drawing cabinet. If Hush stops you, finished work stays safe.");}
            else if(blueprint&&!etched&&!assemblyWelcome&&player.transform.position.z>46.6f){assemblyWelcome=true;Say("PIP / FINAL ASSEMBLY","Your old drawing is safe in the notebook. Let's turn that promise into a real lantern.","The Engraver can work on its own. Leave its core in place and prepare the crate gate while it finishes. Keep an eye on Hush.");}
        }
    }
    public void OpenComfort(){screen.ComfortOpen=true;SyncFreeze();}
    public void CloseComfort(bool save=true){screen.ComfortOpen=false;if(save)player.SaveComfort();SyncFreeze();}
    public void StartMouseLook(){if(player.CaptureMouseLook())RefreshFreeze();}
    public void RefreshFreeze(){Time.timeScale=Frozen?0:1;}
    public void OpenLanguages(){ReleaseCursor();screen.OpenLanguages();}
    public void CloseLanguages(){screen.CloseLanguages();}
    public void OpenCircuit(Machine m){activeCircuit=m;circuitOpenedFrame=Time.frameCount;SyncFreeze();}
    public void CloseCircuit(){activeCircuit=null;SyncFreeze();}
    public void PowerCircuit(){if(!activeCircuit||!activeCircuit.puzzle.Solved)return;var machine=activeCircuit;CloseCircuit();machine.StartWork();}
    public void ToggleMap(){if(!started||ended)return;mapOpen=!mapOpen;SyncFreeze();}
    public void TogglePhoto(){if(!started||ended)return;photoMode=!photoMode;SyncFreeze();}
    public void ResumeDesktop(){desktopHold=false;SyncFreeze();}
    public void HoldForDesktop(){ReleaseCursor();if(!started||Modal)return;desktopHold=true;SyncFreeze();}
    public void TogglePause(){if(!started||ended)return;paused=!paused;SyncFreeze();}
    public void ToggleJournal(){if(!started||ended||Reading||paused)return;journal=!journal;SyncFreeze();}
    public void Begin(){started=true;SyncFreeze();Say("PIP / NIGHT FOREMAN",
        "Welcome to Willow Works. I'm Pip. Let me show you around, one little step at a time.",
        "We have one forgotten pumpkin lantern to finish tonight. First, let's open this door together. Follow the gold marker; your checklist is on the right.");}
    public void Say(string who, params string[] text){screen.ResetDialoguePages(); dialogueOpenedFrame=Time.frameCount;speaker=who; dialogue.Clear(); foreach(var s in text) dialogue.Enqueue(s); line=dialogue.Peek(); SyncFreeze(); }
    public void Advance(){ if(!Reading)return;if(screen.AdvancePage(this))return; dialogue.Dequeue(); if(Reading)line=dialogue.Peek(); SyncFreeze(); }
    void SyncFreeze(){ReleaseCursor();RefreshFreeze();}
    void ReleaseCursor(){if(player)player.ReleaseMouseLook();if(Cursor.lockState!=CursorLockMode.None)Cursor.lockState=CursorLockMode.None;if(!Cursor.visible)Cursor.visible=true;}
    public void Notify(string text,float seconds=3){toast=text;toastUntil=Time.unscaledTime+seconds;}
    public void Hint(){
        hintsRequested++;
        var task=guide.Navigate(guide.Current);
        Notify(task.action+"\n"+task.reason,10);
    }
    public void Complete(string id){
        switch(id){
            case "mould": shell=true; Say("BUTTON / STORES", "A perfect little shell! I kept the mould right where that child left it.","They used to ask who switched off the lights. Not what we'd make them. Who would get home last.","The shell is safe now. Take the power core out and use it on the Ribbon Conveyor.");break;
            case "ribbon": transferred=true; Notify("PACKAGING COMPLETE / The archive door is now unlocked.",5);break;
            case "etch": etched=true; Notify("THE PROMISE IS ENGRAVED / Move the core to the Lantern Press.",5);break;
            case "press": assembled=true; lantern.gameObject.SetActive(true); Say("PIP / OVER THE SPEAKER","Look at that. Every little crooked line, just like the drawing.","I'll be at dispatch. Shall we see who this one's for?");break;
        }
        if(buttonHelper&&(id=="mould"||id=="ribbon"))buttonHelper.Celebrate();
        if(pipHelper&&(id=="etch"||id=="press"))pipHelper.Celebrate();
        if(audioScore) audioScore.Chord();
    }
    public void TalkToButton(){
        if(buttonHelper)buttonHelper.Celebrate();
        if(!shell&&player.transform.position.z<12)Say("BUTTON / STORES","Let's open the first door together. Look for the crate on your left. The gold marker and your checklist will show each step.");
        else if(!shell)Say("BUTTON / STORES","I've kept the old mould polished. Pip said the order might still be wanted someday.","The gold core is all we have left. When the workshop gate is held by a crate, we can borrow its power for the Moulding Table.");
        else if(!transferred)Say("BUTTON / STORES","There it is! A real little shell, just like I remember.","The Ribbon Conveyor is across the room. Take the same core; your shell will stay finished.");
        else if(!blueprint)Say("BUTTON / STORES","I tied the ribbon the way the child liked it. A little crooked. Never trim the ends.","Pip kept the old drawing in the archive. Hush is on safety duty there. He only wants the aisles clear.");
        else Say("BUTTON / STORES","You remember who it was for, don't you?","We kept your drawing. You came back to finish it. I think that makes this a very good last shift.");
    }
    public void ReadDrawing(){
        if(blueprint){Say("YOUR OLD DRAWING","For the last one to leave. So the way home is bright, too.");return;}
        blueprint=true;
        Say("THE BACK OF THE DRAWING","Under the frame is a note in your own childhood handwriting.","'For the last one to leave. So the way home is bright, too.'", "You remember now. The lantern was never waiting for a customer. You drew it for the little helpers who stayed after everyone else went home.","Pip has kept your promise safe all these years. You can finish it tonight.");
        if(audioScore)audioScore.Chord();
    }
    public void Deliver(){
        if(!assembled||!player.held||player.held.kind!=Pickup.Kind.Lantern){Say("PIP / DISPATCH","One last order, then we can all go home. I'll keep the desk warm.");return;}
        player.ReleaseForDelivery();player.hand.gameObject.SetActive(false); ended=true; Time.timeScale=1; ReleaseCursor(); if(audioScore)audioScore.Chord();
    }
    public void ResetSection(){
        if(Frozen)return; resets++;
        bool final=inspector&&inspector.assemblyDuty;var held=player.held;player.PutBackHeld();
        float checkpoint=final?47.2f:31.7f;
        if(held&&held.kind==Pickup.Kind.Core)held.transform.position=new Vector3(1.4f,.6f,checkpoint);
        player.Teleport(new Vector3(0,0,checkpoint),0);
        foreach(var p in FindObjectsByType<Pickup>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            if(p.kind==Pickup.Kind.Crate && (final?p.home.z>46:p.home.z>30&&p.home.z<46))p.ResetHome();
        inspector.ResetPatrol(true);
        Say("HUSH / SAFETY CHECK", "Let's try that delivery again. You're back at the room entrance, and this room's crates are back where you found them. Your finished jobs and drawing are safe.",
            "Check your notebook [J] if you need the core's location. A crate only blocks my view after you place it. The chime can call me away before I spot you.");
    }
    public void Restart(){ Time.timeScale=1; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void Quit(){Time.timeScale=1;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;Application.Quit();}
    public void DrawCircuit(float w,float h){circuitView.Draw(activeCircuit,w,h,Texture2D.whiteTexture);}
    void OnGUI(){UiTheme.BeginPointer();try{screen.Draw(this);}finally{UiTheme.EndPointer();}}
    void OnApplicationFocus(bool focused){ReleaseCursor();RefreshFreeze();if(!focused&&!Application.isBatchMode)HoldForDesktop();}
    void OnDestroy(){if(I==this){Time.timeScale=1;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}}
}
public abstract class Interactable : MonoBehaviour { public abstract string Prompt(PlayerMotor p); public abstract void Use(PlayerMotor p); }
}
