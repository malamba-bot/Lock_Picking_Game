using UnityEngine.InputSystem;
using UnityEngine;

public class PickRotator : MonoBehaviour {

    [SerializeField] int MAX_ANGLE = 0;
    [SerializeField] int MIN_ANGLE = 0;
    private HingeJoint _hingeJoint;
    private JointMotor _jointMotor;

    private float _lastFrameMouseX = 0;

    private InputActionAsset _actionMap;

    private void Start() {
        _hingeJoint = GetComponent<HingeJoint>();
        _jointMotor = _hingeJoint.motor;

        SubscribeToAction();
        SetMotorAttributes();
    }

    private void RotatePick(InputAction.CallbackContext value) {
        float mouseX = value.ReadValue<float>();
        float offset = mouseX - _lastFrameMouseX;

        var hingeLimits = _hingeJoint.limits;
        hingeLimits.max = hingeLimits.min = (hingeLimits.max + offset) % 360;
        _hingeJoint.limits = hingeLimits;
    }


    private void Update() {
    }

    private void SetMotorAttributes() {
        _hingeJoint.useMotor = true;
        _hingeJoint.useLimits = true;
        _hingeJoint.extendedLimits = true;

        var hingeLimits = _hingeJoint.limits;
        hingeLimits.max = MAX_ANGLE;
        hingeLimits.min = MIN_ANGLE;
        _hingeJoint.limits = hingeLimits;
    }

    private void SubscribeToAction() {
        InputActionAsset actionMap = InputSystem.actions;
        InputAction rotatePick = actionMap.FindAction("Rotate");
        rotatePick.performed += RotatePick; 
    }
}

