using System.Collections;
using UnityEngine;
namespace LastLantern {
public class BuildSmokeCheck : MonoBehaviour {
    static bool capture;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install(){foreach(string arg in System.Environment.GetCommandLineArgs())if(arg=="--verify-build"||arg=="--capture-preview"){capture=arg=="--capture-preview";new GameObject("Packaged build check").AddComponent<BuildSmokeCheck>();break;}}
    IEnumerator Start(){
        Application.runInBackground=true;yield return null;
        if(capture){yield return Capture();yield break;}
        var g=FactoryGame.I;
        if(!g||!g.player||!g.inspector||!g.lantern||!g.sharedCore||!FindFirstObjectByType<EndOfShift>()){Debug.LogError("PACKAGED_BUILD_FAIL / Missing serialized references");Application.Quit(1);yield break;}
        g.Begin();while(g.Reading)g.Advance();g.ToggleJournal();
        if(!g.Frozen||Time.timeScale!=0){Debug.LogError("PACKAGED_BUILD_FAIL / Journal");Application.Quit(1);yield break;}
        g.ToggleJournal();yield return new WaitForSecondsRealtime(1);
        Debug.Log("PACKAGED_BUILD_PASS / Scene loaded, references intact, title action, dialogue and journal execute.");Application.Quit(0);
    }
    IEnumerator Capture(){
        var args=System.Environment.GetCommandLineArgs();string dir=Application.temporaryCachePath;
        for(int i=0;i<args.Length-1;i++)if(args[i]=="--preview-dir")dir=args[i+1];System.IO.Directory.CreateDirectory(dir);
        yield return new WaitForSecondsRealtime(1);
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(dir,"00-title-runtime.png"));yield return new WaitForSecondsRealtime(.7f);
        var g=FactoryGame.I;g.Begin();yield return new WaitForSecondsRealtime(.7f);
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(dir,"01-dialogue-runtime.png"));yield return new WaitForSecondsRealtime(.7f);
        while(g.Reading)g.Advance();g.player.enabled=false;g.inspector.enabled=false;
        Machine mould=null;foreach(var m in FindObjectsByType<Machine>(FindObjectsSortMode.None))if(m.machineId=="mould")mould=m;
        g.player.Pick(g.sharedCore);mould.Use(g.player);mould.Use(g.player);yield return new WaitForSecondsRealtime(.7f);
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(dir,"02-circuit-runtime.png"));yield return new WaitForSecondsRealtime(.7f);g.CloseCircuit();
        g.shell=true;g.transferred=true;g.inspectorLesson=true;g.blueprint=true;g.etched=true;g.assembled=true;
        g.player.Teleport(new Vector3(0,0,58.2f),0);g.lantern.gameObject.SetActive(true);g.player.Pick(g.lantern);g.Deliver();
        yield return new WaitForSecondsRealtime(13);
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(dir,"05-ending-runtime.png"));yield return new WaitForSecondsRealtime(1);
        Debug.Log("RUNTIME_PREVIEWS_READY / staged visual capture, not a playtest");Application.Quit(0);
    }

}
}
