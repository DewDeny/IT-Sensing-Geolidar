using TMPro;
using UnityEngine;

public class Tag_LineGreen : MonoBehaviour
{
   public GameObject[] pointsPlaced;
    public Vector3 sourceWorldPos, targetWorldPos, targetPseudoPos;
    Vector2 posSource, posTarget;
    RectTransform rectTransform;
    public float lineDistance, rot;
    int directionAxis;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointsPlaced[1] != null)
        {
            sourceWorldPos = pointsPlaced[0].GetComponent<Tag_Point>().pointWorldCoord;
            targetWorldPos = pointsPlaced[1].GetComponent<Tag_Point>().pointWorldCoord;

            //Determining 'phantom' Point
            if (directionAxis == 1) //vertical
            {
                targetPseudoPos = new Vector3(sourceWorldPos.x, targetWorldPos.y, sourceWorldPos.z);
            }
            else //horizontal (2)
            {
                targetPseudoPos = new Vector3(targetWorldPos.x, sourceWorldPos.y, targetWorldPos.z);
            }

            Vector2 sourcePointAtScreen = Camera.main.WorldToScreenPoint(sourceWorldPos);
            posSource = sourcePointAtScreen;

            Vector2 targetPointAtScreen = Camera.main.WorldToScreenPoint(targetPseudoPos);
            posTarget = targetPointAtScreen;

            /////////////////////////////////////////////////////////////////////

            Vector2 linePos = (posTarget + posSource) / 2;
            transform.position = linePos;

            lineDistance = Vector2.Distance(posTarget, posSource);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, lineDistance);

            rot = Vector2.SignedAngle(Vector2.up, (posTarget - posSource));
            transform.eulerAngles = new Vector3(0, 0, rot);

        }
    }

    public void SourceTargetDirection(GameObject source, GameObject target, int direction) //1=vertical,2=horizontal
    {
        pointsPlaced[0] = source.gameObject;
        pointsPlaced[1] = target.gameObject;
        directionAxis = direction;
    }
}