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
    private AudioSource flywheelSound;
    [SerializeField]
    private AudioSource steamChuntSound;
    [SerializeField]
    private AudioClip steamChuntClip;
    [SerializeField]
    private AudioClip steamChuntSlowClip;
    [SerializeField]
    private AudioSource steamHissSound;

    [SerializeField]
    private float steamPressure = 0;
    private float maxSteamPressure = 100;
    private float maxSteamUsageRate = 0.8f;
    private float maxSteamBuildUpRate = 0.8f;
    private float steamOutletPercentage = 0;
    [SerializeField]
    private GameObject steamPressureGauge;
    private LeversAndGauges steamPressureGaugeScript;

    [SerializeField]
    private float fuelAmount = 0;
    private float maxFuelAmount = 100;
    private float fuelBurnRate = 0.15f;
    [SerializeField]
    private List<GameObject> fuelGameObjects;
    [SerializeField]
    private Light fuelSpotlight;
    [SerializeField]
    private AudioSource fireSound;
    [SerializeField]
    private List<AudioClip> fireClips;
    private AudioClip currentClip;
    [SerializeField]
    private ParticleSystem smoke;
    [SerializeField]
    private ParticleSystem sparks;

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
    private Vector2 pistonRodCrankConnectionMovementRange = new Vector2(3.48f, 4.28f);
    private Vector2 controlRodCrankConnectionMovementRange = new Vector2(3.68f, 4.08f);
    private float pistonRodTotalMovementRange;
    private float controlRodTotalMovementRange;
    private float pistonRodCrankConnectionTotalMovementRange;
    private float controlRodCrankConnectionTotalMovementRange;
    private float connectingArmAngle = 0;

    private GameObject flywheel;
    private GameObject pistonRod;
    private GameObject pistonRodJoint;
    private GameObject largeCrankAxle;
    private GameObject controlRod;
    private GameObject controlRodRotationJoint;

    [SerializeField]
    private GameObject pistonRodCrankConnection;
    [SerializeField]
    private GameObject controlRodCrankConnection;

    [SerializeField]
    private GameObject rpmGauge;
    private LeversAndGauges rpmGaugeScript;

    [SerializeField]
    private GameObject steamOutletLever;
    private Lever steamOutletLeverScript;

    private float lastUpdateTime = 0;
    private float updateInterval = 0.1f;
    private float currentDeltaTime = 0;

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

        pistonRodCrankConnectionTotalMovementRange = pistonRodCrankConnectionMovementRange.y - pistonRodCrankConnectionMovementRange.x;
        controlRodCrankConnectionTotalMovementRange = controlRodCrankConnectionMovementRange.y - controlRodCrankConnectionMovementRange.x;

        steamOutletLeverScript.SetValue(0);
        waterInjectorLeverScript.SetValue(0);
        GlobalSettingsManager.Instance.GameOver = false;
        GlobalSettingsManager.Instance.AllTasksDone = false;

        flywheelSound.volume = 0;
        flywheelSound.Play();

        steamChuntSound.volume = 0;
        steamChuntSound.Play();

        steamHissSound.volume = 0;
        steamHissSound.Play();

        fireSound.clip = fireClips[0];
        fireSound.volume = 0;
        fireSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalSettingsManager.Instance.GameOver)
        {
            if (Time.realtimeSinceStartup > (lastUpdateTime + updateInterval))
            {
                currentDeltaTime = Time.realtimeSinceStartup - lastUpdateTime;

                UpdateFlywheelRPM();
                SetRPMGauge();
                SetSteamPressureGauge();
                SetWaterLevelGauge();
                UpdateSteamPressure();
                UpdateWaterAmount();
                UpdateFuelAmount();

                lastUpdateTime = Time.realtimeSinceStartup;
            }

            if (flywheelRPM > 0)
            {
                RotateFlywheel();
                CalculateConnectingArmAngle();
                RepositionPistonRod();
                RepositionControlRod();
            }
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

    private void CalculateConnectingArmAngle()
    {
        Vector3 crankAxleDirection = largeCrankAxle.transform.position - pistonRodJoint.transform.position;
        connectingArmAngle = Vector3.SignedAngle(pistonRod.transform.forward, crankAxleDirection, pistonRod.transform.right);
    }

    private void RepositionPistonRod()
    {
        float connectionCurrentPosition = pistonRodCrankConnection.transform.position.z;

        float percentageOfFullExtension = (connectionCurrentPosition - pistonRodCrankConnectionMovementRange.x) / pistonRodCrankConnectionTotalMovementRange;

        pistonRod.transform.localPosition = new Vector3(pistonRod.transform.localPosition.x, pistonRod.transform.localPosition.y, pistonRodMovementRange.x + (pistonRodTotalMovementRange * percentageOfFullExtension));

        pistonRodJoint.transform.localEulerAngles = new Vector3(connectingArmAngle + 90, 0, 0);
    }

    private void RepositionControlRod()
    {
        float connectionCurrentPosition = controlRodCrankConnection.transform.position.z;

        float percentageOfFullExtension = (connectionCurrentPosition - controlRodCrankConnectionMovementRange.x) / controlRodCrankConnectionTotalMovementRange;

        controlRod.transform.localPosition = new Vector3(controlRod.transform.localPosition.x, controlRod.transform.localPosition.y, controlRodMovementRange.x + (controlRodTotalMovementRange * percentageOfFullExtension));

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

        if (percentageOfSteamPressure > 0.1 && currentSteamOutletValue > minValueToRotateFlywheel)
        {
            targetFlywheelRPM = flywheelMaxRPM * steamOutletPercentage;
        }

        if(flywheelRPM > targetFlywheelRPM)
        {
            flywheelRPM -= (flywheelNaturalResistance + (flywheelAccelerationPower * percentageOfSteamPressure * steamOutletPercentage)) * currentDeltaTime;

            if(flywheelRPM < targetFlywheelRPM)
            {
                flywheelRPM = targetFlywheelRPM;
            }
        }
        else if(flywheelRPM < targetFlywheelRPM)
        {
            flywheelRPM += (flywheelAccelerationPower * percentageOfSteamPressure * steamOutletPercentage) * currentDeltaTime;

            if (flywheelRPM > targetFlywheelRPM)
            {
                flywheelRPM = targetFlywheelRPM;
            }
        }

        if (flywheelRPM < 0)
            flywheelRPM = 0;

        UpdateFlywheelSound();
        UpdateSteamChuntSound();
    }

    private void UpdateFuelAmount()
    {
        if (fuelAmount > 0)
        {
            fuelAmount -= fuelBurnRate * currentDeltaTime;

            if (fuelAmount < 0)
                fuelAmount = 0;
        }

        UpdateFuelGameObjects();
        UpdateFireSound();
        UpdateFireParticleSystems();
    }

    private void UpdateWaterAmount()
    {
        if(waterAmount < maxWaterAmount)
        {
            waterAmount += maxWaterFillRate * (waterInjectorLeverScript.GetValue() / waterInjectorLeverScript.GetMaxValue()) * currentDeltaTime;

            if (waterAmount > maxWaterAmount)
                waterAmount = maxWaterAmount;
        }

        if(waterAmount > 0)
        {
            float percentageOfFuel = fuelAmount / maxFuelAmount;

            waterAmount -= maxWaterUsageRate * percentageOfFuel * currentDeltaTime;

            if (waterAmount < 0)
                waterAmount = 0;
        }
    }

    private void UpdateSteamPressure()
    {
        if(steamPressure < maxSteamPressure)
        {
            float percentageOfFuel = fuelAmount / maxFuelAmount;

            float optimalWaterLevel = maxWaterAmount / 2;
            float waterLevelMultiplier;
            if(waterAmount <= optimalWaterLevel)
            {
                waterLevelMultiplier = waterAmount / optimalWaterLevel;
            }
            else
            {
                waterLevelMultiplier = (optimalWaterLevel - (waterAmount - optimalWaterLevel)) / optimalWaterLevel;
            }

            steamPressure += maxSteamBuildUpRate * percentageOfFuel * waterLevelMultiplier * currentDeltaTime;

            if (steamPressure > maxSteamPressure)
                steamPressure = maxSteamPressure;
        }

        if(steamPressure > 0)
        {
            steamOutletPercentage = steamOutletLeverScript.GetValue() / steamOutletLeverScript.GetMaxValue();

            steamPressure -= maxSteamUsageRate * steamOutletPercentage * currentDeltaTime;

            if (steamPressure < 0)
                steamPressure = 0;
        }

        if (steamPressure > 90)
            GlobalSettingsManager.Instance.GameOver = true;

        UpdateSteamHissSound();
    }

    private void UpdateFuelGameObjects()
    {
        int nrLogsVisible = fuelAmount > 0 ? (int)(fuelAmount / 10) + 1 : 0;

        for(int i = 0; i < fuelGameObjects.Count; i++)
        {
            if (i < nrLogsVisible)
                fuelGameObjects[i].SetActive(true);
            else
                fuelGameObjects[i].SetActive(false);
        }

        fuelSpotlight.intensity = nrLogsVisible;
    }

    private void UpdateFireSound()
    {
        currentClip = fireSound.clip;

        if(fuelAmount > 0 && fuelAmount <= 40)
        {
            fireSound.clip = fireClips[0];
            fireSound.volume = 1;

            if(currentClip != fireClips[0])
                fireSound.Play();
        }
        else if(fuelAmount > 40 && fuelAmount <= 80)
        {
            fireSound.clip = fireClips[1];
            fireSound.volume = 1;

            if (currentClip != fireClips[1])
                fireSound.Play();
        }
        else if(fuelAmount > 80)
        {
            fireSound.clip = fireClips[2];
            fireSound.volume = 1;

            if (currentClip != fireClips[2])
                fireSound.Play();
        }
        else
        {
            fireSound.volume = 0;
        }
    }

    private void UpdateFireParticleSystems()
    {
        if(fuelAmount > 0)
        {
            if(!smoke.isPlaying)
                smoke.Play();

            if(!sparks.isPlaying)
                sparks.Play();
        }
        else
        {
            smoke.Stop();
            sparks.Stop();
        }
    }

    private void UpdateFlywheelSound()
    {
        if (flywheelRPM > 0)
            flywheelSound.volume = 0.5f;
        else
            flywheelSound.volume = 0;
    }

    private void UpdateSteamChuntSound()
    {
        if(flywheelRPM > 20 && flywheelRPM <=40)
        {
            if(steamChuntSound.clip != steamChuntSlowClip)
            {
                steamChuntSound.clip = steamChuntSlowClip;
                steamChuntSound.Play();
            }

            float normalSpeed = 30f / flywheelMaxRPM;
            float currentFlywheelSpeed = flywheelRPM / flywheelMaxRPM;

            float newSpeed = currentFlywheelSpeed / normalSpeed;

            steamChuntSound.pitch = newSpeed;
            steamChuntSound.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / newSpeed);

            steamChuntSound.volume = 1;
        }
        else if (flywheelRPM > 40)
        {
            if(steamChuntSound.clip != steamChuntClip)
            {
                steamChuntSound.clip= steamChuntClip;
                steamChuntSound.Play();
            }

            float normalSpeed = 60f / flywheelMaxRPM;
            float currentFlywheelSpeed = flywheelRPM / flywheelMaxRPM;

            float newSpeed = currentFlywheelSpeed / normalSpeed;

            steamChuntSound.pitch = newSpeed;
            steamChuntSound.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / newSpeed);

            steamChuntSound.volume = 1;
        }
        else
            steamChuntSound.volume = 0;
    }

    private void UpdateSteamHissSound()
    {
        if (steamPressure > 0)
        {
            steamHissSound.volume = (steamPressure / maxSteamPressure) * steamOutletPercentage;
        }
        else
            steamHissSound.volume = 0;
    }

    public float AddFuel(float amount)
    {
        if((fuelAmount + amount) <= maxFuelAmount)
        {
            fuelAmount += amount;
        }

        return fuelAmount;
    }

    public void CheckWaterLevelTaskConditions(PlayerTask playerTask)
    {
        playerTask.TaskDone = waterAmount >= 45;
    }

    public void CheckFuelLevelTaskConditions(PlayerTask playerTask)
    {
        playerTask.TaskDone = fuelAmount > 95;
    }

    public void CheckPressureLevelTaskConditions(PlayerTask playerTask)
    {
        playerTask.TaskDone = steamPressure >= 70;
    }

    // ABSTRACTION
    public void CheckMaintainRPMTaskConditions(PlayerTask playerTask)
    {
        if (flywheelRPM >= 50)
            playerTask.TaskTimeSeconds -= Time.deltaTime;
        else
            playerTask.TaskTimeSeconds = playerTask.InitialTaskTimeSeconds;

        playerTask.TaskDone = playerTask.TaskTimeSeconds <= 0;
    }

    public void CheckSafeStopConditions(PlayerTask playerTask)
    {
        playerTask.TaskDone = steamPressure < 10;
    }
}
