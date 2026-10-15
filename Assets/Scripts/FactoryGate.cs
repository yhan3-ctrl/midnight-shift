using UnityEngine;
namespace LastLantern {
public class FactoryGate : MonoBehaviour {
    public enum Rule { Plate, Packaging, Drawing }
    public Rule rule; public Transform panel,plate; public TextMesh label; public Vector3 closedPosition; public bool isOpen;
    public bool Evaluate(){
        if(rule==Rule.Packaging)return FactoryGame.I&&FactoryGame.I.shell&&FactoryGame.I.transferred;
        if(rule==Rule.Drawing)return FactoryGame.I&&FactoryGame.I.blueprint;
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
        if(!isOpen&&Vector3.Distance(new Vector3(FactoryGame.I.player.transform.position.x,0,FactoryGame.I.player.transform.position.z),new Vector3(closedPosition.x,0,closedPosition.z))<1.9f)target=closedPosition+Vector3.up*3.7f;
        panel.position=Vector3.MoveTowards(panel.position,target,Time.deltaTime*4);
        if(label)label.text=isOpen?"OPEN":rule==Rule.Plate?"HOLD PLATE TO OPEN":rule==Rule.Packaging?"COMPLETE BOTH MACHINES":"FIND THE DRAWING";
    }
}
}
