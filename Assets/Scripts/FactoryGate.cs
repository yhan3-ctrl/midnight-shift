using UnityEngine;
namespace LastLantern {
public class FactoryGate : MonoBehaviour {
    public enum Rule { Plate, PowerOrPlate, Packaging, Drawing }
    public bool requireLesson; public Rule rule; public GridRelay relay; public Transform panel,plate; public TextMesh label; public Vector3 closedPosition; public bool isOpen;
    public bool CrateHoldsPlate {
        get {
            if(!plate)return false;
            foreach(var c in Physics.OverlapBox(plate.position+Vector3.up*.65f,new Vector3(.86f,.7f,.86f),Quaternion.identity,~0,QueryTriggerInteraction.Ignore)){
                var item=c.GetComponentInParent<Pickup>();if(item&&item.kind==Pickup.Kind.Crate&&!item.carried)return true;
            }
            return false;
        }
    }
    public bool Evaluate(){
        if(requireLesson&&(!FactoryGame.I||!FactoryGame.I.inspectorLesson))return false;
        if(rule==Rule.Packaging)return FactoryGame.I&&FactoryGame.I.shell&&FactoryGame.I.transferred;
        if(rule==Rule.Drawing)return FactoryGame.I&&FactoryGame.I.blueprint;
        if(rule==Rule.PowerOrPlate&&relay&&relay.core)return true;
        if(!plate)return false;
        foreach(var c in Physics.OverlapBox(plate.position+Vector3.up*.65f,new Vector3(.86f,.7f,.86f),Quaternion.identity,~0,QueryTriggerInteraction.Ignore)){
            var item=c.GetComponentInParent<Pickup>();
            if(item&&item.kind==Pickup.Kind.Crate&&!item.carried)return true;
            if(c.GetComponent<PlayerMotor>())return true;
        }
        return false;
    }
    void Update(){
        if(!FactoryGame.I||FactoryGame.I.Frozen)return;isOpen=Evaluate();
        Vector3 target=closedPosition+Vector3.up*(isOpen?3.7f:0);
        // A closing door never pushes the player or a carried object into geometry.
        var player=FactoryGame.I.player.transform.position;
        bool inDoorway=Mathf.Abs(player.z-closedPosition.z)<.65f&&Mathf.Abs(player.x-closedPosition.x)<1.9f;
        if(!isOpen&&panel.position.y-closedPosition.y>1.85f&&inDoorway)target=panel.position;
        panel.position=Vector3.MoveTowards(panel.position,target,Time.deltaTime*4);
        if(label)label.text=requireLesson&&!FactoryGame.I.inspectorLesson?"START THE BREAK-TIME CHIME":isOpen?"OPEN":(rule==Rule.Plate||rule==Rule.PowerOrPlate)?"HOLD PLATE TO OPEN":rule==Rule.Packaging?"COMPLETE BOTH MACHINES":"FIND THE DRAWING";
    }
}
}
