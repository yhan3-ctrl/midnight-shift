using UnityEngine;
namespace LastLantern {
public class CircuitView {
    bool example;float exampleSince;
    string T(string zh,string en)=>UiCopy.Pick(zh,en);
    public void Draw(Machine m,float width,float height,Texture2D unused){
        var g=FactoryGame.I;var p=m.puzzle;float left=width/2-600,top=80;
        UiTheme.Panel(new Rect(left,top,1200,height-110),new Color(.04f,.12f,.14f,.99f));
        UiTheme.Label(new Rect(left+35,top+20,1120,55),UiCopy.Translate(m.displayName)+T(" / 接通电路"," / CONNECT THE CIRCUIT"),34,UiTheme.Gold,true,true);
        UiTheme.Label(new Rect(left+35,top+82,1120,84),T("把左边入口的电，送到右边每一个出口；途中经过所有 ★ 标记。","Join IN to every OUT. Pass through each ★ contact."),27);
        float step=86,gap=8,gridX=left+113,gridY=top+214;
        for(int y=0;y<p.size;y++)for(int x=0;x<p.size;x++){
            int i=p.Index(x,y);Rect r=new Rect(gridX+x*(step+gap),gridY+y*(step+gap),step,step);
            UiTheme.Fill(r,new Color(.16f,.30f,.32f));
            if(UiTheme.Hover(r))UiTheme.Outline(r,UiTheme.Gold);
            if(GUI.Button(r,GUIContent.none,GUIStyle.none)){m.selectedTile=i;p.Rotate(i);g.audioScore.Tick(500+i*9);}
            if(m.selectedTile==i){UiTheme.Fill(new Rect(r.x,r.y,r.width,3),UiTheme.Cream);UiTheme.Fill(new Rect(r.x,r.yMax-3,r.width,3),UiTheme.Cream);UiTheme.Fill(new Rect(r.x,r.y,3,r.height),UiTheme.Cream);UiTheme.Fill(new Rect(r.xMax-3,r.y,3,r.height),UiTheme.Cream);}
            int mask=p.Mask(i);Color c=p.Powered(i)?UiTheme.Gold:new Color(.62f,.74f,.72f);float cx=r.center.x,cy=r.center.y;
            if((mask&CircuitGrid.N)!=0)UiTheme.Fill(new Rect(cx-6,r.y,12,step/2),c);
            if((mask&CircuitGrid.E)!=0)UiTheme.Fill(new Rect(cx,cy-6,step/2,12),c);
            if((mask&CircuitGrid.S)!=0)UiTheme.Fill(new Rect(cx-6,cy,12,step/2),c);
            if((mask&CircuitGrid.W)!=0)UiTheme.Fill(new Rect(r.x,cy-6,step/2,12),c);
            UiTheme.Fill(new Rect(cx-8,cy-8,16,16),c);
            if(p.IsContact(i)){UiTheme.Fill(new Rect(r.x+5,r.y+5,28,28),p.Powered(i)?new Color(.5f,1,.65f):UiTheme.Cream);UiTheme.Label(new Rect(r.x+5,r.y+2,28,32),"★",24,UiTheme.Ink,true,false,TextAnchor.MiddleCenter);}
            if(p.IsOutlet(i))UiTheme.Label(new Rect(r.xMax+12,cy-22,125,50),p.Powered(i)&&(mask&CircuitGrid.E)!=0?"OUT ✓":"OUT →",24,UiTheme.Gold,true,false,TextAnchor.MiddleLeft);
        }
        UiTheme.Label(new Rect(gridX-98,gridY+p.sourceRow*(step+gap)+22,94,55),"IN →",26,UiTheme.Gold,true);
        float sx=left+655;
        UiTheme.Label(new Rect(sx,top+207,500,80),T("出口 ","OUT ")+p.LitOutlets+" / "+p.targets.Length+"     ★ "+p.LitContacts+" / "+p.beacons.Length,31,UiTheme.Gold,true);
        if(example){
            bool connected=((int)(Time.unscaledTime-exampleSince))%2==1;
            UiTheme.Label(new Rect(sx,top+294,490,50),T("点一下，方格转动四分之一圈。","One click turns a tile a quarter turn."),24);
            UiTheme.Label(new Rect(sx,top+370,92,45),"IN →",25,UiTheme.Gold,true);
            for(int n=0;n<2;n++){Rect box=new Rect(sx+100+n*85,top+365,68,68);UiTheme.Fill(box,new Color(.16f,.3f,.32f));bool horizontal=n==0||connected;UiTheme.Fill(horizontal?new Rect(box.x,box.center.y-5,68,10):new Rect(box.center.x-5,box.y,10,68),n==0||connected?UiTheme.Gold:UiTheme.Muted);if(n==1){UiTheme.Fill(new Rect(box.x+4,box.y+4,25,25),connected?new Color(.5f,1,.65f):UiTheme.Cream);UiTheme.Label(new Rect(box.x+4,box.y+1,25,30),"★",21,UiTheme.Ink,true,false,TextAnchor.MiddleCenter);}}
            UiTheme.Label(new Rect(sx+280,top+370,180,45),connected?"→ OUT ✓":"OUT",25,connected?UiTheme.Gold:UiTheme.Muted,true);
            UiTheme.Label(new Rect(sx,top+447,490,40),T("线头对上后，电就能继续走。","Matching ends let the power continue."),23,UiTheme.Muted);
        }else UiTheme.Label(new Rect(sx,top+294,493,180),p.Solved?T("接好啦！\n现在点下面的“启动机器”。","Connected!\nChoose Start machine below."):T("1. 点击方格，接好 IN → OUT。\n2. 必须让电经过每个 ★ 星标。\n3. 星标变绿、出口全亮后启动。","1. Turn tiles to join IN → OUT.\n2. Power MUST pass every ★.\n3. Green stars + lit OUTs: start."),26);
        if(UiTheme.Button(new Rect(sx,top+492,450,55),example?T("收起小例子","Hide the example"):T("看一个简单例子","Show a simple example"),true)){example=!example;exampleSince=Time.unscaledTime;}
        if(example){UiTheme.Label(new Rect(sx,top+563,465,70),T("IN → ── → ┐\n横线传电，弯角转弯，T 形可以分流。","IN → ── → ┐\nStraights carry; corners turn; T shapes split."),24,UiTheme.Gold);}
        else UiTheme.Label(new Rect(sx,top+563,465,70),T("看线路时世界暂停。\n不用着急，按 Tab 可以打开菜单。","The world is paused. Take your time.\nTab opens the menu."),23,UiTheme.Muted);
        float bottom=height-112;
        UiTheme.Label(new Rect(left+35,bottom-62,1125,44),T("鼠标点击：转动   |   方向键：选格   空格：转动   回车：启动   Backspace：返回   Esc：菜单","Click / Space: turn   Arrows: select   Enter: start   Backspace: return   Esc: menu"),23,UiTheme.Muted);
        GUI.enabled=p.Solved;if(UiTheme.Button(new Rect(left+35,bottom,375,65),p.Solved?T("启动机器  →","Start machine  →"):T("先接通出口和星标","Connect OUT + ★ first"),!p.Solved))g.PowerCircuit();GUI.enabled=true;
        if(UiTheme.Button(new Rect(left+430,bottom,220,65),T("重置方格","Reset tiles"),true))p.Reset();
        if(UiTheme.Button(new Rect(left+670,bottom,220,65),T("取回核心","Remove core"),true)){g.CloseCircuit();m.Remove(g.player);}
        if(UiTheme.Button(new Rect(left+910,bottom,255,65),T("返回工厂","Return to factory"),true))g.CloseCircuit();
    }
}
}
