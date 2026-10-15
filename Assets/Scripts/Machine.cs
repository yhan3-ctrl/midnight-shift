using UnityEngine;
namespace LastLantern {
public class Machine : Interactable {
    public string machineId,displayName; public Pickup core; public Transform corePoint,spinner; public Renderer indicator; public TextMesh status;
    public float progress; public bool running,done; public float workSeconds=5;
    public bool Powered=>core!=null;
    public override string Prompt(PlayerMotor p){
        if(p.held&&p.held.kind==Pickup.Kind.Core&&!core)return "E  Insert core / "+displayName;
        if(p.held)return "Place your item first / Q";
        if(core)return "E  "+(running?"Stop & remove core":done?"Remove core":"Start machine")+" / "+displayName;
        return "Needs power core / "+displayName;
    }
    public override void Use(PlayerMotor p){
        if(p.held){
            if(p.held.kind!=Pickup.Kind.Core||core){FactoryGame.I.Notify("This socket accepts one power core.");return;}
            core=p.held;p.held=null;if(machineId=="chime"){done=false;progress=0;}core.carried=false;core.socket=this;core.AttachTo(corePoint);FactoryGame.I.audioScore.Tick(550);return;
        }
        if(!core){FactoryGame.I.Notify("Bring a power core to this machine.");return;}
        if(running||done){Remove(p);return;}
        if(machineId=="ribbon"&&!FactoryGame.I.shell){FactoryGame.I.Notify("Make the shell at the Moulding Table first.");return;}
        if(machineId=="etch"&&!FactoryGame.I.blueprint){FactoryGame.I.Notify("Find the old drawing in the archive first.");return;}
        if(machineId=="press"&&!FactoryGame.I.etched){FactoryGame.I.Notify("Engrave the shell at the Engraving Table first.");return;}
        running=true;if(machineId=="chime"){FactoryGame.I.inspectorLesson=true;FactoryGame.I.Notify("Hush is coming to check the chime. Watch his route, then use the archive plate.",7);}
        FactoryGame.I.audioScore.Tick(440);if(FactoryGame.I.inspector)FactoryGame.I.inspector.Hear(transform.position);
    }
    public void Remove(PlayerMotor p){
        if(!core||p.held)return;running=false;var item=core;core=null;item.socket=null;p.Pick(item);
    }
    void Update(){
        if(!FactoryGame.I||FactoryGame.I.Frozen)return;
        if(!Powered)running=false;
        if(running){
            if(spinner)spinner.Rotate(0,80*Time.deltaTime,0);
            progress+=Time.deltaTime;
            if(progress>=workSeconds){running=false;if(machineId=="chime"){progress=0;done=true;}else if(!done){done=true;FactoryGame.I.Complete(machineId);}}
        }
        if(indicator)indicator.material.color=done?new Color(.5f,.9f,.65f):Powered?new Color(1,.7f,.2f):new Color(.3f,.35f,.38f);
        if(status)status.text=done?"COMPLETE / CORE REUSABLE":running?"WORKING "+Mathf.CeilToInt(progress/workSeconds*100)+"%":Powered?"POWERED / E TO START":"INSERT CORE";
    }
}
}
