using TMPro;
using UnityEngine;

public class Tag_Angle : MonoBehaviour
{
    public GameObject[] pointsPlaced;
    Vector2[] canvasPos;
    Vector3[] worldPos;

    public bool placed = false;
    TextMeshProUGUI txt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        txt = transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();

        canvasPos = new Vector2[3];
        worldPos = new Vector3[3];
    }

    // Update is called once per frame
    void Update()
    {
        if (pointsPlaced[2] != null)
        {
            for (int i = 0; i < pointsPlaced.Length; i++) // Canvas (2D) position of the point marker
            {
                canvasPos[i] = pointsPlaced[i].transform.position;
            }

            Vector2 canvasVector1 = canvasPos[0] - canvasPos[1];
            Vector2 canvasVector2 = canvasPos[2] - canvasPos[1];
            Vector2 canvasCenterOfAngle = (canvasVector2 + canvasVector1) / 2;
            transform.position = canvasPos[1] + canvasCenterOfAngle.normalized * 40;

            if (!placed) // for displaying actual values
            {
                for (int i = 0; i < pointsPlaced.Length; i++) // World (3D) position of the actual position
                {
                    worldPos[i] = pointsPlaced[i].GetComponent<Tag_Point>().pointWorldCoord;
                }

                Vector3 worldVector1 = worldPos[0] - worldPos[1];
                Vector3 worldVector2 = worldPos[2] - worldPos[1];
                float theWorldAngle = Vector3.Angle(worldVector2, worldVector1);
                string txtAngle = theWorldAngle.ToString("f1");
                txt.text = new string(txtAngle + "°");
            }
        }
    }
}