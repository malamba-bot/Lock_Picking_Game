using UnityEngine;
using UnityEngine.InputSystem;

public class LockRakeController : MonoBehaviour 
{
    [Header("Pick Reference")]
    public Transform pickTransform; // drag "pick_final" here
    public float targetAngle = 45f; 
    public float greenZoneTolerance = 8f;


    [Header("Mashing & Movement")]
    public float mashPointsNeeded = 100f;
    public float pointLossRate = 20f; 
    public float rakeLeftMaxAngle = -45f; 
    public float smoothSpeed = 6f;

    private float currentMashScore = 0f;
    private bool pressedA = false; 
    private float currentVisualAngle = 0f;



    [Header("Shaking Effect")]
    public float errorShakeMultiplier = 0.18f;
    private Vector3 initialLocalPos;


    [Header("Bezier Curve References")]
    public LineRenderer rakeCurve; // drag your BezierCurve object here
    public int resolution = 40; // smoothness of the curve

    // the 4 points that shape your custom bezier rake tool!
    public Transform p0_Start;
    public Transform p1_Control;
    public Transform p2_Control;
    public Transform p3_End;


    void Start() {
        initialLocalPos = transform.localPosition;

        if (rakeCurve == null) {
            rakeCurve = GetComponentInChildren<LineRenderer>();
        }
    }


    void Update() 
    {
        if (pickTransform == null) return;

        // processing the angle to match pickrotator translation logic
        float processedPickAngle = pickTransform.eulerAngles.z;
        if (processedPickAngle < 90) processedPickAngle += 360;
        processedPickAngle -= 270; 

        float distanceToTarget = Mathf.Abs(processedPickAngle - targetAngle);
        bool insideGreenZone = distanceToTarget <= greenZoneTolerance;



        // alternating mash inputs
        if (Keyboard.current.aKey.wasPressedThisFrame && !pressedA) 
        {
            currentMashScore += 10f;
            pressedA = true;
            TriggerFeedbackSound(insideGreenZone);
            
            // simpler log
            Debug.Log($"Mash A | Score: {currentMashScore:F0} | Inside Zone: {insideGreenZone}");
        } 
        else if (Keyboard.current.dKey.wasPressedThisFrame && pressedA) 
        {
            currentMashScore += 10f;
            pressedA = false;
            TriggerFeedbackSound(insideGreenZone);

            // simpler log
            Debug.Log($"Mash D | Score: {currentMashScore:F0} | Inside Zone: {insideGreenZone}");
        }

        currentMashScore = Mathf.Max(0f, currentMashScore - (pointLossRate * Time.deltaTime));



        if (insideGreenZone == true) 
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * 12f);

            float mashPct = Mathf.Clamp01(currentMashScore / mashPointsNeeded);
            float targetRotation = mashPct * rakeLeftMaxAngle;

            currentVisualAngle = Mathf.Lerp(currentVisualAngle, targetRotation, Time.deltaTime * smoothSpeed);
            transform.localRotation = Quaternion.Euler(0, 0, currentVisualAngle);

            if (mashPct >= 0.96f) {
                Debug.Log("VICTORY!");
                if (AudioManager.Instance != null) {
                    AudioManager.Instance.PlayVictory();
                }
                enabled = false; 
            }
        } 
        else 
        {
            if (currentMashScore > 3f) {
                if (Time.frameCount % 30 == 0) {
                    // simple warning tell you current angle vs target
                    Debug.LogWarning($"Wrong Spot! Pick: {processedPickAngle:F0} (Target: {targetAngle})");
                }
                transform.localPosition = initialLocalPos + (Vector3)Random.insideUnitCircle * errorShakeMultiplier;
            } else {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * 12f);
            }

            currentVisualAngle = Mathf.Lerp(currentVisualAngle, 0f, Time.deltaTime * smoothSpeed);
            transform.localRotation = Quaternion.Euler(0, 0, currentVisualAngle);
        }


        DrawTrueBezierRake(insideGreenZone);
    }



    private void DrawTrueBezierRake(bool isSafe)
    {
        if (rakeCurve == null || p0_Start == null || p1_Control == null || p2_Control == null || p3_End == null) 
            return;

        rakeCurve.positionCount = resolution;

        for (int i = 0; i < resolution; i++) 
        {
            float t = (float)i / (float)(resolution - 1);

            Vector3 curvePoint = CalculateCubicBezier(t, p0_Start.position, p1_Control.position, p2_Control.position, p3_End.position);

            if (isSafe && currentMashScore > 5f)
            {
                float flexFactor = Mathf.Sin(t * Mathf.PI) * (currentMashScore / mashPointsNeeded) * 0.04f;
                curvePoint.y += flexFactor;
            }

            rakeCurve.SetPosition(i, rakeCurve.transform.InverseTransformPoint(curvePoint));
        }
    }


    private Vector3 CalculateCubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0; 
        point += 3 * uu * t * p1; 
        point += 3 * u * tt * p2; 
        point += ttt * p3;        

        return point;
    }


    private void TriggerFeedbackSound(bool isSafe)
    {
        if (AudioManager.Instance == null) return;

        if (isSafe) {
            AudioManager.Instance.PlayPickMove(); 
        } else {
            if (Time.frameCount % 2 == 0) {
                AudioManager.Instance.PlayLockError();
            }
        }
    }
}