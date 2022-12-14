using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeversAndGauges : MonoBehaviour
{
    [SerializeField]
    protected float minValue = 10;
    [SerializeField]
    protected float maxValue = 90;
    [SerializeField]
    protected float minRotation = 225;
    [SerializeField]
    protected float maxRotation = 135;
    [SerializeField]
    protected bool increaseValOnPosRotation = true;
    [SerializeField]
    protected Vector3 rotationAxle = new Vector3(0, 1, 0);
    [SerializeField]
    protected GameObject rotatingObject;

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    // ENCAPSULATION
    public float GetMaxValue()
    {
        return maxValue;
    }

    public float GetMinValue()
    {
        return minValue;
    }

    public virtual float SetValue(float newValue)
    {
        if(newValue < minValue)
            newValue = minValue;
        else if (newValue > maxValue)
            newValue = maxValue;

        float percentageOfValue = (newValue - minValue) / (maxValue - minValue);

        if (!increaseValOnPosRotation)
            percentageOfValue = 1 - percentageOfValue;

        float rotationRange = (maxRotation <= minRotation ? maxRotation + 360 : maxRotation) - minRotation;

        float newRotation = ((rotationRange * percentageOfValue) + minRotation) % 360;

        rotatingObject.transform.localEulerAngles = new Vector3(newRotation * rotationAxle.x, newRotation * rotationAxle.y, newRotation * rotationAxle.z);

        return newRotation;
    }
}
