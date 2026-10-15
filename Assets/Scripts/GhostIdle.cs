using UnityEngine;
namespace LastLantern {
public class GhostIdle : MonoBehaviour {
    public string role;
    public bool Welcomed {get;private set;}
    public bool Celebrating=>celebration>0;
    public int Celebrations {get;private set;}
    Transform body,left,right;Vector3 home,leftHome,rightHome;
    float wave,celebration;bool atWorkshop;
    void Start(){
        body=transform.Find("Ghost body");if(!body)return;home=body.localPosition;
        left=body.Find("Left sleeve");right=body.Find("Right sleeve");
        if(left)leftHome=left.localPosition;if(right)rightHome=right.localPosition;
    }
    public void Celebrate(){celebration=3;Celebrations++;}
    void Update(){
        if(!body)return;var g=FactoryGame.I;
        if(g&&g.started&&!g.ended){
            if(role=="button"&&!atWorkshop&&g.player.transform.position.z>18.8f){atWorkshop=true;Welcomed=false;transform.position=new Vector3(-7.5f,0,21.4f);}
            Vector3 delta=g.player.transform.position-transform.position;delta.y=0;
            if(delta.magnitude<4.5f){
                if(!Welcomed&&!g.Frozen){Welcomed=true;wave=2.5f;}
                if(delta.sqrMagnitude>.1f)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(-delta),Time.deltaTime*3);
            }
        }
        float dt=g&&g.Frozen?0:Time.deltaTime;
        wave=Mathf.Max(0,wave-dt);celebration=Mathf.Max(0,celebration-dt);
        body.localPosition=home+Vector3.up*(Mathf.Sin(Time.time*2)*.1f+(Celebrating?Mathf.Abs(Mathf.Sin(Time.time*6))*.12f:0));
        body.localRotation=Quaternion.Euler(0,Mathf.Sin(Time.time)*6,Mathf.Sin(Time.time*1.5f)*3);
        if(left){left.localPosition=leftHome;left.localRotation=Quaternion.Euler(0,0,Celebrating?-65:0);}
        if(right){right.localPosition=rightHome;right.localRotation=Quaternion.Euler(0,0,Celebrating?65:wave>0?80+Mathf.Sin(Time.time*12)*25:0);}
    }
}
}
