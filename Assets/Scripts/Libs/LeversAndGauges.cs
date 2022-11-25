using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeversAndGauges : MonoBehaviour
{
    [SerializeField]
    protected int minValue = 10;
    [SerializeField]
    protected int maxValue = 90;
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

    public float SetValue(int newValue)
    {
        if(newValue < minValue)
            newValue = minValue;
        else if (newValue > maxValue)
            newValue = maxValue;

        float percentageOfValue = (float)(newValue - minValue) / (float)(maxValue - minValue);

        if (!increaseValOnPosRotation)
            percentageOfValue = 1 - percentageOfValue;

        float rotationRange = (maxRotation <= minRotation ? maxRotation + 360 : maxRotation) - minRotation;
        float newRotation = ((rotationRange * percentageOfValue) + minRotation) % 360;

        rotatingObject.transform.localEulerAngles = new Vector3(newRotation * rotationAxle.x, newRotation * rotationAxle.y, newRotation * rotationAxle.z);

        return newRotation;
    }
}
