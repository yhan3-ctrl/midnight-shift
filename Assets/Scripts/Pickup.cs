using UnityEngine;
namespace LastLantern {
public class Pickup : Interactable {
    public enum Kind { Crate, Core, Lantern }
    public Kind kind; public string displayName; public Vector3 home; Vector3 restingScale;
    void Awake(){restingScale=transform.lossyScale;}
    public void AttachTo(Transform mount){transform.SetParent(mount);transform.localPosition=Vector3.zero;transform.localRotation=Quaternion.identity;var v=mount.lossyScale;transform.localScale=new Vector3(restingScale.x/v.x,restingScale.y/v.y,restingScale.z/v.z);SetColliders(false);}
    public void PlaceAt(Vector3 point){transform.SetParent(null);transform.position=point;transform.rotation=Quaternion.identity;transform.localScale=restingScale;carried=false;SetColliders(true);}
     public bool carried; public Machine socket;
    public override string Prompt(PlayerMotor p)=>p.held?"Hands full / Q to place":"E  Pick up "+displayName;
    public override void Use(PlayerMotor p){
        if(p.held){FactoryGame.I.Notify("Your hands are full. Press Q to place the item first.");return;}
        var source=socket;var relay=GetComponentInParent<GridRelay>();
        if(p.Pick(this)){if(source){source.running=false;source.core=null;socket=null;}if(relay&&relay.core==this)relay.core=null;}
    }
    public void SetColliders(bool on){foreach(var c in GetComponentsInChildren<Collider>())c.enabled=on;}
    public void ResetHome(){
        if(socket){socket.core=null;socket=null;}
        PlaceAt(home);
    }
}
}
