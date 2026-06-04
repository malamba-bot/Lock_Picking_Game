using UnityEngine.InputSystem;
using UnityEngine;

public class PickRotator : MonoBehaviour {

    [SerializeField] private float sens = 0.3f;
    [SerializeField] private float maxAngle = 90;
    [SerializeField] private float minAngle = -90;
    [SerializeField] private PickSemiCircleOutline semiCircleRim;

    private float _lastFrameMouseX;
    private float _currentSegment;

    private void Start() {
        _lastFrameMouseX = Mouse.current.delta.x.ReadValue();
        SubscribeToAction();
    }

    private void RotatePick(InputAction.CallbackContext value) {
        float mouseX = value.ReadValue<float>();
        float offset = (mouseX - _lastFrameMouseX) * sens * -1;

        var currentAngle = transform.eulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;
        //Debug.Log(currentAngle);
        float clampedAngle = Mathf.Clamp(currentAngle + offset, minAngle, maxAngle);
        float degreesPerSegment = (maxAngle - minAngle) / semiCircleRim.numSegments;
        _currentSegment = Mathf.Floor(clampedAngle + 90 / degreesPerSegment); 
        Debug.Log($"angle {clampedAngle + 90}, seg: {_currentSegment}, perSegment {degreesPerSegment}");
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

