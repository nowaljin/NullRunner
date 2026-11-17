using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.IO;

public class Player25DController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float animFps = 10f;
    public float ppu = 96f;

    SpriteRenderer sr;
    Rigidbody rb;
    readonly Dictionary<string, Sprite[]> anims = new Dictionary<string, Sprite[]>();
    readonly List<Texture2D> keep = new List<Texture2D>();

    Vector2 moveInput;
    string yon = "dir8";
    float timer = 0f;
    int frame = 0;

    void Start()
    {
        const float GROUND_Y = 0.5f;
        transform.position = new Vector3(transform.position.x, GROUND_Y, transform.position.z);
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        foreach (var a in new[] { "Idle", "Walk" })
            for (int d = 1; d <= 8; d++)
                LoadSheet(a, d);

        var shadowTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        shadowTex.LoadImage(File.ReadAllBytes(Application.dataPath + "/Sprites/AdventurerFemale/Shadow.png"));
        shadowTex.Apply();
        keep.Add(shadowTex);
        var sh = new GameObject("GroundShadow");
        sh.transform.SetParent(transform);
        sh.transform.localPosition = new Vector3(0f, -0.45f, 0f);
        var ss = sh.AddComponent<SpriteRenderer>();
        ss.sprite = Sprite.Create(shadowTex, new Rect(0, 0, shadowTex.width, shadowTex.height), new Vector2(0.5f, 0.5f), 64f);
        ss.color = new Color(0f, 0f, 0f, 0.4f);
        ss.sortingOrder = 5;
        ss.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        Debug.Log("[Player25D] anim set: " + anims.Count);
    }

    void LoadSheet(string anim, int dir)
    {
        var path = Application.dataPath + "/Sprites/Knight25D/" + anim + "/Knight_" + anim + "_dir" + dir + ".png";
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(File.ReadAllBytes(path));
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        keep.Add(tex);
        int cols = tex.width / 256, rows = tex.height / 256, n = cols * rows;
        var arr = new Sprite[n];
        for (int i = 0; i < n; i++)
        {
            int cx = i % cols, cy = i / cols;
            arr[i] = Sprite.Create(tex, new Rect(cx * 256, cy * 256, 256, 256), new Vector2(0.5f, 0.5f), ppu);
        }
        anims[anim + "_dir" + dir] = arr;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        moveInput = new Vector2(x, z);
        if (moveInput.sqrMagnitude > 0.01f)
            yon = DirName(moveInput.x, moveInput.y);

        string anim = moveInput.sqrMagnitude > 0.01f ? "Walk" : "Idle";
        var arr = anims[anim + "_" + yon];
        timer += Time.deltaTime;
        frame = (int)(timer * animFps) % arr.Length;
        sr.sprite = arr[frame];
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, 0f, moveInput.y * moveSpeed);
    }

    static string DirName(float x, float z)
    {
        float ang = Mathf.Atan2(z, x) * Mathf.Rad2Deg;
        float[] angles = { -135f, 180f, 135f, 90f, 45f, 0f, -45f, -90f };
        int best = 0;
        float bd = 999f;
        for (int i = 0; i < 8; i++)
        {
            float d = Mathf.Abs(Mathf.DeltaAngle(ang, angles[i]));
            if (d < bd) { bd = d; best = i; }
        }
        return "dir" + (best + 1);
    }
}