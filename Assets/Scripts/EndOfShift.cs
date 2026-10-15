using UnityEngine;
namespace LastLantern {
public class EndOfShift : MonoBehaviour {
    public Transform pip,button,hush,exitDoor; public Camera view;
    bool playing; float time; Vector3 pipStart,buttonStart,hushStart,cameraStart; Quaternion cameraRotation;
    void Update(){
        if(!FactoryGame.I||!FactoryGame.I.ended)return;
        if(!playing){
            playing=true;time=0;pipStart=new Vector3(0,0,60.5f);buttonStart=new Vector3(-1.2f,0,60.6f);hushStart=new Vector3(1.2f,0,60.8f);
            pip.position=pipStart;button.position=buttonStart;hush.position=hushStart;
            foreach(var ghost in new[]{pip,button,hush})ghost.rotation=Quaternion.Euler(0,180,0);
            var inspector=hush.GetComponent<Inspector>();if(inspector){inspector.enabled=false;inspector.visual.localRotation=Quaternion.identity;hush.gameObject.AddComponent<GhostIdle>();}
            var lantern=FactoryGame.I.lantern.transform;lantern.SetParent(pip.Find("Ghost body"));lantern.localPosition=new Vector3(.68f,.02f,0);lantern.localScale=Vector3.one;
            cameraStart=view.transform.position;cameraRotation=view.transform.rotation;
        }
        time+=Time.unscaledDeltaTime;
        if(exitDoor)exitDoor.position=Vector3.Lerp(new Vector3(0,1.65f,64),new Vector3(0,5.5f,64),Mathf.SmoothStep(0,1,time/3));
        float t=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-1.5f)/12));
        pip.position=Vector3.Lerp(pipStart,new Vector3(0,0,72),t);
        button.position=Vector3.Lerp(buttonStart,new Vector3(-1.2f,0,71.5f),t);
        hush.position=Vector3.Lerp(hushStart,new Vector3(1.2f,0,72.5f),t);
        float ct=Mathf.SmoothStep(0,1,time/2.5f);
        Vector3 inside=new Vector3(0,1.9f,62.2f),outside=new Vector3(0,1.9f,66.4f),watch=new Vector3(3.4f,2.1f,67);
        if(time<2.5f)view.transform.position=Vector3.Lerp(cameraStart,inside,ct);
        else if(time<6)view.transform.position=Vector3.Lerp(inside,outside,Mathf.SmoothStep(0,1,(time-2.5f)/3.5f));
        else view.transform.position=Vector3.Lerp(outside,watch,Mathf.SmoothStep(0,1,(time-6)/2.5f));
        var final=Quaternion.LookRotation(new Vector3(0,1.4f,72)-view.transform.position);
        view.transform.rotation=Quaternion.Slerp(cameraRotation,final,ct);
    }
}
}
