using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Script : MonoBehaviour
{

    public Image outlineImg, leftArrowImg, upArrowImg, rightArrowImg, pauseImg;
    private string currentOutline;

    float startHoldTime = 0f;
    float timeTapToChange = 0.2f;
    float timeHoldToActivate = 1.0f;
    float timeBetweenTapAndHold = 0.8f; // Set to (timeHoldToActivate - timeTapToChange)
    float timeHoldToCancel = 4f;
    bool cancelling = false;

    int numberOfRapidPresses = 0;


    void Start()
    {
        outlineImg.transform.position = upArrowImg.transform.position;
        currentOutline = "UpArrow";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startHoldTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (startHoldTime + timeTapToChange >= Time.time)
            {
                ChangeSelection();
            }
            else if (startHoldTime + timeHoldToActivate <= Time.time)
            {
                ActivateSelection();
            }
            else
            {
                cancelling = true;
            }
        }
        if (Input.GetKey(KeyCode.Space) && !cancelling)
        {
            if (startHoldTime + timeHoldToCancel <= Time.time)
            {
                cancelling = true;
            }
            else if (startHoldTime + timeTapToChange < Time.time)
            {
                FillBoxOfSelection();
            }
        }
        else
        {
            DecreaseAllFillBoxes();
        }
    }

    private void ChangeSelection()
    {
        switch (currentOutline)
        {
            case "LeftArrow":
                outlineImg.transform.position = upArrowImg.transform.position;
                currentOutline = "UpArrow";
                break;
            case "UpArrow":
                outlineImg.transform.position = rightArrowImg.transform.position;
                currentOutline = "RightArrow";
                break;
            case "RightArrow":
                outlineImg.transform.position = leftArrowImg.transform.position;
                currentOutline = "LeftArrow";
                break;
        }
    }

    private void ActivateSelection()
    {
        switch (currentOutline)
        {
            case "LeftArrow":
                Debug.Log("Moved Left.");
                break;
            case "UpArrow":
                Debug.Log("Jumped.");
                break;
            case "RightArrow":
                Debug.Log("Moved Right.");
                break;
        }
    }

    private void FillBoxOfSelection()
    {
        switch (currentOutline)
        {
            case "LeftArrow":
                leftArrowImg.fillAmount += 1.0f/timeBetweenTapAndHold * Time.deltaTime;
                break;
            case "UpArrow":
                upArrowImg.fillAmount += 1.0f/timeBetweenTapAndHold * Time.deltaTime;
                break;
            case "RightArrow":
                rightArrowImg.fillAmount += 1.0f/timeBetweenTapAndHold * Time.deltaTime;
                break;
        }
    }

    private void DecreaseAllFillBoxes()
    {
        leftArrowImg.fillAmount -= 0.05f;
        upArrowImg.fillAmount -= 0.05f;
        rightArrowImg.fillAmount -= 0.05f;
        if (leftArrowImg.fillAmount == 0 && upArrowImg.fillAmount == 0 && rightArrowImg.fillAmount == 0)
        {
            cancelling = false;
        }
    }
}
