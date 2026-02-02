using UnityEngine;
//using TMPro;

public class Tag_Point : MonoBehaviour
{
    public Vector3 pointWorldCoord;
    GameObject followTarget;
    GameObject[] shadowTargets;
    bool isFollow, isShadow, isNorth;

    // Update is called once per frame
    void Update()
    {
        if (isFollow)
            pointWorldCoord = followTarget.transform.position;

        if (isShadow)
        {
            pointWorldCoord = new Vector3(
                shadowTargets[1].GetComponent<Tag_Point>().pointWorldCoord.x, 
                shadowTargets[0].GetComponent<Tag_Point>().pointWorldCoord.y,
                shadowTargets[1].GetComponent<Tag_Point>().pointWorldCoord.z);
        }

        if (isNorth)
        {
            float radius = Vector3.Distance(shadowTargets[0].GetComponent<Tag_Point>().pointWorldCoord, shadowTargets[1].GetComponent<Tag_Point>().pointWorldCoord);
            pointWorldCoord = shadowTargets[0].GetComponent<Tag_Point>().pointWorldCoord + (new Vector3(0, 0, radius));
        }

        Vector2 pointAtScreen = Camera.main.WorldToScreenPoint(pointWorldCoord);
        transform.position = pointAtScreen;
    }

    public void BecomeFollower(GameObject givenTarget)
    {
        isFollow = true;
        followTarget = givenTarget;
    }

    public void BecomeShadow(GameObject givenTarget1,GameObject givenTarget2)
    {
        isShadow = true;
        shadowTargets = new GameObject[2];
        shadowTargets[0] = givenTarget1;
        shadowTargets[1] = givenTarget2;
    }

    public void BecomeNorth(GameObject givenTarget1, GameObject givenTarget2)
    {
        isNorth = true;
        shadowTargets = new GameObject[2];
        shadowTargets[0] = givenTarget1;
        shadowTargets[1] = givenTarget2;
    }
}