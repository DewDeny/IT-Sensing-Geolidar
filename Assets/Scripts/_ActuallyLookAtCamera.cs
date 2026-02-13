using UnityEngine;

public class _ActuallyLookAtCamera : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        Vector3 focusPoint=transform.position - cam.position;
        transform.rotation=Quaternion.LookRotation(focusPoint);
    }
}
