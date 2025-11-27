using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointTagger2 : MonoBehaviour
{
    public Vector3 pointWorldCoord;
    public bool placed = false;
    TextMeshProUGUI txt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        txt = transform.GetChild(0).transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pointAtScreen = Camera.main.WorldToScreenPoint(pointWorldCoord);
        transform.position = pointAtScreen;

        if (!placed)
            txt.text = new string("100." + pointWorldCoord.x.ToString("f2") + " / 100." + pointWorldCoord.z.ToString("f2") + " / " + pointWorldCoord.y.ToString("f2"));
    }
}