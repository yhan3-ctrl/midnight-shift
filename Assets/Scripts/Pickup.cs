using UnityEngine;
namespace LastLantern {
public class Pickup : Interactable {
    public enum Kind { Crate, Core, Lantern }
    public Kind kind; public string displayName; public Vector3 home; public bool carried; public Machine socket;
    public override string Prompt(PlayerMotor p)=>p.held?"Hands full / Q to place":"E  Pick up "+displayName;
    public override void Use(PlayerMotor p){
        if(p.held)return;
        if(p.Pick(this)&&socket){socket.core=null;socket=null;}
    }
    public void SetColliders(bool on){foreach(var c in GetComponentsInChildren<Collider>())c.enabled=on;}
    public void ResetHome(){
        if(socket){socket.core=null;socket=null;}
        transform.SetParent(null);transform.position=home;transform.rotation=Quaternion.identity;carried=false;SetColliders(true);
    }
}
}
