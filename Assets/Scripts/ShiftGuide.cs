using System;
using UnityEngine;
namespace LastLantern {
// One immediate action at a time. Progress follows actual world state, not a timed slideshow.
public class ShiftGuide : MonoBehaviour {
    public struct Task {
        public string title, action, reason, key, targetName; public Vector3 target; public bool hasTarget; public float routeDistance;
    }
    public bool looked,walked; Vector3 start; float lookTime; FactoryGame g;
    FactoryGate foyer,archive,assembly; Pickup foyerCrate,overrideCrate,archiveCrate,assemblyCrate;
    Machine mould,ribbon,chime,etch,press;
    void Start(){Ensure();start=g.player.transform.position;}
    void Ensure(){if(g)return;g=GetComponent<FactoryGame>();foreach(var gate in FindObjectsByType<FactoryGate>(FindObjectsSortMode.None)){if(gate.name=="Foyer gate")foyer=gate;else if(gate.name=="Drawing cabinet")archive=gate;else if(gate.name=="Assembly work gate")assembly=gate;}
        foreach(var p in FindObjectsByType<Pickup>(FindObjectsInactive.Include,FindObjectsSortMode.None)){if(p.name=="Foyer crate")foyerCrate=p;else if(p.name=="Workshop override crate")overrideCrate=p;else if(p.name=="Archive crate")archiveCrate=p;else if(p.name=="Assembly crate")assemblyCrate=p;}
        foreach(var m in FindObjectsByType<Machine>(FindObjectsSortMode.None)){switch(m.machineId){case "mould":mould=m;break;case "ribbon":ribbon=m;break;case "chime":chime=m;break;case "etch":etch=m;break;case "press":press=m;break;}}
    }
    void Update(){Ensure();if(!g.started||g.Frozen)return;
        if(Input.GetKey(KeyCode.LeftArrow)||Input.GetKey(KeyCode.RightArrow)||Input.GetKey(KeyCode.UpArrow)||Input.GetKey(KeyCode.DownArrow)||(g.player.MouseLookHeld&&(Mathf.Abs(Input.GetAxisRaw("Mouse X"))+Mathf.Abs(Input.GetAxisRaw("Mouse Y"))>.03f)))lookTime+=Time.unscaledDeltaTime;
        if(lookTime>.25f)looked=true;
        if(Vector3.Distance(g.player.transform.position,foyerCrate.transform.position)<3.3f||g.player.held==foyerCrate)walked=true;
    }
    Task Make(string zh,string en,string azh,string aen,string rzh,string ren,string key,Vector3 target,string nzh,string nen){return new Task{title=UiCopy.Pick(zh,en),action=UiCopy.Pick(azh,aen),reason=UiCopy.Pick(rzh,ren),key=key=="MOUSE"?(g.player.keyboardLook?"ARROWS":g.player.pointerMode?"RIGHT-DRAG":"MOUSE"):key,target=target,targetName=UiCopy.Pick(nzh,nen),hasTarget=true};}
    Task CrateTask(Pickup crate,FactoryGate gate,string chapter){
        if(gate.CrateHoldsPlate)return Make(chapter,chapter,"穿过打开的门","Walk through the open door","让箱子留在板上，门就会一直开着。","Leave the crate on the plate to keep the door open.","W A S D",gate.closedPosition+Vector3.forward*2,"门的中央","Centre doorway");
        if(g.player.held==crate){
            Vector3 target=gate.plate.position;string detail=UiCopy.Pick("绿色箱子轮廓在条纹木板上时，按 Q 键放置。","When the green crate outline is on the striped plate, press the Q key.");
            bool ready=g.player.ReadyOnPlate(gate);
            if(!g.player.Placement(out _,out var why))detail=why;
            var t=Make(chapter,chapter,ready?"按 Q 键，把箱子放到这块木板上":"把箱子带到发光的条纹木板前",ready?"Press Q to place the crate on this plate":"Bring the crate to the glowing striped plate",detail,detail,ready?"Q":"W A S D",target,"条纹压力板","Striped pressure plate");return t;
        }
        if(g.player.held==g.sharedCore){var parking=gate==archive?chime:gate==assembly?etch:null;if(parking&&!parking.core)return Make(chapter,chapter,"先把核心留在操作台","Park the core in the console","机器可以安全保管核心；打开门后再取回来。","The machine holds the core safely. Retrieve it after opening the gate.","E",parking.corePoint.position,parking.displayName,parking.displayName);}
        if(g.player.held)return Make(chapter,chapter,"先腾出双手","Free your hands first","按 Q 把手中的物品放到空地，再拿箱子。","Press Q on clear floor, then pick up the crate.","Q",crate.transform.position,"木箱","Crate");
        return Make(chapter,chapter,"走近木箱，按 E 拿起","Walk to the crate and press E","让屏幕中央的小圆点对准箱子。出现“拾取”提示再按 E。","Aim the centre dot at the crate. Press E when the pickup prompt appears.","E",crate.transform.position,"木箱","Crate");
    }
    Task MachineTask(Machine m,string zh,string en){
        if(m.running){
            if(m.machineId=="etch"){var preparing=CrateTask(assemblyCrate,assembly,UiCopy.Pick("04 / 提前准备门禁","04 / Prepare the gate"));preparing.reason=UiCopy.Pick("核心留在雕刻机里。趁它工作，把箱子搬到条纹板上；留意 Hush。","Leave the core in the Engraver. Prepare the crate gate while it works; watch Hush.");return preparing;}
            var t=Make(zh,en,"正在制作，核心先留在机器里","Crafting: leave the core in place","完成后待办会自动更新。你可以观察作品。","The checklist updates when finished. You can watch the workpiece.","",m.transform.position,"制作中的机器","Machine at work");return t;}
        if(m.core){return Make(zh,en,"靠近操作台，按 E 继续","Press E near the console to continue","旋转线路，接通所有出口和每个星标，再启动机器。","Rotate tiles. Power every OUT and every star, then start the machine.","E",m.corePoint.position,m.displayName,m.displayName);}
        if(g.player.held==g.sharedCore)return Make(zh,en,"把金色核心插进这台机器","Insert the gold core into this machine",m.machineId=="chime"?"插入核心给铃供电，然后按 E 响铃。":"靠近操作台按 E，插入核心并打开线路板。",m.machineId=="chime"?UiCopy.Translate("Insert the core to power the chime. Then press E to ring it."):"Press E near the console to insert the core and open the circuit.","E",m.corePoint.position,m.displayName,m.displayName);
        Vector3 source=g.sharedCore.transform.position;
        return Make(zh,en,"先取回同一枚金色核心","Retrieve the same gold core",UiCopy.Pick("核心位置：", "Core location: ")+UiCopy.Translate(g.CoreLocation),UiCopy.Pick("核心位置：", "Core location: ")+UiCopy.Translate(g.CoreLocation),"E",source,"金色核心","Gold power core");
    }
    public Task Current {
        get {Ensure();var p=g.player;
            if(!g.BasicsComplete&&p.transform.position.z<12&&!g.shell){
                if(!looked&&!p.held&&!foyer.CrateHoldsPlate)return Make("01 / 我来带你熟悉一下","01 / Let me show you around","先看一看左前方的木箱","Look toward the crate on your left",g.player.pointerMode?"按住右键拖动来观察，方向键也可以。":g.player.keyboardLook?"方向键转头，WASD 走动。":"移动鼠标转头，WASD 走动。Esc 释放鼠标并打开菜单。",g.player.pointerMode?"Hold right mouse and drag to look. Arrow keys also turn.":g.player.keyboardLook?"Arrow keys turn. WASD walks.":"Move the mouse to turn. WASD walks. Esc frees your pointer and opens the menu.","MOUSE",foyerCrate.transform.position,"先看这里","Look here first");
                if(!walked&&!p.held&&!foyer.CrateHoldsPlate)return Make("01 / 试着走几步","01 / Take a few steps","按 W 向木箱走近一点","Press W to walk toward the crate","W 前进 · S 后退 · A 左移 · D 右移。","W forward · S back · A left · D right.","W A S D",foyerCrate.transform.position,"木箱","Crate");
                return CrateTask(foyerCrate,foyer,UiCopy.Pick("01 / 打开第一扇门","01 / Open your first door"));
            }
            if(!g.shell){
                var relay=g.workshopRelay;
                if(!relay.OverrideHeld){
                    if(relay.core)return CrateTask(overrideCrate,relay.gate,UiCopy.Pick("02 / 让门保持打开","02 / Keep the door open"));
                    if(p.held==g.sharedCore)return Make("02 / 给门供电","02 / Power the gate","将核心插入左边的紫色柜子","Insert the core into the purple cabinet","对准紫色柜子的正面，按 E。先只负责把门打开。","Aim at the purple cabinet and press E. First, just open the gate.","E",relay.transform.position,"紫色供电柜","Purple gate relay");
                    return Make("02 / 拾取金色核心","02 / Find the gold core","走近金色核心，按 E 拿起","Walk to the gold core and press E","它像一颗发光的金色胶囊，就在门前地上。","Look for a glowing gold capsule on the floor before the gate.","E",g.sharedCore.transform.position,"金色核心","Gold core");
                }
                if(relay.core)return Make("02 / 箱子接替了电力","02 / The crate holds the door","回到紫色柜子，按 E 取回核心","Return to the purple relay and remove the core","箱子让门保持打开。现在可以把电力借给别的机器。","The crate keeps access open. You can now lend the power to a machine.","E",relay.transform.position,"取回核心","Retrieve the core");
                return MachineTask(mould,"02 / 制作灯笼外壳","02 / Make the lantern shell");
            }
            if(!g.transferred)return MachineTask(ribbon,"02 / 给外壳系上丝带","02 / Add the ribbon");
            if(!g.blueprint){
                if(!g.inspectorLesson&&!chime.core&&p.held!=g.sharedCore&&g.sharedCore.transform.position.z<30)return MachineTask(chime,"03 / 带上可复用的核心","03 / Bring the reusable core");
                if(p.transform.position.z<30.5f)return Make("03 / 寻找旧图纸","03 / Find the old drawing","从中央门进入档案室","Enter the archive through the centre door","绕过传送带，从房间中央的门前进。","Walk around the conveyor and through the centre door.","W A S D",new Vector3(0,0,31.4f),"档案室入口","Archive entrance");
                if(!g.inspectorLesson){
                    if(!chime.core)return MachineTask(chime,"03 / 试一试休息铃","03 / Try the break-time chime");
                    return Make("03 / 叫巡逻员去听铃声","03 / Call Hush to the chime","对准休息铃，再按一次 E","Aim at the chime and press E again","Hush 只做安全检查，不会伤害你。铃声能把他叫过去。","Hush only checks safety. He cannot hurt you. The bell calls him over.","E",chime.transform.position,"休息铃","Break-time chime");
                }
                if(!archive.CrateHoldsPlate)return CrateTask(archiveCrate,archive,UiCopy.Pick("03 / 打开图纸柜","03 / Open drawing cabinet"));
                return Make("03 / 看看是谁的图纸","03 / Who drew this?","穿过中央门，对准旧图纸按 E","Go through the centre door and read the drawing","把箱子留在条纹板上。图纸不用拿走，读过就会记下。","Leave the crate on the plate. Reading saves the drawing in your notebook.","E",new Vector3(2.6f,1.5f,44.9f),"旧图纸","Old drawing");
            }
            if(!g.etched)return MachineTask(etch,"04 / 刻下旧日的承诺","04 / Engrave the old promise");
            if(!g.assembled){if(!assembly.CrateHoldsPlate)return CrateTask(assemblyCrate,assembly,UiCopy.Pick("04 / 打开装配区的门","04 / Open the assembly gate"));return MachineTask(press,"04 / 点亮最后一盏灯","04 / Light the last lantern");}
            if(p.held!=g.lantern)return Make("05 / 最后一次送达","05 / One last delivery","走近成品灯笼，按 E 拿起","Pick up the finished lantern with E","成品在机器旁边的小金色台座上。","It's on the small brass pedestal beside the Press.","E",g.lantern.transform.position,"成品灯笼","Finished lantern");
            return Make("05 / 一起下班吧","05 / Let's go home together","把灯笼交给桌后的 Pip","Give the lantern to Pip behind the desk","对准 Pip 按 E，完成你的最后一份订单。","Aim at Pip and press E to finish your last order.","E",g.pipHelper.transform.position+Vector3.up,"Pip / 交付","Pip / Deliver");
        }
    }
    public string Purpose {get{Ensure();
        if(g.player.held==g.sharedCore&&((g.inspectorLesson&&!g.blueprint&&!archive.CrateHoldsPlate)||(g.blueprint&&g.etched&&!assembly.CrateHoldsPlate)))return UiCopy.Translate("Carry one item at a time. Park the core to free your hands for the crate.");
        if(g.sharedCore&&g.player.held!=g.sharedCore&&Current.key=="E"&&Vector3.Distance(Current.target,g.sharedCore.transform.position)<.2f)return UiCopy.Translate("There is one core. Finished work stays finished without power.");
        if(!g.BasicsComplete&&g.player.transform.position.z<12&&!g.shell)return UiCopy.Translate("Leave the crate on the plate to keep the door open.");
        if(!g.shell){if(!g.workshopRelay.OverrideHeld)return UiCopy.Translate("A crate replaces gate power, freeing the core for crafting.");return UiCopy.Translate("Build the shell first; it becomes the lantern's body.");}
        if(!g.transferred)return UiCopy.Translate("The ribbon finishes the shell before we take it to assembly.");
        if(!g.blueprint){if(g.player.transform.position.z<30.5f)return UiCopy.Translate("The old drawing explains who this lantern is for.");if(!g.inspectorLesson)return UiCopy.Translate("The bell calls Hush away so you can move the crate.");if(!archive.CrateHoldsPlate)return UiCopy.Translate("Leave the crate on the plate to keep the door open.");return UiCopy.Translate("The old drawing explains who this lantern is for.");}
        if(!g.etched){if(g.sharedCore.socket==chime&&g.player.held!=g.sharedCore)return UiCopy.Translate("There is one core. Finished work stays finished without power.");if(etch.running)return UiCopy.Translate("The crate holds access open while the core powers the Press.");return UiCopy.Translate("Engrave the promise from your childhood drawing.");}
        if(!g.assembled)return UiCopy.Translate(!assembly.CrateHoldsPlate?"The crate holds access open while the core powers the Press.":"Join the finished parts and light the lantern.");
        return UiCopy.Translate("Give the helpers a light for their way home.");
    }}
    public bool IsCurrentCoreSocket(Interactable socket){if(!socket)return false;var t=Current;if(!t.hasTarget||t.key!="E")return false;Vector3 delta=t.target-g.player.InteractionPoint(socket);delta.y=0;return delta.magnitude<1.1f;}
    // Keep an already reachable action instead of routing to the far side of its furniture.
    public bool CanActHere(Task t){
        if(t.key!="E"||!t.hasTarget)return false;var intended=g.player.IntendedCoreSocket();if(intended)return g.player.CanReach(intended);
        foreach(var item in FindObjectsByType<Interactable>(FindObjectsSortMode.None)){
            if(item is Pickup pick&&pick.carried)continue;
            if(Vector3.Distance(t.target,g.player.InteractionPoint(item))>.75f&&Vector3.Distance(t.target,item.transform.position)>.75f)continue;
            if(g.player.CanReach(item))return true;
        }return false;
    }
    public Task Navigate(Task t){
        if(!t.hasTarget||string.IsNullOrEmpty(t.key)||CanActHere(t)||!g.navigation||t.key=="MOUSE"||t.key=="ARROWS"||t.key=="RIGHT-DRAG")return t;
        g.navigation.Refresh(t);
        if(g.player.IntendedCoreSocket()&&!g.player.NearbyCoreSocket()){t.key=g.player.CoreAccessKey;t.action=g.player.CoreAccessHint();if(t.key=="W A S D"&&g.navigation.Available)t.target=g.navigation.LookAhead(2.6f);return t;}
        if(g.navigation.Points.Count>1&&g.navigation.Distance>4.2f){
            var original=t.targetName;t.routeDistance=g.navigation.Distance;t.target=g.navigation.LookAhead(2.6f);t.key="W A S D";
            t.action=UiCopy.Pick("沿地面金色光路前进","Follow the gold trail");
            t.reason=UiCopy.Pick("前往：","Destination: ")+original;t.targetName=original;
        }
        return t;
    }
    public FactoryGate PlacementGate {get{Ensure();var held=g.player.held;if(held==foyerCrate)return foyer;if(held==overrideCrate)return g.workshopRelay.gate;if(held==archiveCrate)return archive;if(held==assemblyCrate)return assembly;return null;}}
    public FactoryGate CurrentPlateGoal {get{Ensure();if(g.ended)return null;
        if(!g.BasicsComplete&&!foyer.CrateHoldsPlate)return foyer;
        if(!g.shell&&g.workshopRelay.core&&!g.workshopRelay.OverrideHeld)return g.workshopRelay.gate;
        if(g.transferred&&!g.blueprint&&g.inspectorLesson&&!archive.CrateHoldsPlate)return archive;
        if(g.blueprint&&!g.assembled&&(etch.running||g.etched)&&!assembly.CrateHoldsPlate)return assembly;
        return null;
    }}
    public int Chapter=>!g||(!g.BasicsComplete&&g.player.transform.position.z<12)?0:!g.transferred?1:!g.blueprint?2:!g.assembled?3:4;
    public string ChapterTitle {get{switch(Chapter){case 0:return UiCopy.Pick("01 / 第一扇门","01 / FIRST STEPS");case 1:return UiCopy.Pick("02 / 包装区","02 / PACKAGING");case 2:return UiCopy.Pick("03 / 旧图纸","03 / THE ARCHIVE");case 3:return UiCopy.Pick("04 / 最终装配","04 / ASSEMBLY");default:return UiCopy.Pick("05 / 最后送达","05 / DELIVERY");}}}
    public string[] Checklist {get{Ensure();switch(Chapter){
        case 0:return new[]{Row(looked,"看看木箱","Look toward the crate"),Row(walked,"试着移动","Try walking"),Row(foyer.CrateHoldsPlate,"箱子压住条纹板","Leave crate on plate"),Row(g.player.transform.position.z>12,"穿过中央门","Walk through the door")};
        case 1:return new[]{Row(g.workshopRelay.OverrideHeld,"箱子保持门打开","Crate holds the gate"),Row(g.shell,"做好灯笼外壳","Make the shell"),Row(g.transferred,"系好丝带","Add the ribbon")};
        case 2:return new[]{Row(g.inspectorLesson,"试用休息铃","Try the chime"),Row(archive.CrateHoldsPlate,"箱子打开图纸柜","Open drawing cabinet"),Row(g.blueprint,"读一读旧图纸","Read the old drawing")};
        case 3:return new[]{Row(g.etched,"刻下承诺","Engrave the promise"),Row(assembly.CrateHoldsPlate,"箱子保持装配门打开","Keep assembly access"),Row(g.assembled,"点亮灯笼","Light the lantern")};
        default:return new[]{Row(g.player.held==g.lantern||g.ended,"拿起灯笼","Pick up the lantern"),Row(g.ended,"交给 Pip","Deliver to Pip")};
    }}}
    string Row(bool done,string zh,string en)=>(done?"[+] ":"[ ] ")+UiCopy.Pick(zh,en);
}
}
