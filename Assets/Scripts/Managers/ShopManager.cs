using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    //Currency-Tracking
    [SerializeField] public GameObject currencyIndicator;
    public TextMeshProUGUI currencyCounterText;
    [SerializeField] float currencyTextMaxSize;
    [SerializeField] float currencyTextMinSize;
    public int currency = 0;

    //New improvement notification
    [SerializeField] public GameObject newNotif;
    [SerializeField] public TextMeshProUGUI newNotifText;
    public bool newUnlock = false;
    [SerializeField] private GameObject emptyNotif; //Empty notification when no new unlocks are available
    [SerializeField] private TextMeshProUGUI rewardAmountText;


    //Open-Close shop
    public bool isBackgroundToggleReady = false;
    public bool isShopToggleReady = false;
    private bool isBackgroundActive = false;
    [SerializeField] float shopTransitionDelay = 0.5f;
    [SerializeField] public GameObject shopContent;
    private Coroutine updateCurrencyCoroutine;

    [SerializeField] private CitizenManager citizenManager;

    // Start is called before the first frame update
    void Start()
    {
        currencyIndicator.SetActive(false);
        emptyNotif.SetActive(true);
        newNotif.SetActive(false);
        newNotifText.enabled = false;


        shopContent.SetActive(false);
        isBackgroundActive = false;
        Transform currencyCounter = currencyIndicator.transform.Find("CurrencyCounter");
        currencyCounterText = currencyCounter.GetComponent<TextMeshProUGUI>();
        increaseCurrency(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B) && isBackgroundToggleReady && isShopToggleReady)
        {
            ToggleShop();
            StartCoroutine(DelayShopToggle());
        }
    }

    public void ToggleCurrency()
    {
        currencyIndicator.SetActive(!currencyIndicator.activeSelf);

        newNotif.SetActive(!newNotif.activeSelf);
        if (newUnlock == true)
        {
            newNotifText.enabled = true;
        }
    }

    public void ToggleShop()
    {
        //Turn off new notification under currency indicator
        if (newNotifText.enabled == true)
        {
            emptyNotif.SetActive(false); //only matters on first run because is always false after this
            newUnlock = false;
            newNotifText.enabled = false;
        }


        isBackgroundActive = !isBackgroundActive;
        shopContent.SetActive(!shopContent.activeSelf);
        if (shopContent.activeSelf)
        {
            GadgetPurchase.attemptingPurchase = false;
            GadgetPurchase.waitingForLocation = false;
            resetCurrencyIncrease();
        }

        //update reward amount text
        rewardAmountText.text = $"REWARD: ${citizenManager.calcThanks()}";

        Time.timeScale = isBackgroundActive ? 0f : 1f;
    }

    public void resetCurrencyIncrease()
    {
        StopIncreaseCurrency();
        updateCurrency();
        currencyCounterText.fontSize = currencyTextMinSize;
    }
  

    public void StartIncreaseCurrency(int amount)
    {
        updateCurrencyCoroutine = StartCoroutine(increaseCurrency(amount));
    }

    public void StopIncreaseCurrency()
    {
        if (updateCurrencyCoroutine != null)
        {
            StopCoroutine(updateCurrencyCoroutine);
            updateCurrencyCoroutine = null;
        }
    }

    public void updateCurrency()
    {
        currencyCounterText.text = createCurrencyText(currency);
    }


    //Function used in Citizen Manager every once in a while
    private IEnumerator increaseCurrency(int amount) 
    {
        int startCurrency = currency;
        int targetCurrency = currency + amount;
        currency = targetCurrency;

        //Make text bigger
        float elapsedTime = 0f;
        float lerpDuration = 0.2f;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;

            float lerpedSize = Mathf.Lerp(currencyTextMinSize, currencyTextMaxSize, elapsedTime / lerpDuration);
            currencyCounterText.fontSize = lerpedSize;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        //Gradually increase currency text
        elapsedTime = 0f;
        lerpDuration = 0.2f;


        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            int lerpedCurrency = (int)Mathf.Lerp(startCurrency, targetCurrency, elapsedTime / lerpDuration);
            currencyCounterText.text = createCurrencyText(lerpedCurrency);

            yield return null;
        }

        currencyCounterText.text = createCurrencyText(targetCurrency);


        yield return new WaitForSeconds(0.25f);

        //Make text smaller
        elapsedTime = 0f;
        lerpDuration = 0.1f;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;

            float lerpedSize = Mathf.Lerp(currencyTextMaxSize, currencyTextMinSize, elapsedTime / lerpDuration);
            currencyCounterText.fontSize = lerpedSize;

            yield return null;
        }
    }


    private string createCurrencyText(int amount)
    {
        int clampedAmount = Mathf.Clamp(amount, 0, 999);
        return $"${clampedAmount:000}";
    }

    private IEnumerator DelayShopToggle()
    {
        isBackgroundToggleReady = false;
        float elapsedTime = 0f;
        while (elapsedTime < shopTransitionDelay)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        isBackgroundToggleReady = true;

    }
}
