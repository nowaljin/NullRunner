using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHp = 70f;
    public float hp = 70f;
    public float speed = 2.4f;
    public float damage = 12f;
    public float attackRange = 2.0f;
    public float telegraphTime = 0.55f;
    public bool isDead { get { return hp <= 0f; } }

    enum State { Patrol, Chase, Telegraph, Attack, Recover, Stagger }
    State state = State.Patrol;
    float timer;
    Renderer rend;
    Transform player;
    readonly Color normal = new Color(0.55f, 0.2f, 0.6f);

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) { var m = new Material(rend.sharedMaterial); m.color = normal; rend.sharedMaterial = m; }
        var go = GameObject.Find("Player");
        if (go != null) player = go.transform;
        Debug.Log("[Enemy] hazir, player=" + (player != null));
    }

    void Update()
    {
        if (isDead) { if (rend != null) rend.material.color = new Color(0.08f, 0.08f, 0.08f); return; }
        if (player == null) return;
        float dt = Time.deltaTime;
        Vector3 to = player.position - transform.position; to.y = 0f;
        float dist = to.magnitude;
        timer -= dt;

        switch (state)
        {
            case State.Patrol:
                transform.position += new Vector3(Mathf.Sin(Time.time * 0.7f), 0f, Mathf.Cos(Time.time * 0.5f)) * speed * 0.25f * dt;
                if (dist < 9f) { state = State.Chase; Debug.Log("[Enemy] Chase"); }
                break;
            case State.Chase:
                if (dist > 0.1f) transform.position += to.normalized * speed * dt;
                if (dist <= attackRange) { state = State.Telegraph; timer = telegraphTime; Debug.Log("[Enemy] Telegraph"); }
                break;
            case State.Telegraph:
                if (rend != null) rend.material.color = new Color(1f, 0.25f, 0.6f);
                if (timer <= 0f) { state = State.Attack; timer = 0.12f; DoAttack(); }
                break;
            case State.Attack:
                if (timer <= 0f) { state = State.Recover; timer = 0.6f; if (rend != null) rend.material.color = normal; }
                break;
            case State.Recover:
                if (timer <= 0f) state = State.Chase;
                break;
            case State.Stagger:
                if (rend != null) rend.material.color = Color.white;
                if (timer <= 0f) { state = State.Chase; if (rend != null) rend.material.color = normal; }
                break;
        }
    }

    void DoAttack()
    {
        if (player == null) return;
        if (Vector3.Distance(player.position, transform.position) <= attackRange + 0.5f)
        {
            var cc = player.GetComponent<CombatController>();
            if (cc != null) cc.TakeDamage(damage, transform.position);
        }
    }

    public void TakeDamage(float amount, Vector3 from, float knockback)
    {
        if (isDead) return;
        hp -= amount;
        Vector3 dir = (transform.position - from).normalized; dir.y = 0f;
        transform.position += dir * knockback;
        Stagger(0.4f);
        Debug.Log("[Enemy] HP " + hp + "/" + maxHp);
        if (hp <= 0f) { hp = 0f; Debug.Log("[Enemy] Dead"); }
    }

    public void Stagger(float seconds) { state = State.Stagger; timer = seconds; }
}
