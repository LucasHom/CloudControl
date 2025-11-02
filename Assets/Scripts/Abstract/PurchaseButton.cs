using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PurchaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Classes
    [SerializeField] protected FloatingUpgradeText FloatingText;
    [SerializeField] protected ShopManager ShopManager;

    //Prefabs
    [SerializeField] protected GameObject notEnoughTextPrefab;
    [SerializeField] protected GameObject maxedUpgradeTextPrefab;
    [SerializeField] protected GameObject upgradeCostTextPrefab;

    //Child objects
    [SerializeField] protected GameObject PriceObject;
    [SerializeField] protected TextMeshProUGUI StatusText;
    protected TextMeshProUGUI PriceText;
    [SerializeField] private GameObject newText;


    [SerializeField] protected int basePrice = default;


    protected List<GameObject> instantiatedObjects = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        newText.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //determineIsReady();
    //PriceText = PriceObject.GetComponent<TextMeshProUGUI>();
    //PriceText.text = $"${basePrice}";
    public abstract void InitializeVisibleFields();

    public abstract void AttemptPurchase(Action purchaseAction);

    public virtual void clickVisuals(int price)
    {
        StartCoroutine(ClickEffect());
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f) * 1.05f;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }


    //Get rid of prefabs on pause
    public virtual void OnEnable()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }

        instantiatedObjects.Clear();
    }

    private IEnumerator ClickEffect()
    {
        SFXManager.Instance.PlaySFX("buy");
        newText.SetActive(false);
        transform.localScale = new Vector3(1f, 1f, 1f);
        yield return new WaitForSecondsRealtime(0.06f);
        transform.localScale = new Vector3(1f, 1f, 1f) * 1.05f;
    }


    protected void notEnough()
    {
        GameObject notEnoughText = Instantiate(notEnoughTextPrefab, transform);
        instantiatedObjects.Add(notEnoughText);
        RectTransform notEnoughTransform = maxedUpgradeTextPrefab.GetComponent<RectTransform>();
        notEnoughTransform.position = new Vector3(0f, 0f, 0f);

        StartCoroutine(FloatingText.FloatAndDeleteText(notEnoughText, 0.8f));
    }


    protected void maxedUpgrade()
    {
        GameObject maxedUpgradeText = Instantiate(maxedUpgradeTextPrefab, transform);
        instantiatedObjects.Add(maxedUpgradeText);
        RectTransform maxedUpgradeTransform = maxedUpgradeTextPrefab.GetComponent<RectTransform>();
        maxedUpgradeTransform.position = new Vector3(0f, 0f, 0f);

        StartCoroutine(FloatingText.FloatAndDeleteText(maxedUpgradeText, 0.8f));
    }

    //For rank upgrades
    protected void removeOldPrice(int price)
    {
        ShopManager.currency -= price;
        ShopManager.updateCurrency();

        GameObject oldUpgradeCostText = Instantiate(upgradeCostTextPrefab, transform);
        instantiatedObjects.Add(oldUpgradeCostText);
        RectTransform oldCostRectTransform = oldUpgradeCostText.GetComponent<RectTransform>();
        oldCostRectTransform.position = PriceObject.GetComponent<RectTransform>().position;

        oldUpgradeCostText.GetComponent<TextMeshProUGUI>().text = $"${price}";

        StartCoroutine(FloatingText.FloatAndDeleteText(oldUpgradeCostText, 0.4f));
    }
}
