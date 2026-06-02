using UnityEngine;

public class PickRotator : MonoBehaviour {

    private HingeJoint _hingeJoint;
    private JointMotor _jointMotor;

    private void Start() {
        _hingeJoint = GetComponent<HingeJoint>();
        _jointMotor = _hingeJoint.motor;
    }

    private void Update() {
        Debug.Log(_hingeJoint.angle);
    }
}
