using UnityEngine;
using System.Collections.Generic;

/* The template for this script was AI generated @claude.ai */
public class PickSemiCircleOutline : MonoBehaviour {

    const float PICK_CIRCLE_GAP = 0.3f;
    private float _totalRange = 90; 
    private float _degreesPerSegment;

    [SerializeField] private float radius;
    [SerializeField] public int numSegments = 16;
    [SerializeField] private int segmentResolution = 8;
    [SerializeField] private GameObject Pick;


    private LineSegment[] _lineSegments;
    private List<LineSegment> _activeSegments;

    private Material _inactiveMaterial;
    private Material _activeMaterial;

    public class LineSegment { 
        public float startAngle;
        public float endAngle;
        public Color color;
        public LineRenderer lineRenderer;
    }

    public void AddActiveSegment() {
        int segmentNum = _activeSegments.Count;
        LineSegment seg = _lineSegments[segmentNum];
        _activeSegments.Add(seg);
        DrawSegment(seg, _activeMaterial);
    }

    private void Start() { 
        _inactiveMaterial = Resources.Load<Material>("Materials/Pick Semi Circle");
        _activeMaterial = Resources.Load<Material>("Materials/Valid Segment");
        radius = Pick.GetComponent<Renderer>().bounds.size.x + PICK_CIRCLE_GAP;
        DrawSemiCircle();
    }

    private void DrawSemiCircle() {
        _activeSegments = new();
        _lineSegments = new LineSegment[numSegments];
        _degreesPerSegment = _totalRange / numSegments; 
        for (int i = 0; i < numSegments; i++) {
            var seg = new LineSegment();
            _lineSegments[i] = seg; 
            seg.lineRenderer = new GameObject("LineSegment").AddComponent<LineRenderer>();
            seg.startAngle = i * _degreesPerSegment;
            seg.endAngle = (i + 1) * _degreesPerSegment;
            seg.lineRenderer.transform.parent = transform;
            seg.lineRenderer.widthMultiplier = 0.2f;
            DrawSegment(seg, _inactiveMaterial);
        }
        AddActiveSegment();
        AddActiveSegment();
    }

    public void DrawSegment(LineSegment segment, Material material) {
        float degreesPerResolutionStep = _degreesPerSegment / segmentResolution;
        segment.lineRenderer.positionCount = segmentResolution + 1;
        segment.lineRenderer.material = material;
        for (int i = 0; i < segment.lineRenderer.positionCount; i++) {
            float angle = (segment.startAngle + i * degreesPerResolutionStep) * Mathf.Deg2Rad;
            segment.lineRenderer.SetPosition(i, new Vector3(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius,
                        0));
        }
    }
}
