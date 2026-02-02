using UnityEngine;
using TMPro;

public class Tag_Coord : MonoBehaviour
{
    public GameObject pointPlaced;
    Vector3 pointWorldCoor;

    public bool placed = false;
    TextMeshProUGUI txt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        txt = transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(pointPlaced.transform.position.x, pointPlaced.transform.position.y + 40);

        if (!placed)
        {
            pointWorldCoor = pointPlaced.GetComponent<Tag_Point>().pointWorldCoord;
            txt.text = new string(pointWorldCoor.x.ToString("f2") + " / " + pointWorldCoor.z.ToString("f2") + " / " + pointWorldCoor.y.ToString("f2"));
        }
    }
}