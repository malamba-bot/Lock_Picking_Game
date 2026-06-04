using UnityEngine;

/* The template for this script was AI generated @claude.ai */
public class PickSemiCircleOutline : MonoBehaviour {

    const float PICK_CIRCLE_GAP = 0.3f;
    private const float _TOTAL_DEGREES = 180; // Semi circle
    private float _degreesPerSegment;

    [SerializeField] private float radius;
    [SerializeField] public int numSegments = 8;
    [SerializeField] private int segmentResolution = 16;
    [SerializeField] private GameObject Pick;

    private LineSegment[] _lineSegments;

    public class LineSegment { 
        public float startAngle;
        public float endAngle;
        public Color color;
        public LineRenderer lineRenderer;
    }

    private void Start() { 
        radius = Pick.GetComponent<Renderer>().bounds.size.x + PICK_CIRCLE_GAP;
        _lineSegments = new LineSegment[numSegments];
        _degreesPerSegment = _TOTAL_DEGREES / numSegments; 
        for (int i = 0; i < numSegments; i++) {
            var seg = new LineSegment();
            _lineSegments[i] = seg; 
            seg.lineRenderer = new GameObject("LineSegment").AddComponent<LineRenderer>();
            seg.startAngle = i * _degreesPerSegment;
            seg.endAngle = (i + 1) * _degreesPerSegment;
            seg.lineRenderer.transform.parent = transform;
            if (i != 1) {
            seg.lineRenderer.material = Resources.Load<Material>("Materials/Pick Semi Circle");
            } else
            seg.lineRenderer.material = Resources.Load<Material>("Materials/Valid Segment");
            seg.lineRenderer.widthMultiplier = 0.2f;
            DrawSegment(seg);
        }
    }

        public void DrawSegment(LineSegment segment) {
            float degreesPerResolutionStep = _degreesPerSegment / segmentResolution;
            segment.lineRenderer.positionCount = segmentResolution + 1;
            for (int i = 0; i < segment.lineRenderer.positionCount; i++) {
                float angle = (segment.startAngle + i * degreesPerResolutionStep) * Mathf.Deg2Rad;
                segment.lineRenderer.SetPosition(i, new Vector3(
                            Mathf.Cos(angle) * radius,
                            Mathf.Sin(angle) * radius,
                            0));
            }
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
