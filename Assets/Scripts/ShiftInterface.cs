using System;
using System.Collections.Generic;
using UnityEngine;
namespace LastLantern {
public static class UiTheme {
    public static readonly Color Ink=new Color(.10f,.19f,.20f),Cream=new Color(.98f,.94f,.84f),Gold=new Color(.98f,.74f,.35f),Teal=new Color(.10f,.25f,.27f),Muted=new Color(.70f,.79f,.75f);
    static Font body,display;static HashSet<string> overflow=new HashSet<string>();static Dictionary<string,GUIStyle> styles=new Dictionary<string,GUIStyle>();static bool language;
    public static void Setup(){if(!body){body=Resources.Load<Font>("Fonts/NotoSansSC");display=Resources.Load<Font>("Fonts/Fraunces");}if(language!=UiCopy.Chinese){language=UiCopy.Chinese;styles.Clear();}}
    public static GUIStyle Style(int size,Color color,bool bold=false,bool art=false,TextAnchor align=TextAnchor.UpperLeft){Setup();string key=size+"/"+color+"/"+bold+"/"+art+"/"+align;if(!styles.TryGetValue(key,out var s)){s=new GUIStyle(GUI.skin.label){font=(art?display:body),fontSize=size,fontStyle=bold?FontStyle.Bold:FontStyle.Normal,wordWrap=true,alignment=align,richText=false};s.normal.textColor=color;styles[key]=s;}return s;}
    public static void Label(Rect r,string text,int size=24,Color? color=null,bool bold=false,bool art=false,TextAnchor align=TextAnchor.UpperLeft){bool latin=true;foreach(char c in text)if(c>=0x2e80){latin=false;break;}var style=Style(size,color??Cream,bold,art&&latin,align);if(align==TextAnchor.UpperLeft&&style.CalcHeight(new GUIContent(text),r.width)>r.height+3&&overflow.Add(text+"/"+r)){Debug.LogWarning("UI_LAYOUT_OVERFLOW / "+text+" / "+r);}GUI.Label(r,text,style);}
    public static void Fill(Rect r,Color color){var before=GUI.color;GUI.color=color;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=before;}
    public static void Panel(Rect r,Color? color=null){Fill(new Rect(r.x+4,r.y+5,r.width,r.height),new Color(0,0,0,.25f));Fill(r,color??Teal);}
    public static bool Button(Rect r,string text,bool secondary=false){var s=new GUIStyle(GUI.skin.button){font=body,fontSize=24,wordWrap=true,padding=new RectOffset(12,12,8,8)};s.normal.textColor=secondary?Cream:Ink;s.hover.textColor=s.normal.textColor;s.active.textColor=s.normal.textColor;s.normal.background=Texture2D.whiteTexture;s.hover.background=Texture2D.whiteTexture;s.active.background=Texture2D.whiteTexture;s.focused.background=Texture2D.whiteTexture;var old=GUI.backgroundColor;GUI.backgroundColor=secondary?new Color(.18f,.34f,.35f):Gold;Fill(r,secondary?new Color(.18f,.34f,.35f):Gold);bool clicked=GUI.Button(r,text,s);GUI.backgroundColor=old;return clicked;}
    public static void Key(Rect r,string key){Fill(new Rect(r.x,r.y+3,r.width,r.height),new Color(.02f,.07f,.08f));Fill(r,Cream);Label(r,key,23,Ink,true,false,TextAnchor.MiddleCenter);}
}
public class ShiftInterface {
    FactoryGame g;ShiftGuide guide;float w,h,scale;ShiftGuide.Task task;
    string T(string zh,string en)=>UiCopy.Pick(zh,en);
    public void Draw(FactoryGame game){g=game;guide=g.guide;UiTheme.Setup();scale=Mathf.Min(Screen.width/1440f,Screen.height/900f);w=Screen.width/scale;h=Screen.height/scale;GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,Vector3.one*scale);
        if(!g.started){Title();return;}if(g.ended){Ending();return;}
        if(g.paused){Menu();return;}
        if(g.activeCircuit){g.DrawCircuit(w,h);MenuTab();return;}
        task=guide.Navigate(guide.Current);
        if(g.journal){Notebook();MenuTab();return;}
        Calendar();MenuTab();
        if(g.Reading){Dialogue();return;}
        Pointer();Coach();
        UiTheme.Label(new Rect(w/2-15,h/2-25,30,40),"+",27,UiTheme.Cream,false,false,TextAnchor.MiddleCenter);
        if(g.player.focus){UiTheme.Panel(new Rect(w/2-290,h-110,580,44));UiTheme.Label(new Rect(w/2-277,h-110,554,44),UiCopy.Prompt(g.player),23,null,true,false,TextAnchor.MiddleCenter);}
        Controls();
        if(g.ToastVisible){UiTheme.Panel(new Rect(28,100,Mathf.Min(710,w-430),125));UiTheme.Label(new Rect(46,115,Mathf.Min(674,w-466),100),UiCopy.Translate(g.toast),23);}
        if(g.inspector.Alertness>0){UiTheme.Fill(new Rect(w/2-155,110,310,12),UiTheme.Ink);UiTheme.Fill(new Rect(w/2-155,110,310*g.inspector.Alertness,12),UiTheme.Gold);UiTheme.Label(new Rect(w/2-200,128,400,62),T("Hush 正在注意你：绕到货架或放下的箱子后面。","Hush notices you. Move behind a shelf or a placed crate."),22,null,false,false,TextAnchor.MiddleCenter);}
        if(Array.IndexOf(Environment.GetCommandLineArgs(),"--agent-playtest")>=0){var p=g.player.transform.position;UiTheme.Label(new Rect(28,76,850,35),$"AGENT REVIEW / {p.x:F2}, {p.z:F2} / yaw {g.player.transform.eulerAngles.y:F1} pitch {g.player.Pitch:F1}",18);}
    }
    void MenuTab(){UiTheme.Key(new Rect(28,24,65,40),"TAB");UiTheme.Label(new Rect(106,27,410,42),T("菜单 / 释放鼠标 · Esc 也可用","Menu / free mouse · Esc"),22);}
    void Title(){
        UiTheme.Panel(new Rect(48,50,775,h-100),new Color(.045f,.12f,.14f,.98f));
        UiTheme.Label(new Rect(90,82,680,40),"WILLOW WORKS  /  OCTOBER 31",25,UiTheme.Gold);
        UiTheme.Label(new Rect(85,148,690,155),"Midnight Shift\nThe Last Lantern",54,UiTheme.Cream,false,true);
        UiTheme.Label(new Rect(92,325,660,95),T("最后一份订单，三个等你的小幽灵。\n今晚，一起做一盏照亮回家路的灯。","One forgotten order. Three little helpers.\nTonight, make a light for their way home."),28);
        UiTheme.Label(new Rect(92,445,660,65),T("我会一步步带你熟悉工厂。无需先记住所有规则。","I'll show you the first steps. Follow the gold marker."),25,UiTheme.Muted);
        if(UiTheme.Button(new Rect(92,547,635,68),T("跟着 Pip 开始夜班  →","Begin the guided night shift  →")))g.Begin();
        if(UiTheme.Button(new Rect(92,638,225,58),"中文",!UiCopy.Chinese))UiCopy.Language(true);
        if(UiTheme.Button(new Rect(334,638,225,58),"English",UiCopy.Chinese))UiCopy.Language(false);
        if(UiTheme.Button(new Rect(576,638,151,58),T("退出","Quit"),true))g.Quit();
        UiTheme.Label(new Rect(92,724,640,45),T("无倒计时 · 无战斗 · 可随时暂停或退出","No countdown · No combat · Pause or quit anytime"),22,UiTheme.Muted);
        float x=Mathf.Max(860,w-510);UiTheme.Panel(new Rect(x,130,440,585),UiTheme.Cream);
        UiTheme.Label(new Rect(x+30,157,380,65),T("先认识这几个按键","Your first few controls"),30,UiTheme.Ink,true,true);
        ControlCard(x+30,255,"W A S D",T("走动","Walk"),T("W 前进，S 后退，A / D 左右移动","W forward, S back, A / D sideways"));
        ControlCard(x+30,375,"E",T("拿起 / 使用","Pick up / use"),T("先用屏幕中央的圆点对准物品","Aim the centre dot at the item first"));
        ControlCard(x+30,495,"Q",T("放下物品","Place"),T("放到前方一步的位置","Places it one step in front of you"));
        UiTheme.Label(new Rect(x+30,635,385,70),T("鼠标 / 方向键：观察\n空格 / 回车 / 点击：继续对话","Mouse / arrows: look\nSpace / Enter: next line"),21,UiTheme.Ink);
    }
    void ControlCard(float x,float y,string key,string title,string copy){UiTheme.Key(new Rect(x,y,key.Length>3?160:60,43),key);UiTheme.Label(new Rect(x+(key.Length>3?178:80),y,200,45),title,28,UiTheme.Ink,true);UiTheme.Label(new Rect(x,y+54,370,54),copy,22,UiTheme.Ink);}
    void Calendar(){float x=w-365;UiTheme.Panel(new Rect(x,90,337,390),UiTheme.Cream);UiTheme.Fill(new Rect(x,90,337,66),UiTheme.Teal);
        for(int i=0;i<5;i++){UiTheme.Fill(new Rect(x+33+i*62,79,8,26),UiTheme.Gold);}
        UiTheme.Label(new Rect(x+22,104,290,45),T("10 / 31    今晚的待办","10 / 31    TONIGHT"),23,UiTheme.Cream,true);
        UiTheme.Label(new Rect(x+22,177,292,64),guide.ChapterTitle,26,UiTheme.Ink,true,true);
        string[] rows=guide.Checklist;for(int i=0;i<rows.Length;i++){bool done=rows[i].StartsWith("[+]");float y=251+i*49;UiTheme.Fill(new Rect(x+22,y+9,20,20),UiTheme.Ink);UiTheme.Fill(new Rect(x+24,y+11,16,16),UiTheme.Cream);if(done)UiTheme.Label(new Rect(x+18,y-1,30,37),"✓",25,UiTheme.Teal,true,false,TextAnchor.MiddleCenter);UiTheme.Label(new Rect(x+54,y,259,49),rows[i].Substring(4),23,done?new Color(.32f,.47f,.4f):UiTheme.Ink);}
    }
    void Coach(){UiTheme.Panel(new Rect(28,h-356,650,246));UiTheme.Key(new Rect(49,h-339,task.key.Length>3?190:75,39),string.IsNullOrEmpty(task.key)?"...":task.key);UiTheme.Label(new Rect(49,h-290,610,93),task.action,27,UiTheme.Gold,true);UiTheme.Label(new Rect(49,h-190,610,74),task.reason,23);}
    void Controls(){UiTheme.Key(new Rect(28,h-61,143,40),"W A S D");UiTheme.Label(new Rect(183,h-56,130,40),T("走动","Walk"),22);UiTheme.Key(new Rect(287,h-61,52,40),"E");UiTheme.Label(new Rect(350,h-56,145,40),T("拿起 / 使用","Use"),22);UiTheme.Key(new Rect(503,h-61,52,40),"Q");UiTheme.Label(new Rect(567,h-56,130,40),T("放下","Place"),22);
        if(g.player.held)UiTheme.Label(new Rect(w-360,h-91,330,70),T("手中：","Carrying: ")+UiCopy.Translate(g.player.held.displayName),23,UiTheme.Gold);}
    void Pointer(){if(!task.hasTarget)return;var camera=g.player.view;Vector3 target=task.target+Vector3.up*.55f;Vector3 sp=camera.WorldToScreenPoint(target);Vector2 centre=new Vector2(w/2,h/2);Vector2 xy=new Vector2(sp.x/scale,(Screen.height-sp.y)/scale);if(sp.z<0)xy=centre-(xy-centre)*2;
        bool clipped=sp.z<0||xy.x<185||xy.x>w-535||xy.y<170||xy.y>h-442;
        string direction=sp.z<0?"↶":xy.x<185?"←":xy.x>w-535?"→":xy.y<180?"↑":"↓";
        xy.x=Mathf.Clamp(xy.x,185,w-535);xy.y=Mathf.Clamp(xy.y,180,h-442);
        UiTheme.Label(new Rect(xy.x-30,xy.y-20,60,50),clipped?direction:"▼",42,UiTheme.Gold,true,false,TextAnchor.MiddleCenter);
        UiTheme.Panel(new Rect(xy.x-155,xy.y-105,310,80));UiTheme.Label(new Rect(xy.x-148,xy.y-101,296,42),task.targetName,22,null,true,false,TextAnchor.MiddleCenter);
        UiTheme.Label(new Rect(xy.x-148,xy.y-61,296,30),Vector3.Distance(g.player.transform.position,task.target).ToString("0.0")+" m",20,UiTheme.Gold,false,false,TextAnchor.MiddleCenter);
        if(clipped)UiTheme.Label(new Rect(xy.x-145,xy.y+27,290,44),T("朝箭头方向转动视角","Turn toward the arrow"),20,UiTheme.Gold,false,false,TextAnchor.MiddleCenter);
        if(g.player.held){
            if(g.player.Placement(out Vector3 at,out string why)){var q=camera.WorldToScreenPoint(at);if(q.z>0){float x=q.x/scale,y=(Screen.height-q.y)/scale;if(y+40<h-356||x-110>690)UiTheme.Label(new Rect(x-110,y-20,220,60),T("◎  Q 放在这里","◎  Q place here"),25,UiTheme.Gold,true,false,TextAnchor.MiddleCenter);}}
            else {UiTheme.Panel(new Rect(w-365,h-310,337,172));UiTheme.Label(new Rect(w-345,h-295,297,140),UiCopy.Translate(why),23,UiTheme.Gold);}
        }
    }
    void Dialogue(){float left=(w-1040)/2,top=h-355;UiTheme.Panel(new Rect(left,top,1040,322),new Color(.04f,.12f,.14f,.98f));
        UiTheme.Label(new Rect(left+30,top+20,960,40),UiCopy.Translate(g.speaker),25,UiTheme.Gold,true);
        UiTheme.Label(new Rect(left+30,top+77,978,135),UiCopy.Translate(g.line),29);
        UiTheme.Label(new Rect(left+30,top+235,610,75),T("阅读中，工厂已暂停。\n点“继续”，或按空格 / 回车。Tab 打开退出菜单。","Paused for dialogue.\nClick Continue or press Space / Enter."),21,UiTheme.Muted);
        if(UiTheme.Button(new Rect(left+725,top+233,278,63),T("继续  →","Continue  →")))g.Advance();
    }
    void Menu(){UiTheme.Fill(new Rect(0,0,w,h),new Color(.015f,.04f,.05f,.82f));float x=w/2-415;UiTheme.Panel(new Rect(x,55,830,h-110));UiTheme.Label(new Rect(x+40,83,735,65),T("休息一下，随时回来","Take a breath"),45,null,false,true);
        UiTheme.Label(new Rect(x+40,160,735,88),T("游戏已暂停，鼠标已释放。\n你的对话和当前工序会留在这里。","Game paused. Your mouse is free.\nYour dialogue and current work stay here."),25,UiTheme.Muted);
        UiTheme.Label(new Rect(x+40,263,730,50),T("音乐","Music"),25);g.musicVolume=GUI.HorizontalSlider(new Rect(x+190,280,575,35),g.musicVolume,0,1);
        UiTheme.Label(new Rect(x+40,328,730,50),T("音效","Effects"),25);g.effectsVolume=GUI.HorizontalSlider(new Rect(x+190,345,575,35),g.effectsVolume,0,1);
        if(UiTheme.Button(new Rect(x+40,403,350,60),T("试听 / 恢复声音","Test / restore sound"),true))g.audioScore.RestoreAudio(true);
        if(UiTheme.Button(new Rect(x+425,403,340,60),T("切换鼠标 / 方向键观察","Mouse / arrow-only look"),true)){g.player.keyboardLook=!g.player.keyboardLook;g.player.ResetLookInput();}
        if(UiTheme.Button(new Rect(x+40,485,725,65),T("回到刚才的位置  [Tab / Esc]","Resume where you left off  [Tab / Esc]")))g.TogglePause();
        if(UiTheme.Button(new Rect(x+40,572,350,60),T("重新开始这晚","Restart this night"),true))g.Restart();
        if(UiTheme.Button(new Rect(x+425,572,340,60),T("退出游戏 [X]","Quit game [X]"),true))g.Quit();
        if(UiTheme.Button(new Rect(x+40,661,190,52),"中文",!UiCopy.Chinese))UiCopy.Language(true);if(UiTheme.Button(new Rect(x+245,661,190,52),"English",UiCopy.Chinese))UiCopy.Language(false);
        UiTheme.Label(new Rect(x+40,h-165,735,50),T("若按键没有响应，也可用 macOS 的 ⌘ Q 退出。","You can also quit with Cmd Q on Mac or Alt F4 on Windows."),21,UiTheme.Muted);
    }
    void Notebook(){float x=w/2-480;UiTheme.Panel(new Rect(x,70,960,h-140),UiTheme.Cream);UiTheme.Label(new Rect(x+35,95,890,65),T("夜班笔记","The night-shift notebook"),40,UiTheme.Ink,false,true);
        UiTheme.Label(new Rect(x+35,190,890,125),task.action+"\n"+task.reason,27,UiTheme.Ink,true);
        UiTheme.Label(new Rect(x+35,340,890,75),T("金色核心的位置：","Gold core location: ")+UiCopy.Translate(g.CoreLocation),26,UiTheme.Ink);
        UiTheme.Label(new Rect(x+35,445,890,220),T("记住三件事就好：\n1. 箱子留在条纹板上，门就会开着。\n2. 只有一枚核心，但已经做好的东西不会消失。\n3. Hush 不会伤害你；被发现后保留完成进度重试。","Three things to remember:\n1. Leave a crate on a striped plate to keep access.\n2. There is one core; finished work stays finished.\n3. Hush cannot hurt you. Retries keep completed work."),25,UiTheme.Ink);
        if(UiTheme.Button(new Rect(x+35,h-175,890,60),T("合上笔记  [J / Esc]","Close notebook  [J / Esc]")))g.ToggleJournal();
    }
    void Ending(){UiTheme.Panel(new Rect(35,70,600,h-140),new Color(.04f,.12f,.14f,.94f));UiTheme.Label(new Rect(65,100,530,42),T("最后一份订单 / 已送达","THE LAST ORDER / DELIVERED"),25,UiTheme.Gold,true);UiTheme.Label(new Rect(65,175,530,135),T("照亮回家的路。","A light for\nthe way home."),49,null,false,true);UiTheme.Label(new Rect(65,335,530,275),T("Pip 又看了一遍你的纸条。\n\n“这一次，我们一起下班吧。”\n\n门外的万圣节游行，才刚刚开始。","Pip turns your note over once more.\n\n\"Then this time, we all clock out together.\"\n\nOutside, the parade is just beginning."),26);
        UiTheme.Label(new Rect(65,h-250,530,50),T("安全重试：","Safety retries: ")+g.resets,23,UiTheme.Muted);
        if(UiTheme.Button(new Rect(65,h-190,330,65),T("再来一次","Visit again")))g.Restart();if(UiTheme.Button(new Rect(415,h-190,185,65),T("退出","Quit"),true))g.Quit();}
}
}
