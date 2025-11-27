using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ToolPoint : MonoBehaviour
{
    public GameObject pointPref, pointCoordPref;
    public Canvas coordCanvas;
    GameObject pointCurrent, pointCoord;
    Vector3 hitPos;
    public bool startPointing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, Mathf.Infinity) && hit.transform.gameObject.layer != 5 && startPointing)
        {
            hitPos = hit.point;
            pointCurrent.transform.position = hitPos;
            pointCoord.GetComponent<PointCoordinateTag>().pointCoordNumber = hitPos;

            //      pointCoord.transform.position = Input.mousePosition+(Vector3.up*2);
        }

        if (Input.GetMouseButtonDown(0))
        {
            startPointing = false;

        }
    }

    public void StartPointing()
    {
        Vector3 hidePos = Camera.main.transform.position - Vector3.forward * 10;
        pointCurrent = Instantiate(pointPref, hidePos, Quaternion.identity);
        pointCoord = Instantiate(pointCoordPref, hidePos, Quaternion.identity, coordCanvas.transform);
        pointCoord.GetComponent<PointCoordinateTag>().pointCurrent = pointCurrent;
        startPointing = true;
    }
}