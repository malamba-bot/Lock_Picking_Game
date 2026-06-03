using UnityEngine;

/* The template for this script was AI generated @claude.ai */
public class PickSemiCircleOutline : MonoBehaviour {

    private const float _TOTAL_DEGREES = 180; // Semi circle

    [SerializeField] private int numSegments = 8;
    [SerializeField] private int segmentResolution = 16;
    [SerializeField] private GameObject Pick;

    private LineSegment[] _lineSegments;

    public class LineSegment { 
        public float startAngle;
        public float endAngle;
        public Color color;
        public LineRenderer lr;
    }

    const float PICK_CIRCLE_GAP = 0.3f;


    private void Start() { 
        _lineSegments = new LineSegment[numSegments];
        float degreesPerSegment = _TOTAL_DEGREES / numSegments; 
        for (int i = 0; i < numSegments; i++) {
            var seg = new LineSegment();
            _lineSegments[i] = seg; 
            seg.lr = new GameObject("LineSegment").AddComponent<LineRenderer>();
            seg.startAngle = i * degreesPerSegment;
            seg.endAngle = (i + 1) * degreesPerSegment;
            seg.lr.transform.parent = transform;
            seg.lr.material = Resources.Load<Material>("Pick Semi Circle");
            seg.lr.widthMultiplier = 0.02f;
            DrawSegment(seg);
        }
    }

        public void DrawSegment(LineSegment segment) {
            Debug.Log($"start : {segment.startAngle}, end : {segment.endAngle}");
        }

        /*
        var radius = Pick.GetComponent<Renderer>().bounds.size.x + PICK_CIRCLE_GAP;
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = numSegments + 1;

        for (int i = 0; i <= numSegments; i++) {
            float angle = Mathf.PI * i / numSegments;
            lr.SetPosition(i, new Vector3(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius,
                        0));
        }
        */

}
