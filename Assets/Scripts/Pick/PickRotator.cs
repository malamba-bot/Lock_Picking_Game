using UnityEngine.InputSystem;
using UnityEngine;

public class PickRotator : MonoBehaviour {

    private float _lastFrameMouseX = 0;

    private InputActionAsset _actionMap;

    private void Start() {
        SubscribeToAction();
    }

    private void RotatePick(InputAction.CallbackContext value) {
        float mouseX = value.ReadValue<float>();
        float offset = mouseX - _lastFrameMouseX;

    }


    private void Update() {
    }

    private void SubscribeToAction() {
        InputActionAsset actionMap = InputSystem.actions;
        InputAction rotatePick = actionMap.FindAction("Rotate");
        rotatePick.performed += RotatePick; 
    }
}

