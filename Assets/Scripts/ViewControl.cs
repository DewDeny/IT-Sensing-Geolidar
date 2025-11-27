using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewControl : MonoBehaviour
{
    Vector2 mouseInitPos, mousePos;
    Vector3 cameraInitPos, cameraInitRot;
    GameObject cameraObj;
    float cameraDistance;
    public bool moveWorld, stickMouse;
    List<RaycastResult> list;

    // Start is called before the first frame update
    void Start()
    {
        cameraInitPos = transform.localPosition;
        cameraInitRot = transform.localEulerAngles;
        cameraObj = transform.GetChild(0).gameObject;
        cameraDistance = cameraObj.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stickMouse)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, list);

            if (list.Count == 0 || list[0].sortingLayer != 5)
                moveWorld = true;
            else
                moveWorld = false;
        }

        //Rotation control
        if (Input.GetMouseButtonDown(0))
        {
            mouseInitPos = Input.mousePosition;
            stickMouse = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            cameraInitRot = transform.localEulerAngles;
            stickMouse = false;
        }

        if (Input.GetMouseButton(0) && moveWorld)
        {
            mousePos = Input.mousePosition;

            Quaternion thisRotation = Quaternion.identity;
            thisRotation.eulerAngles = new Vector3(
            cameraInitRot.x - (mousePos.y - mouseInitPos.y) / 10,
            cameraInitRot.y + (mousePos.x - mouseInitPos.x) / 10,
            cameraInitRot.z
            );

            transform.localEulerAngles = thisRotation.eulerAngles;
        }

        //Strafing control
        if (Input.GetMouseButtonDown(1))
        {
            mouseInitPos = Input.mousePosition;
            stickMouse = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            cameraInitPos = transform.localPosition;
            stickMouse = false;
        }

        if (Input.GetMouseButton(1) && moveWorld)
        {
            mousePos = Input.mousePosition;

            transform.position = cameraInitPos
                - (transform.right * (mousePos.x - mouseInitPos.x) / 8)
                - (transform.up * (mousePos.y - mouseInitPos.y) / 8);
        }

        //Zooming control
        cameraDistance = cameraDistance - (Input.mouseScrollDelta.y * (cameraObj.transform.localPosition.z / 10));
        cameraObj.transform.localPosition = new Vector3(
            cameraObj.transform.localPosition.x,
            cameraObj.transform.localPosition.y,
            cameraDistance
            );
    }
}