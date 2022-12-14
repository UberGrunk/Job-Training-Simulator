using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddFuelScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float amountToAdd = 10;
    [SerializeField]
    private GameObject steamEngine;
    private SteamEngineController steamEngineScript;
    [SerializeField]
    private GameObject mouseOverIndicator;

    private void Start()
    {
        steamEngineScript = steamEngine.GetComponent<SteamEngineController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        steamEngineScript.AddFuel(amountToAdd);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mouseOverIndicator != null)
            mouseOverIndicator.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouseOverIndicator != null)
            mouseOverIndicator.SetActive(false);
    }
}
