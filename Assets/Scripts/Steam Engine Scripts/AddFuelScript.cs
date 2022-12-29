using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AddFuelScript : MonoBehaviour
{
    [SerializeField]
    private float amountToAdd = 10;
    [SerializeField]
    private GameObject steamEngine;
    private SteamEngineController steamEngineScript;
    [SerializeField]
    private GameObject mouseOverIndicator;
    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Start()
    {
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];

        steamEngineScript = steamEngine.GetComponent<SteamEngineController>();
    }

    private void Update()
    {
        if (!GlobalSettingsManager.Instance.GameOver)
        {
            CheckIsLookedAt();
        }
    }

    private void CheckIsLookedAt()
    {
        if (GlobalSettingsManager.Instance.LookedAtObject != null && GlobalSettingsManager.Instance.LookedAtObject == gameObject)
        {
            if (mouseOverIndicator != null)
                mouseOverIndicator.SetActive(true);

            if(interactAction.WasPressedThisFrame())
            {
                steamEngineScript.AddFuel(amountToAdd);
            }
        }
        else
        {
            if (mouseOverIndicator != null)
                mouseOverIndicator.SetActive(false);
        }
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    steamEngineScript.AddFuel(amountToAdd);
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (mouseOverIndicator != null)
    //        mouseOverIndicator.SetActive(true);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    if (mouseOverIndicator != null)
    //        mouseOverIndicator.SetActive(false);
    //}
}
