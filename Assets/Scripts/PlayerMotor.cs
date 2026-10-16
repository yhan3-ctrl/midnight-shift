using UnityEngine;
namespace LastLantern {
[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour {
    public Camera view; public Transform hand; public Pickup held; public Interactable focus;
    public float speed=4.1f; float pitch,fall,bob,stepClock; CharacterController cc;
    public bool keyboardLook;bool wasDragging;
    public static bool SystemShortcutHeld=>Input.GetKey(KeyCode.LeftCommand)||Input.GetKey(KeyCode.RightCommand)||Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.RightControl)||Input.GetKey(KeyCode.LeftAlt)||Input.GetKey(KeyCode.RightAlt);
    public bool MouseLookHeld=>!keyboardLook&&Input.GetMouseButton(1)&&!SystemShortcutHeld; public float Pitch=>pitch; int ignoreMouseFrames;
    public void ResetLookInput(){ignoreMouseFrames=3;}
    void Awake(){cc=GetComponent<CharacterController>();}
    void Update(){
        if(!FactoryGame.I||FactoryGame.I.Frozen||SystemShortcutHeld){focus=null;wasDragging=false;return;}
        float keyYaw=(Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.LeftArrow)?1:0);
        float keyPitch=(Input.GetKey(KeyCode.DownArrow)?1:0)-(Input.GetKey(KeyCode.UpArrow)?1:0);
        if(Input.GetKeyDown(KeyCode.K)){keyboardLook=!keyboardLook;FactoryGame.I.Notify(keyboardLook?"Arrow-key look / mouse look off. K switches back.":"Hold right mouse to look; release to use your pointer. Arrows also work. K selects arrows only.");ResetLookInput();}
        bool dragging=MouseLookHeld;if(dragging&&!wasDragging)ignoreMouseFrames=2;wasDragging=dragging;
        bool mouse=dragging&&ignoreMouseFrames<=0;if(ignoreMouseFrames>0)ignoreMouseFrames--;
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
    // Precise aim remains possible; nearby visible targets have a forgiving interaction cone.
    public Vector3 InteractionPoint(Interactable item){
        if(item is Machine machine)return machine.corePoint.position;
        if(item is GridRelay relay)return relay.socket.position;
        if(item is StoryPoint)return item.transform.position+Vector3.up*.6f;
        return item.transform.position;
    }
    public bool CanReach(Interactable item){
        if(!item||!item.gameObject.activeInHierarchy||item.transform.IsChildOf(transform))return false;
        Vector3 point=InteractionPoint(item),delta=point-view.transform.position;
        if(delta.magnitude>3.8f||Vector3.Dot(view.transform.forward,delta.normalized)<.38f)return false;
        var hits=Physics.RaycastAll(view.transform.position,delta.normalized,delta.magnitude,~0,QueryTriggerInteraction.Ignore);
        foreach(var hit in hits){
            if(hit.transform.IsChildOf(transform)||hit.transform.IsChildOf(item.transform))continue;
            if(item is Pickup pickup){if(pickup.socket&&hit.transform.IsChildOf(pickup.socket.transform))continue;var relay=pickup.GetComponentInParent<GridRelay>();if(relay&&hit.transform.IsChildOf(relay.transform))continue;}
            return false;
        }
        return true;
    }
    public void ScanFocus(){
        focus=null;var g=FactoryGame.I;if(!g)return;
        // A visible reusable core can be taken directly even while its own collider is disabled in a socket.
        if(!held&&g.sharedCore&&!g.sharedCore.carried&&CanReach(g.sharedCore)){focus=g.sharedCore.socket&&!g.sharedCore.socket.done&&!g.sharedCore.socket.running?(Interactable)g.sharedCore.socket:g.sharedCore;return;}
        if(Physics.Raycast(view.transform.position,view.transform.forward,out RaycastHit hit,3.8f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore))focus=hit.collider.GetComponentInParent<Interactable>();
        if(focus)return;
        float best=0;
        foreach(var item in FindObjectsByType<Interactable>(FindObjectsSortMode.None)){
            if(item is Pickup pickup&&(pickup.carried||pickup.socket))continue;
            if(!(item is Machine)&&!(item is GridRelay)&&!(item is Pickup))continue;
            if(!CanReach(item))continue;
            Vector3 d=InteractionPoint(item)-view.transform.position;float score=Vector3.Dot(view.transform.forward,d.normalized)-d.magnitude*.045f;
            if(score>best){best=score;focus=item;}
        }
    }
    public bool TryInteract(){ScanFocus();if(!focus){FactoryGame.I.Notify(UiCopy.Pick("靠近目标并朝向它，再按 E。","Move near the object, face it, then press E."),3);return false;}focus.Use(this);return true;}
    public bool Pick(Pickup item){
        if(held){FactoryGame.I.Notify("Your hands are full. Place what you're carrying first.");return false;}
        held=item;item.AttachTo(hand);item.carried=true;if(item.kind==Pickup.Kind.Crate){item.transform.localScale*=.26f;item.transform.localPosition=new Vector3(.3f,-.44f,.08f);}
        if(item.kind==Pickup.Kind.Core){item.transform.localScale*=.5f;item.transform.localPosition=new Vector3(.1f,-.25f,0);}
        FactoryGame.I.audioScore.Tick(660);return true;
    }
    public bool Placement(out Vector3 at,out string reason){
        at=Vector3.zero;reason="";
        if(!held){reason=UiCopy.Pick("先拿起一个物品。","Pick up an item first.");return false;}
        Vector3 ahead=transform.position+transform.forward*1.5f+Vector3.up*2;Vector3 surface;
        var target=FactoryGame.I.guide.PlacementGate;Vector3 toward=target&&target.plate?target.plate.position-transform.position:Vector3.zero;toward.y=0;
        bool directPlate=held.kind==Pickup.Kind.Crate&&target&&target.plate&&toward.magnitude<=3&&Vector3.Dot(transform.forward,toward.normalized)>.35f;
        if(directPlate&&Physics.Linecast(view.transform.position,target.plate.position+Vector3.up*.12f,out RaycastHit barrier,~0,QueryTriggerInteraction.Ignore)&&!barrier.transform.IsChildOf(target.plate))directPlate=false;
        if(directPlate)surface=target.plate.position+Vector3.up*.05f;
        else {
            if(!Physics.Raycast(ahead,Vector3.down,out RaycastHit ground,3.5f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore)||ground.normal.y<.85f){reason=UiCopy.Pick("这里放不下，找一块平整的空地。","Find a clear, level patch of floor.");return false;}
            surface=ground.point;
            if(surface.y>.25f){reason=UiCopy.Pick("请放在地上，避开柜子和工作台。","Place it on the floor, away from shelves and worktops.");return false;}
            if(held.kind==Pickup.Kind.Crate){float nearest=1.35f;foreach(var gate in FindObjectsByType<FactoryGate>(FindObjectsSortMode.None)){
                if(!gate.plate||Mathf.Abs(gate.plate.position.y-surface.y)>.25f)continue;
                Vector3 delta=gate.plate.position-ahead;delta.y=0;
                if(delta.magnitude<nearest){nearest=delta.magnitude;surface=gate.plate.position+Vector3.up*.05f;}
            }}
        }
        at=surface+Vector3.up*(held.kind==Pickup.Kind.Crate?.835f:.43f);
        Vector3 half=held.kind==Pickup.Kind.Crate?new Vector3(.58f,.8f,.58f):Vector3.one*.25f;
        Vector3 separation=at-transform.position;separation.y=0;
        if(separation.magnitude<(held.kind==Pickup.Kind.Crate?1.0f:.65f)){reason=UiCopy.Pick("离条纹板太近：退后一步再放。","Too close to the plate: take one step back.");return false;}
        if(Physics.Linecast(view.transform.position,at,out RaycastHit wall,~0,QueryTriggerInteraction.Ignore)&&!wall.collider.transform.IsChildOf(transform)){reason=UiCopy.Pick("墙挡住了落点，请先从门口通过。","The wall blocks this landing spot. Move through the doorway first.");return false;}
        foreach(var c in Physics.OverlapBox(at,half,Quaternion.identity,~0,QueryTriggerInteraction.Ignore)){
            if(c.transform.IsChildOf(transform)||c.transform.IsChildOf(held.transform))continue;
            if(c.bounds.max.y<=surface.y+.08f)continue;
            reason=UiCopy.Pick("前方有东西挡着：换一块空地。","Something blocks this spot. Find clear floor.");return false;
        }
        return true;
    }
    public bool ReadyOnPlate(FactoryGate gate){if(!gate||!gate.plate||!Placement(out var at,out _))return false;var d=at-gate.plate.position;d.y=0;return d.magnitude<.12f;}
    public void TryPlace(){
        if(!Placement(out Vector3 at,out string reason)){FactoryGame.I.Notify(reason,4);return;}
        held.PlaceAt(at);held=null;FactoryGame.I.audioScore.Tick(330);Physics.SyncTransforms();
    }
    public void PutBackHeld(){if(!held)return;held.ResetHome();held=null;}
    public void ReleaseForDelivery(){if(!held)return;var item=held;held=null;item.transform.SetParent(null);item.carried=false;item.transform.position=new Vector3(.2f,1.45f,60);item.SetColliders(false);}
    public void Teleport(Vector3 pos,float yaw){cc.enabled=false;transform.position=pos;transform.rotation=Quaternion.Euler(0,yaw,0);cc.enabled=true;fall=0;pitch=0;view.transform.localRotation=Quaternion.identity;ResetLookInput();}
}
}
