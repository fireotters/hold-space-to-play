using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUi : MonoBehaviour
{
    internal string currentOutline;
    internal float startHoldTime;
    internal const float TimeTapToChange = 0.2f;
    internal bool cancelling;
    internal bool beingHeld;
    public GameObject fadeBlack;

    protected abstract void ChangeSelection();
    protected abstract void ActivateSelection();
    protected abstract void KeepSelectionBoxFilled();
    protected abstract void FillBoxOfSelection();
    protected abstract void DecreaseAllFillBoxes();
    
    public IEnumerator FadeBlack(string toOrFrom, float delay = 0f)
    {
        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 1.2f, fadingAlpha;
        fadeBlack.SetActive(true);
        yield return new WaitForSeconds(delay);
        if (toOrFrom == "from")
        {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f)
            {
                fadingAlpha -= speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
            fadeBlack.SetActive(false);
        }
        else if (toOrFrom == "to")
        {
            fadingAlpha = 0f;
            while (fadingAlpha < 1f)
            {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
    }
}