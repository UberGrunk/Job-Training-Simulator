using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamEngineController : MonoBehaviour
{
    [SerializeField]
    private float flywheelRPM = 0;
    private float flywheelMaxRPM = 100;
    private float flywheelCurrentRotation = 90;
    private float flywheelNaturalResistance = 5;
    private float flywheelAccelerationPower = 10;

    [SerializeField]
    private float steamPressure = 0;
    private float maxSteamPressure = 100;
    private float maxSteamUsageRate = 0.8f;
    private float maxSteamBuildUpRate = 0.8f;
    [SerializeField]
    private GameObject steamPressureGauge;
    private LeversAndGauges steamPressureGaugeScript;

    [SerializeField]
    private float fuelAmount = 0;
    private float maxFuelAmount = 100;
    private float fuelBurnRate = 0.15f;

    [SerializeField]
    private float waterAmount = 0;
    private float maxWaterAmount = 100;
    private float maxWaterUsageRate = 0.2f;
    private float maxWaterFillRate = 0.4f;
    [SerializeField]
    private GameObject waterInjectorLever;
    private Lever waterInjectorLeverScript;
    [SerializeField]
    private GameObject waterLevelGauge;
    private LeversAndGauges waterLevelGaugeScript;

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

    [SerializeField]
    private GameObject rpmGauge;
    private LeversAndGauges rpmGaugeScript;

    [SerializeField]
    private GameObject steamOutletLever;
    private Lever steamOutletLeverScript;

    private void Awake()
    {
        rpmGaugeScript = rpmGauge.GetComponent<LeversAndGauges>();
        steamOutletLeverScript = steamOutletLever.GetComponent<Lever>();
        waterInjectorLeverScript = waterInjectorLever.GetComponent<Lever>();
        steamPressureGaugeScript = steamPressureGauge.GetComponent<LeversAndGauges>();
        waterLevelGaugeScript = waterLevelGauge.GetComponent<LeversAndGauges>();
    }

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

        steamOutletLeverScript.SetValue(0);
        waterInjectorLeverScript.SetValue(0);
        GlobalSettingsManager.Instance.CaptureMouse = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFlywheelRPM();
        SetRPMGauge();
        SetSteamPressureGauge();
        SetWaterLevelGauge();
        UpdateSteamPressure();
        UpdateWaterAmount();
        UpdateFuelAmount();

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

    private void SetRPMGauge()
    {
        rpmGaugeScript.SetValue(flywheelRPM);
    }

    private void SetSteamPressureGauge()
    {
        steamPressureGaugeScript.SetValue(steamPressure);
    }

    private void SetWaterLevelGauge()
    {
        waterLevelGaugeScript.SetValue(waterAmount);
    }

    private void UpdateFlywheelRPM()
    {
        float targetFlywheelRPM = 0;

        float currentSteamOutletValue = steamOutletLeverScript.GetValue();

        float percentageOfSteamPressure = steamPressure / maxSteamPressure;

        float minValueToRotateFlywheel = steamOutletLeverScript.GetMaxValue() - (steamOutletLeverScript.GetMaxValue() * percentageOfSteamPressure);

        float steamOutletPercentage = (currentSteamOutletValue - minValueToRotateFlywheel) / steamOutletLeverScript.GetMaxValue();

        if (currentSteamOutletValue > minValueToRotateFlywheel)
        {
            
            targetFlywheelRPM = flywheelMaxRPM * steamOutletPercentage;
        }

        if(flywheelRPM > targetFlywheelRPM)
        {
            flywheelRPM -= (flywheelNaturalResistance + (flywheelAccelerationPower * percentageOfSteamPressure * steamOutletPercentage)) * Time.deltaTime;
        }
        else if(flywheelRPM < targetFlywheelRPM)
        {
            flywheelRPM += (flywheelAccelerationPower * percentageOfSteamPressure * steamOutletPercentage) * Time.deltaTime;
        }

        if (flywheelRPM < 0)
            flywheelRPM = 0;
    }

    private void UpdateFuelAmount()
    {
        if (fuelAmount > 0)
        {
            fuelAmount -= fuelBurnRate * Time.deltaTime;

            if(fuelAmount < 0)
                fuelAmount = 0;
        }
    }

    private void UpdateWaterAmount()
    {
        if(waterAmount < maxWaterAmount)
        {
            waterAmount += maxWaterFillRate * (waterInjectorLeverScript.GetValue() / waterInjectorLeverScript.GetMaxValue()) * Time.deltaTime;

            if(waterAmount > maxWaterAmount)
                waterAmount = maxWaterAmount;
        }

        if(waterAmount > 0)
        {
            float percentageOfFuel = fuelAmount / maxFuelAmount;

            waterAmount -= maxWaterUsageRate * percentageOfFuel * Time.deltaTime;

            if(waterAmount < 0)
                waterAmount = 0;
        }
    }

    private void UpdateSteamPressure()
    {
        if(steamPressure < maxSteamPressure)
        {
            float percentageOfFuel = fuelAmount / maxFuelAmount;
            float percentageOfWater = waterAmount / maxWaterAmount;

            steamPressure += maxSteamBuildUpRate * percentageOfFuel * percentageOfWater * Time.deltaTime;

            if(steamPressure > maxSteamPressure)
                steamPressure = maxSteamPressure;
        }

        if(steamPressure > 0)
        {
            float steamOutletPercentage = steamOutletLeverScript.GetValue() / steamOutletLeverScript.GetMaxValue();

            steamPressure -= maxSteamUsageRate * steamOutletPercentage * Time.deltaTime;

            if(steamPressure < 0)
                steamPressure = 0;
        }
    }

    public float AddFuel(float amount)
    {
        if((fuelAmount + amount) <= maxFuelAmount)
        {
            fuelAmount += amount;
        }

        return fuelAmount;
    }
}
