using System;
using System.Collections.Generic;
using UnityEngine;
namespace LastLantern {
public static class UiCopy {
    public static readonly string[] Codes={"en","zh-Hans","es","fr","de","ja","ko"};
    public static readonly string[] Names={"English","简体中文","Español","Français","Deutsch","日本語","한국어"};
    public static int Selected {get;private set;}=Normalize(PlayerPrefs.GetInt("shift-language",0));
    public static bool Chinese=>Selected==1;
    [Serializable] public class Entry {public string en;public string[] values;}
    [Serializable] class Catalog {public Entry[] entries;}
    static Catalog catalog;static Dictionary<string,Entry> lines;static Font[] fonts=new Font[7];
    static int Normalize(int language)=>language>=0&&language<Codes.Length?language:0;
    public static void Language(int language,bool persist=true){Selected=Normalize(language);if(persist){PlayerPrefs.SetInt("shift-language",Selected);PlayerPrefs.Save();}}
    public static void Language(bool chinese)=>Language(chinese?1:0);
    static void Ensure(){if(lines!=null)return;lines=new Dictionary<string,Entry>();var asset=Resources.Load<TextAsset>("I18n/catalog");catalog=asset?JsonUtility.FromJson<Catalog>(asset.text):new Catalog{entries=new Entry[0]};foreach(var entry in catalog.entries)lines.Add(entry.en,entry);}
    public static Entry[] Entries {get{Ensure();return catalog.entries;}}
    public static Font BodyFont(int language){language=Normalize(language);if(!fonts[language])fonts[language]=Resources.Load<Font>(language==5?"Fonts/NotoSansJP":language==6?"Fonts/NotoSansKR":"Fonts/NotoSansSC");return fonts[language];}
    public static string Pick(string zh,string en)=>Chinese?zh:Translate(en);
    public static string Translate(string text){if(Selected==0||string.IsNullOrEmpty(text))return text;Ensure();return lines.TryGetValue(text,out var entry)&&entry.values.Length>Selected&&!string.IsNullOrEmpty(entry.values[Selected])?entry.values[Selected]:text;}
    public static string Prompt(PlayerMotor p){
        if(!p.focus)return "";var item=p.focus as Pickup;
        if(item==FactoryGame.I.sharedCore&&!p.held&&(item.socket||item.GetComponentInParent<GridRelay>()))return Translate("E  Retrieve core / ")+Translate(FactoryGame.I.CoreLocation);
        if(item)return p.held?Pick("双手正忙：先按 Q 放下物品","Hands full: place your item with Q"):Pick("E  拿起 ","E  Pick up ")+Translate(item.displayName);
        var machine=p.focus as Machine;
        if(machine){string name=Translate(machine.displayName);if(p.held&&p.held.kind==Pickup.Kind.Core&&machine.Requirement!="")return Translate(machine.Requirement);if(p.held&&p.held.kind==Pickup.Kind.Core)return Pick("E  插入核心 / ","E  Insert core / ")+name;if(p.held)return Pick("先按 Q 放下物品","Place your item with Q first");return machine.core?Translate(machine.done?"E  Retrieve core / ":machine.running?"E  Stop & retrieve core / ":"E  Open / ")+name:Pick("这里需要金色核心 / ","Bring the gold core / ")+name;}
        if(p.focus is GridRelay relay){if(p.held)return p.held.kind==Pickup.Kind.Core?Pick("E  给门禁供电","E  Power the gate"):Pick("先按 Q 放下手中的物品","Place your item with Q first");return relay.core?Pick("E  从供电柜取回核心","E  Retrieve core from relay"):Pick("这里需要金色核心","Bring the gold core here");}
        var story=p.focus as StoryPoint;if(story)return Pick(story.kind==StoryPoint.StoryKind.Button?"E  和 Button 聊聊":story.kind==StoryPoint.StoryKind.Pip?"E  和 Pip 交谈 / 送达灯笼":story.kind==StoryPoint.StoryKind.Drawing?"E  读旧图纸":"E  读纸条",story.Prompt(p));
        return p.focus.Prompt(p);
    }
}
}
