using System;
using System.Collections.Generic;
using UnityEngine;
namespace LastLantern {
public static class UiTheme {
    public static readonly Color Ink=new Color(.10f,.19f,.20f),Cream=new Color(.98f,.94f,.84f),Gold=new Color(.98f,.74f,.35f),Teal=new Color(.10f,.25f,.27f),Muted=new Color(.70f,.79f,.75f);
    static Font body,display;static HashSet<string> overflow=new HashSet<string>();static Dictionary<string,GUIStyle> styles=new Dictionary<string,GUIStyle>();static int language=-1;
    public static void Setup(){if(!display)display=Resources.Load<Font>("Fonts/Fraunces");if(language!=UiCopy.Selected||!body){language=UiCopy.Selected;body=UiCopy.BodyFont(language);styles.Clear();overflow.Clear();}}
    public static GUIStyle Style(int size,Color color,bool bold=false,bool art=false,TextAnchor align=TextAnchor.UpperLeft){Setup();string key=size+"/"+color+"/"+bold+"/"+art+"/"+align;if(!styles.TryGetValue(key,out var s)){s=new GUIStyle(GUI.skin.label){font=(art?display:body),fontSize=size,fontStyle=bold?FontStyle.Bold:FontStyle.Normal,wordWrap=true,alignment=align,richText=false};s.normal.textColor=color;styles[key]=s;}return s;}
    public static void Label(Rect r,string text,int size=24,Color? color=null,bool bold=false,bool art=false,TextAnchor align=TextAnchor.UpperLeft){
        bool latin=true;foreach(char c in text)if(c>=0x2e80){latin=false;break;}
        var style=Style(size,color??Cream,bold,art&&latin,align);var content=new GUIContent(text);int minimum=size>=27?24:Mathf.Min(size,22);
        if(UiCopy.Selected!=0)while(size>minimum&&style.CalcHeight(content,r.width)>r.height+3)style=Style(--size,color??Cream,bold,art&&latin,align);
        if(align==TextAnchor.UpperLeft&&style.CalcHeight(content,r.width)>r.height+3&&overflow.Add(text+"/"+r))Debug.LogWarning("UI_LAYOUT_OVERFLOW / "+UiCopy.Codes[UiCopy.Selected]+" / "+text+" / "+r);
        GUI.Label(r,text,style);
    }
    public static void Fill(Rect r,Color color){var before=GUI.color;GUI.color=color;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=before;}
    public static void Panel(Rect r,Color? color=null){Fill(new Rect(r.x+4,r.y+5,r.width,r.height),new Color(0,0,0,.25f));Fill(r,color??Teal);}
    public static void BeginPointer(){UiPointer.Begin();}
    public static void EndPointer(){UiPointer.End();}
    public static bool Hover(Rect r){return UiPointer.Hover(r);}
    public static void Outline(Rect r,Color color){Fill(new Rect(r.x,r.y,r.width,3),color);Fill(new Rect(r.x,r.yMax-3,r.width,3),color);Fill(new Rect(r.x,r.y,3,r.height),color);Fill(new Rect(r.xMax-3,r.y,3,r.height),color);}
    public static bool Button(Rect r,string text,bool secondary=false,Font face=null){
        Setup();bool hot=Hover(r);var s=new GUIStyle(GUI.skin.button){font=face?face:body,fontSize=24,wordWrap=true,padding=new RectOffset(12,12,8,8)};
        while(s.fontSize>20&&s.CalcHeight(new GUIContent(text),r.width)>r.height+3)s.fontSize--;
        if(s.CalcHeight(new GUIContent(text),r.width)>r.height+3&&overflow.Add(text+"/"+r))Debug.LogWarning("UI_LAYOUT_OVERFLOW / button / "+text+" / "+r);
        s.normal.textColor=secondary?Cream:Ink;s.hover.textColor=s.normal.textColor;s.active.textColor=s.normal.textColor;
        s.normal.background=Texture2D.whiteTexture;s.hover.background=Texture2D.whiteTexture;s.active.background=Texture2D.whiteTexture;s.focused.background=Texture2D.whiteTexture;
        var old=GUI.backgroundColor;Color fill=secondary?new Color(.18f,.34f,.35f):Gold;if(hot)fill=Color.Lerp(fill,Cream,.22f);if(!GUI.enabled)fill=Color.Lerp(fill,Ink,.55f);
        GUI.backgroundColor=fill;Fill(r,fill);bool clicked=GUI.Button(r,text,s);GUI.backgroundColor=old;
        if(hot)Outline(r,Cream);return clicked;
    }
    public static void Key(Rect r,string key){Fill(new Rect(r.x,r.y+3,r.width,r.height),new Color(.02f,.07f,.08f));Fill(r,Cream);Label(r,key,23,Ink,true,false,TextAnchor.MiddleCenter);}
}
public class ShiftInterface {
    FactoryGame g;ShiftGuide guide;float w,h,scale;ShiftGuide.Task task;
    public bool LanguageOpen {get;private set;}public bool ComfortOpen;int languageFocus;bool hasCaptured;
    string dialogueText;string[] dialoguePages;int dialoguePage,dialogueLanguage;
    public void CloseLanguages(){LanguageOpen=false;}
    public void ResetDialoguePages(){dialoguePages=null;dialoguePage=0;}
    public bool AdvancePage(FactoryGame game){if(dialoguePages!=null&&dialogueLanguage==UiCopy.Selected&&dialogueText==UiCopy.Translate(game.line)&&dialoguePage+1<dialoguePages.Length){dialoguePage++;return true;}return false;}
    public void OpenLanguages(){languageFocus=UiCopy.Selected;LanguageOpen=true;}
    public void LanguageKeys(){if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.L)){LanguageOpen=false;return;}if(Input.GetKeyDown(KeyCode.LeftArrow))languageFocus=Mathf.Max(0,languageFocus-1);if(Input.GetKeyDown(KeyCode.RightArrow))languageFocus=Mathf.Min(UiCopy.Codes.Length-1,languageFocus+1);if(Input.GetKeyDown(KeyCode.UpArrow))languageFocus=Mathf.Max(0,languageFocus-2);if(Input.GetKeyDown(KeyCode.DownArrow))languageFocus=Mathf.Min(UiCopy.Codes.Length-1,languageFocus+2);if(Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.Space)){UiCopy.Language(languageFocus);LanguageOpen=false;}}
    string T(string zh,string en)=>UiCopy.Pick(zh,en);
    public void Draw(FactoryGame game){g=game;guide=g.guide;UiTheme.Setup();scale=Mathf.Min(Screen.width/1440f,Screen.height/900f);w=Screen.width/scale;h=Screen.height/scale;GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,Vector3.one*scale);
        if(ComfortOpen){Comfort();return;}
        if(g.photoMode){Photo();return;}
        if(g.mapOpen){g.factoryMap.Draw(g,w,h);return;}
        if(LanguageOpen){Languages();return;}
        if(!g.started){Title();return;}if(g.ended){Ending();return;}
        if(g.paused){Menu();return;}
        if(g.desktopHold){DesktopPause();return;}
        if(g.activeCircuit){g.DrawCircuit(w,h);MenuTab();return;}
        task=guide.Navigate(guide.Current);
        if(g.journal){Notebook();MenuTab();return;}
        if(g.AwaitingLook){LookStart();return;}
        if(g.player.LookCaptured)hasCaptured=true;
        if(g.Reading){if(UiTheme.Button(new Rect(28,22,180,50),T("菜单 [Esc]","Menu [Esc]"),true))g.TogglePause();Dialogue();return;}
        Calendar();MenuTab();g.factoryMap.Mini(g);
        Pointer();Coach();
        UiTheme.Label(new Rect(w/2-15,h/2-25,30,40),"+",27,UiTheme.Cream,false,false,TextAnchor.MiddleCenter);
        if(g.player.held&&!(g.player.focus is StoryPoint)&&g.player.IntendedCoreSocket()&&!g.player.NearbyCoreSocket()){UiTheme.Panel(new Rect(700,h-145,w-728,65));UiTheme.Label(new Rect(713,h-142,w-754,60),g.player.CoreAccessHint(),21,null,true,false,TextAnchor.MiddleCenter);}
        else if(g.player.focus){UiTheme.Panel(new Rect(700,h-135,w-728,44));UiTheme.Label(new Rect(713,h-135,w-754,44),UiCopy.Prompt(g.player),23,null,true,false,TextAnchor.MiddleCenter);}
        Controls();WorkProgress();
        if(g.ToastVisible){UiTheme.Panel(new Rect(28,337,Mathf.Min(710,w-430),125));UiTheme.Label(new Rect(46,352,Mathf.Min(674,w-466),100),UiCopy.Translate(g.toast),23);}
        if(g.inspector.SafePractice&&g.inspectorLesson&&g.player.transform.position.z>31&&g.player.transform.position.z<46){UiTheme.Label(new Rect(w/2-230,249,460,60),T("安全练习 / Hush 不会送你返回","SAFE PRACTICE / No send-backs"),22,UiTheme.Gold,true,false,TextAnchor.MiddleCenter);}
        if(g.inspector.Alertness>0){UiTheme.Fill(new Rect(w/2-155,249,310,12),UiTheme.Ink);UiTheme.Fill(new Rect(w/2-155,249,310*g.inspector.Alertness,12),UiTheme.Gold);UiTheme.Label(new Rect(w/2-200,267,400,62),T("Hush 正在注意你：绕到货架或放下的箱子后面。","Hush notices you. Move behind a shelf or a placed crate."),22,null,false,false,TextAnchor.MiddleCenter);}
        if(Array.IndexOf(Environment.GetCommandLineArgs(),"--agent-playtest")>=0){var p=g.player.transform.position;UiTheme.Label(new Rect(28,76,850,35),$"AGENT REVIEW / {p.x:F2}, {p.z:F2} / yaw {g.player.transform.eulerAngles.y:F1} pitch {g.player.Pitch:F1}",18);}
    }
    void MenuTab(){if(UiTheme.Button(new Rect(28,22,225,54),T("菜单 [Esc]","Menu [Esc]"),true))g.TogglePause();if(UiTheme.Button(new Rect(269,22,305,54),T("截图视图 [F2]","Screenshot view [F2]"),true))g.TogglePhoto();if(UiTheme.Button(new Rect(590,22,225,54),T("地图 [M]","Map [M]"),true))g.ToggleMap();if(!g.Modal&&UiTheme.Button(new Rect(835,22,200,54),T("平视 [R]","Level view [R]"),true))g.player.LevelView();}
    void DesktopPause(){
        UiTheme.Fill(new Rect(0,0,w,h),new Color(.015f,.04f,.05f,.7f));MenuTab();float x=w/2-460,y=h/2-180;UiTheme.Panel(new Rect(x,y,920,360));
        UiTheme.Label(new Rect(x+35,y+25,850,65),T("游戏已暂停","Game paused"),40,UiTheme.Gold,true,true);
        UiTheme.Label(new Rect(x+35,y+110,850,105),T("切换窗口或截图时，我们会暂停游戏。\n暂停期间，E 和 Q 不会操作物品。先按 Enter 继续。","Switching windows or using desktop shortcuts pauses the game.\nE and Q do nothing while paused. Press Enter to resume."),26);
        if(UiTheme.Button(new Rect(x+35,y+255,850,65),T("继续游戏 [Enter]","Resume game [Enter]")))g.ResumeDesktop();
    }
    void Photo(){if(UiTheme.Button(new Rect(28,22,460,54),T("返回游戏 [F2 / Esc]","Return to game [F2 / Esc]"),true))g.TogglePhoto();}
    void Title(){
        UiTheme.Panel(new Rect(48,50,775,h-100),new Color(.045f,.12f,.14f,.98f));
        UiTheme.Label(new Rect(90,82,680,40),"WILLOW WORKS  /  OCTOBER 31",25,UiTheme.Gold);
        UiTheme.Label(new Rect(85,148,690,155),"Midnight Shift\nThe Last Lantern",54,UiTheme.Cream,false,true);
        UiTheme.Label(new Rect(92,325,660,95),T("最后一份订单，三个等你的小幽灵。\n今晚，一起做一盏照亮回家路的灯。","One forgotten order. Three little helpers.\nTonight, make a light for their way home."),28);
        UiTheme.Label(new Rect(92,445,660,65),T("我会一步步带你熟悉工厂。无需先记住所有规则。","I'll show you the first steps. Follow the gold marker."),25,UiTheme.Muted);
        if(UiTheme.Button(new Rect(92,547,635,68),T("跟着 Pip 开始夜班  →","Begin the guided night shift  →")))g.Begin();
        if(UiTheme.Button(new Rect(92,638,463,58),T("语言","Language")+": "+UiCopy.Names[UiCopy.Selected]+"  [L]",true))OpenLanguages();
        if(UiTheme.Button(new Rect(576,638,151,58),T("退出","Quit"),true))g.Quit();
        UiTheme.Label(new Rect(92,724,640,45),T("无倒计时 · 无战斗 · 可随时暂停或退出","No countdown · No combat · Pause or quit anytime"),22,UiTheme.Muted);
        float x=Mathf.Max(860,w-510);UiTheme.Panel(new Rect(x,130,440,665),UiTheme.Cream);
        UiTheme.Label(new Rect(x+30,157,380,65),T("先认识这几个按键","Your first few controls"),30,UiTheme.Ink,true,true);
        ControlCard(x+30,255,"W A S D",T("走动","Walk"),T("W 前进，S 后退，A / D 左右移动","W forward, S back, A / D sideways"));
        ControlCard(x+30,375,"E",T("拿起 / 使用","Pick up / use"),T("先用屏幕中央的圆点对准物品","Aim the centre dot at the item first"));
        ControlCard(x+30,495,"Q",T("放下物品","Place"),T("绿色对准木板后按 Q","Green on the plate? Press Q."));
        UiTheme.Label(new Rect(x+30,635,385,70),T("移动鼠标：转头\nEsc：释放鼠标并打开菜单","Move mouse: look around\nEsc: free pointer / menu"),21,UiTheme.Ink);
        if(UiTheme.Button(new Rect(x+30,723,380,54),T("操作与舒适度","Controls & comfort")+" [C]",true))g.OpenComfort();
    }
    void LookStart(){
        MenuTab();float x=w/2-480;float y=hasCaptured?h/2-125:h/2-230;
        UiTheme.Panel(new Rect(x,y,960,hasCaptured?270:460),new Color(.04f,.12f,.14f,.98f));
        if(!hasCaptured){
            UiTheme.Label(new Rect(x+35,y+24,890,65),T("先试着转头看看","First, try looking around"),38,UiTheme.Gold,true,true);
            UiTheme.Label(new Rect(x+35,y+112,890,138),T("点击下方按钮，然后移动鼠标：左右转头，上下观察。不用按住鼠标键。\nWASD 走动，方向键也能转头。","Click below, then move your mouse to turn left, right, up or down. No button needs to be held.\nWASD walks. Arrow keys also turn."),27);
            UiTheme.Label(new Rect(x+35,y+264,890,80),T("Esc 立即释放鼠标并打开菜单。截图或切换窗口时也会释放鼠标。","Esc frees your pointer and opens the menu. Desktop shortcuts and switching windows also release it."),24,UiTheme.Muted);
        }else UiTheme.Label(new Rect(x+35,y+24,890,95),T("准备继续？移动鼠标转头，Esc 释放鼠标。","Ready to continue? Move the mouse to look. Esc frees your pointer."),27);
        if(UiTheme.Button(new Rect(x+35,y+(hasCaptured?130:365),890,64),T("点击开始转头  [Enter]","Click to look around  [Enter]"))){g.StartMouseLook();hasCaptured=g.player.LookCaptured;}
        if(UiTheme.Button(new Rect(w/2-240,h-95,480,58),T("操作与舒适度","Controls & comfort")+" [C]",true))g.OpenComfort();
    }
    void Comfort(){
        UiTheme.Fill(new Rect(0,0,w,h),new Color(.015f,.04f,.05f,.9f));float x=w/2-520;UiTheme.Panel(new Rect(x,35,1040,h-70));
        UiTheme.Label(new Rect(x+35,57,970,65),T("操作与舒适度","Controls & comfort"),40,UiTheme.Gold,true,true);
        UiTheme.Label(new Rect(x+35,139,970,85),T("默认用鼠标自由转头，Esc 释放鼠标。\n视角不晃动、不自动转向，也没有动态模糊。","Mouse look is the default. Esc frees your pointer.\nNo camera shake, automatic turning or motion blur."),25);
        g.sensitivity=Setting(x,249,T("左右灵敏度","Horizontal sensitivity"),g.sensitivity,.3f,4,g.sensitivity.ToString("0.0"));
        g.verticalSensitivity=Setting(x,319,T("上下灵敏度","Vertical sensitivity"),g.verticalSensitivity,.3f,4,g.verticalSensitivity.ToString("0.0"));
        float fov=Setting(x,389,T("垂直视野范围","Vertical field of view"),g.fieldOfView,55,90,Mathf.RoundToInt(g.fieldOfView)+"°");g.fieldOfView=fov;g.player.view.fieldOfView=fov;
        g.player.speed=Setting(x,459,T("走路速度","Walking speed"),g.player.speed,2,4.5f,g.player.speed.ToString("0.0"));
        string[] modes={T("鼠标自由转头","Mouse look"),T("自由鼠标：右键拖动","Free pointer: right-drag"),T("仅用方向键转头","Arrow keys only")};
        for(int i=0;i<3;i++)if(UiTheme.Button(new Rect(x+35+i*325,542,310,68),(g.player.LookMode==i?"✓ ":"")+modes[i],g.player.LookMode!=i))g.player.SetLookMode(i);
        if(UiTheme.Button(new Rect(x+35,631,470,60),g.player.handMotion?T("手部晃动：开","Hand motion: on"):T("手部晃动：关","Hand motion: off"),true))g.player.handMotion=!g.player.handMotion;
        if(UiTheme.Button(new Rect(x+535,631,470,60),T("恢复默认设置","Restore defaults"),true))g.player.ComfortDefaults();
        UiTheme.Label(new Rect(x+35,709,970,52),T("选择你感觉舒适的视野和速度。关闭时会保存。","Choose the view and speed that feel comfortable. Saved when closed."),22,UiTheme.Muted);
        if(UiTheme.Button(new Rect(x+35,h-110,970,58),T("← 返回","← Back"),true))g.CloseComfort();
    }
    float Setting(float x,float y,string label,float value,float min,float max,string shown){UiTheme.Label(new Rect(x+35,y-8,390,55),label,24);UiTheme.Hover(new Rect(x+445,y,450,35));value=GUI.HorizontalSlider(new Rect(x+445,y+8,450,35),value,min,max);UiTheme.Label(new Rect(x+920,y-8,85,55),shown,24,UiTheme.Gold,true,false,TextAnchor.MiddleCenter);return value;}
    void Languages(){
        UiTheme.Fill(new Rect(0,0,w,h),new Color(.015f,.04f,.05f,.88f));float x=w/2-480;UiTheme.Panel(new Rect(x,65,960,h-130));
        UiTheme.Label(new Rect(x+35,95,890,65),T("语言","Language"),42,null,false,true);
        UiTheme.Label(new Rect(x+35,177,890,70),T("默认英文，选择会保存。","English is the default. Your choice is saved."),25,UiTheme.Muted);
        for(int i=0;i<UiCopy.Codes.Length;i++){Rect r=new Rect(x+35+(i%2)*455,270+(i/2)*80,435,64);if(i==languageFocus){UiTheme.Fill(new Rect(r.x-3,r.y-3,r.width+6,r.height+6),UiTheme.Cream);}if(UiTheme.Button(r,UiCopy.Names[i],i!=UiCopy.Selected,UiCopy.BodyFont(i))){UiCopy.Language(i);LanguageOpen=false;}}
        UiTheme.Label(new Rect(x+35,622,890,65),T("方向键选择 · 回车确认 · Esc 返回","Arrows: choose   Enter: confirm   Esc: back"),23,UiTheme.Muted);
        if(UiTheme.Button(new Rect(x+35,h-185,890,60),T("返回 [Esc]","Back [Esc]"),true))LanguageOpen=false;
    }
    void ControlCard(float x,float y,string key,string title,string copy){UiTheme.Key(new Rect(x,y,key.Length>3?160:60,43),key);UiTheme.Label(new Rect(x+(key.Length>3?178:80),y,370-(key.Length>3?178:80),45),title,28,UiTheme.Ink,true);UiTheme.Label(new Rect(x,y+54,370,54),copy,22,UiTheme.Ink);}
    void Calendar(){float x=w-330;UiTheme.Panel(new Rect(x,94,302,210),UiTheme.Cream);UiTheme.Fill(new Rect(x,94,302,48),UiTheme.Teal);
        UiTheme.Label(new Rect(x+18,101,270,38),T("10 / 31    今晚的待办","10 / 31    TONIGHT"),22,null,true);
        UiTheme.Label(new Rect(x+18,151,266,76),guide.ChapterTitle,24,UiTheme.Ink,true,true);
        string[] rows=guide.Checklist;int done=0;foreach(var row in rows)if(row.StartsWith("[+]"))done++;
        if(UiTheme.Button(new Rect(x+18,238,266,48),T("待办 [J]","Tasks [J]")+"  "+done+" / "+rows.Length,true))g.ToggleJournal();
    }
    void Coach(){var socket=g.player.NearbyCoreSocket();if(socket&&guide.IsCurrentCoreSocket(socket)){task.key="E / Q";task.action=T("按 E 或 Q，把核心插入机器","Press E or Q to insert the core");task.reason=T("不用把核心放在桌上。靠近机器就能插入。","No need to drop it on the worktop. Insert it directly near the machine.");if(socket is Machine machine&&machine.Requirement!=""){task.action=UiCopy.Translate(machine.Requirement);task.reason=T("先完成这一步，核心会留在手中。","Finish this step first. The core stays in your hands.");}}else if(g.player.IntendedCoreSocket()&&!socket){task.key=g.player.CoreAccessKey;task.action=g.player.CoreAccessHint();}
        float height=246;float top=h-95-height;
        UiTheme.Panel(new Rect(28,top,565,height));
        UiTheme.Key(new Rect(46,top+15,task.key.Length>3?190:75,38),string.IsNullOrEmpty(task.key)?"...":UiCopy.Translate(task.key));
        UiTheme.Label(new Rect(46,top+65,528,96),task.action,25,UiTheme.Gold,true);
        UiTheme.Label(new Rect(46,top+164,528,72),T("为什么：","Why: ")+guide.Purpose,21,UiTheme.Muted);
        UiTheme.Label(new Rect(255,top+18,315,38),T("提示 [H]","Help [H]"),21,UiTheme.Muted,false,false,TextAnchor.MiddleRight);
    }
    void WorkProgress(){
        foreach(var machine in UnityEngine.Object.FindObjectsByType<Machine>(FindObjectsSortMode.None))if(machine.running&&machine.machineId!="chime"){
            float x=w-330,y=327;UiTheme.Panel(new Rect(x,y,302,142));
            UiTheme.Label(new Rect(x+18,y+10,190,36),T("制作中","CRAFTING"),22,UiTheme.Gold,true);
            UiTheme.Label(new Rect(x+210,y+10,74,36),Mathf.FloorToInt(machine.WorkFraction*100)+"%",22,null,true,false,TextAnchor.MiddleRight);
            UiTheme.Label(new Rect(x+18,y+48,266,54),UiCopy.Translate(machine.displayName),20);
            UiTheme.Fill(new Rect(x+18,y+111,266,12),UiTheme.Ink);UiTheme.Fill(new Rect(x+18,y+111,266*machine.WorkFraction,12),UiTheme.Gold);break;
        }
    }
    void Controls(){UiTheme.Key(new Rect(28,h-61,143,40),"W A S D");UiTheme.Label(new Rect(183,h-56,130,40),T("走动","Walk"),22);UiTheme.Key(new Rect(287,h-61,52,40),"E");UiTheme.Label(new Rect(350,h-56,145,40),(g.player.NearbyCoreSocket()?T("插入","Insert"):T("拿起 / 使用","Use")),22);UiTheme.Key(new Rect(503,h-61,52,40),"Q");UiTheme.Label(new Rect(567,h-56,130,40),(g.player.NearbyCoreSocket()?T("插入","Insert"):T("放下","Place")),22);
        UiTheme.Label(new Rect(720,h-77,360,75),g.player.pointerMode?T("右键拖动：转头","Right-drag: look"):g.player.keyboardLook?T("方向键：转头","Arrows: look"):T("移动鼠标：转头\nEsc：菜单 / 释放鼠标","Mouse: look around\nEsc: menu / free pointer"),20,UiTheme.Muted);
        if(g.player.held)UiTheme.Label(new Rect(w-360,h-91,330,70),T("手中：","Carrying: ")+UiCopy.Translate(g.player.held.displayName),23,UiTheme.Gold);}
    void Pointer(){if(!task.hasTarget||string.IsNullOrEmpty(task.key))return;
        float heading=FactoryPath.Heading(g.player.transform.forward,g.player.transform.position,task.target);
        string direction=Mathf.Abs(heading)>120?T("转身","Turn around"):heading < -22?T("← 向左","← Turn left"):heading>22?T("向右 →","Turn right →"):T("前方","Straight ahead");
        float x=w/2-210;UiTheme.Panel(new Rect(x,95,420,108));
        UiTheme.Label(new Rect(x+15,99,390,40),direction,27,UiTheme.Gold,true,false,TextAnchor.MiddleCenter);
        UiTheme.Label(new Rect(x+15,139,390,34),task.targetName,22,null,true,false,TextAnchor.MiddleCenter);
        UiTheme.Label(new Rect(x+15,172,390,29),(task.routeDistance>0?task.routeDistance:Vector3.Distance(g.player.transform.position,task.target)).ToString("0.0")+" m",21,UiTheme.Gold,false,false,TextAnchor.MiddleCenter);
    }

    void Dialogue(){float left=(w-1040)/2,top=h-355;UiTheme.Panel(new Rect(left,top,1040,322),new Color(.04f,.12f,.14f,.98f));
        UiTheme.Label(new Rect(left+30,top+20,960,40),UiCopy.Translate(g.speaker),25,UiTheme.Gold,true);
        string translated=UiCopy.Translate(g.line);
        if(dialoguePages==null||dialogueText!=translated||dialogueLanguage!=UiCopy.Selected){
            dialogueText=translated;dialogueLanguage=UiCopy.Selected;dialoguePage=0;var pages=new List<string>();string rest=translated;var style=UiTheme.Style(29,UiTheme.Cream);
            while(rest.Length>0){int lo=1,hi=rest.Length;while(lo<hi){int mid=(lo+hi+1)/2;if(style.CalcHeight(new GUIContent(rest.Substring(0,mid)),978)<=135)lo=mid;else hi=mid-1;}int cut=lo;if(cut<rest.Length&&!char.IsWhiteSpace(rest[cut-1])&&!char.IsWhiteSpace(rest[cut])){int space=rest.LastIndexOf(' ',cut-1,cut);if(space>cut*2/3)cut=space;}pages.Add(rest.Substring(0,cut).Trim());rest=rest.Substring(cut).TrimStart();}
            dialoguePages=pages.Count>0?pages.ToArray():new[]{""};
        }
        UiTheme.Label(new Rect(left+30,top+77,978,135),dialoguePages[dialoguePage],29);
        if(dialoguePages.Length>1)UiTheme.Label(new Rect(left+905,top+20,100,40),(dialoguePage+1)+" / "+dialoguePages.Length,21,UiTheme.Muted,false,false,TextAnchor.MiddleRight);
        UiTheme.Label(new Rect(left+30,top+235,610,75),T("阅读中，工厂已暂停。\n点“继续”，或按空格 / 回车。Tab 打开退出菜单。","Paused for dialogue.\nClick Continue or press Space / Enter."),21,UiTheme.Muted);
        if(UiTheme.Button(new Rect(left+725,top+233,278,63),T("继续  →","Continue  →")))g.Advance();
    }
    void Menu(){UiTheme.Fill(new Rect(0,0,w,h),new Color(.015f,.04f,.05f,.82f));float x=w/2-415;UiTheme.Panel(new Rect(x,55,830,h-110));UiTheme.Label(new Rect(x+40,83,735,65),T("休息一下，随时回来","Take a breath"),45,null,false,true);
        UiTheme.Label(new Rect(x+40,160,735,88),T("游戏已暂停，鼠标已释放。\n你的对话和当前工序会留在这里。","Game paused. Your mouse is free.\nYour dialogue and current work stay here."),25,UiTheme.Muted);
        UiTheme.Label(new Rect(x+40,263,730,50),T("音乐","Music"),25);g.musicVolume=GUI.HorizontalSlider(new Rect(x+190,280,575,35),g.musicVolume,0,1);
        UiTheme.Label(new Rect(x+40,328,730,50),T("音效","Effects"),25);g.effectsVolume=GUI.HorizontalSlider(new Rect(x+190,345,575,35),g.effectsVolume,0,1);
        if(UiTheme.Button(new Rect(x+40,403,350,60),T("试听 / 恢复声音","Test / restore sound"),true))g.audioScore.RestoreAudio(true);
        if(UiTheme.Button(new Rect(x+425,403,340,60),T("操作与舒适度","Controls & comfort")+" [C]",true))g.OpenComfort();
        if(UiTheme.Button(new Rect(x+40,485,725,65),T("回到刚才的位置  [Tab / Esc]","Resume where you left off  [Tab / Esc]")))g.TogglePause();
        if(UiTheme.Button(new Rect(x+40,572,350,60),T("重新开始这晚","Restart this night"),true))g.Restart();
        if(UiTheme.Button(new Rect(x+425,572,340,60),T("退出游戏 [X]","Quit game [X]"),true))g.Quit();
        if(UiTheme.Button(new Rect(x+40,661,725,52),T("语言","Language")+": "+UiCopy.Names[UiCopy.Selected]+"  [L]",true))OpenLanguages();
        UiTheme.Label(new Rect(x+40,h-165,735,50),T("若按键没有响应，也可用 macOS 的 ⌘ Q 退出。","You can also quit with Cmd Q on Mac or Alt F4 on Windows."),21,UiTheme.Muted);
    }
    void Notebook(){float x=w/2-480;UiTheme.Panel(new Rect(x,70,960,h-140),UiTheme.Cream);UiTheme.Label(new Rect(x+35,95,890,65),T("夜班笔记","The night-shift notebook"),40,UiTheme.Ink,false,true);
        UiTheme.Label(new Rect(x+35,190,890,125),task.action+"\n"+task.reason,27,UiTheme.Ink,true);
        UiTheme.Label(new Rect(x+35,340,890,75),T("金色核心的位置：","Gold core location: ")+UiCopy.Translate(g.CoreLocation),26,UiTheme.Ink);
        var rows=guide.Checklist;for(int i=0;i<rows.Length;i++)UiTheme.Label(new Rect(x+35,430+i*44,890,44),rows[i].Replace("[+]","✓").Replace("[ ]","□"),24,UiTheme.Ink);
        UiTheme.Label(new Rect(x+35,625,890,95),g.blueprint?UiCopy.Translate("For the last one to leave. So the way home is bright, too."):T("完成的工序会保留。箱子留在板上，门就会开着。","Finished work stays finished. Leave crates on plates to keep access."),24,UiTheme.Ink);
        if(UiTheme.Button(new Rect(x+35,h-175,890,60),T("合上笔记  [J]","Close notebook  [J]")))g.ToggleJournal();
    }
    void Ending(){UiTheme.Panel(new Rect(35,70,600,h-140),new Color(.04f,.12f,.14f,.94f));UiTheme.Label(new Rect(65,100,530,42),T("最后一份订单 / 已送达","THE LAST ORDER / DELIVERED"),25,UiTheme.Gold,true);UiTheme.Label(new Rect(65,175,530,135),T("照亮回家的路。","A light for\nthe way home."),49,null,false,true);UiTheme.Label(new Rect(65,335,530,275),T("Pip 又看了一遍你的纸条。\n\n“这一次，我们一起下班吧。”\n\n门外的万圣节游行，才刚刚开始。","Pip turns your note over once more.\n\n\"Then this time, we all clock out together.\"\n\nOutside, the parade is just beginning."),26);
        UiTheme.Label(new Rect(65,h-250,530,50),T("安全重试：","Safety retries: ")+g.resets,23,UiTheme.Muted);
        if(UiTheme.Button(new Rect(65,h-190,330,65),T("再来一次","Visit again")))g.Restart();if(UiTheme.Button(new Rect(415,h-190,185,65),T("退出","Quit"),true))g.Quit();}
}
}
