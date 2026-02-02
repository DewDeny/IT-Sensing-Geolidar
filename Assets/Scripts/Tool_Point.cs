using System;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Tool_Point : MonoBehaviour
{
    //Base functions
    public GameObject pointPref, coordPref, distancePref, anglePref, linePref, lineGreenPref, circlePref;
    public GameObject pointsGroup, tagsGroup, nonCanvasGroup;
    GameObject pointBeingDragged, tagBeingDragged, lineBeingDragged;
    public GameObject[] temp_PointStorage, temp_TagStorage;
    int tool;
    Vector3 hitPos;
    bool startPointing;
    int pointAmount;

    //----------------------------------------------
    //Notes & Links
    //https://github.com/CristobalBL/pointcloud-unity-example githubpoint cloud thingy??
    //https://www.lidarusa.com/sample-data.html USA lidar data & viewer
    //https://discussions.unity.com/t/import-point-cloud-to-unity/736241/ Putting e57(?) file into array for Particle usage(?)
    //----------------------------------------------

    // Start is called blablabla
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, Mathf.Infinity) && hit.transform.gameObject.layer != 5 && startPointing) //Dragging the point marker
        {
            hitPos = hit.point;
            pointBeingDragged.GetComponent<Tag_Point>().pointWorldCoord = hitPos;
        }

        if (Input.GetMouseButtonDown(0) && startPointing) //Placing down the point marker
        {
            switch (tool)
            {
                case 1:
                    if (pointAmount == 1)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            temp_TagStorage[i].GetComponent<Tag_Angle>().placed = true;
                        }
                    }
                    break;

                case 2:
                    tagBeingDragged.GetComponent<Tag_Coord>().placed = true;
                    break;

                case 3:
                    if (tagBeingDragged != null)
                        tagBeingDragged.GetComponent<Tag_Distance>().placed = true;
                    break;
            }

            pointAmount -= 1;
            if (pointAmount == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    temp_PointStorage[i] = null;
                    temp_TagStorage[i] = null;
                }

                pointBeingDragged = null;
                lineBeingDragged = null;
                tagBeingDragged = null;

                startPointing = false;
            }
            else
                StartPointing(tool);
        }

        if (Input.GetMouseButtonDown(1) | Input.GetKeyDown(KeyCode.Escape)) //Cancelling the marker
        {
            Destroy(pointBeingDragged);
            Destroy(tagBeingDragged);
            Destroy(lineBeingDragged);
            pointAmount = 0;
            startPointing = false;
        }
    }

    public void StartPointing(int toolNumber) //Spawn a point marker
    {
        tool = toolNumber;
        Vector2 hidePos = hitPos;

        switch (tool)
        {
            case 1: //Angle Measurement

                if (pointAmount < 3)
                {
                    lineBeingDragged = Instantiate(linePref, hidePos, Quaternion.identity, pointsGroup.transform);
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[0] = pointBeingDragged;
                }

                pointBeingDragged = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform);
                int temp_SlotNumberAngle = 3 - pointAmount;
                temp_PointStorage[temp_SlotNumberAngle] = pointBeingDragged;

                if (pointAmount < 3)
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[1] = pointBeingDragged;

                if (pointAmount == 1)
                {
                    lineBeingDragged = Instantiate(linePref, hidePos, Quaternion.identity, pointsGroup.transform);
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[0] = pointBeingDragged;
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[1] = temp_PointStorage[0];

                    for (int i = 0; i < 3; i++)
                    {
                        tagBeingDragged = Instantiate(anglePref, hidePos, Quaternion.identity, tagsGroup.transform);
                        temp_TagStorage[i] = tagBeingDragged;

                        tagBeingDragged.GetComponent<Tag_Angle>().pointsPlaced[0] = temp_PointStorage[Mathf.RoundToInt(Mathf.Repeat(i, 3))];
                        tagBeingDragged.GetComponent<Tag_Angle>().pointsPlaced[1] = temp_PointStorage[Mathf.RoundToInt(Mathf.Repeat(i + 1, 3))];
                        tagBeingDragged.GetComponent<Tag_Angle>().pointsPlaced[2] = temp_PointStorage[Mathf.RoundToInt(Mathf.Repeat(i + 2, 3))];
                    }
                }
                break;

            case 2: //Point measurement
                pointBeingDragged = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform);
                tagBeingDragged = Instantiate(coordPref, hidePos, Quaternion.identity, tagsGroup.transform);
                tagBeingDragged.GetComponent<Tag_Coord>().pointPlaced = pointBeingDragged;
                break;

            case 3: //Distance measurement
                if (pointAmount == 1)
                {
                    lineBeingDragged = Instantiate(linePref, hidePos, Quaternion.identity, pointsGroup.transform);
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[0] = pointBeingDragged;
                    tagBeingDragged = Instantiate(distancePref, hidePos, Quaternion.identity, tagsGroup.transform);
                    tagBeingDragged.GetComponent<Tag_Distance>().pointsPlaced[0] = pointBeingDragged;
                }

                pointBeingDragged = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform);

                if (pointAmount == 1)
                {
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[1] = pointBeingDragged;
                    tagBeingDragged.GetComponent<Tag_Distance>().pointsPlaced[1] = pointBeingDragged;
                    pointAmount += 1;
                }
                break;

            case 4://Height
                if (pointAmount == 1)
                {
                    lineBeingDragged = Instantiate(linePref, hidePos, Quaternion.identity, pointsGroup.transform);
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[0] = pointBeingDragged;
                }

                pointBeingDragged = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform);
                int temp_SlotNumberHeight = 2 - pointAmount;
                temp_PointStorage[temp_SlotNumberHeight] = pointBeingDragged;

                if (pointAmount == 1)
                {
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[1] = pointBeingDragged;

                    lineBeingDragged = Instantiate(lineGreenPref, hidePos, Quaternion.identity, pointsGroup.transform); //green line 
                    Tag_LineGreen tagLineGreen = lineBeingDragged.GetComponent<Tag_LineGreen>();
                    tagLineGreen.SourceTargetDirection(temp_PointStorage[0], temp_PointStorage[1], 2); //horizontal

                    temp_TagStorage[0] = lineBeingDragged;

                    lineBeingDragged = Instantiate(lineGreenPref, hidePos, Quaternion.identity, pointsGroup.transform); //green line
                    tagLineGreen = lineBeingDragged.GetComponent<Tag_LineGreen>();
                    tagLineGreen.SourceTargetDirection(temp_PointStorage[1], temp_PointStorage[0], 1); //vertical

                    temp_TagStorage[1] = lineBeingDragged;

                    tagBeingDragged = Instantiate(distancePref, hidePos, Quaternion.identity, tagsGroup.transform);
                    tagBeingDragged.GetComponent<Tag_Distance>().isHeight = true;
                    tagBeingDragged.GetComponent<Tag_Distance>().soloPosObject = lineBeingDragged;
                    tagBeingDragged.GetComponent<Tag_Distance>().pointsPlaced[0] = temp_PointStorage[0];
                    tagBeingDragged.GetComponent<Tag_Distance>().pointsPlaced[1] = temp_PointStorage[1];
                }
                break;

            case 5: //Circle
                if (pointAmount == 1)
                {
                    pointBeingDragged = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform); //center of circle

                    lineBeingDragged = Instantiate(linePref, hidePos, Quaternion.identity, tagsGroup.transform); //radius line
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[0] = temp_PointStorage[1];
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[1] = pointBeingDragged;

                    tagBeingDragged = Instantiate(distancePref, hidePos, Quaternion.identity, tagsGroup.transform); //distance
                    tagBeingDragged.GetComponent<Tag_Distance>().pointsPlaced[0] = temp_PointStorage[1];
                    tagBeingDragged.GetComponent<Tag_Distance>().pointsPlaced[1] = pointBeingDragged;

                    lineBeingDragged = Instantiate(circlePref, hidePos, Quaternion.identity, nonCanvasGroup.transform); //the circle
                    lineBeingDragged.GetComponent<Tag_Circle>().pointsPlaced[0] = temp_PointStorage[0];
                    lineBeingDragged.GetComponent<Tag_Circle>().pointsPlaced[1] = temp_PointStorage[1];

                    pointBeingDragged.GetComponent<Tag_Point>().BecomeFollower(lineBeingDragged);
                }

                pointBeingDragged = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform);
                int temp_SlotNumCircle = 3 - pointAmount;
                temp_PointStorage[temp_SlotNumCircle] = pointBeingDragged;

                if (pointAmount == 1)
                {
                    lineBeingDragged.GetComponent<Tag_Circle>().pointsPlaced[2] = temp_PointStorage[2];
                }
                break;

            case 6: //Azimuth
                pointBeingDragged = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform);
                int temp_SlotNumAzim = 2 - pointAmount;
                temp_PointStorage[temp_SlotNumAzim] = pointBeingDragged;

                if (pointAmount == 1)
                {
                    //line between 0 and 1
                    lineBeingDragged = Instantiate(linePref, hidePos, Quaternion.identity, pointsGroup.transform);
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[0] = temp_PointStorage[0];
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[1] = temp_PointStorage[1];

                    //Shadow point of 1
                    GameObject shadowPoint = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform);
                    shadowPoint.GetComponent<Tag_Point>().BecomeShadow(temp_PointStorage[0], temp_PointStorage[1]);

                    //line, between 0 and Shadow
                    lineBeingDragged = Instantiate(linePref, hidePos, Quaternion.identity, pointsGroup.transform);
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[0] = temp_PointStorage[0];
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[1] = shadowPoint;

                    //Circle line
                    lineBeingDragged = Instantiate(circlePref, hidePos, Quaternion.identity, nonCanvasGroup.transform);
                    lineBeingDragged.GetComponent<Tag_Circle>().BecomePerimeter(temp_PointStorage[0], temp_PointStorage[1]);

                    //North point
                    GameObject northPoint = Instantiate(pointPref, hidePos, Quaternion.identity, pointsGroup.transform);
                    northPoint.GetComponent<Tag_Point>().BecomeNorth(temp_PointStorage[0], temp_PointStorage[1]);

                    //line, between 0 and North
                    lineBeingDragged = Instantiate(linePref, hidePos, Quaternion.identity, pointsGroup.transform);
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[0] = temp_PointStorage[0];
                    lineBeingDragged.GetComponent<Tag_Line>().pointsPlaced[1] = northPoint;

                    //angle tag between North and 1
                    tagBeingDragged = Instantiate(anglePref, hidePos, Quaternion.identity, tagsGroup.transform);
                    tagBeingDragged.GetComponent<Tag_Angle>().pointsPlaced[0] = northPoint;
                    tagBeingDragged.GetComponent<Tag_Angle>().pointsPlaced[1] = temp_PointStorage[0];
                    tagBeingDragged.GetComponent<Tag_Angle>().pointsPlaced[2] = shadowPoint;
                }
                break;
        }
        startPointing = true;
    }

    // ACCESSED BY BUTTONS
    public void Measure_Angle()
    {
        pointAmount = 3;
        StartPointing(1);
    }

    public void Measure_Point()
    {
        pointAmount = 1;
        StartPointing(2);
    }

    public void Measure_Distance()
    {
        pointAmount = 2;
        StartPointing(3);
    }

    public void Measure_Height()
    {
        pointAmount = 2;
        StartPointing(4);
    }

    public void Measure_Circle()
    {
        pointAmount = 3;
        StartPointing(5);
    }

    public void Measure_Azimuth()
    {
        pointAmount = 2;
        StartPointing(6);
    }

    //area
    //cube
    //sphere
    //surface/projection
    //annotation

    public void Measure_Remove_All()
    {
        foreach (Transform offsprings in pointsGroup.transform)
        {
            Destroy(offsprings.gameObject);
        }

        foreach (Transform offsprings in tagsGroup.transform)
        {
            Destroy(offsprings.gameObject);
        }

        foreach (Transform offsprings in nonCanvasGroup.transform)
        {
            Destroy(offsprings.gameObject);
        }
    }
}