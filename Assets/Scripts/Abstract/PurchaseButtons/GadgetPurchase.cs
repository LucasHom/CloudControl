using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class GadgetPurchase : PurchaseButton
{
    //Ready
    [SerializeField] protected bool isReady = false;
    [SerializeField] protected GameObject notReadyTextPrefab;
    protected int maxGadget = 2;

    //Colors
    private Color readyWhite = Color.white;
    private Color readyRed = new Color(193f / 255f, 64f / 255f, 72f / 255f);

    //openpipes
    public static bool locationSelected = false;
    public static GameObject nextPipe;
    public static bool waitingForLocation = false;
    public static GadgetPurchase recntlyClickedGButton = null;
    public static Action purchaseAction;
    public static bool attemptingPurchase = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void InitializeVisibleFields()
    {
        determineIsReady();
        PriceText = PriceObject.GetComponent<TextMeshProUGUI>();
        PriceText.text = $"${basePrice}";
    }

    public override void OnEnable()
    {
        base.OnEnable();
        determineIsReady();
    }

    public abstract void determineIsReady();

    //Override for gadget purchase
    public override void AttemptPurchase(Action upgradeAction)
    {
        attemptingPurchase = true;
        recntlyClickedGButton = this;
        if (isReady)
        {
            if (ShopManager.currency >= basePrice)
            {
                purchaseAction = upgradeAction;
                waitingForLocation = true;
                StartCoroutine(WaitForClickThenUpgrade());
            }
            else
            {
                notEnough();
            }
        }
        else
        {
            NotReady();
        }

    }

    private IEnumerator WaitForClickThenUpgrade()
    {
        SFXManager.Instance.PlaySFX("select");
        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetMouseButtonDown(0))
            {   
                clicked = true;
            }
            yield return null;
        }

        attemptingPurchase = false;

        yield return new WaitForSecondsRealtime(0.2f);
        if (!attemptingPurchase)
        {
            waitingForLocation = false;
        }

        
    }


    public void FinalPurchase()
    {
        locationSelected = false;

        clickVisuals(basePrice);
        ShopManager.currency -= basePrice;
        ShopManager.updateCurrency();

        purchaseAction();
        determineIsReady();
    }



    public abstract string GetNotReadyFloatText();

    public abstract string GetStatusAmount();

    protected void NotReady()
    {
        GameObject notReadyText = Instantiate(notReadyTextPrefab, transform);
        notReadyText.GetComponent<TextMeshProUGUI>().text = GetNotReadyFloatText();
        instantiatedObjects.Add(notReadyText);
        RectTransform notReadyTransform = maxedUpgradeTextPrefab.GetComponent<RectTransform>();
        notReadyTransform.position = new Vector3(0f, 0f, 0f);

        StartCoroutine(FloatingText.FloatAndDeleteText(notReadyText, 0.8f));
    }

    public void setReadyVisual(bool ready)
    {
        StatusText.text = $"Ready<space=-0.025>:<scale=0.3> </scale>{GetStatusAmount()}";
        if (ready)
        {
            StatusText.fontSize = 0.5f;
            StatusText.color = readyWhite;
            isReady = true;
        }
        else
        {
            StatusText.fontSize = 0.5f;
            StatusText.color = readyRed;
            isReady = false;
        }
    }
}
