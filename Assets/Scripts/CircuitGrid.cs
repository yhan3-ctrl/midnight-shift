using System;
using System.Collections.Generic;
namespace LastLantern {
// Four authored, solvable routing puzzles. Bits: North, East, South, West.
[Serializable]
public class CircuitGrid {
    public const int N=1,E=2,S=4,W=8;
    public int size,sourceRow;public int[] shapes,turns;public int[] targets,beacons;public string lesson;
    int[] initial; bool[] powered;
    public static CircuitGrid Create(string id){
        if(id=="chime")return null;
        int stage=id=="mould"?0:id=="ribbon"?1:id=="etch"?2:3;
        var p=new CircuitGrid{size=stage<2?3:4,sourceRow=stage==2?2:1};int total=p.size*p.size;
        p.shapes=new int[total];p.turns=new int[total];p.powered=new bool[total];
        for(int y=0;y<p.size;y++)for(int x=0;x<p.size;x++){int i=y*p.size+x;p.shapes[i]=(x+y)%3==0?N|S:N|E;p.turns[i]=(x*3+y*2+stage+1)%4;}
        if(stage==0){
            p.Set(0,1,W|N);p.Set(0,0,S|E);p.Set(1,0,W|E);p.Set(2,0,W|S);p.Set(2,1,N|E);p.Set(1,1,N|E|W);
            p.targets=new[]{p.Index(2,1)};p.beacons=new[]{p.Index(1,0)};
            p.lesson="Click a tile to rotate it clockwise. Feed power from IN to the outlet on the right, passing through the marked brass contact. Straight pieces carry power; corners turn it; T pieces split it. Power crosses only matching edges; unused ends may stay open.";
        }else if(stage==1){
            p.Set(0,1,W|E);p.Set(1,1,W|N|S);p.Set(1,0,S|E);p.Set(2,0,W|E);p.Set(1,2,N|E);p.Set(2,2,W|E);
            p.targets=new[]{p.Index(2,0),p.Index(2,2)};p.beacons=new[]{p.Index(1,1)};
            p.lesson="The conveyor needs both outlets. Use a T junction to split the one incoming supply. Lit tiles show where power can actually flow; a line that only looks close does not connect.";
        }else if(stage==2){
            p.Set(0,2,W|E);p.Set(1,2,W|N);p.Set(1,1,S|N);p.Set(1,0,S|E);p.Set(2,0,W|S);p.Set(2,1,N|E);p.Set(3,1,W|S);p.Set(3,2,N|E);
            p.targets=new[]{p.Index(3,2)};p.beacons=new[]{p.Index(1,0),p.Index(2,1)};
            p.lesson="The engraver must receive power through both marked contacts before the outlet can count. Plan the route around the whole board rather than taking the shortest line.";
        }else{
            p.Set(0,1,W|E);p.Set(1,1,W|N|S);p.Set(1,0,S|E);p.Set(2,0,W|E);p.Set(3,0,W|S);p.Set(3,1,N|E);
            p.Set(1,2,N|E);p.Set(2,2,W|S);p.Set(2,3,N|E);p.Set(3,3,W|E);
            p.targets=new[]{p.Index(3,1),p.Index(3,3)};p.beacons=new[]{p.Index(2,0),p.Index(2,3)};
            p.lesson="A shell and a light: both outputs must receive power through the marked contacts. Combine turning and splitting. There may be more than one valid route.";
        }
        p.initial=(int[])p.turns.Clone();p.Refresh();return p;
    }
    void Set(int x,int y,int mask){shapes[Index(x,y)]=mask;}
    public int Index(int x,int y)=>y*size+x;
    public int Mask(int index){int m=shapes[index],r=turns[index]&3;return ((m<<r)|(m>>(4-r)))&15;}
    public void Rotate(int index){turns[index]=(turns[index]+1)&3;Refresh();}
    public void Reset(){Array.Copy(initial,turns,turns.Length);Refresh();}
    public bool Powered(int index)=>powered[index];
    public bool IsContact(int index)=>Array.IndexOf(beacons,index)>=0;
    public bool IsOutlet(int index)=>Array.IndexOf(targets,index)>=0;
    public bool Solved {get{Refresh();foreach(int i in beacons)if(!powered[i])return false;foreach(int i in targets)if(!powered[i]||(Mask(i)&E)==0)return false;return true;}}
    public int LitOutlets {get{Refresh();int n=0;foreach(int i in targets)if(powered[i]&&(Mask(i)&E)!=0)n++;return n;}}
    public int LitContacts {get{Refresh();int n=0;foreach(int i in beacons)if(powered[i])n++;return n;}}
    public void Refresh(){
        Array.Clear(powered,0,powered.Length);int start=Index(0,sourceRow);if((Mask(start)&W)==0)return;
        var queue=new Queue<int>();queue.Enqueue(start);powered[start]=true;
        int[] dx={0,1,0,-1},dy={-1,0,1,0},edge={N,E,S,W},reverse={S,W,N,E};
        while(queue.Count>0){int i=queue.Dequeue(),x=i%size,y=i/size,m=Mask(i);for(int d=0;d<4;d++){
            int nx=x+dx[d],ny=y+dy[d];if(nx<0||nx>=size||ny<0||ny>=size||(m&edge[d])==0)continue;
            int next=Index(nx,ny);if(powered[next]||(Mask(next)&reverse[d])==0)continue;powered[next]=true;queue.Enqueue(next);
        }}
    }
}
}
