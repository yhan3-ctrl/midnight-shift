using UnityEngine;
namespace LastLantern {
public class StoryPoint : Interactable {
    public enum StoryKind { Drawing, Pip, Note }
    public StoryKind kind;
    public string note;
    public override string Prompt(PlayerMotor p)=>kind==StoryKind.Pip?"E  Talk to Pip / deliver lantern":kind==StoryKind.Drawing?"E  Read the old drawing":"E  Read";
    public override void Use(PlayerMotor p){if(kind==StoryKind.Drawing)FactoryGame.I.ReadDrawing();else if(kind==StoryKind.Pip)FactoryGame.I.Deliver();else FactoryGame.I.Say("WILLOW WORKS / STAFF NOTE",note);}
}
}
