using UnityEngine;

public class ButtonPromptUI : MonoBehaviour 
{
    [Header("a button settings")]
    public GameObject aButtonHighlighted;

    public GameObject aButtonNormal;

    [Header("d button settings")]
    public GameObject dButtonHighlighted;
    public GameObject dButtonNormal;

    public void SetWaitingForA() 
    {
        // turns A bright, turns D dull
        if (aButtonHighlighted) aButtonHighlighted.SetActive(true);
        if (aButtonNormal) aButtonNormal.SetActive(false);

        if (dButtonHighlighted) dButtonHighlighted.SetActive(false);
        if (dButtonNormal) dButtonNormal.SetActive(true);
    }

    public void SetWaitingForD() 
    {
        // turns D bright, turns A dull
        if (aButtonHighlighted) aButtonHighlighted.SetActive(false);
        if (aButtonNormal) aButtonNormal.SetActive(true);

        if (dButtonHighlighted) dButtonHighlighted.SetActive(true);
        if (dButtonNormal) dButtonNormal.SetActive(false);
    }

    public void HideBoth() 
    {
        // turns both dull when you win
        if (aButtonHighlighted) aButtonHighlighted.SetActive(false);
        if (aButtonNormal) aButtonNormal.SetActive(true);

        if (dButtonHighlighted) dButtonHighlighted.SetActive(false);
        if (dButtonNormal) dButtonNormal.SetActive(true);
    }
}