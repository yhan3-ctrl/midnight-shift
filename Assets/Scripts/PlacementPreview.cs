using UnityEngine;
using UnityEngine.Rendering;
namespace LastLantern {
// World-space feedback uses the same validated point as Q. Letters never mark a location.
public class PlacementPreview : MonoBehaviour {
    LineRenderer[] box=new LineRenderer[12],plate=new LineRenderer[8];Material material;FactoryGame g;Light targetLight;
    public FactoryGate LitTarget {get;private set;}
    static readonly int[,] edges={{0,1},{1,2},{2,3},{3,0},{4,5},{5,6},{6,7},{7,4},{0,4},{1,5},{2,6},{3,7}};
    void Start(){g=GetComponent<FactoryGame>();var shader=Resources.Load<Shader>("PlacementOutline");if(!shader){enabled=false;return;}material=new Material(shader);for(int i=0;i<box.Length;i++)box[i]=Line("Crate landing outline");for(int i=0;i<plate.Length;i++)plate[i]=Line("Target pressure plate");var lightObject=new GameObject("Current placement target glow");lightObject.transform.SetParent(transform);lightObject.layer=2;targetLight=lightObject.AddComponent<Light>();targetLight.type=LightType.Point;targetLight.color=new Color(.32f,1,.65f);targetLight.range=3;targetLight.intensity=1.3f;targetLight.shadows=LightShadows.None;targetLight.enabled=false;}
    LineRenderer Line(string label){var o=new GameObject(label);o.transform.SetParent(transform);o.layer=2;var line=o.AddComponent<LineRenderer>();line.sharedMaterial=material;line.useWorldSpace=true;line.positionCount=2;line.startWidth=line.endWidth=.035f;line.shadowCastingMode=ShadowCastingMode.Off;line.receiveShadows=false;line.enabled=false;return line;}
    void Show(LineRenderer line,Vector3 a,Vector3 b,Color color){line.enabled=true;line.startColor=line.endColor=color;line.SetPosition(0,a);line.SetPosition(1,b);}
    void LateUpdate(){
        foreach(var l in box)if(l)l.enabled=false;foreach(var l in plate)if(l)l.enabled=false;
        LitTarget=null;if(targetLight)targetLight.enabled=false;
        if(!g||!g.started||g.ended||(g.Frozen&&!g.photoMode&&!g.desktopHold))return;
        LitTarget=g.guide.CurrentPlateGoal;
        if(LitTarget&&LitTarget.plate){
            Vector3 c=LitTarget.plate.position+Vector3.up*.09f;targetLight.transform.position=c+Vector3.up*.5f;targetLight.enabled=true;
            Vector3[] corners={c+new Vector3(-.98f,0,-.98f),c+new Vector3(.98f,0,-.98f),c+new Vector3(.98f,0,.98f),c+new Vector3(-.98f,0,.98f)};
            for(int i=0;i<4;i++){plate[i].startWidth=plate[i].endWidth=.12f;Show(plate[i],corners[i],corners[(i+1)%4],new Color(.32f,1,.65f,.95f));}
            // Fixed upright corner beacons remain visible from a level view, without moving the route.
            for(int i=0;i<4;i++){plate[i+4].startWidth=plate[i+4].endWidth=.075f;Show(plate[i+4],corners[i],corners[i]+Vector3.up*.65f,new Color(.32f,1,.65f,.65f));}
        }
        if(!g.player.held||g.player.held.kind!=Pickup.Kind.Crate){
            var focus=g.player.focus;if(!g.Frozen&&focus&&!(focus is StoryPoint)&&g.player.CanReach(focus)){
                Bounds bounds;var collider=focus.GetComponent<Collider>();
                if(focus is Machine||focus is GridRelay||!collider)bounds=new Bounds(g.player.InteractionPoint(focus),Vector3.one*.46f);
                else bounds=collider.bounds;
                DrawBox(bounds.center,bounds.extents+Vector3.one*.035f,new Color(1,.85f,.48f,1));
            }return;
        }
        var p=g.player;if(p.held.kind!=Pickup.Kind.Crate)return;var gate=g.guide.PlacementGate;
        bool valid=p.Placement(out var at,out _);if(at==Vector3.zero)return;
        bool onPlate=valid&&gate&&new Vector2(at.x-gate.plate.position.x,at.z-gate.plate.position.z).magnitude<.12f;
        Color color=!valid?new Color(1,.3f,.25f,.9f):onPlate?new Color(.3f,1,.55f,.95f):new Color(.65f,.8f,1,.8f);
        Vector3 half=p.held.kind==Pickup.Kind.Crate?new Vector3(.6f,.825f,.6f):Vector3.one*.27f;
        DrawBox(at,half,color);
    }
    void DrawBox(Vector3 at,Vector3 half,Color color){
        Vector3[] points=new Vector3[8];for(int i=0;i<8;i++){float x=(i%4==0||i%4==3)?-half.x:half.x;float z=i%4<2?-half.z:half.z;points[i]=at+new Vector3(x,i<4?-half.y:half.y,z);}
        for(int i=0;i<12;i++)Show(box[i],points[edges[i,0]],points[edges[i,1]],color);
    }
    void OnDestroy(){if(material)Destroy(material);}
}
}
