using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 9f, 8f);
    public float smooth = 5f;

    void Start()
    {
        if (target == null)
        {
            var go = GameObject.Find("Player");
            if (go != null) target = go.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
        transform.rotation = Quaternion.Euler(44f, 0f, 0f);
    }
}