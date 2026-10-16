using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace LastLantern {
// A small physics-checked floor grid. Walls, shelves, crates and closed gates are real obstacles.
public class FactoryPath : MonoBehaviour {
    const float Step=.6f,MinX=-8.4f,MinZ=-9;const int Columns=29,Rows=121,Count=Columns*Rows;
    public readonly List<Vector3> Points=new List<Vector3>();
    public float Distance {get;private set;}public bool Available=>Points.Count>1;
    FactoryGame g;Material material;LineRenderer[] trail,beacons;float nextPlan;Vector3 plannedTarget,plannedFrom;int[] occupancy=new int[Count];
    readonly Collider[] overlaps=new Collider[32];
    void Start(){g=GetComponent<FactoryGame>();material=new Material(Resources.Load<Shader>("PlacementOutline"));trail=new LineRenderer[90];for(int i=0;i<trail.Length;i++){var o=new GameObject("Gold navigation trail");o.transform.SetParent(transform);o.layer=2;var l=o.AddComponent<LineRenderer>();l.sharedMaterial=material;l.positionCount=2;l.startWidth=l.endWidth=.38f;l.shadowCastingMode=ShadowCastingMode.Off;l.receiveShadows=false;l.enabled=false;trail[i]=l;}beacons=new LineRenderer[10];for(int i=0;i<beacons.Length;i++){var o=new GameObject("Raised gold route marker");o.transform.SetParent(transform);o.layer=2;var l=o.AddComponent<LineRenderer>();l.sharedMaterial=material;l.positionCount=7;l.startWidth=l.endWidth=.075f;l.shadowCastingMode=ShadowCastingMode.Off;l.receiveShadows=false;l.enabled=false;beacons[i]=l;}}
    // Heading only: target height and camera pitch must never ask the player to look at the floor.
    public static float Heading(Vector3 forward,Vector3 from,Vector3 target){forward.y=0;Vector3 delta=target-from;delta.y=0;return delta.sqrMagnitude<.01f?0:Vector3.SignedAngle(forward,delta,Vector3.up);}
    public void RefreshVisuals(){DrawTrail();}
    int Cell(Vector3 p)=>Mathf.Clamp(Mathf.RoundToInt((p.z-MinZ)/Step),0,Rows-1)*Columns+Mathf.Clamp(Mathf.RoundToInt((p.x-MinX)/Step),0,Columns-1);
    Vector3 Position(int i)=>new Vector3(MinX+(i%Columns)*Step,.065f,MinZ+(i/Columns)*Step);
    bool Clear(int i){
        if(occupancy[i]!=0)return occupancy[i]>0;
        Vector3 p=Position(i);int n=Physics.OverlapCapsuleNonAlloc(p+Vector3.up*.38f,p+Vector3.up*1.45f,.34f,overlaps,~0,QueryTriggerInteraction.Ignore);
        bool clear=n<overlaps.Length;
        for(int j=0;j<n&&clear;j++){var c=overlaps[j];if(c.transform.IsChildOf(g.player.transform)||c.GetComponentInParent<GhostIdle>())continue;if(c.bounds.max.y<.14f)continue;clear=false;}
        occupancy[i]=clear?1:-1;return clear;
    }
    int Nearest(Vector3 target,bool isGoal){int best=-1;float score=float.MaxValue;int cell=Cell(target),cx=cell%Columns,cz=cell/Columns;for(int dz=-5;dz<=5;dz++)for(int dx=-5;dx<=5;dx++){
        int x=cx+dx,z=cz+dz;if(x<0||x>=Columns||z<0||z>=Rows)continue;int i=z*Columns+x;Vector3 pos=Position(i);float distance=Vector2.Distance(new Vector2(pos.x,pos.z),new Vector2(target.x,target.z));
        if(distance>3||!Clear(i))continue;
        float value=distance+(isGoal?Vector3.Distance(pos,g.player.transform.position)*.018f:0);if(value<score){score=value;best=i;}
    }return best;}
    public void Refresh(ShiftGuide.Task task,bool force=false){
        if(!g)g=GetComponent<FactoryGame>();if(!task.hasTarget)return;
        var from=g.player.transform.position;
        if(!force&&Time.unscaledTime<nextPlan&&Vector3.Distance(task.target,plannedTarget)<.3f&&Vector3.Distance(from,plannedFrom)<1.5f)return;
        nextPlan=Time.unscaledTime+.8f;plannedFrom=from;plannedTarget=task.target;Build(from,task.target);
    }
    public bool Build(Vector3 from,Vector3 target){
        if(!g)g=GetComponent<FactoryGame>();System.Array.Clear(occupancy,0,Count);Points.Clear();Distance=0;
        int first=Nearest(from,false),last=Nearest(target,true);if(first<0||last<0)return false;
        var cost=new float[Count];var parent=new int[Count];var closed=new bool[Count];for(int i=0;i<Count;i++){cost[i]=float.MaxValue;parent[i]=-1;}
        var open=new Heap();cost[first]=0;open.Add(first,0);int goal=-1;
        while(open.Count>0){int i=open.Pop();if(closed[i])continue;closed[i]=true;if(i==last){goal=i;break;}int x=i%Columns,z=i/Columns;
            for(int d=0;d<4;d++){int nx=x+(d==0?1:d==1?-1:0),nz=z+(d==2?1:d==3?-1:0);if(nx<0||nx>=Columns||nz<0||nz>=Rows)continue;int j=nz*Columns+nx;
                if(closed[j]||!Clear(j)||cost[i]+Step>=cost[j])continue;cost[j]=cost[i]+Step;parent[j]=i;
                float heuristic=(Mathf.Abs(nx-last%Columns)+Mathf.Abs(nz-last/Columns))*Step;open.Add(j,cost[j]+heuristic);
            }
        }
        if(goal<0)return false;var reverse=new List<Vector3>();for(int i=goal;i>=0;i=parent[i]){reverse.Add(Position(i));if(i==first)break;}reverse.Reverse();
        Points.Add(new Vector3(from.x,.065f,from.z));
        // Compress only collinear segments, preserving checked turns around obstacles.
        for(int i=1;i<reverse.Count;i++){if(i==reverse.Count-1||Vector3.Cross(reverse[i]-reverse[i-1],reverse[i+1]-reverse[i]).sqrMagnitude>.001f)Points.Add(reverse[i]);}
        if(Points.Count==1)Points.Add(Position(goal));
        var corners=Points.ToArray();Points.Clear();Points.Add(corners[0]);
        for(int i=0;i<corners.Length-1;){int next=i+1;for(int j=corners.Length-1;j>i+1;j--)if(Corridor(corners[i],corners[j])){next=j;break;}Points.Add(corners[next]);i=next;}
        for(int i=1;i<Points.Count;i++)Distance+=Vector3.Distance(Points[i-1],Points[i]);return true;
    }
    bool Corridor(Vector3 from,Vector3 to){Vector3 d=to-from;foreach(var hit in Physics.CapsuleCastAll(from+Vector3.up*.38f,from+Vector3.up*1.45f,.34f,d.normalized,d.magnitude,~0,QueryTriggerInteraction.Ignore)){
        if(hit.transform.IsChildOf(g.player.transform)||hit.collider.GetComponentInParent<GhostIdle>()||hit.collider.bounds.max.y<.14f)continue;return false;
    }return true;}
    public Vector3 LookAhead(float distance){if(Points.Count==0)return plannedTarget;for(int i=1;i<Points.Count;i++){float d=Vector3.Distance(Points[i-1],Points[i]);if(distance<=d)return Vector3.Lerp(Points[i-1],Points[i],distance/Mathf.Max(.001f,d));distance-=d;}return Points[Points.Count-1];}
    void LateUpdate(){DrawTrail();}
    void DrawTrail(){if(trail==null)return;foreach(var l in trail)l.enabled=false;foreach(var l in beacons)l.enabled=false;if(!g||!g.started||g.ended||g.mapOpen||g.paused||g.Reading||g.activeCircuit||g.journal||g.photoMode||g.desktopHold||g.ComfortOpen||g.AwaitingLook)return;
        var objective=g.guide.Current;if(string.IsNullOrEmpty(objective.key)||g.guide.CanActHere(objective))return;Refresh(objective);if(!Available)return;float walked=0;int drawn=0;Color c=new Color(1,.78f,.30f,1);
        for(int i=1;i<Points.Count&&drawn<trail.Length;i++){Vector3 a=Points[i-1]+Vector3.up*.055f,b=Points[i]+Vector3.up*.055f;float len=Vector3.Distance(a,b);for(float t=0;t<len&&drawn<trail.Length;t+=.85f){var l=trail[drawn++];l.SetPosition(0,Vector3.Lerp(a,b,t/Mathf.Max(.001f,len)));l.SetPosition(1,Vector3.Lerp(a,b,Mathf.Min(t+.56f,len)/Mathf.Max(.001f,len)));l.startColor=l.endColor=c;l.enabled=true;}walked+=len;if(walked>32)break;}
        // Stationary lantern-shaped markers rise into a level camera's view. Real depth testing hides them behind walls.
        Vector3 side=g.player.view.transform.right;side.y=0;side.Normalize();int count=0;
        for(float distance=2;distance<Mathf.Min(Distance,26)&&count<beacons.Length;distance+=2.8f){
            Vector3 floor=LookAhead(distance),centre=floor+Vector3.up*.95f;var l=beacons[count++];l.startColor=l.endColor=new Color(1,.9f,.55f,1);
            l.SetPositions(new[]{floor,centre-Vector3.up*.18f,centre-side*.18f,centre+Vector3.up*.18f,centre+side*.18f,centre-Vector3.up*.18f,centre});l.enabled=true;
        }
    }
    void OnDestroy(){if(material)Destroy(material);}
    class Heap {
        struct Entry{public int cell;public float score;public Entry(int c,float s){cell=c;score=s;}}
        List<Entry> items=new List<Entry>();public int Count=>items.Count;
        public void Add(int cell,float score){var e=new Entry(cell,score);items.Add(e);int i=items.Count-1;while(i>0){int p=(i-1)/2;if(items[p].score<=score)break;items[i]=items[p];i=p;}items[i]=e;}
        public int Pop(){int result=items[0].cell;var end=items[items.Count-1];items.RemoveAt(items.Count-1);if(items.Count==0)return result;int i=0;while(i*2+1<items.Count){int c=i*2+1;if(c+1<items.Count&&items[c+1].score<items[c].score)c++;if(items[c].score>=end.score)break;items[i]=items[c];i=c;}items[i]=end;return result;}
    }
}
}
