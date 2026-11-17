using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    void Start() { Invoke(nameof(Snap), 2.5f); }
    void Snap()
    {
        ScreenCapture.CaptureScreenshot("C:/Users/student/Desktop/BozkurtIlker/Unity/NullRunner/Logs/screen.png");
        Debug.Log("[ScreenShot] cekildi");
    }
}