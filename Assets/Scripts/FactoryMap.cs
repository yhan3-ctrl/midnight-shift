using UnityEngine;
namespace LastLantern {
// A readable floor plan shares its route with the actual floor trail.
public class FactoryMap {
    FactoryGame g;string T(string zh,string en)=>UiCopy.Pick(zh,en);
    Vector2 Point(Rect r,Vector3 world)=>new Vector2(r.x+(world.x+9)/18*r.width,r.yMax-(world.z+10)/74*r.height);
    void Line(Vector2 a,Vector2 b,Color color,float thick=3){var old=GUI.matrix;float angle=Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg;GUI.matrix=old*Matrix4x4.TRS(new Vector3(a.x,a.y,0),Quaternion.Euler(0,0,angle),Vector3.one);UiTheme.Fill(new Rect(0,-thick/2,Vector2.Distance(a,b),thick),color);GUI.matrix=old;}
    void Box(Rect r,float x,float z,float width,float depth,Color color){Vector2 p=Point(r,new Vector3(x-width/2,0,z+depth/2));UiTheme.Fill(new Rect(p.x,p.y,width/18*r.width,depth/74*r.height),color);}
    void Plan(Rect r,bool large){
        UiTheme.Fill(r,UiTheme.Ink);
        Color[] colors={new Color(.14f,.29f,.30f),new Color(.19f,.35f,.35f),new Color(.24f,.32f,.38f),new Color(.29f,.31f,.35f)};
        float[] ends={-10,12,30,46,64};for(int i=0;i<4;i++)Box(r,0,(ends[i]+ends[i+1])/2,17.6f,ends[i+1]-ends[i]-.3f,colors[i]);
        foreach(float z in new[]{12f,18,30,43.5f,46,54}){Box(r,-5.5f,z,7,.3f,UiTheme.Muted);Box(r,5.5f,z,7,.3f,UiTheme.Muted);}
        Box(r,0,23,6,1.3f,UiTheme.Ink);Box(r,-5.8f,39.3f,3,1.4f,UiTheme.Ink);Box(r,5.9f,39.3f,3,1.4f,UiTheme.Ink);
        foreach(var m in Object.FindObjectsByType<Machine>(FindObjectsSortMode.None))Box(r,m.transform.position.x,m.transform.position.z,2.5f,1.5f,UiTheme.Ink);
        var nav=g.navigation;if(nav){nav.Refresh(g.guide.Current);for(int i=1;i<nav.Points.Count;i++)Line(Point(r,nav.Points[i-1]),Point(r,nav.Points[i]),UiTheme.Gold,large?4:2);}
        if(large){
            foreach(var gate in Object.FindObjectsByType<FactoryGate>(FindObjectsSortMode.None))if(gate.plate){var marker=Point(r,gate.plate.position);UiTheme.Fill(new Rect(marker.x-13,marker.y-12,26,25),UiTheme.Gold);UiTheme.Label(new Rect(marker.x-13,marker.y-15,26,30),"P",20,UiTheme.Ink,true,false,TextAnchor.MiddleCenter);}
            foreach(var crate in Object.FindObjectsByType<Pickup>(FindObjectsSortMode.None))if(crate.kind==Pickup.Kind.Crate&&!crate.carried){var marker=Point(r,crate.transform.position);UiTheme.Fill(new Rect(marker.x-11,marker.y-11,23,23),new Color(.65f,.44f,.25f));UiTheme.Label(new Rect(marker.x-11,marker.y-15,23,30),"C",20,UiTheme.Cream,true,false,TextAnchor.MiddleCenter);}
            string[] rooms={T("入口","Entrance"),T("包装区","Packaging"),T("档案室","Archive"),T("装配区","Assembly")};float[] centres={1,27.5f,37,48};
            for(int i=0;i<4;i++){var label=Point(r,new Vector3(0,0,centres[i]));UiTheme.Fill(new Rect(label.x-78,label.y-16,156,32),new Color(.04f,.12f,.14f,.9f));UiTheme.Label(new Rect(label.x-78,label.y-16,156,32),rooms[i],20,UiTheme.Cream,true,false,TextAnchor.MiddleCenter);}
            int n=0;foreach(var world in new[]{new Vector3(-5.4f,0,15.3f),new Vector3(-5.4f,0,20),new Vector3(5.4f,0,25),new Vector3(-6.3f,0,35),new Vector3(2.6f,0,44.9f),new Vector3(-5.4f,0,50.8f),new Vector3(5.4f,0,57.5f),new Vector3(0,0,60.3f)}){
                var pin=Point(r,world);UiTheme.Fill(new Rect(pin.x-12,pin.y-12,25,25),UiTheme.Cream);UiTheme.Label(new Rect(pin.x-12,pin.y-14,25,30),(++n).ToString(),20,UiTheme.Ink,true,false,TextAnchor.MiddleCenter);
            }
        }
        Vector2 goal=Point(r,g.guide.Current.target);float size=large?16:5;Color gold=UiTheme.Gold;
        UiTheme.Fill(new Rect(goal.x-size,goal.y-size,size*2,3),gold);UiTheme.Fill(new Rect(goal.x-size,goal.y+size-3,size*2,3),gold);UiTheme.Fill(new Rect(goal.x-size,goal.y-size,3,size*2),gold);UiTheme.Fill(new Rect(goal.x+size-3,goal.y-size,3,size*2),gold);
        if(g.sharedCore&&!g.sharedCore.carried){var core=Point(r,g.sharedCore.transform.position);UiTheme.Fill(new Rect(core.x-4,core.y-4,8,8),new Color(.55f,1,.75f));}
        Vector2 here=Point(r,g.player.transform.position);Vector2 forward=new Vector2(Mathf.Sin(g.player.transform.eulerAngles.y*Mathf.Deg2Rad),-Mathf.Cos(g.player.transform.eulerAngles.y*Mathf.Deg2Rad));
        UiTheme.Fill(new Rect(here.x-5,here.y-5,10,10),UiTheme.Cream);Line(here,here+forward*(large?22:12),UiTheme.Cream,3);
    }
    public void Mini(FactoryGame game){g=game;Rect panel=new Rect(28,92,225,230);UiTheme.Panel(panel);if(GUI.Button(panel,GUIContent.none,GUIStyle.none))g.ToggleMap();UiTheme.Label(new Rect(43,98,195,46),T("地图 [M]","Map [M]"),23,UiTheme.Gold,true);Plan(new Rect(43,146,57,158),false);
        string[] rooms={T("装配区","Assembly"),T("档案室","Archive"),T("包装区","Packaging"),T("入口","Entrance")};for(int i=0;i<4;i++)UiTheme.Label(new Rect(111,150+i*39,128,36),rooms[i],20);
    }
    public void Draw(FactoryGame game,float w,float h){g=game;UiTheme.Fill(new Rect(0,0,w,h),new Color(.02f,.06f,.07f,.96f));float x=w/2-650;UiTheme.Panel(new Rect(x,30,1300,h-60));
        UiTheme.Label(new Rect(x+30,53,900,60),T("Willow Works / 工厂地图","Willow Works / Factory map"),35,UiTheme.Gold,true,true);
        if(UiTheme.Button(new Rect(x+1020,52,250,52),T("返回 [M / Esc]","Back [M / Esc]"),true))g.ToggleMap();
        Rect plan=new Rect(x+38,145,400,h-205);Plan(plan,true);
        UiTheme.Label(new Rect(x+458,145,785,55),g.guide.ChapterTitle,29,UiTheme.Gold,true);
        var task=g.guide.Current;UiTheme.Label(new Rect(x+458,210,785,85),task.action,28,null,true);
        UiTheme.Label(new Rect(x+458,305,785,88),T("白色：你与朝向   金色：当前路线 / 目标\n绿色：核心   深色：机器 / 障碍","White: you + facing   Gold: route / goal\nGreen: core   Dark: machines / obstacles"),23,UiTheme.Muted);
        UiTheme.Label(new Rect(x+458,398,785,46),"P / "+UiCopy.Translate("Striped pressure plate")+"     C / "+UiCopy.Translate("Crate"),23,UiTheme.Gold);
        UiTheme.Label(new Rect(x+458,442,785,73),T("沿金色地面光路走。路线会绕过货架和传送带。\n这张图暂停世界，不会替你完成谜题。","Follow the gold trail around shelves and the conveyor.\nThe map pauses the world; it does not solve puzzles."),24);
        string[] names={"WORKSHOP RELAY","MOULDING TABLE","RIBBON CONVEYOR","BREAK-TIME CHIME",T("旧图纸","Old drawing"),"ENGRAVING TABLE","LANTERN PRESS",T("Pip / 交付","Pip / Deliver")};
        for(int i=0;i<names.Length;i++){int col=i/4,row=i%4;UiTheme.Label(new Rect(x+458+col*390,526+row*57,380,55),(i+1)+"  "+UiCopy.Translate(names[i]),23);}
        UiTheme.Label(new Rect(x+458,h-105,785,45),T("入口 → 包装区 → 档案室 → 装配区 → 交付","Entrance → Packaging → Archive → Assembly → Dispatch"),22,UiTheme.Gold);
    }
}
}
