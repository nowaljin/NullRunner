using UnityEngine;

public class CombatHUD : MonoBehaviour
{
    void OnGUI()
    {
        GUI.color = Color.white;
        GUI.Label(new Rect(16, 14, 620, 24), "WASD hareket | J: hafif (3x kombo) | K: agir | L: dodge/i-frame | I: block/parry");
        var cc = Object.FindFirstObjectByType<CombatController>();
        var e = Object.FindFirstObjectByType<EnemyController>();
        if (cc != null)
        {
            GUI.Label(new Rect(16, 40, 300, 20), "Player HP  " + cc.hp + " / " + cc.maxHp);
            DrawBar(new Rect(16, 62, 240, 12), cc.hp / cc.maxHp, new Color(0.2f, 0.9f, 0.5f));
        }
        if (e != null)
        {
            GUI.Label(new Rect(16, 84, 300, 20), "Enemy HP  " + e.hp + " / " + e.maxHp);
            DrawBar(new Rect(16, 106, 240, 12), e.hp / e.maxHp, new Color(0.9f, 0.2f, 0.4f));
        }
    }

    static void DrawBar(Rect r, float ratio, Color color)
    {
        GUI.color = new Color(0.15f, 0.15f, 0.2f);
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = color;
        GUI.DrawTexture(new Rect(r.x, r.y, r.width * Mathf.Clamp01(ratio), r.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
