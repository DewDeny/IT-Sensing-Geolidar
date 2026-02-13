using TMPro;
using UnityEngine;

public class Tag_Line : MonoBehaviour
{
    public GameObject[] pointsPlaced;
    RectTransform rectTransform;
    public float lineDistance, rot;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointsPlaced[1] != null)
        {
            Vector2 linePos = (pointsPlaced[1].transform.position + pointsPlaced[0].transform.position) / 2;
            transform.position = linePos;

            lineDistance = Vector2.Distance(pointsPlaced[1].transform.position, pointsPlaced[0].transform.position);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, lineDistance);

            rot = Vector2.SignedAngle(pointsPlaced[0].transform.up, (pointsPlaced[1].transform.position - pointsPlaced[0].transform.position));
            transform.eulerAngles = new Vector3(0, 0, rot);
        }
    }
}