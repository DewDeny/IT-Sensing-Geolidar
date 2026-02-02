using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Tag_Circle : MonoBehaviour
{
    public GameObject[] pointsPlaced;
    Vector3[] pointPos;
    Vector3 circleCenter;
    bool isAzimuth;

    void Start()
    {
        pointPos = new Vector3[3];    
    }

    // Update is called once per frame
    void Update()
    {
        if (isAzimuth)
        {
            float radius = Vector3.Distance(pointsPlaced[0].GetComponent<Tag_Point>().pointWorldCoord, pointsPlaced[1].GetComponent<Tag_Point>().pointWorldCoord);

            transform.position = pointsPlaced[0].GetComponent<Tag_Point>().pointWorldCoord;
            transform.localScale = new Vector3(radius, radius, radius) * 2;

            Quaternion _lookRotation = Quaternion.LookRotation(Vector3.forward, Vector3.right);
            transform.rotation = _lookRotation;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                pointPos[i] = pointsPlaced[i].GetComponent<Tag_Point>().pointWorldCoord;
            }

            if (pointPos[2] != null)
            {
                Vector3 v1 = pointPos[1] - pointPos[0];
                Vector3 v2 = pointPos[2] - pointPos[0];
                float v1v1 = Vector3.Dot(v1, v1);
                float v2v2 = Vector3.Dot(v2, v2);
                float v1v2 = Vector3.Dot(v1, v2);

                float b = 0.5f / (v1v1 * v2v2 - v1v2 * v1v2);
                float k1 = b * v2v2 * (v1v1 - v1v2);
                float k2 = b * v1v1 * (v2v2 - v1v2);
                circleCenter = pointPos[0] + v1 * k1 + v2 * k2;

                float radius = Vector3.Distance(circleCenter, pointPos[0]);

                transform.position = circleCenter;
                transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);

                //rotation
                Vector3 mid_direction = (pointPos[1] - circleCenter).normalized;
                Vector3 first_direction = (pointPos[0] - circleCenter).normalized;
                Quaternion _lookRotation = Quaternion.LookRotation(mid_direction, first_direction);
                transform.rotation = _lookRotation;
            }
        }
    }

    public void BecomePerimeter(GameObject givenTarget1, GameObject givenTarget2)
    {
        isAzimuth = true;
        pointsPlaced[0] = givenTarget1;
        pointsPlaced[1] = givenTarget2;
    }
}