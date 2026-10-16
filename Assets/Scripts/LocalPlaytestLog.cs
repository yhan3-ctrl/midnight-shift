using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace LastLantern {
// Local, anonymous session evidence only. No network transport or player identity data.
public class LocalPlaytestLog : MonoBehaviour {
    [Serializable]public class Event {public float seconds;public string action;}
    [Serializable]public class Record {
        public string version="0.8", startedUtc, reviewer="unattributed";
        public bool automated,completed;
        public float elapsedSeconds,activeSeconds,readingSeconds,pausedSeconds,journalSeconds;
        public int safetyRetries,hintsRequested;
        public List<Event> events=new List<Event>();
    }
    Record record;string path,lastMode="";int progress=-1;bool saved;
    void Start(){record=new Record{startedUtc=DateTime.UtcNow.ToString("O"),automated=Array.IndexOf(Environment.GetCommandLineArgs(),"--capture-languages")>=0||Application.isBatchMode||Array.IndexOf(Environment.GetCommandLineArgs(),"--capture-preview")>=0||Array.IndexOf(Environment.GetCommandLineArgs(),"--agent-playtest")>=0};path=Path.Combine(Application.persistentDataPath,"Playtests","shift-"+DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")+".json");}
    void Update(){
        var g=FactoryGame.I;if(g==null||record==null||!g.started)return;
        string mode=g.ended?"ending":g.photoMode?"screenshot":g.desktopHold?"desktop":g.paused?"paused":g.journal?"journal":g.Reading?"reading":g.activeCircuit?"routing":"active";
        if(mode!=lastMode){record.events.Add(new Event{seconds=record.elapsedSeconds,action=mode});lastMode=mode;}
        int flags=(g.shell?1:0)|(g.transferred?2:0)|(g.inspectorLesson?4:0)|(g.blueprint?8:0)|(g.etched?16:0)|(g.assembled?32:0);
        if(flags!=progress){record.events.Add(new Event{seconds=record.elapsedSeconds,action="progress-mask:"+flags});progress=flags;}
        record.safetyRetries=g.resets;record.hintsRequested=g.hintsRequested;record.completed=g.ended;
        if(g.ended){if(!saved)Save();return;}
        float dt=Time.unscaledDeltaTime;record.elapsedSeconds+=dt;
        if(g.paused||g.photoMode||g.desktopHold)record.pausedSeconds+=dt;else if(g.journal)record.journalSeconds+=dt;else if(g.Reading)record.readingSeconds+=dt;else record.activeSeconds+=dt;
    }
    void Save(){if(record==null||record.elapsedSeconds<=0)return;try{Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(record,true));saved=true;}catch(Exception e){Debug.LogWarning("Could not save local playtest log: "+e.Message);}}
    void OnApplicationQuit(){Save();}
    void OnDestroy(){Save();}
}
}
