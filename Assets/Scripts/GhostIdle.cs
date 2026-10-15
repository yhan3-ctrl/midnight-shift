using UnityEngine;
namespace LastLantern {
public class GhostIdle : MonoBehaviour {
    Transform body; Vector3 home;
    void Start(){body=transform.Find("Ghost body");if(body)home=body.localPosition;}
    void Update(){if(!body)return;body.localPosition=home+Vector3.up*Mathf.Sin(Time.time*2)*.1f;body.localRotation=Quaternion.Euler(0,Mathf.Sin(Time.time)*6,Mathf.Sin(Time.time*1.5f)*3);}
}
}
