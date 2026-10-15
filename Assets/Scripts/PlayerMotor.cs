using UnityEngine;
namespace LastLantern {
[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour {
    public Camera view; public Transform hand; public Pickup held; public Interactable focus;
    public float speed=4.1f; float pitch,fall,bob; CharacterController cc;
    void Awake(){cc=GetComponent<CharacterController>();}
    void Update(){
        if(!FactoryGame.I||FactoryGame.I.Frozen){focus=null;return;}
        transform.Rotate(0,Input.GetAxisRaw("Mouse X")*FactoryGame.I.sensitivity,0);
        pitch=Mathf.Clamp(pitch-Input.GetAxisRaw("Mouse Y")*FactoryGame.I.sensitivity,-72,72);
        view.transform.localRotation=Quaternion.Euler(pitch,0,0);
        Vector3 move=Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")),1);
        fall=cc.isGrounded?-2:fall-22*Time.deltaTime;
        cc.Move((transform.TransformDirection(move)*speed+Vector3.up*fall)*Time.deltaTime);
        bob+=move.magnitude*Time.deltaTime*6;
        hand.localPosition=Vector3.Lerp(hand.localPosition,new Vector3(.25f,-.33f+Mathf.Sin(bob)*.012f,.8f),Time.deltaTime*12);
        focus=null;
        if(Physics.Raycast(view.transform.position,view.transform.forward,out RaycastHit hit,3.1f,~0,QueryTriggerInteraction.Ignore))focus=hit.collider.GetComponentInParent<Interactable>();
        if(Input.GetKeyDown(KeyCode.E)&&focus)focus.Use(this);
        if(Input.GetKeyDown(KeyCode.Q)&&held)TryPlace();
        if(transform.position.y < -4)Teleport(new Vector3(0,0,-6),0);
    }
    public bool Pick(Pickup item){
        if(held){FactoryGame.I.Notify("Your hands are full. Place what you're carrying first.");return false;}
        held=item;item.carried=true; item.SetColliders(false);item.transform.SetParent(hand);item.transform.localPosition=Vector3.zero;item.transform.localRotation=Quaternion.identity;
        FactoryGame.I.audioScore.Tick(660);return true;
    }
    public void TryPlace(){
        Vector3 ahead=transform.position+transform.forward*1.5f+Vector3.up*2;
        if(!Physics.Raycast(ahead,Vector3.down,out RaycastHit ground,3.5f,~0,QueryTriggerInteraction.Ignore)||ground.normal.y<.85f){FactoryGame.I.Notify("Find a clear, level surface to place it.");return;}
        Vector3 at=ground.point+Vector3.up*(held.kind==Pickup.Kind.Crate?.61f:.43f);
        Vector3 half=held.kind==Pickup.Kind.Crate?new Vector3(.58f,.58f,.58f):Vector3.one*.25f;
        foreach(var c in Physics.OverlapBox(at,half,Quaternion.identity,~0,QueryTriggerInteraction.Ignore)){
            if(c.transform.IsChildOf(transform)||c.transform.IsChildOf(held.transform))continue;
            if(c.bounds.max.y<=ground.point.y+.08f)continue;
            FactoryGame.I.Notify("There's not enough room here. Try a clear patch of floor.");return;
        }
        held.transform.SetParent(null);held.transform.position=at;held.transform.rotation=Quaternion.identity;held.carried=false;held.SetColliders(true);held=null;FactoryGame.I.audioScore.Tick(330);
        Physics.SyncTransforms();
    }
    public void PutBackHeld(){if(!held)return;held.ResetHome();held=null;}
    public void ReleaseForDelivery(){if(!held)return;var item=held;held=null;item.transform.SetParent(null);item.carried=false;item.transform.position=new Vector3(.2f,1.45f,60);item.SetColliders(false);}
    public void Teleport(Vector3 pos,float yaw){cc.enabled=false;transform.position=pos;transform.rotation=Quaternion.Euler(0,yaw,0);cc.enabled=true;fall=0;pitch=0;}
}
}
