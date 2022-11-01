using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeversAndGauges : MonoBehaviour
{
    [SerializeField]
    private int minValue = 10;
    [SerializeField]
    private int maxValue = 90;
    [SerializeField]
    private float minRotation = 225;
    [SerializeField]
    private float maxRotation = 135;
    [SerializeField]
    private bool increaseValOnPosRotation = true;
    [SerializeField]
    private Vector3 rotationAxle = new Vector3(0, 1, 0);
    [SerializeField]
    private GameObject rotatingObject;

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    public double SetValue(int newValue)
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
