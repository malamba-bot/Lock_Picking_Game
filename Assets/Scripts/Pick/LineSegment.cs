using UnityEngine;

/* The template for this script was AI generated @claude.ai */
public class LineSegment : MonoBehaviour {

    const float PICK_CIRCLE_GAP = 0.3f;

    [SerializeField] private int segments = 16;
    [SerializeField] private GameObject Pick;

    private void Start() { 
        var radius = Pick.GetComponent<Renderer>().bounds.size.x + PICK_CIRCLE_GAP;
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++) {
            float angle = Mathf.PI * i / segments;
            lr.SetPosition(i, new Vector3(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius,
                        0));
        }
    }

}
