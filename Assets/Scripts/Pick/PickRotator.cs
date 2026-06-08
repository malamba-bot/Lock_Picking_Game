using UnityEngine.InputSystem;
using UnityEngine;

public class PickRotator : MonoBehaviour {

    [SerializeField] private float sens = 0.3f;
    [SerializeField] private float maxAngle = 0;
    [SerializeField] private float minAngle = -180;
    [SerializeField] private PickSemiCircleOutline semiCircleRim;
    [SerializeField] private GameObject brokenPick;

    [HideInInspector] public float totalRange;
    [HideInInspector] public float angle;
    [HideInInspector] public float integrity = 5;

    private float _lastFrameMouseX;
    private float _currentSegment;

    private void Start() {
        Cursor.visible = false;
        _lastFrameMouseX = Mouse.current.delta.x.ReadValue();
        totalRange = maxAngle - minAngle;
        SubscribeToAction();
    }

    private void RotatePick(InputAction.CallbackContext value) {
        float mouseX = value.ReadValue<float>();
        float offset = (mouseX - _lastFrameMouseX) * sens * -1;

        var currentAngle = transform.eulerAngles.z;
        if (currentAngle < 90) currentAngle += 360;
        currentAngle -= 270;
        //Debug.Log(currentAngle);
        var extremes = semiCircleRim.extremeAngles;
        float clampedAngle = Mathf.Clamp(currentAngle + offset, extremes.min, extremes.max);
        this.angle = clampedAngle;
        offset = clampedAngle - currentAngle;

        Vector3 angle = new Vector3(
                0,0,
                offset);
        transform.Rotate(angle);
        brokenPick.transform.Rotate(angle);

        _lastFrameMouseX = mouseX;
    }

    private void SubscribeToAction() {
        InputActionAsset actionMap = InputSystem.actions;
        InputAction rotatePick = actionMap.FindAction("Rotate");
        rotatePick.performed += RotatePick; 
    }

    private void OnDisable() {
        InputActionAsset actionMap = InputSystem.actions;
        InputAction rotatePick = actionMap.FindAction("Rotate");
        rotatePick.performed -= RotatePick; 
    }

}
