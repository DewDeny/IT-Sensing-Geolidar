using UnityEngine;

public class _CircleRenderer : MonoBehaviour
{
    public float ThetaScale = 0.01f;
    public float radius = 1;
    private int Size;
    private LineRenderer LineDrawer;
    private float Theta = 0f;

    void Start()
    {
        LineDrawer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        radius = transform.parent.transform.localScale.x/2;

        Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        // LineDrawer.SetVertexCount(Size); (obsolete)
        LineDrawer.positionCount = Size;
        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(Theta);
            float y = radius * Mathf.Sin(Theta);
            Vector3 nodePos = new(x, y, 0);
            LineDrawer.SetPosition(i, transform.position + transform.rotation * nodePos);
        }
    }
}