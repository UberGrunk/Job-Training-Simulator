using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : LeversAndGauges
{
    public int GetValue()
    {
        //Get current rotation around rotationAxle
        float currentRotation = (rotatingObject.transform.localEulerAngles.x * rotationAxle.x) + (rotatingObject.transform.localEulerAngles.y * rotationAxle.y) + (rotatingObject.transform.localEulerAngles.z * rotationAxle.z);

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
        int valueRange = maxValue - minValue;

        //Calculate current value based on current rotation percentage
        int currentValue = minValue + (int)((valueRange * percentageOfRotation) + 0.5f);

        //Check and adjust to value boundaries
        if (currentValue < minValue)
            currentValue = minValue;
        else if (currentValue > maxValue)
            currentValue = maxValue;

        return currentValue;
    }

    //TODO: Click and drag interaction that changes the rotation of the rotatingObject
}
