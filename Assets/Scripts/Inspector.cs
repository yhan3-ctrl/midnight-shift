using UnityEngine;
namespace LastLantern {
public class Inspector : MonoBehaviour {
    public enum State { Patrol, Investigate, Alert, Approach, Search, Return }
    public State state;
    public Transform visual,eyes,armLeft,armRight;
    public Vector3[] route;
    public float Alertness=>alert; public bool assemblyDuty;
    int waypoint; float timer,alert,graceUntil; Vector3 destination,lastSeen; Quaternion facing;
    void Start(){ResetPatrol();}
    public void ResetPatrol(bool grantGrace=false){graceUntil=grantGrace?Time.time+2.5f:0;if(route==null||route.Length==0)return;transform.position=route[0];waypoint=1;state=State.Patrol;alert=0;timer=0;facing=Quaternion.identity;}
    public void Hear(Vector3 location){
        if(state==State.Alert||state==State.Approach)return;
        float min=assemblyDuty?47:31,max=assemblyDuty?54:46;
        if(location.z<min||location.z>max)return;
        destination=new Vector3(Mathf.Clamp(location.x,-7,7),0,Mathf.Clamp(location.z-1.6f,min+1,max-1));state=State.Investigate;timer=0;
    }
    public bool CanSeePlayer(){
        if(Time.time<graceUntil||(!assemblyDuty&&!FactoryGame.I.inspectorLesson))return false;
        var p=FactoryGame.I.player;Vector3 target=p.transform.position+Vector3.up*1.1f;
        Vector3 delta=target-eyes.position;
        float lower=assemblyDuty?47:31.5f,upper=assemblyDuty?53.3f:44.8f;
        if(p.transform.position.z<lower||p.transform.position.z>upper||delta.magnitude>7)return false;
        if(Vector3.Angle(transform.forward,new Vector3(delta.x,0,delta.z))>55)return false;
        return Physics.Raycast(eyes.position,delta.normalized,out RaycastHit hit,delta.magnitude+.1f,~0,QueryTriggerInteraction.Ignore)&&hit.collider.GetComponent<PlayerMotor>();
    }
    void Update(){
        if(!FactoryGame.I||FactoryGame.I.Frozen)return;
        float dt=Time.deltaTime;
        if(!assemblyDuty&&FactoryGame.I.blueprint&&FactoryGame.I.player.transform.position.z>46.8f){
            assemblyDuty=true;route=new[]{new Vector3(4,0,52),new Vector3(-1,0,50),new Vector3(-1,0,52),new Vector3(4,0,50)};ResetPatrol(true);
            FactoryGame.I.Notify("Hush: 'Final inspection! I'll check the machines as you finish each job.'",7);
        }
        bool seen=CanSeePlayer();
        if(seen&&state!=State.Alert&&state!=State.Approach)FactoryGame.I.audioScore.Tick(880);
        if(seen){lastSeen=FactoryGame.I.player.transform.position;lastSeen.y=0;alert=Mathf.Min(1,alert+dt/.95f);if(state!=State.Approach)state=State.Alert;if(alert>=1)state=State.Approach;}
        else {alert=Mathf.Max(0,alert-dt*.5f);if(state==State.Alert||state==State.Approach){state=State.Search;timer=0;destination=lastSeen;}}
        switch(state){
            case State.Patrol: Move(route[waypoint],1.45f);if(Distance(route[waypoint])<.3f)waypoint=(waypoint+1)%route.Length;break;
            case State.Investigate: Move(destination,1.8f);if(Distance(destination)<.4f){timer+=dt;if(timer>5){state=State.Return;timer=0;}}break;
            case State.Alert: Face(lastSeen);break;
            case State.Approach: Move(lastSeen,2.8f);if(Distance(lastSeen)<1.1f)FactoryGame.I.ResetSection();break;
            case State.Search: Move(destination,1.5f);timer+=dt;if(timer>4){state=State.Return;timer=0;}break;
            case State.Return: Move(route[waypoint],1.6f);if(Distance(route[waypoint])<.4f)state=State.Patrol;break;
        }
        if(visual){visual.localPosition=Vector3.up*(1.1f+Mathf.Sin(Time.time*2.5f)*.12f);visual.localRotation=Quaternion.Euler(0,180,Mathf.Sin(Time.time*2)*4);}
        if(armLeft)armLeft.localRotation=Quaternion.Euler(0,0,18+Mathf.Sin(Time.time*3)*12+alert*28);
        if(armRight)armRight.localRotation=Quaternion.Euler(0,0,-18-Mathf.Sin(Time.time*3)*12-alert*28);
    }
    float Distance(Vector3 p)=>Vector2.Distance(new Vector2(transform.position.x,transform.position.z),new Vector2(p.x,p.z));
    void Face(Vector3 p){Vector3 d=p-transform.position;d.y=0;if(d.sqrMagnitude>.01f)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(d),Time.deltaTime*5);}
    void Move(Vector3 p,float speed){
        Vector3 d=p-transform.position;d.y=0;if(d.magnitude<.15f)return;Face(p);
        Vector3 step=d.normalized*speed*Time.deltaTime;
        if(!Physics.SphereCast(transform.position+Vector3.up*.65f,.36f,step.normalized,out RaycastHit hit,step.magnitude+.12f,~0,QueryTriggerInteraction.Ignore)||hit.collider.GetComponent<PlayerMotor>())transform.position+=step;
        else {
            Vector3 side=Vector3.Cross(Vector3.up,d.normalized);if(transform.position.x>0)side=-side;
            if(!Physics.SphereCast(transform.position+Vector3.up*.65f,.36f,side,out _,step.magnitude+.12f,~0,QueryTriggerInteraction.Ignore))transform.position+=side*step.magnitude;
        }
    }
}
}
