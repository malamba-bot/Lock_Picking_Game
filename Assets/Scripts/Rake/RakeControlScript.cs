using UnityEngine;
using UnityEngine.InputSystem;

public class LockRake : MonoBehaviour {
    [Header("Setup References")]
    public Transform pickTransform; 

    [Header("Debug")]
    public bool enableDebugLogs = false;
    
    [Header("Sweet Spot Settings")]
    public float targetAngle = 45f; 
    public float greenZoneTolerance = 6f; 


    [Header("Rake Shaking & Movement")]
    public float shakeRequirement = 100f; // total mash points needed to loosen the lock
    public float chargeDecay = 15f; // how fast the mash charge drops down over time
    public float rakeLeftMaxRotation = -60f; // slides/rotates to the left on success
    
    private float currentShakeCharge = 0f;
    private bool lastWasA = false; // tracking variable to make sure they alternate A and D


    private float rakeVisualAngle = 0f;
    private Vector3 startPos;
    public float failShakeAmount = 0.15f;

    private bool hasZoneState = false;
    private bool wasInGreenZone = false;


    void Start()
    {
        startPos = transform.localPosition;
        if (pickTransform == null) {
            DebugLog("Missing pickTransform reference; Update will early-out.");
        }
    }

    void Update()
    {
        if (pickTransform == null){
            return; 
        }

        // Extract and normalize the current pick angle
        float currentPickAngle = pickTransform.eulerAngles.z;

        if (currentPickAngle > 180) {
            currentPickAngle -= 360;
            }

        // Distance check to see if pick is inside the green zone
        float distanceToTarget = Mathf.Abs(currentPickAngle - targetAngle);
        bool inGreenZone = distanceToTarget <= greenZoneTolerance;

        if (!hasZoneState)
        {
            hasZoneState = true;
            wasInGreenZone = inGreenZone;
            DebugLog($"Initial green zone={inGreenZone} angle={currentPickAngle:0.0} target={targetAngle:0.0} tol={greenZoneTolerance:0.0}");
        }
        else if (inGreenZone != wasInGreenZone)
        {
            wasInGreenZone = inGreenZone;
            DebugLog($"Green zone {(inGreenZone ? "entered" : "exited")} angle={currentPickAngle:0.0} target={targetAngle:0.0}");
        }



        //  mash detection logic (A and D keys) 
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            if (!lastWasA)
            {
                currentShakeCharge += 12f; // gained charge
                lastWasA = true;
                PlayMoveOrErrorAudio(inGreenZone);

                DebugLog($"Mash A accepted inGreenZone={inGreenZone} charge={currentShakeCharge:0.0}");
            }
            else
            {
                DebugLog($"Mash A rejected (needs D) inGreenZone={inGreenZone} charge={currentShakeCharge:0.0}");
            }
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            if (lastWasA)
            {
                currentShakeCharge += 12f; 
                lastWasA = false;
                PlayMoveOrErrorAudio(inGreenZone);

                DebugLog($"Mash D accepted inGreenZone={inGreenZone} charge={currentShakeCharge:0.0}");
            }
            else
            {

                DebugLog($"Mash D rejected (needs A) inGreenZone={inGreenZone} charge={currentShakeCharge:0.0}");
            }
        }

        // charge naturally decays over time if you stop mashing
        float previousCharge = currentShakeCharge;

        currentShakeCharge = Mathf.Max(0f, currentShakeCharge - (chargeDecay * Time.deltaTime));

        if (previousCharge > 0f && Mathf.Approximately(currentShakeCharge, 0f))
        {
            DebugLog("Shake charge decayed to 0.");
        }


        // visual logic 
        if (inGreenZone == true) 
        {
            // Reset position back to normal if it was failing/shaking earlier
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * 10f);

            // Calculate progress based on how much you have mashed the lock open
            float mashRatio = Mathf.Clamp01(currentShakeCharge / shakeRequirement);
            float targetLeftRotation = mashRatio * rakeLeftMaxRotation;

            rakeVisualAngle = Mathf.Lerp(rakeVisualAngle, targetLeftRotation, Time.deltaTime * 5f);
            
            transform.localRotation = Quaternion.Euler(0, 0, rakeVisualAngle);


            // VICTORY CHECK: Did we mash it completely over to the left?
            if (mashRatio >= 0.95f) 
            {
                if (AudioManager.Instance != null) {
                    AudioManager.Instance.PlayVictory();
                }
                Debug.Log("Lock picked successfully!");
                enabled = false; // shut down script execution loop
            }
        }
        else 
        {
            // OUTSIDE GREEN ZONE: If player is mashing, visually vibrate the lock wildly in place
            if (currentShakeCharge > 5f) 
            {
                transform.localPosition = startPos + (Vector3)Random.insideUnitCircle * failShakeAmount;
            }
            else 
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * 10f);
            }

            // Rake doesn't move left because we aren't in the sweet spot

            rakeVisualAngle = Mathf.Lerp(rakeVisualAngle, 0f, Time.deltaTime * 5f);
            transform.localRotation = Quaternion.Euler(0, 0, rakeVisualAngle);
        }
    }


    // Helper method to play dynamic feedback sounds depending on location state
    private void PlayMoveOrErrorAudio(bool isSafe)
    {
        if (AudioManager.Instance == null) {
            return;
        }
        if (isSafe) 
        {
            // Plays your alternating sound effects as it works its way open

            AudioManager.Instance.PlayPickMove();
        }
        else 
        {
            // Wrong position! Triggers the lock error sound directly
            AudioManager.Instance.PlayLockError();
        }
    }

    private void DebugLog(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }
        Debug.Log($"[LockRake] {message}", this);
    }
}