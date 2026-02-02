// example script for making object always look towards camera

using UnityEngine;

namespace pointcloudviewer.examples
{
    public class LookAtCamera : MonoBehaviour
    {
        Transform cam;

        void Start()
        {
            cam = Camera.main.transform;
        }

        void LateUpdate()
        {
            transform.LookAt(cam);
        }
    }
}