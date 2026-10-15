using UnityEngine;
namespace LastLantern {
// Native game UI for manipulating the same routing model used by machine start checks.
public class CircuitView {
    Texture2D tile,lit,wire,contact;GUIStyle text,title,small,action,contactText;
    Texture2D Solid(Color c){var t=new Texture2D(1,1);t.SetPixel(0,0,c);t.Apply();return t;}
    void Setup(){if(tile)return;tile=Solid(new Color(.12f,.23f,.25f));lit=Solid(new Color(1,.76f,.35f));wire=Solid(new Color(.5f,.59f,.58f));contact=Solid(new Color(.95f,.85f,.58f));
        text=new GUIStyle(GUI.skin.label){fontSize=20,wordWrap=true};text.normal.textColor=new Color(.95f,.91f,.81f);
        title=new GUIStyle(text){fontSize=29,fontStyle=FontStyle.Bold};small=new GUIStyle(text){fontSize=16};
        action=new GUIStyle(GUI.skin.button){fontSize=18};contactText=new GUIStyle(small){fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter};contactText.normal.textColor=new Color(.08f,.14f,.16f);
    }
    public void Draw(Machine machine,float width,float height,Texture2D background){
        Setup();var g=FactoryGame.I;var p=machine.puzzle;float left=width/2-480,top=height/2-360;
        GUI.DrawTexture(new Rect(left,top,960,720),background);
        GUI.Label(new Rect(left+40,top+28,880,46),machine.displayName+" / POWER ROUTING",title);
        GUI.Label(new Rect(left+40,top+88,870,80),p.lesson,small);
        float step=88,gap=8,gridX=left+100,gridY=top+205;
        for(int y=0;y<p.size;y++)for(int x=0;x<p.size;x++){
            int i=p.Index(x,y);Rect r=new Rect(gridX+x*(step+gap),gridY+y*(step+gap),step,step);
            GUI.DrawTexture(r,tile);if(GUI.Button(r,GUIContent.none,GUIStyle.none)){machine.selectedTile=i;p.Rotate(i);g.audioScore.Tick(500+i*9);}
            if(machine.selectedTile==i){GUI.DrawTexture(new Rect(r.x,r.y,r.width,2),contact);GUI.DrawTexture(new Rect(r.x,r.yMax-2,r.width,2),contact);GUI.DrawTexture(new Rect(r.x,r.y,2,r.height),contact);GUI.DrawTexture(new Rect(r.xMax-2,r.y,2,r.height),contact);}
            int mask=p.Mask(i);var color=p.Powered(i)?lit:wire;float cx=r.center.x,cy=r.center.y;
            if((mask&CircuitGrid.N)!=0)GUI.DrawTexture(new Rect(cx-5,r.y,10,step/2),color);
            if((mask&CircuitGrid.E)!=0)GUI.DrawTexture(new Rect(cx,cy-5,step/2,10),color);
            if((mask&CircuitGrid.S)!=0)GUI.DrawTexture(new Rect(cx-5,cy,10,step/2),color);
            if((mask&CircuitGrid.W)!=0)GUI.DrawTexture(new Rect(r.x,cy-5,step/2,10),color);
            GUI.DrawTexture(new Rect(cx-7,cy-7,14,14),color);
            if(p.IsContact(i)){GUI.DrawTexture(new Rect(r.x+5,r.y+5,17,17),contact);GUI.Label(new Rect(r.x+6,r.y+3,20,22),p.Powered(i)?"+":"o",contactText);}
            if(p.IsOutlet(i)){GUI.Label(new Rect(r.xMax+10,cy-13,85,30),p.Powered(i)&&(mask&CircuitGrid.E)!=0?"OUT +":"OUT o",small);}
        }
        GUI.Label(new Rect(gridX-65,gridY+p.sourceRow*(step+gap)+28,65,30),"IN >",small);
        float sx=left+595;
        GUI.Label(new Rect(sx,top+210,310,90),$"OUTLETS  {p.LitOutlets} / {p.targets.Length}\nCONTACTS  {p.LitContacts} / {p.beacons.Length}",text);
        GUI.Label(new Rect(sx,top+330,310,120),p.Solved?"The circuit is connected. Send power when you're ready.":"Follow the lit path from IN. Rotate the next tile so both touching edges meet.",text);
        GUI.Label(new Rect(sx,top+475,310,80),"The workshop is paused while you plan. Your tile positions are kept if you close this panel.",small);
        GUI.Label(new Rect(left+40,top+588,880,28),"Click to turn. Arrows select, Space turns, Enter sends power, Esc closes.",small);
        GUI.enabled=p.Solved;
        if(GUI.Button(new Rect(left+40,top+625,275,54),"Send power / start machine",action))g.PowerCircuit();
        GUI.enabled=true;
        if(GUI.Button(new Rect(left+330,top+625,175,54),"Reset tiles",action))p.Reset();
        if(GUI.Button(new Rect(left+520,top+625,180,54),"Remove core",action)){g.CloseCircuit();machine.Remove(g.player);}
        if(GUI.Button(new Rect(left+715,top+625,200,54),"Close  [Esc]",action))g.CloseCircuit();
    }
}
}
