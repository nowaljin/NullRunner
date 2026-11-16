using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor;

public class AdventurerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float animFrameTime = 0.11f;

    SpriteRenderer sr;
    Rigidbody2D rb;
    Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

    Vector2 moveInput;
    string yon = "Down";
    string anim = "idle";
    float timer = 0f;
    int frame = 0;
    bool dead = false;
    float jumpTimer = 0f;
    float dashTimer = 0f;

    static readonly string[] AnimKeys = { "Idle", "walk", "Dash", "death", "Jump" };

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        string[] folders = { "Walk", "Idle", "Dash", "Death", "Jump - NEW/Normal", "Shadow" };
        foreach (var f in folders)
        {
            var path = "Assets/Sprites/AdventurerFemale/" + f;
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(path))
                if (o is Sprite s) sprites[s.name] = s;
        }
        Debug.Log("[Adventurer] sprite yuklendi: " + sprites.Count);
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
        if (dead) frame = Mathf.Min(frame, 5);
        else frame %= 6;

        string key = AnimKeys[System.Array.IndexOf(AnimKeys, anim)] + "_" + yon + "_" + frame;
        if (sprites.ContainsKey(key)) sr.sprite = sprites[key];
    }

    void FixedUpdate()
    {
        Vector2 v = moveInput;
        if (dashTimer > 0f) v *= 2.2f;
        rb.linearVelocity = v * moveSpeed;
    }

    string YonBul(Vector2 v)
    {
        float x = v.x, y = v.y;
        if (y < -0.33f) return x > 0.33f ? "Right_Down" : (x < -0.33f ? "Left_Down" : "Down");
        if (y > 0.33f) return x > 0.33f ? "Right_Up" : (x < -0.33f ? "Left_Up" : "Up");
        return x > 0f ? "Right_Up" : "Left_Up";
    }
}