using UnityEngine;
using UnityEngine.UI;

public class MapLineDrawer : MonoBehaviour
{
    [SerializeField] private Transform nodesParent;
    [SerializeField] private RectTransform linesParent;
    [SerializeField] private float lineThickness = 6f;
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.5f);

    private void Start()
    {
        DrawLines();
    }

    private void DrawLines()
    {
        MapNode[] nodes = nodesParent.GetComponentsInChildren<MapNode>();

        foreach (MapNode node in nodes)
        {
            foreach (MapNode next in node.NextNodes)
            {
                if (next != null)
                {
                    CreateLine(node.transform.position, next.transform.position);
                }
            }
        }
    }

    private void CreateLine(Vector3 worldFrom, Vector3 worldTo)
    {
        Vector2 from = linesParent.InverseTransformPoint(worldFrom);
        Vector2 to = linesParent.InverseTransformPoint(worldTo);

        GameObject lineObject = new GameObject("Line", typeof(RectTransform), typeof(Image));
        lineObject.transform.SetParent(linesParent, false);

        Image image = lineObject.GetComponent<Image>();
        image.color = lineColor;
        image.raycastTarget = false;

        RectTransform rect = lineObject.GetComponent<RectTransform>();
        Vector2 direction = to - from;

        rect.sizeDelta = new Vector2(direction.magnitude, lineThickness);
        rect.localPosition = (from + to) / 2f;
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
}