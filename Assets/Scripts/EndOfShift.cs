using UnityEngine;
namespace LastLantern {
public class EndOfShift : MonoBehaviour {
    public Transform pip,button,hush,exitDoor; public Camera view;
    bool playing;float time;
    public float Elapsed=>time;
    // The helpers queue in the clear space behind dispatch. Only one crosses the narrow door at a time.
    public static Vector3 ParadePosition(int helper,float elapsed){
        float t=elapsed-3-helper*2.6f;float side=helper==1?-2.1f:helper==2?2.1f:0;
        Vector3 start=new Vector3(side,0,62.3f),queue=new Vector3(0,0,62.3f),outside=new Vector3(0,0,66.2f);
        if(t<=0)return start;
        if(t<1.6f)return Vector3.Lerp(start,queue,t/1.6f);
        if(t<5.6f)return Vector3.Lerp(queue,outside,(t-1.6f)/4);
        return Vector3.Lerp(outside,new Vector3(side,0,72),Mathf.SmoothStep(0,1,Mathf.Clamp01((t-5.6f)/4)));
    }
    void Update(){
        if(!FactoryGame.I||!FactoryGame.I.ended)return;
        if(!playing){
            playing=true;time=0;
            var inspector=hush.GetComponent<Inspector>();if(inspector){inspector.enabled=false;inspector.visual.localRotation=Quaternion.identity;if(!hush.GetComponent<GhostIdle>())hush.gameObject.AddComponent<GhostIdle>();}
            var lantern=FactoryGame.I.lantern.transform;lantern.SetParent(pip.Find("Ghost body"));lantern.localPosition=new Vector3(.82f,.02f,-.65f);lantern.localScale=Vector3.one;
            // A cinematic cut avoids sweeping the first-person camera through the desk and helpers.
            view.transform.position=new Vector3(4.8f,2.7f,76);
        }
        time+=Time.unscaledDeltaTime;
        if(exitDoor)exitDoor.position=Vector3.Lerp(new Vector3(0,1.65f,64),new Vector3(0,5.5f,64),Mathf.SmoothStep(0,1,time/3));
        Transform[] helpers={pip,button,hush};float centreZ=0;
        for(int i=0;i<helpers.Length;i++){
            helpers[i].position=ParadePosition(i,time);centreZ+=helpers[i].position.z/3;
            Quaternion heading=Quaternion.Euler(0,180,0);
            if(time>12.6f+i*2.6f)heading=Quaternion.LookRotation(helpers[i].position-view.transform.position);
            helpers[i].rotation=Quaternion.Slerp(helpers[i].rotation,heading,Time.unscaledDeltaTime*3);
        }
        // Keep the whole group on the open right side of the ending card.
        view.transform.LookAt(new Vector3(2.8f,1.2f,centreZ));
    }
}
}
