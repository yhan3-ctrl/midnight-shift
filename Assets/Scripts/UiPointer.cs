using UnityEngine;
namespace LastLantern {
public static class UiPointer {
    static bool hovered,applied;static Texture2D hand;
    public static bool HandVisible=>applied;
    public static void Begin(){hovered=false;}
    public static bool Hover(Rect rect){
        if(!GUI.enabled||!Cursor.visible||(FactoryGame.I&&FactoryGame.I.player&&FactoryGame.I.player.LookCaptured))return false;
        Vector3 screen=new Vector3(Input.mousePosition.x,Screen.height-Input.mousePosition.y,0);
        bool inside=rect.Contains((Vector2)GUI.matrix.inverse.MultiplyPoint3x4(screen));if(inside)hovered=true;return inside;
    }
    public static void End(){if(Event.current.type!=EventType.Repaint)return;bool wanted=hovered&&Cursor.visible&&!PlayerMotor.SystemShortcutHeld;if(applied==wanted)return;
        if(wanted&&!hand)hand=CreateHand();Cursor.SetCursor(wanted?hand:null,new Vector2(9,2),CursorMode.Auto);applied=wanted;
    }
    static Texture2D CreateHand(){
        Vector2[] shape={new Vector2(8,2),new Vector2(12,2),new Vector2(12,12),new Vector2(15,10),new Vector2(18,11),new Vector2(21,12),new Vector2(24,15),new Vector2(24,24),new Vector2(21,29),new Vector2(11,29),new Vector2(5,22),new Vector2(3,17),new Vector2(5,15),new Vector2(8,18)};
        bool[,] mask=new bool[32,32];for(int y=0;y<32;y++)for(int x=0;x<32;x++){bool inside=false;for(int i=0,j=shape.Length-1;i<shape.Length;j=i++){var a=shape[i];var b=shape[j];if((a.y>y)!=(b.y>y)&&x<(b.x-a.x)*(y-a.y)/(b.y-a.y)+a.x)inside=!inside;}mask[x,y]=inside;}
        var t=new Texture2D(32,32,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Point;for(int y=0;y<32;y++)for(int x=0;x<32;x++){bool edge=false;for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++)if(x+dx>=0&&x+dx<32&&y+dy>=0&&y+dy<32&&mask[x+dx,y+dy])edge=true;t.SetPixel(x,31-y,mask[x,y]?Color.white:edge?new Color(.03f,.08f,.09f):Color.clear);}t.Apply();return t;
    }
}
}
