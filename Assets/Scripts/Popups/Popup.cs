using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] GameObject unitInfo;
    [SerializeField] PopupUnitEffects popupUnitEffects;
    [SerializeField] GameObject continueText;

    [SerializeField] TextMeshProUGUI unitTypeText;
    [SerializeField] TextMeshProUGUI unitDescText;
    [SerializeField] TextMeshProUGUI unitNameText;
    [SerializeField] Image unitImage;
    [SerializeField] Image pipeImage;

    private FadeInUI fadeInUI;

    public static bool IsPopupOpen = false;

    private void Awake()
    {
        fadeInUI = unitInfo.GetComponent<FadeInUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        IsPopupOpen = true;
        Time.timeScale = 0f; // Pause the game
        unitInfo.SetActive(false);
        continueText.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (popupUnitEffects.isIdle)
        {
            unitInfo.SetActive(true);
            fadeInUI.FadeIn();

            if (unitInfo.GetComponent<FadeInUI>().isVisible)
            {
                continueText.SetActive(true);
                // Check if the user presses any key
                if (Input.anyKeyDown)
                {
                    SFXManager.Instance.PlaySFX("select");
                    Time.timeScale = 1f; // Resume the game
                    IsPopupOpen = false;
                    Destroy(gameObject);
                }
            }
        }
    }

    public void SetUnitInfo(string unitType, string unitDesc, string unitName, Sprite unitSprite, float spriteSizeMult, Sprite pipeSprite = null, Color pipeColor = default)
    {
        unitTypeText.text = unitType;
        unitDescText.text = unitDesc;
        unitNameText.text = unitName;

        if (pipeSprite)
        {
            //Enable correct images
            pipeImage.enabled = true;

            //Setup pipe item image
            pipeImage.sprite = pipeSprite;
            unitImage.sprite = unitSprite;
            pipeImage.color = pipeColor;

            unitImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(11.1f, 22f);
            pipeImage.GetComponent<RectTransform>().sizeDelta = new Vector2(40.625f, 48.75f);
            unitImage.GetComponent<RectTransform>().sizeDelta = new Vector2(unitSprite.rect.width * spriteSizeMult, unitSprite.rect.height * spriteSizeMult);
        }
        else
        {
            //Enable correct images
            pipeImage.enabled = false;

            //Setup default image
            unitImage.sprite = unitSprite;
            unitImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 14.92f);
            unitImage.GetComponent<RectTransform>().sizeDelta = new Vector2(unitSprite.rect.width * spriteSizeMult, unitSprite.rect.height * spriteSizeMult);
            
        }  
    }


}