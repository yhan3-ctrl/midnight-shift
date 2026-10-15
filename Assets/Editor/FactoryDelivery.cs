using UnityEditor;
using UnityEditor.SceneManagement;
public static class FactoryDelivery {
    public static void Prepare(){FactoryBuild.CreateScene();FactoryVerification.Capture();EditorSceneManager.OpenScene("Assets/Scenes/WillowWorks.unity");FactoryBuild.BuildMac();}
}
