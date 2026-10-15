using UnityEngine;
namespace LastLantern {
// Original synthesized score. No external recordings or licensed samples.
public class AudioScore : MonoBehaviour {
    AudioSource music,effects; AudioClip[] arrangements; AudioClip tick,chord,step; int current=-1;
    void Awake(){
        music=gameObject.AddComponent<AudioSource>();music.loop=true;music.spatialBlend=0;
        effects=gameObject.AddComponent<AudioSource>();effects.spatialBlend=0;
        arrangements=new AudioClip[3];for(int i=0;i<3;i++)arrangements[i]=MakeScore(i);
        tick=Tone(440,.12f);step=MakeFootstep();chord=MakeChord();
    }
    void Update(){
        if(!FactoryGame.I)return;var g=FactoryGame.I;int next=g.ended||g.assembled?2:g.shell?1:0;
        if(next!=current){float t=music.isPlaying?music.time:0;current=next;music.clip=arrangements[next];music.time=t%music.clip.length;music.Play();}
        music.volume=g.musicVolume*(g.Reading?.48f:1);effects.volume=g.effectsVolume;
        if(g.paused){if(music.isPlaying)music.Pause();}else if(!music.isPlaying)music.UnPause();
    }
    public void Step(){effects.pitch=.9f+Mathf.Sin(Time.time*7)*.08f;effects.PlayOneShot(step,.23f);}
    public void Tick(float hz){effects.pitch=hz/440;effects.PlayOneShot(tick,.5f);}
    public void Chord(){effects.pitch=1;effects.PlayOneShot(chord,.65f);}
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
