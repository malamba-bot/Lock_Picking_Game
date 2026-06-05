using UnityEngine;
using UnityEngine.InputSystem;

public class LockRakeController : MonoBehaviour 
{
    [Header("pick reference")]
    public Transform pickTransform; // drag "pick_final" here
    public PickSemiCircleOutline semiCircleRim; 

    [Header("ui script")]
    public ButtonPromptUI buttonUI; // drag your UI script here!


    [Header("progression settings")]
    public int segmentsToUnlock = 5; // how many green chunks to win
    public int pressesPerSegment = 10; // how many alternating button presses per chunk
    public float rakeLeftMaxAngle = -45f; 
    public float smoothSpeed = 6f;

    private int currentPresses = 0;
    private int currentSegmentsUnlocked = 0;
    private bool waitingForD = false; 
    private float currentVisualAngle = 0f;


    [Header("shaking effect")]
    public float errorShakeMultiplier = 0.18f;
    private Vector3 initialLocalPos;
    private float shakeTimer = 0f;


    void Start() {
        initialLocalPos = transform.localPosition;
        
        // initialize the buttons so A is lit up first
        if (buttonUI != null) {
            buttonUI.SetWaitingForA();
        }
    }


    void Update() 
    {
        if (pickTransform == null || semiCircleRim == null) return;

        // 1. check if we have already won
        bool isWinState = currentSegmentsUnlocked >= segmentsToUnlock;

        if (isWinState) 
        {
            // slide rake left and win!
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * 12f);
            currentVisualAngle = Mathf.Lerp(currentVisualAngle, rakeLeftMaxAngle, Time.deltaTime * smoothSpeed);
            transform.localRotation = Quaternion.Euler(0, 0, currentVisualAngle);

            if (Mathf.Abs(currentVisualAngle - rakeLeftMaxAngle) < 1f) {
                Debug.Log("lock picked! victory!");
                enabled = false; 
            }
            Debug.Log("lock picked! victory!");
            return; // completely stop doing other math
        }


        // 2. figure out where the "sweet spot" is
        float processedPickAngle = pickTransform.eulerAngles.z;
        if (processedPickAngle < 90) processedPickAngle += 360;
        processedPickAngle -= 270; 

        float degreesPerSegment = 90f / semiCircleRim.numSegments;
        float zoneMin = semiCircleRim.extremeAngles.min; // the bottom edge of the green area
        float zoneMax = zoneMin + degreesPerSegment; // the top edge of just the newest segment

        // the player MUST hover over the newest added green segment at the bottom!
        bool insideGreenZone = processedPickAngle >= (zoneMin - 1f) && processedPickAngle <= (zoneMax + 1f);


        // 3. read the key presses
        bool hitA = Keyboard.current.aKey.wasPressedThisFrame;
        bool hitD = Keyboard.current.dKey.wasPressedThisFrame;

        if (hitA || hitD) 
        {
            if (insideGreenZone) 
            {
                // check if they hit the right alternating key
                if (hitA && !waitingForD) {
                    RegisterGoodPress();
                    waitingForD = true;
                    if (buttonUI != null) buttonUI.SetWaitingForD();
                } 
                else if (hitD && waitingForD) {
                    RegisterGoodPress();
                    waitingForD = false;
                    if (buttonUI != null) buttonUI.SetWaitingForA();
                }
                else {
                    // wrong button pressed! trigger a shake
                    shakeTimer = 0.3f;
                }
            }
            else 
            {
                // mashed while their mouse was outside the newest green tip
                shakeTimer = 0.3f;
            }
        }


        // 4. shaking feedback
        if (shakeTimer > 0f) {
            shakeTimer -= Time.deltaTime;
            float randomJitter = Random.Range(-errorShakeMultiplier * 50f, errorShakeMultiplier * 50f);
            transform.localRotation = Quaternion.Euler(0, 0, randomJitter);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * 15f);
        } else {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * 15f);
        }
    }


    // logic to add points and spawn new green segments
    private void RegisterGoodPress()
    {
        currentPresses++;
        Debug.Log($"good press! {currentPresses} / {pressesPerSegment}");

        if (currentPresses >= pressesPerSegment)
        {
            currentPresses = 0;
            currentSegmentsUnlocked++;
            
            // this tells your old script to draw a new green piece, 
            // which also perfectly un-traps the pick allowing it to move lower!
            semiCircleRim.AddActiveSegment();
            
            Debug.Log($"segment unlocked! total: {currentSegmentsUnlocked} / {segmentsToUnlock}");

            if (currentSegmentsUnlocked >= segmentsToUnlock) {
                if (buttonUI != null) buttonUI.HideBoth();
            }
        }
    }
}