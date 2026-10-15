using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;
using LastLantern;

public static class FactoryBuild {
    static Material teal, dark, timber, brass, cream, orange, mint, ink, violet, glass;
    static Transform world; static Font font; static Material worldText; static Transform buttonGhost,exitDoor;
    [MenuItem("Willow Works/Build factory scene")]
    public static void CreateScene(){
        Directory.CreateDirectory("Assets/Materials");Directory.CreateDirectory("Assets/Scenes");Directory.CreateDirectory("Documentation");
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        teal=Mat("Paint - deep teal",new Color(.09f,.25f,.28f));dark=Mat("Paint - midnight",new Color(.045f,.09f,.12f));
        timber=Mat("Wood - honey",new Color(.44f,.26f,.13f));brass=Mat("Metal - warm brass",new Color(.68f,.48f,.23f),.45f);
        cream=Mat("Paper - ivory",new Color(.91f,.84f,.67f));orange=Mat("Pumpkin - apricot",new Color(.95f,.38f,.12f));
        mint=Mat("Ghost - seafoam",new Color(.62f,.89f,.78f));ink=Mat("Ink",new Color(.04f,.08f,.1f));violet=Mat("Fabric - plum",new Color(.3f,.17f,.3f));
        glass=Mat("Core - warm light",new Color(1,.78f,.3f));glass.EnableKeyword("_EMISSION");glass.SetColor("_EmissionColor",new Color(1,.6f,.15f)*1.2f);
        font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        font.RequestCharactersInTexture("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 /.-%&!",64);
        worldText=AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/World text.mat");if(!worldText){worldText=new Material(Shader.Find("Willow/WorldText"));AssetDatabase.CreateAsset(worldText,"Assets/Materials/World text.mat");}worldText.mainTexture=font.material.mainTexture;
        world=new GameObject("WILLOW WORKS / handcrafted factory").transform;
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.43f,.49f,.5f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.09f,.15f,.18f);RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.009f;
        var sun=new GameObject("Autumn moon").AddComponent<Light>();sun.type=LightType.Directional;sun.color=new Color(.7f,.8f,1);sun.intensity=.5f;sun.transform.rotation=Quaternion.Euler(48,-35,0);sun.shadows=LightShadows.Soft;
        QualitySettings.shadowDistance=45;QualitySettings.antiAliasing=4;
        FloorAndShell();
        var game=new GameObject("Night shift / game state").AddComponent<FactoryGame>();game.audioScore=game.gameObject.AddComponent<AudioScore>();game.gameObject.AddComponent<LocalPlaytestLog>();
        var po=new GameObject("Player / first person");po.layer=2;po.transform.position=new Vector3(0,0,-7);var cc=po.AddComponent<CharacterController>();cc.height=1.8f;cc.center=Vector3.up*.9f;cc.radius=.3f;cc.stepOffset=.25f;cc.slopeLimit=45;
        var player=po.AddComponent<PlayerMotor>();game.player=player;
        var cam=new GameObject("Player view").AddComponent<Camera>();cam.tag="MainCamera";cam.transform.SetParent(po.transform);cam.transform.localPosition=new Vector3(0,1.65f,0);cam.fieldOfView=68;cam.nearClipPlane=.06f;cam.farClipPlane=140;cam.backgroundColor=new Color(.075f,.12f,.16f);cam.clearFlags=CameraClearFlags.SolidColor;cam.gameObject.AddComponent<AudioListener>();player.view=cam;
        var hand=new GameObject("Carry pose").transform;hand.SetParent(cam.transform);hand.localPosition=new Vector3(.25f,-.33f,.8f);player.hand=hand;
        // Two visible mitten hands make carrying legible without importing character assets.
        var left=Shape("Left mitten",PrimitiveType.Capsule,Vector3.zero,new Vector3(.12f,.19f,.13f),violet,hand,false);left.transform.localPosition=new Vector3(-.42f,-.22f,.04f);left.transform.localRotation=Quaternion.Euler(0,0,-25);
        var right=Shape("Right mitten",PrimitiveType.Capsule,Vector3.zero,new Vector3(.12f,.19f,.13f),violet,hand,false);right.transform.localPosition=new Vector3(.42f,-.22f,.04f);right.transform.localRotation=Quaternion.Euler(0,0,25);
        // Foyer: crate holds the route open. Plate is visible beside the gate.
        Sign("WILLOW WORKS",new Vector3(0,3.1f,-9.65f),2.4f,cream,180);
        Sign("TOYS, LANTERNS & LITTLE KINDNESSES",new Vector3(0,2.5f,-9.64f),.65f,cream,180);
        RoomTitle("01 / THE LAST NIGHT SHIFT",new Vector3(0,3.25f,10.7f));
        Gate("Foyer gate",12,FactoryGate.Rule.Plate,new Vector3(-4.8f,.05f,8));
        Crate("Foyer crate",new Vector3(-4.8f,.61f,3.8f));
        Board("CRATE + PLATE = OPEN DOOR",new Vector3(-4.8f,2.65f,9.8f),3.8f);
        var welcome=Board("AUNT MAY'S NOTE\nE TO READ",new Vector3(4.8f,1.6f,-4),2.4f).AddComponent<StoryPoint>();welcome.kind=StoryPoint.StoryKind.Note;welcome.note="The helpers only return on Halloween. If the lights are on, please tell them I left the side door open for the parade. Your old drawing is still in the archive. Love, Aunt May.";
        Desk(new Vector3(5.2f,0,3));buttonGhost=Ghost("Button",new Vector3(5.2f,0,3),false).transform;
        game.buttonHelper=buttonGhost.GetComponent<GhostIdle>();game.buttonHelper.role="button";
        var buttonTalk=buttonGhost.gameObject.AddComponent<StoryPoint>();buttonTalk.kind=StoryPoint.StoryKind.Button;var buttonCol=buttonGhost.gameObject.AddComponent<BoxCollider>();buttonCol.center=Vector3.up*1.2f;buttonCol.size=new Vector3(1.3f,1.9f,1);
        for(int i=0;i<3;i++){Shape("Lunch tin",PrimitiveType.Cube,new Vector3(4.5f+i*.65f,1.07f,2.8f),new Vector3(.42f,.18f,.3f),i==1?orange:teal);}
        Pumpkin(new Vector3(-6.6f,.45f,-5),.8f);Pumpkin(new Vector3(6.4f,.36f,8),.6f);
        // Packaging: both machines share the same physical core.
        RoomTitle("02 / PACKAGING",new Vector3(0,3.25f,28.7f));
        game.sharedCore=Core(new Vector3(0,.6f,16));
        var relayGate=Gate("Workshop power gate",18,FactoryGate.Rule.PowerOrPlate,new Vector3(4.8f,.05f,16));
        relayGate.relay=Relay(new Vector3(-5.4f,0,15.3f));relayGate.relay.gate=relayGate;game.workshopRelay=relayGate.relay;
        Crate("Workshop override crate",new Vector3(5,.61f,20));
        Board("POWER OPENS THE GATE\nA CRATE KEEPS IT OPEN WITHOUT POWER\nBRING THE CRATE BACK, THEN REUSE THE CORE",new Vector3(5.4f,2.3f,17.5f),4.2f);
        MachineAt("mould","MOULDING TABLE",new Vector3(-5.4f,0,20),"1 / MAKE THE SHELL");
        MachineAt("ribbon","RIBBON CONVEYOR",new Vector3(5.4f,0,25),"2 / SEND IT TO ASSEMBLY");
        Gate("Packaging gate",30,FactoryGate.Rule.Packaging,Vector3.zero);
        Board("ONE CORE / TWO JOBS\nFINISHED WORK STAYS FINISHED\nE TO REMOVE A CORE AFTER A JOB",new Vector3(-6.5f,2.4f,28.5f),3.2f);
        Conveyor(new Vector3(0,.75f,23),6);
        // Archive: a safe observation alcove introduces Hush without a surprise attack.
        RoomTitle("03 / THE ARCHIVE",new Vector3(0,3.25f,44.8f));
        MachineAt("chime","BREAK-TIME CHIME",new Vector3(-6.3f,0,35),"POWER & START TO CALL HUSH");
        Crate("Archive crate",new Vector3(-4,.61f,32.8f));
        Crate("Movable cover",new Vector3(3.8f,.61f,35));
        Shelf(new Vector3(-5.8f,0,39.3f));Shelf(new Vector3(5.9f,0,39.3f));
        Gate("Drawing cabinet",43.5f,FactoryGate.Rule.Plate,new Vector3(5.5f,.05f,41.3f),false).requireLesson=true;
        var drawing=Board("",new Vector3(2.6f,1.5f,44.9f),2.3f).AddComponent<StoryPoint>();drawing.name="Notice / AN OLD DRAWING";drawing.kind=StoryPoint.StoryKind.Drawing;
        DrawingSketch(new Vector3(2.6f,1.5f,44.9f),drawing.transform);
        Gate("Assembly entrance",46,FactoryGate.Rule.Drawing,Vector3.zero);
        Board("HUSH IS ON SAFETY DUTY\nWATCH HIS ROUTE. CRATES BLOCK SIGHT.\nSTART THE CHIME TO DRAW HIM AWAY.\nHE WILL NEVER HURT YOU.",new Vector3(6.4f,2.75f,32),3.4f);
        var ig=Ghost("Hush / safety inspector",new Vector3(0,0,35),true);var ins=ig.AddComponent<Inspector>();game.inspector=ins;
        ins.visual=ig.transform.Find("Ghost body");ins.eyes=new GameObject("Sight origin").transform;ins.eyes.SetParent(ig.transform);ins.eyes.localPosition=new Vector3(0,1.45f,.35f);
        ins.armLeft=ins.visual.Find("Left sleeve");ins.armRight=ins.visual.Find("Right sleeve");
        ins.route=new[]{new Vector3(-2.3f,0,35.5f),new Vector3(2.3f,0,35.5f),new Vector3(2.3f,0,40.5f),new Vector3(-2.3f,0,40.5f)};
        // Assembly: same core, a reversible gate layout, persistent intermediate output.
        RoomTitle("04 / A LIGHT FOR THE WAY HOME",new Vector3(0,3.25f,62.8f));
        MachineAt("etch","ENGRAVING TABLE",new Vector3(-5.4f,0,50.8f),"3 / FOLLOW THE OLD DRAWING");
        Crate("Assembly crate",new Vector3(4.8f,.61f,49));
        Gate("Assembly work gate",54,FactoryGate.Rule.Plate,new Vector3(-2f,.05f,51.5f),false);
        MachineAt("press","LANTERN PRESS",new Vector3(5.4f,0,57.5f),"4 / FINISH THE LANTERN");
        var lanternRoot=new GameObject("The last lantern");lanternRoot.transform.position=new Vector3(3.6f,1.1f,58.5f);var lp=lanternRoot.AddComponent<Pickup>();lp.kind=Pickup.Kind.Lantern;lp.displayName="the last lantern";lp.home=lanternRoot.transform.position;
        var finished=LanternModel(lanternRoot.transform.position,.7f,lanternRoot.transform);finished.portable=true;var finalCollider=lanternRoot.AddComponent<SphereCollider>();finalCollider.radius=.48f;
        Shape("Lantern base",PrimitiveType.Cylinder,new Vector3(3.6f,.45f,58.5f),new Vector3(.8f,.45f,.8f),brass);
        game.lantern=lp;lanternRoot.SetActive(false);
        Desk(new Vector3(0,0,60));var pip=Ghost("Pip / night foreman",new Vector3(0,0,60.5f),false);var talk=pip.AddComponent<StoryPoint>();talk.kind=StoryPoint.StoryKind.Pip;var talkCollider=pip.AddComponent<BoxCollider>();talkCollider.center=Vector3.up*1.2f;talkCollider.size=new Vector3(1.5f,2,1.2f);game.pipHelper=pip.GetComponent<GhostIdle>();game.pipHelper.role="pip";
        Board("DISPATCH / PIP\nONE LAST ORDER",new Vector3(-3.8f,1.8f,61),2.6f);
        Pumpkin(new Vector3(-6,.5f,60),.9f);
        var ending=game.gameObject.AddComponent<EndOfShift>();ending.pip=pip.transform;ending.button=buttonGhost;ending.hush=ig.transform;ending.exitDoor=exitDoor;ending.view=cam;
        Festival();
        PlayerSettings.bundleVersion="0.7";PlayerSettings.companyName="Willow Works";PlayerSettings.productName="Midnight Shift - The Last Lantern";
        PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=false;PlayerSettings.resizableWindow=true;PlayerSettings.allowFullscreenSwitch=false;
        var ps=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);var active=ps.FindProperty("activeInputHandler");if(active!=null){active.intValue=0;ps.ApplyModifiedPropertiesWithoutUndo();}
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),"Assets/Scenes/WillowWorks.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/WillowWorks.unity",true)};
        AssetDatabase.SaveAssets();Debug.Log("FACTORY_SCENE_READY");
    }
    static Material Mat(string name,Color color,float metal=0){string path="Assets/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find("Standard"));AssetDatabase.CreateAsset(m,path);}m.color=color;m.SetFloat("_Metallic",metal);m.SetFloat("_Glossiness",.25f);return m;}
    static GameObject Shape(string name,PrimitiveType type,Vector3 pos,Vector3 scale,Material mat,Transform parent=null,bool collision=true){var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent?parent:world);g.transform.position=pos;var inherited=g.transform.parent.lossyScale;g.transform.localScale=new Vector3(scale.x/inherited.x,scale.y/inherited.y,scale.z/inherited.z);g.GetComponent<Renderer>().sharedMaterial=mat;if(!collision)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
    static GameObject Box(string name,Vector3 pos,Vector3 size,Material m,Transform parent=null,bool collision=true)=>Shape(name,PrimitiveType.Cube,pos,size,m,parent,collision);
    static TextMesh Sign(string text,Vector3 pos,float size,Material mat,float yaw=0,Transform parent=null){
        var g=new GameObject(text.Split('\n')[0]);g.transform.SetParent(parent?parent:world);g.transform.position=pos;g.transform.rotation=Quaternion.Euler(0,yaw,0);var t=g.AddComponent<TextMesh>();t.text=text;t.font=font;t.fontSize=96;t.characterSize=size*.065f;t.anchor=TextAnchor.MiddleCenter;t.alignment=TextAlignment.Center;t.color=mat.color;g.GetComponent<MeshRenderer>().sharedMaterial=worldText;g.AddComponent<WorldLabel>();return t;
    }
    static GameObject Board(string text,Vector3 pos,float width){var g=Box("Notice / "+text.Split('\n')[0],pos,new Vector3(width,1.3f,.13f),dark);var textMesh=Sign(text,pos+Vector3.back*.08f,.61f,cream);var fit=textMesh.GetComponent<WorldLabel>();fit.maxWidth=width-.25f;fit.maxHeight=1.1f;return g;}
    static void RoomTitle(string s,Vector3 p){Box("Overhead sign",p,new Vector3(9,.65f,.2f),dark);var label=Sign(s,p+Vector3.back*.12f,.95f,cream);label.GetComponent<WorldLabel>().maxWidth=8.6f;label.GetComponent<WorldLabel>().maxHeight=.5f;}
    static void FloorAndShell(){
        Box("Foundation",new Vector3(0,-.2f,27),new Vector3(18,.4f,74),timber);
        for(int z=-9;z<64;z+=2)for(int x=-8;x<9;x+=2)Box("Floorboard",new Vector3(x,.008f,z),new Vector3(1.97f,.02f,1.97f),(x+z)%4==0?teal:dark,null,false);
        Box("Ceiling",new Vector3(0,4.95f,27),new Vector3(18,.15f,74),dark);
        Box("West wall",new Vector3(-9,2.5f,27),new Vector3(.35f,5,74),teal);Box("East wall",new Vector3(9,2.5f,27),new Vector3(.35f,5,74),teal);
        Box("Front wall",new Vector3(0,2.5f,-10),new Vector3(18,5,.3f),teal);foreach(int side in new[]{-1,1})Box("Back wall",new Vector3(side*5.4f,2.5f,64),new Vector3(7.2f,5,.3f),teal);
        Box("Exit header",new Vector3(0,4.2f,64),new Vector3(3.6f,1.6f,.3f),teal);
        exitDoor=Box("Clock-out door",new Vector3(0,1.65f,64),new Vector3(3.5f,3.3f,.22f),timber).transform;
        for(int z=-6;z<64;z+=7){
            Box("Ceiling beam",new Vector3(0,4.6f,z),new Vector3(18,.2f,.28f),timber);
            foreach(int side in new[]{-1,1}){
                Box("Wall pillar",new Vector3(side*8.7f,2.2f,z),new Vector3(.25f,4.4f,.35f),timber);
                Box("Blue window",new Vector3(side*8.79f,2.8f,z+2),new Vector3(.06f,1.65f,2.8f),dark,null,false);
                Box("Window cross",new Vector3(side*8.7f,2.8f,z+2),new Vector3(.08f,.07f,2.8f),brass,null,false);
                Box("Window cross",new Vector3(side*8.7f,2.8f,z+2),new Vector3(.08f,1.65f,.07f),brass,null,false);
            }
            Shape("Pendant shade",PrimitiveType.Cylinder,new Vector3(0,3.9f,z),new Vector3(1.1f,.12f,1.1f),brass,null,false);
            Shape("Pendant glow",PrimitiveType.Sphere,new Vector3(0,3.7f,z),Vector3.one*.25f,glass,null,false);
            var l=new GameObject("Warm pendant light").AddComponent<Light>();l.transform.SetParent(world);l.transform.position=new Vector3(0,3.45f,z);l.type=LightType.Point;l.range=12;l.intensity=1.6f;l.color=new Color(1,.77f,.48f);
            for(int i=-3;i<=3;i++){var flag=Box("Halloween bunting",new Vector3(i*2,3.6f,z+2),new Vector3(.45f,.55f,.06f),i%2==0?orange:violet,null,false);flag.transform.rotation=Quaternion.Euler(0,0,i*6);}
        }
    }
    static void Festival(){
        Box("Parade square",new Vector3(0,-.2f,73),new Vector3(22,.4f,19),dark);
        Sign("TONIGHT, WE LEAVE TOGETHER",new Vector3(0,4.5f,77),1.2f,cream);
        for(int side=-1;side<=1;side+=2)for(int i=0;i<3;i++){
            Vector3 p=new Vector3(side*8,1.8f,67+i*5);
            Box("Village house",p,new Vector3(4,3.6f,3.6f),i%2==0?teal:violet);
            var roof=Box("Village roof",p+Vector3.up*2.1f,new Vector3(4.6f,.4f,4.2f),timber);roof.transform.rotation=Quaternion.Euler(0,0,side*8);
            Box("Lit village window",p+new Vector3(-side*2.03f,.3f,0),new Vector3(.06f,1.3f,1.7f),glass,null,false);
            Pumpkin(new Vector3(side*4.5f,.4f,67+i*4),.7f);
        }
        for(int i=-3;i<=3;i++){
            Vector3 pos=new Vector3(i*2,3.3f,73);
            Shape("Parade lantern",PrimitiveType.Sphere,pos,Vector3.one*.5f,i%2==0?orange:glass,null,false);
            var l=new GameObject("Festival light").AddComponent<Light>();l.transform.SetParent(world);l.transform.position=pos;l.type=LightType.Point;l.range=8;l.intensity=1.8f;l.color=new Color(1,.64f,.3f);
        }
    }
    static FactoryGate Gate(string name,float z,FactoryGate.Rule rule,Vector3 platePos,bool full=true){
        var root=new GameObject(name);root.transform.SetParent(world);var g=root.AddComponent<FactoryGate>();g.rule=rule;
        // Wall spans the room, so progression cannot be bypassed by walking around the door.
        foreach(int side in new[]{-1,1})Box("Divider",new Vector3(side*5.3f,1.8f,z),new Vector3(7.4f,3.6f,.3f),full?teal:timber,root.transform);
        Box("Door header",new Vector3(0,3.7f,z),new Vector3(3.2f,.4f,.5f),brass,root.transform);
        var panel=Box("Sliding gate",new Vector3(0,1.65f,z),new Vector3(3.1f,3.3f,.23f),dark,root.transform);g.panel=panel.transform;g.closedPosition=panel.transform.position;
        for(int i=-1;i<=1;i++)Box("Gate brass rib",new Vector3(i*.8f,1.65f,z-.14f),new Vector3(.07f,3.2f,.04f),brass,panel.transform,false);
        g.label=Sign(rule==FactoryGate.Rule.Plate?"HOLD PLATE TO OPEN":"WORK IN PROGRESS",new Vector3(0,3.15f,z-.2f),.57f,cream);
        if(rule==FactoryGate.Rule.Plate||rule==FactoryGate.Rule.PowerOrPlate){var p=Box("Striped pressure plate",platePos,new Vector3(1.9f,.1f,1.9f),brass);g.plate=p.transform;for(int i=-2;i<=2;i++)Box("Plate stripe",platePos+new Vector3(i*.32f,.06f,0),new Vector3(.12f,.015f,1.8f),dark,null,false);}
        return g;
    }
    static Pickup Crate(string name,Vector3 pos){pos.y=.835f;var root=Box(name,pos,new Vector3(1.2f,1.65f,1.2f),timber);var p=root.AddComponent<Pickup>();p.kind=Pickup.Kind.Crate;p.displayName=name;p.home=pos;
        for(int i=-1;i<=1;i+=2){Box("Crate band",pos+new Vector3(i*.4f,0,-.61f),new Vector3(.09f,1.2f,.03f),brass,root.transform,false);Box("Crate band",pos+new Vector3(0,i*.4f,-.62f),new Vector3(1.2f,.09f,.03f),brass,root.transform,false);}return p;}
    static Pickup Core(Vector3 pos){var root=Shape("Shared power core",PrimitiveType.Capsule,pos,new Vector3(.42f,.42f,.42f),glass);var p=root.AddComponent<Pickup>();p.kind=Pickup.Kind.Core;p.displayName="power core";p.home=pos;return p;}
    static GridRelay Relay(Vector3 pos){
        var root=new GameObject("WORKSHOP RELAY");root.transform.SetParent(world);root.transform.position=pos;
        Box("Relay cabinet",pos+Vector3.up*.7f,new Vector3(1.5f,1.4f,1.1f),violet,root.transform);
        var r=root.AddComponent<GridRelay>();var sock=new GameObject("Relay socket").transform;sock.SetParent(root.transform);sock.position=pos+Vector3.up*1.7f;r.socket=sock;
        Shape("Relay rim",PrimitiveType.Cylinder,pos+Vector3.up*1.45f,new Vector3(.7f,.05f,.7f),brass,root.transform,false);
        r.label=Sign("NEEDS CORE / PLATE OVERRIDE",pos+new Vector3(0,.9f,-.57f),.36f,cream);
        Sign("WORKSHOP RELAY",pos+new Vector3(0,.6f,-.57f),.43f,cream);return r;
    }
    static Machine MachineAt(string id,string name,Vector3 pos,string instruction){
        var root=Box(name,pos+Vector3.up*.6f,new Vector3(2.5f,1.2f,1.5f),teal);var m=root.AddComponent<Machine>();m.machineId=id;m.displayName=name;
        Box("Brass worktop",pos+Vector3.up*1.22f,new Vector3(2.65f,.12f,1.65f),brass,root.transform);
        var sock=new GameObject("Core socket").transform;sock.SetParent(root.transform);sock.position=pos+new Vector3(.75f,1.6f,0);m.corePoint=sock;
        Shape("Socket rim",PrimitiveType.Cylinder,pos+new Vector3(.75f,1.32f,0),new Vector3(.65f,.05f,.65f),dark,root.transform,false);
        m.indicator=Shape("Status lamp",PrimitiveType.Sphere,pos+new Vector3(-.85f,1.48f,-.25f),Vector3.one*.22f,cream,root.transform,false).GetComponent<Renderer>();
        var wheel=Shape("Work spindle",PrimitiveType.Cylinder,pos+new Vector3(-.2f,1.52f,.45f),new Vector3(.38f,.18f,.38f),orange,root.transform,false);m.spinner=wheel.transform;
        Box("Machine label",pos+new Vector3(0,.8f,-.77f),new Vector3(2.1f,.5f,.04f),dark,root.transform,false);
        var nameplate=Sign(name,pos+new Vector3(0,.85f,-.8f),.57f,cream);nameplate.GetComponent<WorldLabel>().maxWidth=2.1f;
        m.status=Sign("INSERT CORE",pos+new Vector3(0,.55f,-.8f),.44f,cream);m.status.GetComponent<WorldLabel>().maxWidth=2.25f;
        if(id!="chime"){
            Shape("Workpiece resting tray",PrimitiveType.Cylinder,pos+new Vector3(-.5f,1.35f,-.2f),new Vector3(.72f,.07f,.72f),dark,root.transform,false);
            var output=LanternModel(pos+new Vector3(-.5f,1.65f,-.2f),.58f,root.transform);output.station=m;
            output.caption=Sign("AWAITING CRAFT",pos+new Vector3(0,1.08f,-.84f),.27f,cream);
        }
        Board(instruction,pos+new Vector3(0,2.6f,.6f),3);return m;
    }
    static void DrawingSketch(Vector3 centre,Transform parent){
        var layer=new GameObject("Childhood sketch").transform;layer.SetParent(parent);parent=layer;
        Box("Old drawing paper",centre+Vector3.back*.08f,new Vector3(2.08f,1.13f,.02f),cream,parent,false);
        Sign("AN OLD DRAWING",centre+new Vector3(0,.46f,-.105f),.38f,ink,0,parent);
        Sign("E TO TURN IT OVER",centre+new Vector3(0,-.47f,-.105f),.32f,ink,0,parent);
        Vector3 pumpkin=centre+new Vector3(-.34f,-.02f,-.11f);
        for(int i=0;i<16;i++){
            float a=i*Mathf.PI/8,b=(i+1)*Mathf.PI/8;
            SketchStroke(pumpkin+new Vector3(Mathf.Cos(a)*.32f,Mathf.Sin(a)*.25f,0),pumpkin+new Vector3(Mathf.Cos(b)*.32f,Mathf.Sin(b)*.25f,0),orange,parent);
        }
        SketchStroke(pumpkin+new Vector3(-.1f,.23f,0),pumpkin+new Vector3(-.1f,.37f,0),violet,parent);
        SketchStroke(pumpkin+new Vector3(-.1f,.37f,0),pumpkin+new Vector3(.1f,.34f,0),violet,parent);
        SketchStroke(pumpkin+new Vector3(.1f,.34f,0),pumpkin+new Vector3(.1f,.24f,0),violet,parent);
        foreach(int side in new[]{-1,1})Box("Child's ink eye",pumpkin+new Vector3(side*.12f,.05f,-.01f),new Vector3(.045f,.055f,.012f),ink,parent,false);
        SketchStroke(pumpkin+new Vector3(-.12f,-.09f,0),pumpkin+new Vector3(.12f,-.11f,0),ink,parent);
        Vector3 ghost=centre+new Vector3(.44f,-.01f,-.11f);
        Vector3[] outline={new Vector3(-.18f,-.22f,0),new Vector3(-.18f,.12f,0),new Vector3(-.1f,.23f,0),new Vector3(.1f,.23f,0),new Vector3(.18f,.12f,0),new Vector3(.18f,-.22f,0),new Vector3(.07f,-.15f,0),new Vector3(0,-.23f,0),new Vector3(-.07f,-.15f,0),new Vector3(-.18f,-.22f,0)};
        for(int i=1;i<outline.Length;i++)SketchStroke(ghost+outline[i-1],ghost+outline[i],violet,parent);
        foreach(int side in new[]{-1,1})Box("Child's ghost eye",ghost+new Vector3(side*.065f,.04f,-.01f),new Vector3(.035f,.05f,.012f),ink,parent,false);
        SketchStroke(ghost+new Vector3(-.18f,-.04f,0),pumpkin+new Vector3(.3f,.04f,0),violet,parent);
    }
    static void SketchStroke(Vector3 a,Vector3 b,Material inkMaterial,Transform parent){
        var mark=Box("Crayon stroke",(a+b)/2,new Vector3(.025f,Vector3.Distance(a,b),.014f),inkMaterial,parent,false);mark.transform.rotation=Quaternion.FromToRotation(Vector3.up,b-a);
    }
    static LanternDisplay LanternModel(Vector3 pos,float size,Transform parent){
        var root=new GameObject("Lantern / visible craft");root.transform.SetParent(parent);root.transform.position=pos;
        var display=root.AddComponent<LanternDisplay>();display.halfHeight=size*.4f;
        var model=new GameObject("Craft stages").transform;model.SetParent(root.transform,false);display.model=model;
        for(int i=0;i<8;i++){float a=i*Mathf.PI/4;Shape("Ceramic pumpkin rib",PrimitiveType.Sphere,pos+new Vector3(Mathf.Cos(a)*size*.22f,0,Mathf.Sin(a)*size*.22f),new Vector3(size*.7f,size*.8f,size*.7f),orange,model,false);}
        Shape("Brass lid",PrimitiveType.Cylinder,pos+Vector3.up*size*.43f,new Vector3(size*.43f,size*.045f,size*.43f),brass,model,false);
        foreach(int side in new[]{-1,1})Box("Carry handle",pos+new Vector3(side*size*.25f,size*.7f,0),new Vector3(size*.055f,size*.5f,size*.06f),brass,model,false);
        Box("Carry handle bridge",pos+Vector3.up*size*.94f,new Vector3(size*.55f,size*.055f,size*.06f),brass,model,false);
        var ribbon=new GameObject("Plum ribbon and bow").transform;ribbon.SetParent(model,false);display.ribbon=ribbon;
        Box("Ribbon sash",pos+new Vector3(0,-size*.12f,-size*.5f),new Vector3(size*.7f,size*.12f,size*.055f),violet,ribbon,false);
        foreach(int side in new[]{-1,1}){var bow=Shape("Ribbon loop",PrimitiveType.Sphere,pos+new Vector3(side*size*.13f,-size*.12f,-size*.55f),new Vector3(size*.23f,size*.17f,size*.07f),violet,ribbon,false);bow.transform.rotation=Quaternion.Euler(0,0,side*25);}
        var inscription=new GameObject("Little engraved promise").transform;inscription.SetParent(model,false);display.inscription=inscription;
        Box("Ivory promise tag",pos+new Vector3(size*.33f,-size*.13f,-size*.43f),new Vector3(size*.23f,size*.3f,size*.035f),cream,inscription,false);
        for(int i=0;i<3;i++)Box("Handwritten line",pos+new Vector3(size*.33f,(-.06f-i*.055f)*size,-size*.46f),new Vector3(size*(i==2?.1f:.17f),size*.018f,size*.012f),ink,inscription,false);
        var glow=new GameObject("Lit eyes and smile").transform;glow.SetParent(model,false);display.glow=glow;
        foreach(int side in new[]{-1,1})Shape("Unlit eye cutout",PrimitiveType.Sphere,pos+new Vector3(side*size*.18f,size*.12f,-size*.47f),new Vector3(size*.15f,size*.17f,size*.05f),dark,model,false);
        foreach(int side in new[]{-1,1})Shape("Lit lantern eye",PrimitiveType.Sphere,pos+new Vector3(side*size*.18f,size*.12f,-size*.505f),new Vector3(size*.12f,size*.14f,size*.04f),glass,glow,false);
        Box("Lantern smile",pos+new Vector3(0,-size*.015f,-size*.555f),new Vector3(size*.19f,size*.045f,size*.025f),glass,glow,false);
        var light=new GameObject("Lantern warmth").AddComponent<Light>();light.transform.SetParent(model,false);light.type=LightType.Point;light.color=new Color(1,.64f,.28f);light.range=3;light.intensity=.65f;display.warmth=light;
        return display;
    }
    static void Desk(Vector3 pos){Box("Desk top",pos+Vector3.up*.9f,new Vector3(2.8f,.18f,1.1f),timber);foreach(int a in new[]{-1,1})foreach(int b in new[]{-1,1})Box("Desk leg",pos+new Vector3(a*1.2f,.4f,b*.4f),new Vector3(.12f,.8f,.12f),brass);}
    static void Conveyor(Vector3 pos,float length){Box("Conveyor base",pos,new Vector3(length,.4f,1.6f),brass);for(float x=-length/2+.3f;x<length/2;x+=.42f){var r=Shape("Conveyor roller",PrimitiveType.Cylinder,pos+new Vector3(x,.28f,0),new Vector3(.2f,.7f,.2f),dark);r.transform.rotation=Quaternion.Euler(90,0,0);}}
    static void Shelf(Vector3 p){
        var root=new GameObject("Solid archive shelf");root.transform.SetParent(world);root.transform.position=p;
        var collider=root.AddComponent<BoxCollider>();collider.center=Vector3.up*1.2f;collider.size=new Vector3(3,2.4f,1.4f);
        for(int i=0;i<3;i++)Box("Archive shelf",p+Vector3.up*(.4f+i*.8f),new Vector3(3,.12f,1.4f),timber,root.transform,false);
        for(int i=-1;i<=1;i+=2)Box("Shelf post",p+new Vector3(i*1.4f,1.2f,0),new Vector3(.15f,2.4f,1.4f),brass,root.transform,false);
        for(int i=-1;i<=1;i++)Box("Stored toy box",p+new Vector3(i*.9f,.78f,0),new Vector3(.7f,.6f,1),i==0?orange:violet,root.transform,false);
    }
    static GameObject Pumpkin(Vector3 pos,float size,Transform parent=null){var g=new GameObject("Pumpkin lantern");g.transform.SetParent(parent?parent:world);g.transform.position=pos;for(int i=0;i<8;i++){float a=i*Mathf.PI/4;Shape("Pumpkin lobe",PrimitiveType.Sphere,pos+new Vector3(Mathf.Cos(a)*size*.23f,0,Mathf.Sin(a)*size*.23f),new Vector3(size*.7f,size*.8f,size*.7f),orange,g.transform,false);}Shape("Stem",PrimitiveType.Cylinder,pos+Vector3.up*size*.5f,new Vector3(size*.13f,size*.16f,size*.13f),timber,g.transform,false);for(int i=-1;i<=1;i+=2)Shape("Warm lantern eye",PrimitiveType.Sphere,pos+new Vector3(i*size*.18f,size*.1f,-size*.49f),Vector3.one*size*.14f,glass,g.transform,false);var col=g.AddComponent<SphereCollider>();col.radius=size*.55f;return g;}
    static GameObject Ghost(string name,Vector3 pos,bool inspector){
        var root=new GameObject(name);root.transform.SetParent(world);root.transform.position=pos;
        var body=new GameObject("Ghost body").transform;body.SetParent(root.transform);body.position=pos+Vector3.up*1.1f;
        Shape("Soft ghost",PrimitiveType.Sphere,body.position,new Vector3(1.1f,1.4f,.85f),inspector?cream:mint,body,false);
        for(int i=-1;i<=1;i++)Shape("Scalloped hem",PrimitiveType.Sphere,body.position+new Vector3(i*.3f,-.5f,0),new Vector3(.4f,.45f,.65f),inspector?cream:mint,body,false);
        for(int i=-1;i<=1;i+=2)Shape("Eye",PrimitiveType.Sphere,body.position+new Vector3(i*.19f,.18f,-.4f),new Vector3(.12f,.18f,.06f),ink,body,false);
        Shape("Work cap",PrimitiveType.Cylinder,body.position+Vector3.up*.65f,new Vector3(1,.12f,.8f),inspector?orange:violet,body,false);
        Shape("Left sleeve",PrimitiveType.Capsule,body.position+new Vector3(-.6f,0,0),new Vector3(.25f,.35f,.3f),mint,body,false);
        Shape("Right sleeve",PrimitiveType.Capsule,body.position+new Vector3(.6f,0,0),new Vector3(.25f,.35f,.3f),mint,body,false);
        if(inspector)root.transform.rotation=Quaternion.Euler(0,180,0);
        else root.AddComponent<GhostIdle>();return root;
    }
    [MenuItem("Willow Works/Build Mac playable")]
    public static void BuildMac(){
        if(!File.Exists("Assets/Scenes/WillowWorks.unity"))CreateScene();Directory.CreateDirectory("Builds/Mac");
        var arch=typeof(UnityEditor.OSXStandalone.UserBuildSettings).GetProperty("architecture");arch.SetValue(null,Enum.Parse(arch.PropertyType,"x64ARM64"));
        var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/WillowWorks.unity"},locationPathName="Builds/Mac/Midnight Shift.app",target=BuildTarget.StandaloneOSX,options=BuildOptions.None});
        if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Mac build failed: "+report.summary.result);
        Debug.Log("MAC_BUILD_READY / "+report.summary.totalSize);
    }
    [MenuItem("Willow Works/Build Windows playable (module required)")]
    public static void BuildWindows(){BuildOther(BuildTarget.StandaloneWindows64,"Windows","Midnight Shift.exe");}
    [MenuItem("Willow Works/Build Linux playable (module required)")]
    public static void BuildLinux(){BuildOther(BuildTarget.StandaloneLinux64,"Linux","Midnight Shift.x86_64");}
    static void BuildOther(BuildTarget target,string folder,string filename){
        if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone,target))throw new Exception("Install the "+folder+" Build Support module for this Editor in Unity Hub first.");
        Directory.CreateDirectory("Builds/"+folder);
        var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/WillowWorks.unity"},locationPathName="Builds/"+folder+"/"+filename,target=target,options=BuildOptions.None});
        if(r.summary.result!=BuildResult.Succeeded)throw new Exception(folder+" build failed: "+r.summary.result);
    }

}
