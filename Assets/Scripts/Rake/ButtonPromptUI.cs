using UnityEngine;

public class ButtonPromptUI : MonoBehaviour 
{
    public enum TargetKey { W, A, S, D, None }

    [Header("W Button")]
    public GameObject wBright;
    public GameObject wDull;

    [Header("A Button")]
    public GameObject aBright;
    public GameObject aDull;

    [Header("S Button")]
    public GameObject sBright;
    public GameObject sDull;

    [Header("D Button")]
    public GameObject dBright;
    public GameObject dDull;

    public void SetTarget(TargetKey key) 
    {
        // 1. Reset everything to dull first
        if (wBright) wBright.SetActive(false); if (wDull) wDull.SetActive(true);
        if (aBright) aBright.SetActive(false); if (aDull) aDull.SetActive(true);
        if (sBright) sBright.SetActive(false); if (sDull) sDull.SetActive(true);
        if (dBright) dBright.SetActive(false); if (dDull) dDull.SetActive(true);

        // 2. Light up only the specific button we need
        switch (key) 
        {
            case TargetKey.W:
                if (wBright) wBright.SetActive(true); if (wDull) wDull.SetActive(false);
                break;
            case TargetKey.A:
                if (aBright) aBright.SetActive(true); if (aDull) aDull.SetActive(false);
                break;
            case TargetKey.S:
                if (sBright) sBright.SetActive(true); if (sDull) sDull.SetActive(false);
                break;
            case TargetKey.D:
                if (dBright) dBright.SetActive(true); if (dDull) dDull.SetActive(false);
                break;
        }
    }
}