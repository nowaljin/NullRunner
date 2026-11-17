using UnityEngine;
using System.IO;

public class GroundSprite : MonoBehaviour
{
    void Start()
    {
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(File.ReadAllBytes(Application.dataPath + "/Sprites/Ground.png"));
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16f);
    }
}