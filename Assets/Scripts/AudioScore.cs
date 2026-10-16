using UnityEngine;
namespace LastLantern {
// Original synthesized score. No external recordings or licensed samples.
public class AudioScore : MonoBehaviour {
    AudioSource music,effects,work;AudioClip[] arrangements,workClips;AudioClip tick,chord,step,reward,startCue,bell;Machine[] machines;
    public int CircuitRewards {get;private set;}public int MachineStarts {get;private set;}public Machine ActiveWork {get;private set;}int current=-1;
    bool menuPaused,rebuildPending,silentReview;float nextRecovery;public int Recoveries {get;private set;}
    public bool MusicPlaying=>music&&music.isPlaying;
    void Awake(){
        var args=System.Environment.GetCommandLineArgs();silentReview=System.Array.IndexOf(args,"--capture-languages")>=0||Application.isBatchMode||System.Array.IndexOf(args,"--capture-preview")>=0||System.Array.IndexOf(args,"--verify-build")>=0||System.Array.IndexOf(args,"--agent-playtest")>=0;
        music=gameObject.AddComponent<AudioSource>();music.loop=true;music.spatialBlend=0;music.playOnAwake=false;music.ignoreListenerPause=true;
        effects=gameObject.AddComponent<AudioSource>();effects.spatialBlend=0;effects.playOnAwake=false;effects.ignoreListenerPause=true;
        work=gameObject.AddComponent<AudioSource>();work.loop=true;work.playOnAwake=false;work.spatialBlend=0;
        music.mute=silentReview;effects.mute=silentReview;work.mute=silentReview;
        BuildClips();AudioSettings.OnAudioConfigurationChanged+=ConfigurationChanged;
    }
    void BuildClips(){
        if(arrangements!=null)foreach(var clip in arrangements)if(clip)Destroy(clip);
        if(work)work.Stop();if(workClips!=null)foreach(var clip in workClips)if(clip)Destroy(clip);if(reward)Destroy(reward);if(startCue)Destroy(startCue);if(bell)Destroy(bell);
        if(tick)Destroy(tick);if(step)Destroy(step);if(chord)Destroy(chord);
        arrangements=new AudioClip[3];for(int i=0;i<3;i++)arrangements[i]=MakeScore(i);
        tick=Tone(440,.12f);step=MakeFootstep();chord=MakeChord();reward=MakeReward();startCue=MakeStart();bell=MakeBell();workClips=new AudioClip[4];for(int i=0;i<4;i++)workClips[i]=MakeWork(i);current=-1;
    }
    void ConfigurationChanged(bool changed){rebuildPending=true;nextRecovery=Time.unscaledTime+.25f;}
    void OnApplicationFocus(bool focus){if(focus)nextRecovery=Time.unscaledTime+.1f;}
    public void RestoreAudio(bool test=false){
        AudioListener.pause=false;AudioListener.volume=1;
        if(rebuildPending){BuildClips();rebuildPending=false;}
        var g=FactoryGame.I;int next=g&&(g.ended||g.assembled)?2:g&&g.shell?1:0;
        if(current!=next||music.clip!=arrangements[next]){current=next;music.clip=arrangements[next];}
        music.enabled=true;music.mute=silentReview;effects.mute=silentReview;work.mute=silentReview;
        if(!music.isPlaying){if(menuPaused){music.UnPause();menuPaused=false;}if(!music.isPlaying)music.Play();Recoveries++;}
        if(test){effects.volume=g?Mathf.Max(g.effectsVolume,.4f):.65f;effects.pitch=1;effects.PlayOneShot(chord,.65f);}
        nextRecovery=Time.unscaledTime+1;
    }
    void Start(){machines=FindObjectsByType<Machine>(FindObjectsSortMode.None);}
    public void RefreshWork(){
        var g=FactoryGame.I;Machine active=null;if(g&&!g.Frozen&&machines!=null)foreach(var m in machines)if(m&&m.running&&m.machineId!="chime"){active=m;break;}
        ActiveWork=active;if(!active){if(work.isPlaying)work.Pause();return;}
        int i=active.machineId=="mould"?0:active.machineId=="ribbon"?1:active.machineId=="etch"?2:3;
        if(work.clip!=workClips[i]){work.Stop();work.clip=workClips[i];}float distance=Vector3.Distance(g.player.transform.position,active.transform.position);work.volume=g.effectsVolume*.35f*Mathf.Clamp01(1-distance/14);work.mute=silentReview;
        if(!work.isPlaying){work.UnPause();if(!work.isPlaying)work.Play();}
    }
    public void CircuitSuccess(){CircuitRewards++;effects.pitch=1;effects.PlayOneShot(reward,.75f);}
    public void MachineStart(string id){MachineStarts++;effects.pitch=1;effects.PlayOneShot(id=="chime"?bell:startCue,.75f);}
    void Update(){
        var g=FactoryGame.I;if(!g)return;RefreshWork();
        if(rebuildPending&&Time.unscaledTime>=nextRecovery)RestoreAudio();
        int next=g.ended||g.assembled?2:g.shell?1:0;
        if(next!=current){float t=music.isPlaying?music.time:0;current=next;music.clip=arrangements[next];music.time=t%music.clip.length;menuPaused=false;music.Play();}
        music.volume=g.musicVolume*(g.Reading?.65f:1);effects.volume=g.effectsVolume;
        if(g.paused){if(music.isPlaying){music.Pause();menuPaused=true;}}
        else if(!music.isPlaying&&Time.unscaledTime>=nextRecovery)RestoreAudio();
    }
    public void Step(){effects.pitch=.9f+Mathf.Sin(Time.time*7)*.08f;effects.PlayOneShot(step,.23f);}
    public void Tick(float hz){effects.pitch=hz/440;effects.PlayOneShot(tick,.5f);}
    public void Chord(){effects.pitch=1;effects.PlayOneShot(chord,.65f);}
    void OnDestroy(){AudioSettings.OnAudioConfigurationChanged-=ConfigurationChanged;}
    static AudioClip MakeReward(){
        const int sr=22050;var d=new float[sr];int n=0;foreach(float hz in new[]{523.25f,659.25f,783.99f}){int offset=(int)(n++*.11f*sr);for(int j=0;j<sr-offset;j++){float t=j/(float)sr;d[offset+j]+=(Mathf.Sin(2*Mathf.PI*hz*t)+.12f*Mathf.Sin(4*Mathf.PI*hz*t))*Mathf.Exp(-t*6)*Mathf.Min(1,t/.008f)*.2f;}}return Clip("Circuit / bright three-note reward",d,sr);
    }
    static AudioClip MakeBell(){const int sr=22050;var d=new float[sr];for(int i=0;i<d.Length;i++){float t=i/(float)sr;d[i]=(Mathf.Sin(2*Mathf.PI*784*t)+.35f*Mathf.Sin(2*Mathf.PI*1176*t))*Mathf.Exp(-t*5)*Mathf.Min(1,t/.004f)*.22f;}return Clip("Chime / friendly break bell",d,sr);}
    static AudioClip MakeStart(){const int sr=22050;var d=new float[sr/2];for(int i=0;i<d.Length;i++){float t=i/(float)sr;float clunk=Mathf.Sin(2*Mathf.PI*95*t)*Mathf.Exp(-t*30);float spin=Mathf.Sin(2*Mathf.PI*(170*t+85*t*t))*Mathf.Sin(Mathf.PI*t/.5f)*.45f;d[i]=(clunk+spin)*.3f;}return Clip("Machine / start clunk and spin",d,sr);}
    static AudioClip MakeWork(int type){
        const int sr=22050;var d=new float[sr];uint seed=(uint)(9127+type);float low=0;
        for(int i=0;i<d.Length;i++){float t=i/(float)sr;seed=seed*1664525+1013904223;float noise=((seed>>8)/(float)0xFFFFFF)*2-1;low=low*.92f+noise*.08f;float cycle=(t*(type==1?6:4))%1;float pulse=Mathf.Exp(-cycle*22);float tone=Mathf.Sin(2*Mathf.PI*(type==2?360:type==3?75:120)*t);float value=type==0?low*.8f+tone*.12f:type==1?low*.3f+pulse*tone*.65f:type==2?low*.3f+tone*pulse*.5f:tone*.25f+pulse*low;float fade=Mathf.Min(1,Mathf.Min(t/.01f,(1-t)/.01f));d[i]=value*fade*.38f;}
        return Clip(new[]{"Moulding / soft lathe","Ribbon / paper rollers","Engraving / delicate tool ticks","Press / low mechanism"}[type],d,sr);
    }
    static AudioClip Clip(string name,float[] data,int rate){var c=AudioClip.Create(name,data.Length,1,rate,false);c.SetData(data,0);return c;}
    static AudioClip MakeFootstep(){
        int sr=22050;var d=new float[(int)(sr*.12f)];uint seed=3197;float low=0;
        for(int i=0;i<d.Length;i++){seed=seed*1664525+1013904223;float noise=((seed>>8)/(float)0xFFFFFF)*2-1;low=low*.82f+noise*.18f;float t=i/(float)sr;d[i]=(low*.9f+Mathf.Sin(2*Mathf.PI*78*t)*.3f)*Mathf.Exp(-t*42)*Mathf.Min(1,t/.004f);}
        var c=AudioClip.Create("Soft wooden footstep",d.Length,1,sr,false);c.SetData(d,0);return c;
    }
    static AudioClip Tone(float hz,float seconds){int sr=22050;var data=new float[(int)(sr*seconds)];for(int i=0;i<data.Length;i++){float t=i/(float)sr;data[i]=Mathf.Sin(2*Mathf.PI*hz*t)*Mathf.Exp(-t*25)*.25f;}var c=AudioClip.Create("Soft interaction",data.Length,1,sr,false);c.SetData(data,0);return c;}
    static AudioClip MakeChord(){int sr=22050;var d=new float[sr*2];foreach(float hz in new[]{261.63f,329.63f,392f,523.25f})for(int i=0;i<d.Length;i++){float t=i/(float)sr;d[i]+=Mathf.Sin(2*Mathf.PI*hz*t)*Mathf.Exp(-t*3)*.085f;}var c=AudioClip.Create("Lantern chord",d.Length,1,sr,false);c.SetData(d,0);return c;}
    static AudioClip MakeScore(int layer){
        const int sr=22050;const float beat=.6f;int length=Mathf.RoundToInt(sr*beat*64);var d=new float[length];
        int[] melody={76,79,81,79,76,74,72,74,76,79,83,81,79,76,74,72,74,76,79,76,74,72,69,72,74,76,79,74,72,71,69,67};
        for(int n=0;n<32;n++)AddNote(d,sr,n*beat*2,1.65f,melody[n],.11f,true);
        int[] roots={48,45,41,43,48,45,41,43};
        for(int bar=0;bar<8;bar++){
            AddNote(d,sr,bar*beat*8,beat*7.6f,roots[bar],.045f,false);
            if(layer>0)for(int k=0;k<8;k++)AddNote(d,sr,(bar*8+k)*beat,beat*1.8f,roots[bar]+12+(k%3==1?7:k%3==2?12:0),.045f,true);
            if(layer>1)AddNote(d,sr,bar*beat*8,beat*7.6f,roots[bar]+19,.032f,false);
        }
        // A short fade at both ends avoids a discontinuity at the loop boundary.
        for(int i=0;i<length;i++){float edge=Mathf.Min(1,Mathf.Min(i/(sr*.12f),(length-1-i)/(sr*.3f)));d[i]=(float)System.Math.Tanh(d[i])*edge;}
        var c=AudioClip.Create("Willow Works / arrangement "+layer,length,1,sr,false);c.SetData(d,0);return c;
    }
    static void AddNote(float[] d,int sr,float start,float duration,int midi,float gain,bool pluck){
        int first=(int)(start*sr),count=(int)(duration*sr);float hz=440*Mathf.Pow(2,(midi-69)/12f);
        for(int j=0;j<count&&first+j<d.Length;j++){
            float t=j/(float)sr;float env=pluck?Mathf.Exp(-t*3.2f):Mathf.Sin(Mathf.PI*j/count)*.5f;
            float wave=Mathf.Sin(2*Mathf.PI*hz*t)+.2f*Mathf.Sin(2*Mathf.PI*hz*2*t)+.08f*Mathf.Sin(2*Mathf.PI*hz*3*t);
            d[first+j]+=wave*env*gain*Mathf.Min(1,t/.012f);
        }
    }
}
}
