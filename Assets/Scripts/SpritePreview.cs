using UnityEngine;
using System.IO;

public class SpritePreview : MonoBehaviour
{
    void Start()
    {
        var path = "Assets/Sprites/Knight25D/Walk/Knight_Walk_dir8.png";
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(File.ReadAllBytes(Path.GetFullPath(path)));
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f), 64f);
        sr.sortingOrder = 10;
    }
}