using UnityEngine;
namespace LastLantern {
// Visual manufacturing evidence, never an extra pickup or progression requirement.
public class LanternDisplay : MonoBehaviour {
    public Machine station;
    public Transform model, ribbon, inscription, glow;
    public Light warmth;
    public TextMesh caption; public float halfHeight=.232f;
    public bool portable;
    public int Stage {get;private set;}
    public bool Visible=>model&&model.gameObject.activeSelf;
    public void Refresh(){
        var g=FactoryGame.I;if(!g||!model)return;
        bool visible=true;float scale=1;
        if(portable)Stage=4;
        else if(station){
            switch(station.machineId){
                case "mould": Stage=1;visible=g.shell||station.progress>0;scale=g.shell?1:Mathf.Lerp(.15f,1,Mathf.Clamp01(station.progress/station.workSeconds));break;
                case "ribbon": Stage=g.transferred?2:1;visible=g.shell;break;
                case "etch": Stage=g.etched?3:2;visible=g.blueprint;break;
                case "press": Stage=3;visible=g.etched&&!g.assembled;break;
                default: visible=false;break;
            }
        }else visible=false;
        model.gameObject.SetActive(visible);model.localScale=Vector3.one*scale;
        model.localPosition=Vector3.down*halfHeight*(1-scale);
        if(caption&&station){
            string id=station.machineId;
            caption.text=station.running?(id=="mould"?"SHAPING THE SHELL":id=="ribbon"?"TYING THE RIBBON":id=="etch"?"ENGRAVING YOUR PROMISE":"ADDING THE LIGHT"):
                station.done?(id=="mould"?"SHELL / KEEP WHEN UNPOWERED":id=="ribbon"?"RIBBON / READY FOR ASSEMBLY":id=="etch"?"YOUR PROMISE / ENGRAVED":"LANTERN / READY TO COLLECT"):
                id=="mould"?"SHELL / AWAITING POWER":id=="ribbon"?"RIBBON / AWAITING CRAFT":id=="etch"?"PROMISE / AWAITING ENGRAVING":"LIGHT / AWAITING FINISH";
        }
        if(ribbon)ribbon.gameObject.SetActive(Stage>=2);
        if(inscription)inscription.gameObject.SetActive(Stage>=3);
        if(glow)glow.gameObject.SetActive(Stage>=4);
        if(warmth)warmth.enabled=visible&&Stage>=4;
    }
    void Update(){Refresh();}
}
}
