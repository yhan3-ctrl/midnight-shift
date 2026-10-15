using UnityEngine;
namespace LastLantern {
// Reusable continuous-power gate: a crate can preserve access while its core is reused.
public class GridRelay : Interactable {
    public Pickup core; public Transform socket; public TextMesh label;
    public override string Prompt(PlayerMotor p)=>core?"E  Remove core / this gate will lose power":p.held&&p.held.kind==Pickup.Kind.Core?"E  Power the workshop gate":"Needs core / WORKSHOP RELAY";
    public override void Use(PlayerMotor p){
        if(core){if(p.held){FactoryGame.I.Notify("Place what you're carrying first.");return;}var item=core;core=null;p.Pick(item);return;}
        if(!p.held||p.held.kind!=Pickup.Kind.Core){FactoryGame.I.Notify("Insert the power core to open the workshop. The striped plate is a manual override.",6);return;}
        core=p.held;p.held=null;core.carried=false;core.AttachTo(socket);FactoryGame.I.audioScore.Tick(550);
    }
    void Update(){if(label)label.text=core?"GATE POWERED / CORE REMOVABLE":"NEEDS CORE / PLATE OVERRIDE";}
}
}
