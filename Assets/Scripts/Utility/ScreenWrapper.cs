using UnityEngine;

public class ScreenWrapper : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 pos = transform.position;
        Vector3 viewportPos = cam.WorldToViewportPoint(pos);

        if (viewportPos.x < -0.05f) viewportPos.x = 1.05f;
        else if (viewportPos.x > 1.05f) viewportPos.x = -0.05f;

        if (viewportPos.y < -0.05f) viewportPos.y = 1.05f;
        else if (viewportPos.y > 1.05f) viewportPos.y = -0.05f;

        transform.position = cam.ViewportToWorldPoint(viewportPos);
    }
}
