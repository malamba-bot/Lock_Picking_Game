using UnityEngine.InputSystem;
using UnityEngine;

public class PickRotator : MonoBehaviour {

    [SerializeField] private float sens = 0.3f;
    [SerializeField] private float maxAngle = 0;
    [SerializeField] private float minAngle = -180;
    [SerializeField] private PickSemiCircleOutline semiCircleRim;

    [HideInInspector] public float totalRange;

    private float _lastFrameMouseX;
    private float _currentSegment;

    private void Start() {
        _lastFrameMouseX = Mouse.current.delta.x.ReadValue();
        totalRange = maxAngle - minAngle;
        SubscribeToAction();
    }

    private void RotatePick(InputAction.CallbackContext value) {
        float mouseX = value.ReadValue<float>();
        float offset = (mouseX - _lastFrameMouseX) * sens * -1;

        var currentAngle = transform.eulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;
        //Debug.Log(currentAngle);
        float clampedAngle = Mathf.Clamp(currentAngle + offset, minAngle, maxAngle);
        offset = clampedAngle - currentAngle;

        Vector3 angle = new Vector3(
                0,0,
                offset);
        transform.Rotate(angle);

        _lastFrameMouseX = mouseX;
    }

    private void SubscribeToAction() {
        InputActionAsset actionMap = InputSystem.actions;
        InputAction rotatePick = actionMap.FindAction("Rotate");
        rotatePick.performed += RotatePick; 
    }
}
