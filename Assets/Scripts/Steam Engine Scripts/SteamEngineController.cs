using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamEngineController : MonoBehaviour
{
    [SerializeField]private float flywheelRPM = 10;
    private float flywheelCurrentRotation = 90;
    private Vector2 pistonRodMovementRange = new Vector2(1.41f, 1.81f);
    private Vector2 controlRodMovementRange = new Vector2(1.25f, 1.45f);
    private float pistonRodTotalMovementRange;
    private float controlRodTotalMovementRange;
    private bool pistonRodExtending = true;
    private float connectingArmAngle = 0;
    private float secondsPerHalfRotation;

    private GameObject flywheel;
    private GameObject pistonRod;
    private GameObject pistonRodJoint;
    private GameObject largeCrankAxle;
    private GameObject controlRod;
    private GameObject controlRodRotationJoint;

    // Start is called before the first frame update
    void Start()
    {
        flywheel = transform.Find("Flywheel").transform.Find("Flywheel Base").gameObject;
        largeCrankAxle = flywheel.transform.Find("Axle").transform.Find("Large Crank Inner").transform.Find("Large Crank Axle").gameObject;
        pistonRod = transform.Find("Piston Rod").gameObject;
        controlRod = transform.Find("Control Rod").gameObject;
        pistonRodJoint = pistonRod.transform.Find("Piston Rod Joint").gameObject;
        controlRodRotationJoint = controlRod.transform.Find("Rotation Joint").gameObject;
        pistonRodTotalMovementRange = pistonRodMovementRange.y - pistonRodMovementRange.x;
        controlRodTotalMovementRange = controlRodMovementRange.y - controlRodMovementRange.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (flywheelRPM > 0)
        {
            RotateFlywheel();
            CalculateSecondsPerHalfRotation();
            CalculateConnectingArmAngle();
            RepositionPistonRod();
            RepositionControlRod();
        }
    }

    private void RotateFlywheel()
    {
        float rotationDegreesPerSecond = flywheelRPM * 6;

        flywheelCurrentRotation += rotationDegreesPerSecond * Time.deltaTime;

        if(flywheelCurrentRotation > 360)
        {
            flywheelCurrentRotation = flywheelCurrentRotation % 360;
        }

        flywheel.transform.localEulerAngles = new Vector3(flywheelCurrentRotation, 0, 90);
    }

    private void CalculateSecondsPerHalfRotation()
    {
        secondsPerHalfRotation = (1 / (flywheelRPM / 60)) / 2;
    }

    private void CalculateConnectingArmAngle()
    {
        Vector3 crankAxleDirection = largeCrankAxle.transform.position - pistonRodJoint.transform.position;
        connectingArmAngle = Vector3.SignedAngle(pistonRod.transform.forward, crankAxleDirection, pistonRod.transform.right);
    }

    private void RepositionPistonRod()
    {
        float distanceToMove = (pistonRodTotalMovementRange / secondsPerHalfRotation) * Time.deltaTime * 2;

        if(pistonRodExtending)
        {
            if(pistonRod.transform.localPosition.z + distanceToMove >= pistonRodMovementRange.y)
            {
                pistonRod.transform.localPosition = new Vector3(pistonRod.transform.localPosition.x, pistonRod.transform.localPosition.y, pistonRodMovementRange.y);
                pistonRodExtending = false;
            }
            else
            {
                pistonRod.transform.Translate(0, 0, distanceToMove);
            }
        }
        else
        {
            if (pistonRod.transform.localPosition.z - distanceToMove <= pistonRodMovementRange.x)
            {
                pistonRod.transform.localPosition = new Vector3(pistonRod.transform.localPosition.x, pistonRod.transform.localPosition.y, pistonRodMovementRange.x);
                pistonRodExtending = true;
            }
            else
            {
                pistonRod.transform.Translate(0, 0, -distanceToMove);
            }
        }

        pistonRodJoint.transform.localEulerAngles = new Vector3(connectingArmAngle + 90, 0, 0);
    }

    private void RepositionControlRod()
    {
        float distanceToMove = (controlRodTotalMovementRange / secondsPerHalfRotation) * Time.deltaTime * 2;

        if (!pistonRodExtending)
        {
            if (controlRod.transform.localPosition.z + distanceToMove >= controlRodMovementRange.y)
            {
                controlRod.transform.localPosition = new Vector3(controlRod.transform.localPosition.x, controlRod.transform.localPosition.y, controlRodMovementRange.y);
            }
            else
            {
                controlRod.transform.Translate(0, 0, distanceToMove);
            }
        }
        else
        {
            if (controlRod.transform.localPosition.z - distanceToMove <= controlRodMovementRange.x)
            {
                controlRod.transform.localPosition = new Vector3(controlRod.transform.localPosition.x, controlRod.transform.localPosition.y, controlRodMovementRange.x);
            }
            else
            {
                controlRod.transform.Translate(0, 0, -distanceToMove);
            }
        }

        controlRodRotationJoint.transform.localEulerAngles = new Vector3(-connectingArmAngle * 0.4f, 0, 0);
    }
}
