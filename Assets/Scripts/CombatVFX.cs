using UnityEngine;

public static class CombatVFX
{
    public static void Slash(Vector3 pos, Vector3 dir, Color color, float size)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "VFX_Slash";
        Object.Destroy(go.GetComponent<Collider>());
        go.transform.position = pos;
        go.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 0f, 35f);
        go.transform.localScale = new Vector3(0.08f, 0.08f, size);
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.color = color;
        go.GetComponent<Renderer>().sharedMaterial = m;
        Object.Destroy(go, 0.16f);
    }

    public static void Spark(Vector3 pos, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "VFX_Spark";
        Object.Destroy(go.GetComponent<Collider>());
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * 0.35f;
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.color = color;
        go.GetComponent<Renderer>().sharedMaterial = m;
        Object.Destroy(go, 0.2f);
    }
}
