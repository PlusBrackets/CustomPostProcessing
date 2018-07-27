using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureDebug : MonoBehaviour {

    public static TextureDebug Ins;
    public Texture2D target; 

    private void Awake()
    {
        Ins = this;
    }

    private void OnGUI()
    {
        if(target!=null)
        {
            Rect pos = new Rect(0, 0, target.width, target.height);
            GUI.DrawTexture(pos, target);
        }
    }

    private void OnDestroy()
    {
        Ins = null;
    }

}
