using UnityEngine.InputSystem;
using UnityEngine;

public class PickRotator : MonoBehaviour {

    [SerializeField] private float sens = 0.1f;

    private float _lastFrameMouseX;

    private void Start() {
        _lastFrameMouseX = Mouse.current.delta.x.ReadValue();
        SubscribeToAction();
    }

    private void RotatePick(InputAction.CallbackContext value) {
        float mouseX = value.ReadValue<float>();
        float offset = (mouseX - _lastFrameMouseX) * sens * -1;

        var currentAngle = transform.eulerAngles;
        Debug.Log(offset);
        Vector3 angle = new Vector3(
                0,0,
                offset);
        transform.Rotate(angle);
        Debug.Log($"changed to {angle}");


        _lastFrameMouseX = mouseX;

    }

    private void SubscribeToAction() {
        InputActionAsset actionMap = InputSystem.actions;
        InputAction rotatePick = actionMap.FindAction("Rotate");
        rotatePick.performed += RotatePick; 
    }
}

