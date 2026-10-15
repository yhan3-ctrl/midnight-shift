using UnityEngine;
namespace LastLantern {
[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour {
    public Camera view; public Transform hand; public Pickup held; public Interactable focus;
    public float speed=4.1f; float pitch,fall,bob,stepClock; CharacterController cc;
    public bool keyboardLook; public float Pitch=>pitch; int ignoreMouseFrames;
    public void ResetLookInput(){ignoreMouseFrames=3;}
    void Awake(){cc=GetComponent<CharacterController>();}
    void Update(){
        if(!FactoryGame.I||FactoryGame.I.Frozen){focus=null;return;}
        float keyYaw=(Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.LeftArrow)?1:0);
        float keyPitch=(Input.GetKey(KeyCode.DownArrow)?1:0)-(Input.GetKey(KeyCode.UpArrow)?1:0);
        if(Input.GetKeyDown(KeyCode.K)){keyboardLook=!keyboardLook;FactoryGame.I.Notify(keyboardLook?"Arrow-key look / mouse look off. K switches back.":"Mouse and arrow-key look. K switches to arrows only.");ResetLookInput();}
        bool mouse=!keyboardLook&&ignoreMouseFrames<=0;if(ignoreMouseFrames>0)ignoreMouseFrames--;
        transform.Rotate(0,(mouse?Input.GetAxisRaw("Mouse X")*FactoryGame.I.sensitivity:0)+keyYaw*85*Time.deltaTime,0);
        pitch=Mathf.Clamp(pitch-(mouse?Input.GetAxisRaw("Mouse Y")*FactoryGame.I.sensitivity:0)+keyPitch*65*Time.deltaTime,-72,72);
        view.transform.localRotation=Quaternion.Euler(pitch,0,0);
        // Unity legacy axes also contain arrow keys; reserve those exclusively for looking.
        Vector3 move=Vector3.ClampMagnitude(new Vector3((Input.GetKey(KeyCode.D)?1:0)-(Input.GetKey(KeyCode.A)?1:0),0,(Input.GetKey(KeyCode.W)?1:0)-(Input.GetKey(KeyCode.S)?1:0)),1);
        fall=cc.isGrounded?-2:fall-22*Time.deltaTime;
        cc.Move((transform.TransformDirection(move)*speed+Vector3.up*fall)*Time.deltaTime);
        bob+=move.magnitude*Time.deltaTime*6;
        stepClock+=move.magnitude*Time.deltaTime;if(stepClock>.48f&&cc.isGrounded&&cc.velocity.magnitude>.2f){stepClock=0;FactoryGame.I.audioScore.Step();}
        hand.localPosition=Vector3.Lerp(hand.localPosition,new Vector3(.25f,-.33f+Mathf.Sin(bob)*.012f,.8f),Time.deltaTime*12);
        ScanFocus();
        if(Input.GetKeyDown(KeyCode.E))TryInteract();
        if(Input.GetKeyDown(KeyCode.Q)&&held)TryPlace();
        if(transform.position.y < -4)Teleport(new Vector3(0,0,-6),0);
    }
    public void ScanFocus(){
        focus=null;
        if(Physics.Raycast(view.transform.position,view.transform.forward,out RaycastHit hit,3.1f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore))focus=hit.collider.GetComponentInParent<Interactable>();
    }
    public bool TryInteract(){ScanFocus();if(!focus)return false;focus.Use(this);return true;}
    public bool Pick(Pickup item){
        if(held){FactoryGame.I.Notify("Your hands are full. Place what you're carrying first.");return false;}
        held=item;item.AttachTo(hand);item.carried=true;if(item.kind==Pickup.Kind.Crate){item.transform.localScale*=.45f;item.transform.localPosition=new Vector3(.15f,-.36f,.08f);}
        if(item.kind==Pickup.Kind.Core){item.transform.localScale*=.5f;item.transform.localPosition=new Vector3(.1f,-.25f,0);}
        FactoryGame.I.audioScore.Tick(660);return true;
    }
    public bool Placement(out Vector3 at,out string reason){
        at=Vector3.zero;reason="";
        if(!held){reason=UiCopy.Pick("先拿起一个物品。","Pick up an item first.");return false;}
        Vector3 ahead=transform.position+transform.forward*1.5f+Vector3.up*2;
        if(!Physics.Raycast(ahead,Vector3.down,out RaycastHit ground,3.5f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore)||ground.normal.y<.85f){reason=UiCopy.Pick("这里放不下，找一块平整的空地。","Find a clear, level patch of floor.");return false;}
        Vector3 surface=ground.point;
        if(surface.y>.25f){reason=UiCopy.Pick("请放在地上，避开柜子和工作台。","Place it on the floor, away from shelves and worktops.");return false;}
        if(held.kind==Pickup.Kind.Crate){float nearest=1.35f;foreach(var gate in FindObjectsByType<FactoryGate>(FindObjectsSortMode.None)){
            if(!gate.plate||Mathf.Abs(gate.plate.position.y-surface.y)>.25f)continue;
            Vector3 delta=gate.plate.position-ahead;delta.y=0;
            if(delta.magnitude<nearest){nearest=delta.magnitude;surface=gate.plate.position+Vector3.up*.05f;}
        }}
        at=surface+Vector3.up*(held.kind==Pickup.Kind.Crate?.835f:.43f);
        Vector3 half=held.kind==Pickup.Kind.Crate?new Vector3(.58f,.8f,.58f):Vector3.one*.25f;
        Vector3 separation=at-transform.position;separation.y=0;
        if(separation.magnitude<(held.kind==Pickup.Kind.Crate?1.0f:.65f)){reason=UiCopy.Pick("离条纹板太近：退后一步再放。","Too close to the plate: take one step back.");return false;}
        foreach(var c in Physics.OverlapBox(at,half,Quaternion.identity,~0,QueryTriggerInteraction.Ignore)){
            if(c.transform.IsChildOf(transform)||c.transform.IsChildOf(held.transform))continue;
            if(c.bounds.max.y<=surface.y+.08f)continue;
            reason=UiCopy.Pick("前方有东西挡着：换一块空地。","Something blocks this spot. Find clear floor.");return false;
        }
        return true;
    }
    public void TryPlace(){
        if(!Placement(out Vector3 at,out string reason)){FactoryGame.I.Notify(reason,4);return;}
        held.PlaceAt(at);held=null;FactoryGame.I.audioScore.Tick(330);Physics.SyncTransforms();
    }
    public void PutBackHeld(){if(!held)return;held.ResetHome();held=null;}
    public void ReleaseForDelivery(){if(!held)return;var item=held;held=null;item.transform.SetParent(null);item.carried=false;item.transform.position=new Vector3(.2f,1.45f,60);item.SetColliders(false);}
    public void Teleport(Vector3 pos,float yaw){cc.enabled=false;transform.position=pos;transform.rotation=Quaternion.Euler(0,yaw,0);cc.enabled=true;fall=0;pitch=0;view.transform.localRotation=Quaternion.identity;ResetLookInput();}
}
}
