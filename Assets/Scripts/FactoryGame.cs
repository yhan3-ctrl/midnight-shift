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
    int circuitOpenedFrame;
    public Machine activeCircuit; CircuitView circuitView=new CircuitView();
    public bool started, paused, ended, journal;
    public bool shell, transferred, blueprint, etched, assembled, inspectorLesson;
    public int resets, hintsRequested;
    public float playSeconds;
    public string toast="", speaker="", line="";
    public bool Reading => dialogue.Count>0;
    public bool Frozen => !started || paused || Reading || ended || journal || activeCircuit!=null;
    readonly Queue<string> dialogue=new Queue<string>();
    float toastUntil;
    public float musicVolume=.38f, effectsVolume=.65f, sensitivity=1.7f;
    GUIStyle title, body, small, heading, button, prompt;
    Texture2D paper;
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
            if(!shell) return "01  /  MAKE THE LANTERN SHELL\nPower the workshop gate. Bring its crate back to the override plate, then reuse the core.";
            if(!transferred) return "02  /  SHARE THE POWER\nThe shell stays finished. Move the SAME core to the Ribbon Conveyor.";
            if(!blueprint) return "03  /  FIND THE OLD DRAWING\nPower the Break-time Chime to call Hush, then hold the archive plate with a crate.";
            if(!etched) return "04  /  FINISH YOUR OLD PROMISE\nBring the power core to the Engraving Table in Final Assembly.";
            if(!assembled) return "05  /  LIGHT THE WAY HOME\nKeep the assembly gate open with a crate. Move the core to the Lantern Press.";
            return "06  /  ONE LAST DELIVERY\nPick up the finished lantern and give it to Pip at the dispatch desk.";
        }
    }
    void Awake(){ I=this; Time.timeScale=1; }
    void Start(){ SetCursor(true); }
    void Update(){
        if(!started&&(Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.Space))){Begin();return;}
        if(started&&!paused&&!Reading&&!ended&&!journal) playSeconds+=Time.unscaledDeltaTime;
        if(activeCircuit){
            if(Time.frameCount==circuitOpenedFrame)return;
            if(Input.GetKeyDown(KeyCode.Escape)){CloseCircuit();return;}
            var grid=activeCircuit.puzzle;int x=activeCircuit.selectedTile%grid.size,y=activeCircuit.selectedTile/grid.size;
            if(Input.GetKeyDown(KeyCode.LeftArrow))x=Mathf.Max(0,x-1);if(Input.GetKeyDown(KeyCode.RightArrow))x=Mathf.Min(grid.size-1,x+1);
            if(Input.GetKeyDown(KeyCode.UpArrow))y=Mathf.Max(0,y-1);if(Input.GetKeyDown(KeyCode.DownArrow))y=Mathf.Min(grid.size-1,y+1);
            activeCircuit.selectedTile=grid.Index(x,y);
            if(Input.GetKeyDown(KeyCode.Space)){grid.Rotate(activeCircuit.selectedTile);audioScore.Tick(550);}
            if(Input.GetKeyDown(KeyCode.Return)&&grid.Solved)PowerCircuit();return;
        }
        if(Input.GetKeyDown(KeyCode.Escape)&&started&&!ended&&!Reading){if(journal)ToggleJournal();else TogglePause();}
        if(Input.GetKeyDown(KeyCode.J))ToggleJournal();
        if(Reading&&(Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.Return))) Advance();
        if(started&&!Frozen&&Input.GetKeyDown(KeyCode.H)) Hint();
    }
    public void OpenCircuit(Machine m){activeCircuit=m;circuitOpenedFrame=Time.frameCount;SyncFreeze();}
    public void CloseCircuit(){activeCircuit=null;SyncFreeze();}
    public void PowerCircuit(){if(!activeCircuit||!activeCircuit.puzzle.Solved)return;var machine=activeCircuit;CloseCircuit();machine.StartWork();}
    public void TogglePause(){if(!started||ended||Reading||journal)return;paused=!paused;SyncFreeze();}
    public void ToggleJournal(){if(!started||ended||Reading||paused)return;journal=!journal;SyncFreeze();}
    public void Begin(){ started=true; SyncFreeze(); Say("PIP / NIGHT FOREMAN",
        "Oh! You must be Aunt May's helper. Welcome to the last night shift.",
        "The factory is closing, but there's still one order on my desk. A pumpkin lantern. No readable name... just a child's drawing.",
        "Would you help us finish it? Button left a crate by the striped plate. Try leaving it there to hold the door open.",
        "WASD to walk. Mouse or arrow keys to look. E to pick up or use. Q to place. H for a hint. There's no timer to beat."); }
    public void Say(string who, params string[] text){ speaker=who; dialogue.Clear(); foreach(var s in text) dialogue.Enqueue(s); line=dialogue.Peek(); SyncFreeze(); }
    public void Advance(){ if(!Reading)return; dialogue.Dequeue(); if(Reading)line=dialogue.Peek(); SyncFreeze(); }
    void SyncFreeze(){ Time.timeScale=Frozen?0:1; SetCursor(Frozen); }
    void SetCursor(bool free){ Cursor.lockState=free?CursorLockMode.None:CursorLockMode.Locked; Cursor.visible=free; }
    public void Notify(string text,float seconds=3){toast=text;toastUntil=Time.unscaledTime+seconds;}
    public void Hint(){hintsRequested++;
        string h=!shell&&player&&player.transform.position.z<12?"E picks up the foyer crate. Stand one step before the striped plate, facing it, and press Q. Leave the crate there and walk through the centre door.":
        !shell?"Leave the foyer crate on its plate. Power the Workshop Relay, fetch the crate beyond its gate, and leave that crate on the override plate before taking the core to Moulding.":
        !transferred?"E on a completed or running machine removes its core. Carry it to the Ribbon Conveyor; the finished shell will remain.":
        !blueprint?"The archive gate opens while a crate rests on its plate. Stored crates block Hush's sight. A powered Chime distracts him.":
        !etched?"Take your core north to the Engraving Table. The old drawing is saved in your journal; you do not need to carry it.":
        !assembled?"Place the assembly crate on the plate before moving your core through the gate to the Lantern Press.":
        "The finished lantern is on the brass pedestal beside the Press. Carry it to Pip's desk.";
        Notify(h,10);
    }
    public void Complete(string id){
        switch(id){
            case "mould": shell=true; Say("BUTTON / STORES", "A perfect little shell! I kept the mould right where that child left it.","They used to ask who switched off the lights. Not what we'd make them. Who would get home last.","The shell is safe now. Take the power core out and use it on the Ribbon Conveyor.");break;
            case "ribbon": transferred=true; Notify("PACKAGING COMPLETE / The archive door is now unlocked.",5);break;
            case "etch": etched=true; Notify("THE PROMISE IS ENGRAVED / Move the core to the Lantern Press.",5);break;
            case "press": assembled=true; lantern.gameObject.SetActive(true); Say("PIP / OVER THE SPEAKER","Look at that. Every little crooked line, just like the drawing.","I'll be at dispatch. Shall we see who this one's for?");break;
        }
        if(audioScore) audioScore.Chord();
    }
    public void ReadDrawing(){
        if(blueprint){Say("YOUR OLD DRAWING","For the last one to leave. So the way home is bright, too.");return;}
        blueprint=true;
        Say("THE BACK OF THE DRAWING","Under the frame is a note in your own childhood handwriting.","'For the last one to leave. So the way home is bright, too.'", "You remember now. The lantern was never waiting for a customer. You drew it for the little helpers who stayed after everyone else went home.","Pip has kept your promise safe all these years. You can finish it tonight.");
        if(audioScore)audioScore.Chord();
    }
    public void Deliver(){
        if(!assembled||!player.held||player.held.kind!=Pickup.Kind.Lantern){Say("PIP / DISPATCH","One last order, then we can all go home. I'll keep the desk warm.");return;}
        player.ReleaseForDelivery();player.hand.gameObject.SetActive(false); ended=true; Time.timeScale=1; SetCursor(true); if(audioScore)audioScore.Chord();
    }
    public void ResetSection(){
        if(Frozen)return; resets++;
        bool final=inspector&&inspector.assemblyDuty;var held=player.held;player.PutBackHeld();
        float checkpoint=final?47.2f:31.7f;
        if(held&&held.kind==Pickup.Kind.Core)held.transform.position=new Vector3(1.4f,.6f,checkpoint);
        player.Teleport(new Vector3(0,0,checkpoint),0);
        foreach(var p in FindObjectsByType<Pickup>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            if(p.kind==Pickup.Kind.Crate && (final?p.home.z>46:p.home.z>30&&p.home.z<46))p.ResetHome();
        inspector.ResetPatrol(true); Notify("Hush: 'Safety check! Let's try that delivery again.' Your finished work is safe.",6);
    }
    public void Restart(){ Time.timeScale=1; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    void Styles(){
        if(title!=null)return;
        paper=new Texture2D(1,1);paper.SetPixel(0,0,new Color(.055f,.08f,.095f,.96f));paper.Apply();
        title=new GUIStyle(GUI.skin.label){fontSize=52,fontStyle=FontStyle.Bold,wordWrap=true};title.normal.textColor=new Color(1,.84f,.57f);
        heading=new GUIStyle(title){fontSize=19};
        body=new GUIStyle(GUI.skin.label){fontSize=22,wordWrap=true};body.normal.textColor=new Color(.96f,.93f,.85f);
        small=new GUIStyle(body){fontSize=15};
        prompt=new GUIStyle(body){alignment=TextAnchor.MiddleCenter,fontSize=19};
        button=new GUIStyle(GUI.skin.button){fontSize=20,padding=new RectOffset(12,12,10,10)};
        button.normal.background=Solid(new Color(.91f,.72f,.42f));button.normal.textColor=new Color(.08f,.14f,.16f);button.hover.background=Solid(new Color(1,.85f,.6f));button.hover.textColor=button.normal.textColor;button.active.background=Solid(new Color(.73f,.53f,.27f));button.active.textColor=button.normal.textColor;
    }
    Texture2D Solid(Color c){var t=new Texture2D(1,1);t.SetPixel(0,0,c);t.Apply();return t;}
    void Panel(Rect r){GUI.DrawTexture(r,paper);}
    void OnGUI(){
        Styles(); float scale=Mathf.Min(Screen.width/1440f,Screen.height/900f); GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,Vector3.one*scale);
        float w=Screen.width/scale,h=Screen.height/scale;
        if(!started){
            Panel(new Rect(65,80,640,h-160)); GUI.Label(new Rect(100,110,560,30),"A HALLOWEEN NIGHT AT WILLOW WORKS",heading);
            GUI.Label(new Rect(98,172,560,140),"MIDNIGHT SHIFT\nThe Last Lantern",title);
            GUI.Label(new Rect(100,345,520,110),"One old order. Three little ghosts.\nA promise you left here long ago.",body);
            GUI.Label(new Rect(100,477,510,85),"Explore a warm toy factory, share its last power core, and make a light for the ones who stayed late.",small);
            if(GUI.Button(new Rect(100,610,330,58),"Begin the night shift",button))Begin();
            GUI.Label(new Rect(100,690,510,100),"WASD  Walk    Mouse / arrows  Look    E  Use\nQ  Place    H  Hint    J  Journal    Esc  Pause\nEarly playable / v0.3 — timing not yet playtested",small);return;
        }
        if(ended){
            Panel(new Rect(40,80,570,h-160));GUI.Label(new Rect(75,110,500,40),"THE LAST ORDER / DELIVERED",heading);
            GUI.Label(new Rect(72,185,500,125),"A light for\nthe way home.",title);
            GUI.Label(new Rect(75,355,495,170),"Pip turns your note over once more.\n\n\"Then this time, we all clock out together.\"\n\nOutside, the parade is just beginning.",body);
            GUI.Label(new Rect(75,h-250,490,65),$"Active play: {TimeSpan.FromSeconds(playSeconds):mm\\:ss}\nSafety retries: {resets}",small);
            if(GUI.Button(new Rect(75,h-175,490,55),"Visit the factory again",button))Restart();return;
        }
        Panel(new Rect(24,24,560,104));GUI.Label(new Rect(42,35,525,86),Objective,small);
        GUI.Label(new Rect(w-310,28,280,35),"WILLOW WORKS / OCTOBER 31",small);
        if(!Reading&&!paused&&!journal&&!activeCircuit){
            GUI.Label(new Rect(w/2-12,h/2-15,24,30),"·",prompt);
            if(player&&player.focus){ Panel(new Rect(w/2-290,h-155,580,58)); GUI.Label(new Rect(w/2-280,h-148,560,48),player.focus.Prompt(player),prompt); }
            GUI.Label(new Rect(30,h-48,1000,30),player&&player.held?"E  Use socket / machine    Q  Place "+player.held.displayName+"    H  Hint    J  Journal    Esc  Pause":"WASD  Walk    E  Interact    Q  Place    H  Hint    J  Journal    Esc  Pause",small);
        }
        if(Time.unscaledTime<toastUntil){Panel(new Rect(w/2-380,145,760,80));GUI.Label(new Rect(w/2-360,158,720,66),toast,small);}
        if(inspector&&inspector.Alertness>0&&!Reading&&!paused&&!journal){GUI.color=new Color(1,.65f,.3f);GUI.Box(new Rect(w/2-140,240,280*inspector.Alertness,7),GUIContent.none);GUI.color=Color.white;GUI.Label(new Rect(w/2-140,254,280,30),"HUSH / "+inspector.state.ToString().ToUpper(),small);}
        if(activeCircuit){circuitView.Draw(activeCircuit,w,h,paper);return;}
        if(Reading){ Panel(new Rect(w/2-470,h-300,940,250));GUI.Label(new Rect(w/2-435,h-278,870,34),speaker,heading);GUI.Label(new Rect(w/2-435,h-228,865,135),line,body);if(GUI.Button(new Rect(w/2+220,h-115,210,45),"Continue  [Space]",button))Advance(); }
        if(journal){
            Panel(new Rect(w/2-440,70,880,h-140));
            GUI.Label(new Rect(w/2-395,105,790,65),"The night-shift notebook",title);
            string notes="CURRENT JOB\n"+Objective+"\n\nPOWER CORE / "+CoreLocation+"\n\nWORKSHOP NOTES\nA crate keeps a striped plate pressed. A powered relay can hold a door open, too. Find a way to keep access when the core is needed elsewhere.\n\nOne core can serve several machines. Completed work stays finished after power is removed. E removes a core from a finished machine.\n\nHush follows a regular route. Placed crates block his sight. A starting machine in his work area calls him over. During a safety retry, your finished work is kept.";
            if(blueprint)notes+="\n\nTHE OLD DRAWING\nFor the last one to leave. So the way home is bright, too.\nThe lantern is a gift for the helpers. You drew it years ago.";
            GUI.Label(new Rect(w/2-395,195,790,h-370),notes,small);
            if(GUI.Button(new Rect(w/2-160,h-150,320,50),"Close notebook  [J]",button)){ToggleJournal();}
        }
        if(paused){Panel(new Rect(w/2-310,120,620,h-240));GUI.Label(new Rect(w/2-265,155,530,60),"Take a breath",title);
            GUI.Label(new Rect(w/2-265,260,530,30),"Music",body);musicVolume=GUI.HorizontalSlider(new Rect(w/2-265,305,530,25),musicVolume,0,1);
            GUI.Label(new Rect(w/2-265,345,530,30),"Effects",body);effectsVolume=GUI.HorizontalSlider(new Rect(w/2-265,390,530,25),effectsVolume,0,1);
            GUI.Label(new Rect(w/2-265,430,530,30),"Mouse sensitivity",body);sensitivity=GUI.HorizontalSlider(new Rect(w/2-265,475,530,25),sensitivity,.5f,3.5f);
            if(GUI.Button(new Rect(w/2-265,535,530,52),"Return to the factory",button)){TogglePause();}
            if(GUI.Button(new Rect(w/2-265,605,530,48),"Restart this night",button))Restart();
        }
    }
    void OnApplicationFocus(bool focused){if(!focused&&!Application.isBatchMode&&started&&!Frozen){paused=true;SyncFreeze();}}
    void OnDestroy(){if(I==this){Time.timeScale=1;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}}
}
public abstract class Interactable : MonoBehaviour { public abstract string Prompt(PlayerMotor p); public abstract void Use(PlayerMotor p); }
}
