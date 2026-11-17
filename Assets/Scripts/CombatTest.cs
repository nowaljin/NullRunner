using UnityEngine;

public class CombatTest : MonoBehaviour
{
    float t;
    int step;
    void Update()
    {
        t += Time.deltaTime;
        var p = GameObject.Find("Player");
        var e = GameObject.Find("Enemy");
        if (p == null || e == null) return;
        var cc = p.GetComponent<CombatController>();
        if (cc == null) return;
        if (step == 0 && t > 3f) { cc.StartAttack(false); Debug.Log("[Test] light 1"); step = 1; }
        else if (step == 1 && t > 3.6f) { cc.StartAttack(false); Debug.Log("[Test] light 2 (kombo)"); step = 2; }
        else if (step == 2 && t > 5f) { cc.StartAttack(true); Debug.Log("[Test] heavy"); step = 3; }
        else if (step == 3 && t > 6.5f) { cc.TakeDamage(10f, e.transform.position); Debug.Log("[Test] oyuncu hasar"); step = 4; }
        else if (step == 4 && t > 8f) { Debug.Log("[Test] bitti"); step = 5; }
    }
}
