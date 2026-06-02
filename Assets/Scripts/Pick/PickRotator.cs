using UnityEngine.InputSystem;
using UnityEngine;

public class PickRotator : MonoBehaviour {

    [SerializeField] int MAX_ANGLE = 270;
    [SerializeField] int MIN_ANGLE = 90;
    private HingeJoint _hingeJoint;
    private JointMotor _jointMotor;

    private InputActionAsset _actionMap;

    private void Start() {
        _hingeJoint = GetComponent<HingeJoint>();
        _jointMotor = _hingeJoint.motor;

        SubscribeToAction();
        SetMotorAttributes();
    }

    private void RotatePick(InputAction.CallbackContext value) {
        //Debug.Log(value);
    }


    private void Update() {
        Debug.Log(_hingeJoint.angle);
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

