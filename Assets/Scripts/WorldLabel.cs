using UnityEngine;
namespace LastLantern {
[ExecuteAlways]
public class WorldLabel : MonoBehaviour {
    TextMesh label;
    void OnEnable(){label=GetComponent<TextMesh>();Font.textureRebuilt+=Rebuilt;Refresh();}
    void OnDisable(){Font.textureRebuilt-=Rebuilt;}
    void Rebuilt(Font f){if(label&&label.font==f)Refresh();}
    void Refresh(){if(!label||!label.font)return;var r=GetComponent<MeshRenderer>();if(r&&r.sharedMaterial)r.sharedMaterial.mainTexture=label.font.material.mainTexture;}
}
}
