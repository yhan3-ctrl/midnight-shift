using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
namespace LastLantern {
// Explicitly staged UI inspection, never an unassisted playthrough.
public class LocalizationPreview : MonoBehaviour {
    string directory;FactoryGame g;int captures,pageContinuations;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install(){if(Array.IndexOf(Environment.GetCommandLineArgs(),"--capture-languages")>=0)new GameObject("Language layout review").AddComponent<LocalizationPreview>();}
    IEnumerator Snap(string code,string name){
        if(!g.Frozen)g.player.ScanFocus();Debug.Log("LOCALE_CAPTURE / "+code+" / "+name);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(directory,code+"-"+name+".png"));yield return new WaitForSecondsRealtime(.18f);captures++;
    }
    void Close(){for(int i=0;i<100&&g.Reading;i++)g.Advance();}
    IEnumerator Start(){
        Application.runInBackground=true;yield return null;g=FactoryGame.I;g.player.enabled=false;g.player.SetLookMode(1,false);g.inspector.enabled=false;FindFirstObjectByType<EndOfShift>().enabled=false;
        var args=Environment.GetCommandLineArgs();directory=Application.temporaryCachePath;for(int i=0;i<args.Length-1;i++)if(args[i]=="--preview-dir")directory=args[i+1];Directory.CreateDirectory(directory);
        var machines=FindObjectsByType<Machine>(FindObjectsSortMode.None);var mould=machines.Single(x=>x.machineId=="mould");var press=machines.Single(x=>x.machineId=="press");
        string staffNote=UiCopy.Entries.Single(x=>x.en.StartsWith("The helpers only return on Halloween.")).en;
        int initial=UiCopy.Selected;
        for(int language=0;language<UiCopy.Codes.Length;language++){
            Close();typeof(FactoryGame).GetField("workshopWelcome",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(g,false);g.CloseLanguages();g.mapOpen=false;g.paused=false;g.journal=false;g.activeCircuit=null;g.started=false;g.ended=false;g.shell=false;g.transferred=false;g.blueprint=false;g.etched=false;g.assembled=false;g.inspectorLesson=false;g.inspector.assemblyDuty=false;g.inspector.ResetPatrol();g.guide.looked=false;g.guide.walked=false;g.player.Teleport(new Vector3(0,0,-7),0);UiCopy.Language(language,false);string code=UiCopy.Codes[language];
            yield return Snap(code,"title");g.OpenLanguages();yield return Snap(code,"languages");g.CloseLanguages();
            g.Begin();g.Say("WILLOW WORKS / STAFF NOTE",staffNote);yield return Snap(code,"dialogue");Close();string stress=UiCopy.Translate(staffNote)+"\n\n"+UiCopy.Translate(staffNote);g.Say("WILLOW WORKS / STAFF NOTE",stress);yield return Snap(code,"dialogue-stress");g.Advance();if(!g.Reading||g.line!=stress){Debug.LogError("LOCALIZATION_PREVIEWS_FAIL / Long dialogue skipped: "+code);Application.Quit(1);yield break;}pageContinuations++;yield return Snap(code,"dialogue-page2");Close();
            g.player.SetLookMode(0,false);yield return Snap(code,"look-start");// Stage HUD only; reflection does not claim physical mouse input or capture.
            var captureField=typeof(PlayerMotor).GetField("lookCaptured",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);captureField.SetValue(g.player,true);yield return Snap(code,"mouse-hud");captureField.SetValue(g.player,false);g.OpenComfort();yield return Snap(code,"comfort");g.CloseComfort(false);g.player.SetLookMode(1,false);yield return Snap(code,"guide");g.ToggleMap();yield return Snap(code,"map");g.ToggleMap();g.ToggleJournal();yield return Snap(code,"notebook");g.ToggleJournal();g.TogglePause();yield return Snap(code,"menu");g.TogglePause();
            g.OpenCircuit(mould);yield return Snap(code,"circuit3");g.CloseCircuit();g.OpenCircuit(press);yield return Snap(code,"circuit4");g.CloseCircuit();
            g.player.Teleport(new Vector3(0,0,15),0);yield return null;yield return Snap(code,"shift-briefing");Close();yield return Snap(code,"packaging");
            var workGateCrate=FindObjectsByType<Pickup>(FindObjectsSortMode.None).Single(x=>x.name=="Workshop override crate");var workGateHome=workGateCrate.transform.position;workGateCrate.PlaceAt(g.workshopRelay.gate.plate.position+Vector3.up*.885f);g.player.Teleport(new Vector3(-5.4f,0,18.6f),0);g.player.Pick(g.sharedCore);mould.Use(g.player);g.CloseCircuit();mould.enabled=false;mould.running=true;mould.progress=mould.workSeconds*.5f;Physics.SyncTransforms();yield return null;g.Notify("",0);yield return Snap(code,"crafting-progress");mould.running=false;mould.enabled=true;mould.Remove(g.player);g.player.PutBackHeld();mould.progress=0;workGateCrate.PlaceAt(workGateHome);
            var overrideCrate=FindObjectsByType<Pickup>(FindObjectsSortMode.None).Single(x=>x.name=="Workshop override crate");Vector3 oldCrate=overrideCrate.transform.position;overrideCrate.PlaceAt(g.workshopRelay.gate.plate.position+Vector3.up*.885f);g.player.Pick(g.sharedCore);g.player.Teleport(new Vector3(-5.4f,0,18.9f),0);Physics.SyncTransforms();g.player.ScanFocus();yield return Snap(code,"core-insert-level");g.HoldForDesktop();yield return Snap(code,"desktop-pause");g.ResumeDesktop();g.player.PutBackHeld();overrideCrate.PlaceAt(oldCrate);Physics.SyncTransforms();

            g.shell=true;g.transferred=true;g.player.Pick(g.sharedCore);g.player.Teleport(new Vector3(5.4f,0,23.1f),0);g.player.view.transform.LookAt(new Vector3(7.3f,.8f,27.5f));yield return new WaitForSecondsRealtime(1);g.navigation.Refresh(g.guide.Current,true);g.player.view.transform.LookAt(new Vector3(g.navigation.LookAhead(4).x,g.player.view.transform.position.y,g.navigation.LookAhead(4).z));yield return Snap(code,"archive-route");g.ToggleMap();yield return Snap(code,"archive-map");g.ToggleMap();g.player.PutBackHeld();g.player.Teleport(new Vector3(0,0,29.5f),0);yield return Snap(code,"archive");
            g.player.Pick(g.sharedCore);g.player.Teleport(new Vector3(-4,0,31.3f),0);var archiveCrate=FindObjectsByType<Pickup>(FindObjectsSortMode.None).Single(x=>x.name=="Archive crate");g.player.view.transform.LookAt(archiveCrate.transform.position);g.navigation.Refresh(g.guide.Current,true);g.Notify("",0);yield return Snap(code,"core-blocked-by-crate");g.player.PutBackHeld();
            var chime=machines.Single(x=>x.machineId=="chime");g.player.Pick(g.sharedCore);chime.Use(g.player);chime.Use(g.player);g.player.Teleport(new Vector3(-6.3f,0,33.3f),0);yield return Snap(code,"archive-safe");chime.Remove(g.player);yield return Snap(code,"chime-core-retrieved");g.player.PutBackHeld();
            g.blueprint=true;g.etched=true;g.player.Teleport(new Vector3(0,0,49),0);yield return Snap(code,"assembly");
            g.assembled=true;g.lantern.gameObject.SetActive(true);g.player.Teleport(new Vector3(0,0,59),0);g.player.Pick(g.lantern);yield return Snap(code,"delivery");g.player.PutBackHeld();g.ended=true;yield return Snap(code,"ending");
        }
        UiCopy.Language(initial,false);Debug.Log("LOCALIZATION_PREVIEWS_PASS / "+captures+" staged screens; "+pageContinuations+" multi-page dialogues advanced without skipping their source line. Preferences untouched.");Application.Quit(0);
    }
}
}
