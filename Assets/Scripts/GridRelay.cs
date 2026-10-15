using UnityEngine;
namespace LastLantern {
// Reusable continuous-power gate: a crate can preserve access while its core is reused.
public class GridRelay : Interactable {
    public Pickup core; public Transform socket; public TextMesh label; public FactoryGate gate; bool overrideWasHeld;
    public bool OverrideHeld=>gate&&gate.CrateHoldsPlate;
    public override string Prompt(PlayerMotor p)=>core?(OverrideHeld?"E  Remove core / crate keeps this gate open":"E  Remove core / this gate will lose power"):p.held&&p.held.kind==Pickup.Kind.Core?"E  Power the workshop gate":"Needs core / WORKSHOP RELAY";
    public override void Use(PlayerMotor p){
        if(core){if(p.held){FactoryGame.I.Notify("Place what you're carrying first.");return;}var item=core;core=null;p.Pick(item);return;}
        if(!p.held||p.held.kind!=Pickup.Kind.Core){FactoryGame.I.Notify("Insert the power core to open the workshop. The striped plate is a manual override.",6);return;}
        core=p.held;p.held=null;core.carried=false;core.AttachTo(socket);FactoryGame.I.audioScore.Tick(550);
    }
    void Update(){
        bool held=OverrideHeld;
        if(held&&!overrideWasHeld&&FactoryGame.I&&!FactoryGame.I.Frozen)FactoryGame.I.Notify("CRATE OVERRIDE SET / The gate will stay open. You can now take the core to Moulding.",7);
        overrideWasHeld=held;
        if(label)label.text=held?"PLATE HOLDS GATE / CORE FREE":core?"GATE POWERED / CORE REMOVABLE":"NEEDS CORE / PLATE OVERRIDE";
    }
}
}
