using PointCloudViewer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//next step = utilize Line Renderer's "Size" parameter to chain multiple lines for Distance Measurement, instead of making another line.

public class PlaceMarkersCUSTOM : MonoBehaviour
{
    Camera cam;
    public PointCloudManager pointCloudManager;
    // public GameObject prefab; //old
    //  public Vector3 offset = new Vector3(0, 10, 0); //old

    [Header("LIDAR requirements")]
    public GameObject markerContainer;
    public GameObject pointPref, linePref, coordinatePref, distancePref;

    //LIDAR Measurement
    GameObject draggedMarker, draggedLine, draggedTag;
    bool isDraggingMarker;
    int toolNum, markerOnHand;
    Vector3 tempPos;

    void Start()
    {
        cam = Camera.main;

        // subscribe to event listener
        PointCloudManager.PointWasSelected -= ProcessPointToTool; // unsubscribe just in case
        PointCloudManager.PointWasSelected += ProcessPointToTool;
    }

    public virtual void Update()
    {
        // dont do pick if over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (isDraggingMarker)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            pointCloudManager.RunPointPickingThread(ray); //scanning for nearist point from raycast
                                                          //  ToolProcessUpdate();

            if (Input.GetMouseButtonDown(0))
            {
                markerOnHand--;
                Debug.Log("marker on hand = " + markerOnHand);

                if (markerOnHand <= 0)
                {
                    draggedMarker = null;
                    draggedLine = null;
                    isDraggingMarker = false;
                }
                else
                    ToolStart(toolNum);
            }
        }
    }

    void ProcessPointToTool(Vector3 pos) //basically Update with built-in raycast position
    {
        if (draggedMarker != null)
        {
            draggedMarker.transform.position = pos;
            Debug.Log("placement going");
            TMP_Text txt;

            switch (toolNum)
            {
                case 1: //angle
                    break;

                case 2: //point
                    txt = draggedTag.GetComponentInChildren<TMP_Text>();
                    txt.text = new string(pos.x.ToString("f2") + " / " + pos.z.ToString("f2") + " / " + pos.y.ToString("f2"));
                    break;

                case 3: //distance
                    if (markerOnHand == 1)
                    {
                        LineRenderer lineData = draggedLine.GetComponent<LineRenderer>();
                        lineData.SetPosition(1, draggedMarker.transform.position);

                        Vector3 midPos = (draggedMarker.transform.position + tempPos) / 2;
                        draggedTag.transform.position = midPos;

                        float dist = Vector3.Distance(tempPos, draggedMarker.transform.position);
                        txt = draggedTag.GetComponentInChildren<TMP_Text>();
                        txt.text = new string(dist.ToString("f2"));
                    }
                    break;
            }
        }
    }

    void ToolStart(int caseNum) //mmmmmm
    {
        toolNum = caseNum;

        switch (caseNum)
        {
            case 1: //angle
                break;

            case 2: //point
                draggedMarker = Instantiate(pointPref, Vector3.zero, Quaternion.identity, markerContainer.transform);
                draggedTag = Instantiate(coordinatePref, Vector3.zero, Quaternion.identity, draggedMarker.transform);
                draggedTag.transform.localPosition = Vector3.up * 5;
                isDraggingMarker = true;
                break;

            case 3: //distance
                if (markerOnHand == 1)
                {
                    draggedLine = Instantiate(linePref, Vector3.zero, Quaternion.identity, markerContainer.transform);
                    LineRenderer lineData = draggedLine.GetComponent<LineRenderer>();
                    lineData.SetPosition(0, draggedMarker.transform.position);
                    draggedTag = Instantiate(distancePref, Vector3.zero, Quaternion.identity, markerContainer.transform);
                    tempPos = draggedMarker.transform.position;
                }

                draggedMarker = Instantiate(pointPref, Vector3.zero, Quaternion.identity, markerContainer.transform);
                isDraggingMarker = true;
                break;
        }
    }

    public void Measure_Angle()
    {
        markerOnHand = 3;
        ToolStart(1);
    }

    public void Measure_Point()
    {
        markerOnHand = 1;
        ToolStart(2);
    }

    public void Measure_Distance()
    {
        markerOnHand = 2;
        ToolStart(3);
    }

    public void Measure_Height()
    {
        markerOnHand = 2;
        ToolStart(4);
    }

    public void Measure_Circle()
    {
        markerOnHand = 3;
        ToolStart(5);
    }

    public void Measure_Azimuth()
    {
        markerOnHand = 2;
        ToolStart(6);
    }

    //area
    //cube
    //sphere
    //surface/projection
    //annotation

    void OnDestroy()
    {
        // unsubscribe
        PointCloudManager.PointWasSelected -= ProcessPointToTool;
    }
}