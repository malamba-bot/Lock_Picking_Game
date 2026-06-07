using UnityEngine;
using UnityEngine.InputSystem;

public class LockRakeController : MonoBehaviour 
{

    
    [Header("pick reference")]
    public Transform pickTransform; 
    public PickSemiCircleOutline semiCircleRim; 
    public GameObject pick;

    [Header("ui script")]
    public ButtonPromptUI buttonUI; 


    [Header("progression settings")]
    public int segmentsToUnlock = 5; 
    public int pressesPerSegment = 10; 
    public float rakeLeftMaxAngle = -45f; 
    public float smoothSpeed = 6f;

    private int currentPresses = 0;
    private int currentSegmentsUnlocked = 0;
    private float currentVisualAngle = 0f;

    // tracks which WASD key is currently required
    private ButtonPromptUI.TargetKey currentRequiredKey;


    [Header("shaking & click-clack effect")]
    public float errorShakeMultiplier = 0.18f;
    public float clickClackBumpAmount = 8f; 
    public float clickClackHoldTime = 0.15f; // ADDED: how long it freezes on a hit!
    
    private Vector3 initialLocalPos;
    private float shakeTimer = 0f;
    private float clickClackTargetAngle = 0f; // UPDATED
    private float clickClackDelayTimer = 0f; // ADDED: tracks the pause
    
    private bool hasPlayedVictory = false; 
    private PickRotator _pickRotator;


    void Start() {
        initialLocalPos = transform.localPosition;
        PickNextKey(); // picks the very first random starting key
        _pickRotator = pick.GetComponent<PickRotator>();
    }

    // randomly selects the next required key (and updates the UI)
    void PickNextKey() {
        ButtonPromptUI.TargetKey nextKey = currentRequiredKey;
        
        // keep rolling until we get a DIFFERENT key (prevents back-to-back W's)
        while (nextKey == currentRequiredKey) {
            nextKey = (ButtonPromptUI.TargetKey)Random.Range(0, 4); // 0=W, 1=A, 2=S, 3=D
        }
        
        currentRequiredKey = nextKey;
        
        if (buttonUI != null) {
            buttonUI.SetTarget(currentRequiredKey);
        }
    }


    void Update() 
    {
        if (pickTransform == null || semiCircleRim == null) return;

        bool isWinState = currentSegmentsUnlocked >= segmentsToUnlock;

        if (isWinState) 
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * 12f);
            currentVisualAngle = Mathf.Lerp(currentVisualAngle, rakeLeftMaxAngle, Time.deltaTime * smoothSpeed);
            transform.localRotation = Quaternion.Euler(0, 0, currentVisualAngle);

            if (!hasPlayedVictory) {
                hasPlayedVictory = true;
                if (AudioManager.Instance != null) {
                    AudioManager.Instance.PlayVictory();
                }
            }

            if (Mathf.Abs(currentVisualAngle - rakeLeftMaxAngle) < 1f) {
                Debug.Log("lock picked! victory!");
                enabled = false; 
            }
            return; 
        }

        // figure out where the "sweet spot" is
        /*
           float processedPickAngle = pickTransform.eulerAngles.z;
        if (processedPickAngle < 90) processedPickAngle += 360;
        processedPickAngle -= 270; 

        float degreesPerSegment = 90f / semiCircleRim.numSegments;
        float zoneMin = semiCircleRim.extremeAngles.min; 
        float zoneMax = zoneMin + degreesPerSegment; 
        */

        bool insideGreenZone = 
            _pickRotator.angle <= semiCircleRim.extremeAngles.min + 5f;

            //processedPickAngle >= (zoneMin - 1f) && processedPickAngle <= (zoneMax + 1f);


        // check all 4 keys
        bool hitW = Keyboard.current.wKey.wasPressedThisFrame;
        bool hitA = Keyboard.current.aKey.wasPressedThisFrame;
        bool hitS = Keyboard.current.sKey.wasPressedThisFrame;
        bool hitD = Keyboard.current.dKey.wasPressedThisFrame;

        bool anyKeyHit = hitW || hitA || hitS || hitD;

        if (anyKeyHit) 
        {
            if (insideGreenZone) 
            {
                // check if the specific key they hit matches the current required key
                bool hitCorrect = false;
                if (currentRequiredKey == ButtonPromptUI.TargetKey.W && hitW) hitCorrect = true;
                else if (currentRequiredKey == ButtonPromptUI.TargetKey.A && hitA) hitCorrect = true;
                else if (currentRequiredKey == ButtonPromptUI.TargetKey.S && hitS) hitCorrect = true;
                else if (currentRequiredKey == ButtonPromptUI.TargetKey.D && hitD) hitCorrect = true;

                if (hitCorrect) {
                    RegisterGoodPress();
                    PickNextKey(); // grab a new random key for them to press!
                } else {
                    TriggerError(); // they pressed the wrong key in the sequence!
                }
            }
            else 
            {
                TriggerError(); // outside the green zone
            }
        }


        // ADDED: the new mechanical pause logic!
        if (shakeTimer > 0f) {
            shakeTimer -= Time.deltaTime;
            float randomJitter = Random.Range(-errorShakeMultiplier * 50f, errorShakeMultiplier * 50f);
            transform.localRotation = Quaternion.Euler(0, 0, randomJitter);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * 15f);
        } else {
            if (clickClackDelayTimer > 0f) {
                // freezes the rake in the bumped position
                clickClackDelayTimer -= Time.deltaTime;
                transform.localRotation = Quaternion.Euler(0, 0, clickClackTargetAngle);
            } else {
                // drops back to normal smoothly once the pause is over
                clickClackTargetAngle = Mathf.Lerp(clickClackTargetAngle, 0f, Time.deltaTime * 15f);
                transform.localRotation = Quaternion.Euler(0, 0, clickClackTargetAngle);
            }
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * 15f);
        }
    }


    private void RegisterGoodPress()
    {
        currentPresses++;
        Debug.Log($"good press! {currentPresses} / {pressesPerSegment}");

        // randomly jolts the rake up or down on a good press so it looks chaotic!
        if (Random.value > 0.5f) {
            clickClackTargetAngle = clickClackBumpAmount; 
        } else {
            clickClackTargetAngle = -clickClackBumpAmount; 
        }

        // ADDED: start the freeze-frame timer
        clickClackDelayTimer = clickClackHoldTime;
        
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayPickMove();
        }

        if (currentPresses >= pressesPerSegment)
        {
            currentPresses = 0;
            currentSegmentsUnlocked++;
            
            semiCircleRim.AddActiveSegment();
            
            Debug.Log($"segment unlocked! total: {currentSegmentsUnlocked} / {segmentsToUnlock}");

            if (currentSegmentsUnlocked >= segmentsToUnlock) {
                if (buttonUI != null) buttonUI.SetTarget(ButtonPromptUI.TargetKey.None); // hides all bright buttons on win
            }
        }
    }

    private void TriggerError() 
    {
        shakeTimer = 0.3f;
        _pickRotator.integrity -= 1;
        Debug.Log($"integrity left: {_pickRotator.integrity}");
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayLockError();
        }
    }
}
