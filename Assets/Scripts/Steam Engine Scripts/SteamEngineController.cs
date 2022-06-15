using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamEngineController : MonoBehaviour
{
    [SerializeField]private float flywheelRPM = 10;
    private float flywheelCurrentRotation = 90;
    private Vector2 pistonRodMovementRange = new Vector2(1.41f, 1.81f);
    private float pistonRodTotalMovementRange;
    private bool pistonRodExtending = true;

    private GameObject flywheel;
    private GameObject pistonRod;

    // Start is called before the first frame update
    void Start()
    {
        flywheel = transform.Find("Flywheel").transform.Find("Flywheel Base").gameObject;
        pistonRod = transform.Find("Piston Rod").gameObject;
        pistonRodTotalMovementRange = pistonRodMovementRange.y - pistonRodMovementRange.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (flywheelRPM > 0)
        {
            RotateFlywheel();
            RepositionControlArm();
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

    private void RepositionControlArm()
    {
        float secondsPerHalfRotation = (1 / (flywheelRPM / 60)) / 2;

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
    }
}
