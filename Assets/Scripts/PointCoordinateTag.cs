using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointCoordinateTag : MonoBehaviour
{
    public GameObject pointCurrent;
    public Vector3 pointCoordNumber;
    TextMeshProUGUI txt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        txt = transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 coordPosition = pointCurrent.transform.position;
        Vector2 pointAtScreen = Camera.main.WorldToScreenPoint(coordPosition);
        transform.position = pointAtScreen + (Vector2.up * 50);

        txt.text = new string("100."+pointCoordNumber.x.ToString("f2") + " / 100." + pointCoordNumber.z.ToString("f2") + " / " + pointCoordNumber.y.ToString("f2"));
    }
}