using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ViewControl : MonoBehaviour
{
    ViewController moveController;
    Vector2 movement;

    Vector2 mouseInitPos, mousePos;
    Vector3 cameraInitPos, cameraInitRot;
    GameObject cameraObj;
    float cameraDistance;
    public bool moveWorld, stickMouse;
    List<RaycastResult> list;

    void Awake()
    {
        moveController = new ViewController();
        moveController.Player.Move.performed += ctx =>
        {
            movement = ctx.ReadValue<Vector2>();
        };
        moveController.Player.Move.canceled += ctx =>
        {
            movement = Vector2.zero;
        };
    }

    void OnEnable()
    {
        moveController.Enable();
    }

    void OnDisable()
    {
        moveController.Disable();
    }

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
        //WASD CONTROLS
      //  Vector3 movementCam = new Vector3(movement.x,0,movement.y);
      //          transform.localPosition += movementCam*50 * Time.deltaTime;
       // Debug.Log(movement);

        //MOUSE CONTROLS
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

    public void View_Change(int viewAngle)
    {
        switch (viewAngle)
        {
            case 1:
                transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case 2:
                transform.eulerAngles = new Vector3(0, -90, 0);
                break;
            case 3:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case 4:
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            case 5:
                transform.eulerAngles = new Vector3(90, 0, 0);
                break;
        }
    }
}