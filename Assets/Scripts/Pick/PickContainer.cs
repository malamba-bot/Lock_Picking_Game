using UnityEngine;

public class PickContainer : MonoBehaviour {

    [SerializeField] public GameObject pick;
    [SerializeField] public GameObject brokenPick;

    private PickRotator _pickRotator;

    private void Start() {
        brokenPick.SetActive(false);
        _pickRotator = pick.GetComponent<PickRotator>();
    }

    public float GetAngle() {
        return _pickRotator.angle;
    }

    public float GetIntegrity() {
        return _pickRotator.integrity;
    }

    public void Damage() {
        _pickRotator.integrity -= 1;
        if (_pickRotator.integrity == 0) {
            BreakPick();
        }
    }

    public void BreakPick() {
        pick.SetActive(false);
        brokenPick.SetActive(true);
        
        // triger game over
        if (GameMenuManager.Instance != null) GameMenuManager.Instance.ShowGameOver();

    }
}
