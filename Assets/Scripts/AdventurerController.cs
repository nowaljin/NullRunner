using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.IO;

public class AdventurerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float animFrameTime = 0.11f;

    SpriteRenderer sr;
    Rigidbody rb;
    Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    List<Texture2D> keep = new List<Texture2D>();

    Vector2 moveInput;
    string yon = "Down";
    string anim = "idle";
    float timer = 0f;
    int frame = 0;
    bool dead = false;
    float jumpTimer = 0f;
    float dashTimer = 0f;

    static readonly string[] AnimNames = { "idle", "walk", "dash", "death", "jump" };

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        var root = Application.dataPath + "/Sprites/AdventurerFemale";
        foreach (var path in Directory.GetFiles(root, "*.png", SearchOption.AllDirectories))
        {
            var parts = Path.GetFileNameWithoutExtension(path).Split('_');
            string a = parts[0].ToLowerInvariant();
            if (System.Array.IndexOf(AnimNames, a) < 0) continue;
            string y = parts.Length > 1 ? string.Join("_", parts, 1, parts.Length - 1) : "Down";
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(File.ReadAllBytes(path));
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            keep.Add(tex);
            int n = tex.width / 48;
            for (int i = 0; i < n; i++)
            {
                var spr = Sprite.Create(tex, new Rect(i * 48, 0, 48, 64), new Vector2(0.5f, 0.5f), 64f);
                sprites[a + "_" + y + "_" + i] = spr;
            }
        }
        Debug.Log("[Adventurer] sprite: " + sprites.Count);

        var shadowTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        shadowTex.LoadImage(File.ReadAllBytes(Application.dataPath + "/Sprites/AdventurerFemale/Shadow.png"));
        shadowTex.Apply();
        keep.Add(shadowTex);
        var shadowGO = new GameObject("GroundShadow");
        shadowGO.transform.SetParent(transform);
        shadowGO.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        var ss = shadowGO.AddComponent<SpriteRenderer>();
        ss.sprite = Sprite.Create(shadowTex, new Rect(0, 0, shadowTex.width, shadowTex.height), new Vector2(0.5f, 0.5f), 64f);
        ss.color = new Color(0f, 0f, 0f, 0.45f);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float y = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        moveInput = new Vector2(x, y);

        if (kb.xKey.wasPressedThisFrame) dead = true;
        if (!dead)
        {
            if (kb.spaceKey.wasPressedThisFrame) jumpTimer = 0.7f;
            if (kb.leftShiftKey.wasPressedThisFrame) dashTimer = 0.35f;
        }
        jumpTimer = Mathf.Max(0f, jumpTimer - Time.deltaTime);
        dashTimer = Mathf.Max(0f, dashTimer - Time.deltaTime);

        if (dead) anim = "death";
        else if (jumpTimer > 0f) anim = "jump";
        else if (dashTimer > 0f) anim = "dash";
        else if (moveInput.sqrMagnitude > 0.01f) anim = "walk";
        else anim = "idle";

        if (moveInput.sqrMagnitude > 0.01f) yon = YonBul(moveInput);

        timer += Time.deltaTime;
        frame = (int)(timer / animFrameTime);
        if (dead) frame = Mathf.Min(frame, 7);
        else frame %= 8;

        string key = anim + "_" + yon + "_" + frame;
        if (sprites.ContainsKey(key)) sr.sprite = sprites[key];
    }

    void FixedUpdate()
    {
        Vector2 v = moveInput;
        if (dashTimer > 0f) v *= 2.2f;
        rb.linearVelocity = new Vector3(v.x * moveSpeed, 0f, v.y * moveSpeed);
    }

    string YonBul(Vector2 v)
    {
        float x = v.x, y = v.y;
        if (y < -0.33f) return x > 0.33f ? "Right_Down" : (x < -0.33f ? "Left_Down" : "Down");
        if (y > 0.33f) return x > 0.33f ? "Right_Up" : (x < -0.33f ? "Left_Up" : "Up");
        return x > 0f ? "Right_Up" : "Left_Up";
    }
}
