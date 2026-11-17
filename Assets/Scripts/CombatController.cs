using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    public float maxHp = 100f;
    public float hp = 100f;
    public float lightDamage = 14f;
    public float heavyDamage = 32f;
    public float attackRange = 2.4f;
    public bool debugLight, debugHeavy, debugDodge, debugDamage;

    public bool movementLocked { get { return attackTimer > 0f || dodgeTimer > 0f; } }

    float attackTimer, attackHitAt, attackCooldown, comboWindow, heavyWindup;
    float dodgeTimer, iFrame, blockTime = -1f, staggerTime;
    bool attackHitDone;
    int comboStep;
    Vector3 facing = Vector3.forward, dodgeDir;
    Renderer rend;
    Color normalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) normalColor = rend.material.color;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        float dt = Time.deltaTime;
        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        if (x != 0f || z != 0f) facing = new Vector3(x, 0f, z).normalized;

        attackTimer = Mathf.Max(0f, attackTimer - dt);
        attackCooldown = Mathf.Max(0f, attackCooldown - dt);
        comboWindow = Mathf.Max(0f, comboWindow - dt);
        dodgeTimer = Mathf.Max(0f, dodgeTimer - dt);
        iFrame = Mathf.Max(0f, iFrame - dt);
        staggerTime = Mathf.Max(0f, staggerTime - dt);

        if (kb.iKey.isPressed) { if (blockTime < 0f) blockTime = 0f; else blockTime += dt; }
        else blockTime = -1f;

        if (debugLight) { debugLight = false; StartAttack(false); }
        if (debugHeavy) { debugHeavy = false; StartAttack(true); }
        if (debugDodge) { debugDodge = false; dodgeTimer = 0.32f; iFrame = 0.3f; dodgeDir = facing; Debug.Log("[Combat] Dodge (i-frame)"); }
        if (debugDamage) { debugDamage = false; TakeDamage(10f, new Vector3(0f, 0.5f, 7f)); }
        if (staggerTime <= 0f)
        {
            if (kb.lKey.wasPressedThisFrame && dodgeTimer <= 0f)
            {
                dodgeTimer = 0.32f; iFrame = 0.3f;
                dodgeDir = (x != 0f || z != 0f) ? new Vector3(x, 0f, z).normalized : facing;
                Debug.Log("[Combat] Dodge (i-frame)");
            }
            if (attackTimer <= 0f && attackCooldown <= 0f)
            {
                if (kb.kKey.wasPressedThisFrame) StartAttack(true);
                else if (kb.jKey.wasPressedThisFrame) StartAttack(false);
            }
            if (attackHitAt > 0f)
            {
                attackHitAt -= dt;
                if (attackHitAt <= 0f && !attackHitDone) { DoHit(); attackHitDone = true; }
            }
        }

        if (dodgeTimer > 0f) transform.position += dodgeDir * 7f * dt;

        if (rend != null)
        {
            Color c = normalColor;
            if (iFrame > 0f) c = Color.cyan;
            else if (blockTime >= 0f) c = Color.gray;
            else if (attackTimer > 0f) c = Color.white;
            rend.material.color = c;
        }
    }

    public void StartAttack(bool heavy)
    {
        if (heavy)
        {
            heavyWindup = 0.32f; attackHitAt = 0.32f;
            attackTimer = 0.55f; attackCooldown = 0.8f; comboStep = 0;
        }
        else
        {
            comboStep = comboWindow > 0f ? (comboStep % 3) + 1 : 1;
            comboWindow = 0.55f; heavyWindup = 0.12f; attackHitAt = 0.12f;
            attackTimer = 0.28f; attackCooldown = 0.18f;
        }
        attackHitDone = false;
        CombatVFX.Slash(transform.position + facing * 0.9f + Vector3.up * 0.4f, facing,
            heavy ? new Color(1f, 0.4f, 0.1f) : new Color(0.4f, 1f, 1f), heavy ? 1.8f : 1.1f);
        Debug.Log("[Combat] " + (heavy ? "Heavy" : "Light") + " combo=" + comboStep);
    }

    void DoHit()
    {
        bool heavy = heavyWindup > 0.2f;
        float dmg = heavy ? heavyDamage : lightDamage * (comboStep == 3 ? 1.8f : 1f);
        float range = heavy ? attackRange + 0.8f : attackRange;
        int hits = 0;
        foreach (var e in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
        {
            if (e == null || e.isDead) continue;
            Vector3 to = e.transform.position - transform.position; to.y = 0f;
            if (to.magnitude <= range && Vector3.Dot(facing, to.normalized) > 0.15f)
            {
                e.TakeDamage(dmg, transform.position, heavy ? 1.2f : 0.45f);
                hits++;
            }
        }
        if (hits > 0) Debug.Log("[Combat] Hit " + hits + " enemy, dmg=" + dmg);
    }

    public void TakeDamage(float amount, Vector3 from)
    {
        if (iFrame > 0f) { Debug.Log("[Combat] i-frame, no damage"); return; }
        if (blockTime >= 0f)
        {
            if (blockTime <= 0.18f)
            {
                Debug.Log("[Combat] PARRY!");
                foreach (var e in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
                    if (e != null && !e.isDead && Vector3.Distance(e.transform.position, from) < 0.2f) e.Stagger(1.2f);
                CombatVFX.Slash(transform.position + Vector3.up, (transform.position - from).normalized, Color.yellow, 1.5f);
                return;
            }
            amount *= 0.2f;
            Debug.Log("[Combat] Blocked, dmg=" + amount);
        }
        hp -= amount;
        CombatVFX.Spark(transform.position + Vector3.up * 0.5f, new Color(1f, 0.2f, 0.3f));
        Debug.Log("[Combat] Player HP " + hp);
        if (hp <= 0f) hp = 0f;
    }
}
