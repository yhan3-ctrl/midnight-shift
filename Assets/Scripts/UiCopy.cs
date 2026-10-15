using System.Collections.Generic;
using UnityEngine;
namespace LastLantern {
public static class UiCopy {
    public static bool Chinese=PlayerPrefs.GetInt("shift-language",0)==1;
    public static string Pick(string zh,string en)=>Chinese?zh:en;
    public static void Language(bool chinese){Chinese=chinese;PlayerPrefs.SetInt("shift-language",chinese?1:0);}
    static readonly Dictionary<string,string> lines=new Dictionary<string,string>{
        {"PIP / NIGHT FOREMAN","PIP / 夜班领班"},{"BUTTON / STORES","BUTTON / 仓库小助手"},{"PIP / OVER THE SPEAKER","PIP / 工厂广播"},{"PIP / DISPATCH","PIP / 送货台"},{"HUSH / SAFETY CHECK","HUSH / 安全检查"},{"THE BACK OF THE DRAWING","旧图纸的背面"},{"YOUR OLD DRAWING","你小时候的图纸"},
        {"Welcome to Willow Works. I'm Pip. Let me show you around, one little step at a time.","欢迎来到柳树玩具工厂，我是 Pip。我来带你熟悉一下吧，一步一步来。"},
        {"We have one forgotten pumpkin lantern to finish tonight. First, let's open this door together. Follow the gold marker; your checklist is on the right.","今晚，我们要完成一盏被遗忘的南瓜灯。先一起打开这扇门：跟着金色箭头走，右边的挂历会记下待办。"},
        {"A perfect little shell! I kept the mould right where that child left it.","外壳做好啦！那个孩子留下的模具，我一直都好好保存着。"},
        {"They used to ask who switched off the lights. Not what we'd make them. Who would get home last.","那个孩子总会问：最后是谁来关灯？谁要到很晚才能回家？"},
        {"The shell is safe now. Take the power core out and use it on the Ribbon Conveyor.","外壳已经做好，不会消失。取出金色核心，再把它借给右边的丝带机。"},
        {"Look at that. Every little crooked line, just like the drawing.","你看，每一条歪歪扭扭的小线条，都和图纸上一样。"},
        {"I'll be at dispatch. Shall we see who this one's for?","我在送货台等你。一起看看这盏灯是送给谁的吧？"},
        {"Let's try that delivery again. You're back at the room entrance, and this room's crates are back where you found them. Your finished jobs and drawing are safe.","我们再试一次吧。你回到了本区入口，箱子也回到了原位。已经完成的工序和读过的图纸都会保留。"},
        {"Check your notebook [J] if you need the core's location. A crate only blocks my view after you place it. The chime can call me away before I spot you.","右边的待办会继续指路，J 可以查看核心位置。放下的箱子能挡住我的视线；在我发现你之前，铃声可以把我引开。"},
        {"Under the frame is a note in your own childhood handwriting.","相框背后有一张小纸条，字迹是小时候的你写的。"},
        {"'For the last one to leave. So the way home is bright, too.'","“送给最后一个下班的人。这样，回家的路也会亮亮的。”"},
        {"For the last one to leave. So the way home is bright, too.","送给最后一个下班的人。这样，回家的路也会亮亮的。"},
        {"You remember now. The lantern was never waiting for a customer. You drew it for the little helpers who stayed after everyone else went home.","你想起来了。这盏灯从来不是为顾客准备的，而是送给那些等所有人都回家后，仍留在工厂里的小幽灵们。"},
        {"Pip has kept your promise safe all these years. You can finish it tonight.","这些年，Pip 一直替你保存着这个承诺。今晚，你终于可以完成它。"},
        {"Let's open the first door together. Look for the crate on your left. The gold marker and your checklist will show each step.","我们先一起打开第一扇门吧。找找左边的木箱，金色箭头和右侧待办会一步步带你完成。"},
        {"I've kept the old mould polished. Pip said the order might still be wanted someday.","我一直把旧模具擦得亮亮的。Pip 说，也许有一天，还有人想把这份订单做完。"},
        {"The gold core is all we have left. When the workshop gate is held by a crate, we can borrow its power for the Moulding Table.","工厂只剩一枚金色核心。等箱子接替它保持门打开，我们就能把核心借给成型机。"},
        {"There it is! A real little shell, just like I remember.","就是它！一只真正的小灯壳，和记忆里的一样。"},
        {"The Ribbon Conveyor is across the room. Take the same core; your shell will stay finished.","丝带机在房间另一侧。把同一枚核心拿过去就好，外壳会保留。"},
        {"I tied the ribbon the way the child liked it. A little crooked. Never trim the ends.","我照着那个孩子喜欢的样子系好了丝带。有一点点歪，也舍不得剪掉末端。"},
        {"Pip kept the old drawing in the archive. Hush is on safety duty there. He only wants the aisles clear.","Pip 把旧图纸放在档案室。Hush 在那里做安全巡查，他只是希望过道保持畅通。"},
        {"You remember who it was for, don't you?","你已经想起这是送给谁的了，对吗？"},
        {"We kept your drawing. You came back to finish it. I think that makes this a very good last shift.","我们保存了你的图纸，而你回来完成了它。这真是一个很好的最后夜班。"},
        {"One last order, then we can all go home. I'll keep the desk warm.","最后一份订单完成，我们就可以一起回家。我会在送货台等你。"},
        {"PACKAGING COMPLETE / The archive door is now unlocked.","包装完成！档案室的门已经打开，跟着待办继续前进。"},
        {"THE PROMISE IS ENGRAVED / Move the core to the Lantern Press.","承诺已经刻好。按右侧待办，把核心借给最后的灯笼压制机。"},
        {"Hush is coming to check the chime. Watch his route, then use the archive plate.","Hush 正往休息铃走。观察他的路线，再搬箱子去条纹板。"},
        {"Your hands are full. Place what you're carrying first.","双手正忙，先按 Q 放下手中的物品。"},
        {"This socket accepts one power core.","这个插座需要一枚金色核心。"},
        {"Bring a power core to this machine.","把金色核心带到这台机器前。"},
        {"Arrow-key look / mouse look off. K switches back.","现在仅用方向键观察，鼠标不再转动视角。按 K 可切回。"},
        {"Mouse and arrow-key look. K switches to arrows only.","现在可用鼠标和方向键观察。按 K 可切换为仅方向键。"},
        {"Make the shell first","先制作灯笼外壳"},{"Read the old drawing first","先读旧图纸"},{"Engrave the shell first","先完成雕刻"},
        {"WILLOW WORKS / STAFF NOTE","柳树工厂 / 员工纸条"},
        {"The helpers only return on Halloween. If the lights are on, please tell them I left the side door open for the parade. Your old drawing is still in the archive. Love, Aunt May.","小助手们只在万圣节回来。如果灯还亮着，请告诉他们：我为游行留好了侧门。你小时候的图纸，还放在档案室里。——爱你的 May 阿姨"},
        {"Nice work! The crate keeps the door open. Leave it there and follow the marker.","做得好！箱子会让门一直开着。把它留在这里，跟着箭头穿门吧。"},
        {"PIP / ARCHIVE TOUR","PIP / 认识档案室"},{"PIP / FINAL ASSEMBLY","PIP / 最终装配"},
        {"This is Hush, our safety inspector. He follows a route and checks noisy machines. He cannot hurt you.","这位是安全巡逻员 Hush。他会沿路线巡视，也会去查看响动的机器。他不会伤害你。"},
        {"Try the chime first. Then use the crate to open the drawing cabinet. If Hush stops you, finished work stays safe.","先试试休息铃，再用箱子打开图纸柜。如果 Hush 拦下你，已完成的工作仍会保留。"},
        {"Your old drawing is safe in the notebook. Let's turn that promise into a real lantern.","旧图纸已经记在笔记里。我们把这个承诺做成一盏真正的灯吧。"},
        {"The Engraver can work on its own. Leave its core in place and prepare the crate gate while it finishes. Keep an eye on Hush.","雕刻机可以自己工作。把核心留在里面，趁它加工时搬箱子准备门禁，也要留意 Hush。"},
        {"In your hands","在你手中"},{"Workshop Relay","紫色门禁供电柜"},{"Packaging entrance","包装区入口"},{"Packaging floor","包装区地上"},{"Archive floor","档案室地上"},{"Assembly floor","装配区地上"},{"Not located","尚未找到"},
        {"MOULDING TABLE","成型机"},{"RIBBON CONVEYOR","丝带机"},{"BREAK-TIME CHIME","休息铃"},{"ENGRAVING TABLE","雕刻机"},{"LANTERN PRESS","灯笼压制机"},
        {"Foyer crate","入口木箱"},{"Workshop override crate","车间木箱"},{"Archive crate","档案室木箱"},{"Assembly crate","装配区木箱"},{"Movable cover","可移动掩体"},{"power core","金色核心"},{"the last lantern","最后一盏灯"}
    };
    public static string Translate(string text){if(!Chinese||string.IsNullOrEmpty(text))return text;return lines.TryGetValue(text,out var zh)?zh:text;}
    public static string Prompt(PlayerMotor p){
        if(!p.focus)return "";var item=p.focus as Pickup;
        if(item)return p.held?Pick("双手正忙：先按 Q 放下物品","Hands full: place your item with Q"):Pick("E  拿起 ","E  Pick up ")+Translate(item.displayName);
        var machine=p.focus as Machine;
        if(machine){string name=Translate(machine.displayName);if(p.held&&p.held.kind==Pickup.Kind.Core&&machine.Requirement!="")return Translate(machine.Requirement);if(p.held&&p.held.kind==Pickup.Kind.Core)return Pick("E  插入核心 / ","E  Insert core / ")+name;if(p.held)return Pick("先按 Q 放下物品","Place your item with Q first");return machine.core?Pick(machine.done?"E  取回核心 / ":machine.running?"E  停止并取回核心 / ":"E  打开 / ",machine.done?"E  Retrieve core / ":machine.running?"E  Stop & retrieve core / ":"E  Open / ")+name:Pick("这里需要金色核心 / ","Bring the gold core / ")+name;}
        if(p.focus is GridRelay relay){if(p.held)return p.held.kind==Pickup.Kind.Core?Pick("E  给门禁供电","E  Power the gate"):Pick("先按 Q 放下手中的物品","Place your item with Q first");return relay.core?Pick("E  从供电柜取回核心","E  Retrieve core from relay"):Pick("这里需要金色核心","Bring the gold core here");}
        var story=p.focus as StoryPoint;if(story)return Pick(story.kind==StoryPoint.StoryKind.Button?"E  和 Button 聊聊":story.kind==StoryPoint.StoryKind.Pip?"E  和 Pip 交谈 / 送达灯笼":story.kind==StoryPoint.StoryKind.Drawing?"E  读旧图纸":"E  读纸条",story.Prompt(p));
        return p.focus.Prompt(p);
    }
}
}
