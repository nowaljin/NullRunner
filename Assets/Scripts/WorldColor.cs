using UnityEngine;

public class WorldColor : MonoBehaviour
{
    void Start()
    {
        Color dark = new Color(0.07f, 0.07f, 0.13f);
        Color wall = new Color(0.14f, 0.10f, 0.22f);
        Color cyan = new Color(0.15f, 0.95f, 1f);
        Color magenta = new Color(1f, 0.15f, 0.7f);
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            string n = go.name;
            if (n.StartsWith("Building")) SetColor(go, dark);
            else if (n.StartsWith("Wall")) SetColor(go, wall);
            else if (n == "Neon_L") SetColor(go, cyan);
            else if (n == "Neon_R") SetColor(go, magenta);
            else if (n == "NeonLight_L") { var l = go.GetComponent<Light>(); if (l != null) l.color = cyan; }
            else if (n == "NeonLight_R") { var l = go.GetComponent<Light>(); if (l != null) l.color = magenta; }
        }
    }

    static void SetColor(GameObject go, Color c)
    {
        var r = go.GetComponent<Renderer>();
        if (r == null) return;
        var m = new Material(r.sharedMaterial);
        m.color = c;
        if (go.name.StartsWith("Neon"))
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", c);
        }
        r.sharedMaterial = m;
    }
}