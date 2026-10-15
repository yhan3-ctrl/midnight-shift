using UnityEngine;
namespace LastLantern {
[ExecuteAlways]
public class WorldLabel : MonoBehaviour {
    TextMesh label;public float maxWidth,maxHeight;
    void OnEnable(){label=GetComponent<TextMesh>();Font.textureRebuilt+=Rebuilt;Refresh();}
    void OnDisable(){Font.textureRebuilt-=Rebuilt;}
    void LateUpdate(){if(!label)return;var r=GetComponent<MeshRenderer>();if(!r)return;var size=r.bounds.size;float factor=1;if(maxWidth>0&&size.x>maxWidth)factor=Mathf.Min(factor,maxWidth/size.x);if(maxHeight>0&&size.y>maxHeight)factor=Mathf.Min(factor,maxHeight/size.y);if(factor<.99f)label.characterSize*=factor;}
    void Rebuilt(Font f){if(label&&label.font==f)Refresh();}
    void Refresh(){if(!label||!label.font)return;var r=GetComponent<MeshRenderer>();if(r&&r.sharedMaterial)r.sharedMaterial.mainTexture=label.font.material.mainTexture;}
}
}
