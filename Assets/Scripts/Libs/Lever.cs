using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Lever : LeversAndGauges, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool dragging = false;
    private float draggingSensitivity = 0.25f;
    private Vector2 lastMousePosition;
    [SerializeField]
    private LeverOrientation leverOrientation;
    [SerializeField]
    private GameObject mouseOverIndicator;

    private new void Update()
    {
        if(dragging)
        {
            UpdateLeverValue();
        }

        base.Update();
    }

    public float GetValue()
    {
        //Get current rotation around rotationAxle
        float currentRotation = (rotatingObject.transform.localEulerAngles.x * rotationAxle.x) + (rotatingObject.transform.localEulerAngles.y * rotationAxle.y) + (rotatingObject.transform.localEulerAngles.z * rotationAxle.z);
        //float currentRotation = (rotatingObject.transform.eulerAngles.x * rotationAxle.x) + (rotatingObject.transform.eulerAngles.y * rotationAxle.y) + (rotatingObject.transform.eulerAngles.z * rotationAxle.z);

        //Calculate total range of rotation
        float rotationRange = (maxRotation <= minRotation ? maxRotation + 360 : maxRotation) - minRotation;

        //Calculate current rotation percentage of total rotation range
        float percentageOfRotation = currentRotation - minRotation;
        if (percentageOfRotation < 0)
            percentageOfRotation += 360;
        percentageOfRotation /= rotationRange;

        //Invert percentage if value range should be inverted
        if (!increaseValOnPosRotation)
            percentageOfRotation = 1 - percentageOfRotation;

        //Calculate total range of values
        float valueRange = maxValue - minValue;

        //Calculate current value based on current rotation percentage
        float currentValue = minValue + (valueRange * percentageOfRotation);

        //Check and adjust to value boundaries
        if (currentValue < minValue)
            currentValue = minValue;
        else if (currentValue > maxValue)
            currentValue = maxValue;

        return currentValue;
    }

    //TODO: Click and drag interaction that changes the rotation of the rotatingObject
    public void OnPointerDown(PointerEventData eventData)
    {
        GlobalSettingsManager.Instance.CaptureMouse = false;
        dragging = true;
        lastMousePosition = Mouse.current.position.ReadValue();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GlobalSettingsManager.Instance.CaptureMouse = true;
        dragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(mouseOverIndicator != null)
            mouseOverIndicator.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouseOverIndicator != null)
            mouseOverIndicator.SetActive(false);
    }

    private float UpdateLeverValue()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        float currentLeverValue = GetValue();
        float distanceMoved = 0;

        switch(leverOrientation)
        {
            case LeverOrientation.positiveUp:
                distanceMoved = currentMousePosition.y - lastMousePosition.y;
                break;
            case LeverOrientation.positiveDown:
                distanceMoved = lastMousePosition.y - currentMousePosition.y;
                break;
            case LeverOrientation.positiveRight:
                distanceMoved = currentMousePosition.x - lastMousePosition.x;
                break;
            case LeverOrientation.positiveLeft:
                distanceMoved = lastMousePosition.x - currentMousePosition.x;
                break;
        }

        currentLeverValue += distanceMoved * draggingSensitivity;

        if(currentLeverValue > maxValue)
            currentLeverValue = maxValue;
        else if(currentLeverValue < minValue)
            currentLeverValue = minValue;

        lastMousePosition = currentMousePosition;
        
        return SetValue(currentLeverValue);
    }
}

public enum LeverOrientation
{
    positiveUp,
    positiveDown,
    positiveRight,
    positiveLeft
}
