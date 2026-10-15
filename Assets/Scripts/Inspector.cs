using UnityEngine;
namespace LastLantern {
public class Inspector : MonoBehaviour {
    public enum State { Patrol, Investigate, Alert, Approach, Search, Return }
    public State state;
    public Transform visual,eyes,armLeft,armRight;
    public Vector3[] route;
    public float Alertness=>alert;
    int waypoint; float timer,alert; Vector3 destination,lastSeen; Quaternion facing;
    void Start(){ResetPatrol();}
    public void ResetPatrol(){if(route==null||route.Length==0)return;transform.position=route[0];waypoint=1;state=State.Patrol;alert=0;timer=0;facing=Quaternion.identity;}
    public void Hear(Vector3 location){if(state==State.Alert||state==State.Approach)return;if(location.z<31||location.z>46)return;destination=new Vector3(Mathf.Clamp(location.x,-7,7),0,Mathf.Clamp(location.z,33,43));state=State.Investigate;timer=0;}
    bool SeesPlayer(){
        var p=FactoryGame.I.player;Vector3 target=p.transform.position+Vector3.up*1.1f;
        Vector3 delta=target-eyes.position;
        if(p.transform.position.z<31.5f||p.transform.position.z>44.8f||delta.magnitude>7)return false;
        if(Vector3.Angle(transform.forward,new Vector3(delta.x,0,delta.z))>55)return false;
        return Physics.Raycast(eyes.position,delta.normalized,out RaycastHit hit,delta.magnitude+.1f,~0,QueryTriggerInteraction.Ignore)&&hit.collider.GetComponent<PlayerMotor>();
    }
    void Update(){
        if(!FactoryGame.I||FactoryGame.I.Frozen)return;
        float dt=Time.deltaTime;
        bool seen=SeesPlayer();
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
        if(visual){visual.localPosition=Vector3.up*(1.1f+Mathf.Sin(Time.time*2.5f)*.12f);visual.localRotation=Quaternion.Euler(0,0,Mathf.Sin(Time.time*2)*4);}
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
