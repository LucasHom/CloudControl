using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    //Spawning
    public int ballsRemaining;
    private float minXSpawn = -7;
    private float maxXSpawn = 7;
    [SerializeField] private GameObject spawnNotificationPrefab;
    private Vector3 randomSpawnPosition;


    //Wave Tracking
    private WaveInfo waveInfo;
    [SerializeField] float timeBetweenWave = 4f;
    [SerializeField] public int currentWave = 0;
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private int currentSubWaveIndex = 0;

    private bool checkSubWaveIsOver = false;
    private bool subWaveIsOver = true;


    //Cloud
    [SerializeField] private float maxCloudHeight = 45f;
    [SerializeField] private float maxWaves = 10;
    private float cloudHeightChange;
    [SerializeField] private CloudMovement cloudMovement;
    [SerializeField] private CloudBoss cloudBoss;

    //Camera
    private CameraManager cameraManager;
    private CinemachineBrain cinemachineBrain;


    //Transition Text
    [SerializeField] TextMeshProUGUI waveNumText;
    [SerializeField] TextMeshProUGUI waveDescriptionText;
    [SerializeField] private GameObject titleCard;

    //Shop
    private ShopManager shopManager;
    [SerializeField] private ButtonSecurityManager buttonSecurityManager;
    [SerializeField] private bool shopLocked;

    //Citizens
    [SerializeField] CitizenManager girlfriend;
    [SerializeField] GameObject citizenHealthIndicator;
    [SerializeField] int maxThanksIncrease = 3;

    //Popups
    Queue<Action> unlockQueue = new Queue<Action>();
    [SerializeField] GameObject popupPrefab;
    //  Popups images
    [SerializeField] Sprite waterTankImage;
    [SerializeField] Sprite netImage;
    [SerializeField] Sprite pigeonImage;
    [SerializeField] Sprite reloadSpeedImage;
    [SerializeField] Sprite umbrellaImage;
    [SerializeField] Sprite swiftShoeImage;
    [SerializeField] Sprite medpackImage;

    //  Popup pipe images
    [SerializeField] Sprite pipeImage;
    [SerializeField] Sprite waterProjImage;
    [SerializeField] Sprite freezeGustImage;
    [SerializeField] Sprite shieldBubbleImage;

    // Tutorial
    private bool tutorialEnabled = true; // Set to false to disable tutorial popups
    [SerializeField] private GameObject tutorialPrefab;
    [SerializeField] private GameObject CCPVan;

    //Testing
    [SerializeField] private bool testBossWave;


    private void Awake()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        cameraManager = GetComponent<CameraManager>();
        shopManager = GetComponent<ShopManager>();
        waveInfo = GetComponent<WaveInfo>();
    }

    void Start()
    {
        unlockQueue.Enqueue(() =>
        {
            createPopup("Upgrade", "Increase water capacity", "Water Tank", waterTankImage, 4f);
            ButtonSecurityManager.Unlock("WaterCapacityButton");
        });
        unlockQueue.Enqueue(() =>
        {
            createPopup("Item", "Catches up to 15 purified sludge", "Splash Net", netImage, 2.5f);
            ButtonSecurityManager.Unlock("NetButton");
        });
        unlockQueue.Enqueue(() =>
        {
            createPopup("Pipe", "Periodically shoots water", "Water pipe", waterProjImage, 3.2f, pipeImage, new Color(6f / 255f, 154f / 255f, 1f));
            ButtonSecurityManager.Unlock("WaterPipeButton");
        });
        unlockQueue.Enqueue(() =>
        {
            createPopup("Upgrade", "Increase reload speed", "Reload speed", reloadSpeedImage, 4f);
            ButtonSecurityManager.Unlock("ReloadSpeedButton");
        });
        unlockQueue.Enqueue(() =>
        {
            createPopup("Item", "Shields Midge from incoming sludge", "Umbrella", umbrellaImage, 3f);
            ButtonSecurityManager.Unlock("UmbrellaButton");
        });
        unlockQueue.Enqueue(() =>
        {
            createPopup("Pipe", "Periodically shoots a cold gust, momentarily freezing contacted sludge", "Freeze pipe", freezeGustImage, 2f, pipeImage, new Color(71f / 255f, 227f / 255f, 1f));
            ButtonSecurityManager.Unlock("FreezePipeButton");
        });
        unlockQueue.Enqueue(() =>
        {
            createPopup("Upgrade", "Increase your speed", "Swift shoe", swiftShoeImage, 4f);
            ButtonSecurityManager.Unlock("PlayerSpeedButton");
        });
        unlockQueue.Enqueue(() =>
        {
            createPopup("Item", "Crack open to heal Midge", "Medpack", medpackImage, 2.5f);
            ButtonSecurityManager.Unlock("MedpackButton");
        });
        unlockQueue.Enqueue(() =>
        {
            createPopup("Pipe", "Generates shield bubbles to deflect sludge", "Shield bubble pipe", shieldBubbleImage, 2f, pipeImage, new Color(102f / 255f, 148f / 255f, 172f / 255f));
            ButtonSecurityManager.Unlock("ShieldBubblePipeButton");
        });

        cloudHeightChange = (maxCloudHeight - cloudMovement.startingCloudHeight) / maxWaves;
        citizenHealthIndicator.SetActive(false);

        randomSpawnPosition = new Vector3(UnityEngine.Random.Range(minXSpawn, maxXSpawn), cloudMovement.transform.position.y, 0f);

        if (testBossWave)
        {
            currentWave = 10;
            currentWaveIndex = 9;
            cloudMovement.startingCloudHeight = maxCloudHeight - cloudHeightChange;
            shopManager.currency = 999;
            shopLocked = false;
        }

        //Lock all pipes
        if (shopLocked)
        {
            buttonSecurityManager.Lock("WaterCapacityButton");
            buttonSecurityManager.Lock("ReloadSpeedButton");
            buttonSecurityManager.Lock("PlayerSpeedButton");

            buttonSecurityManager.Lock("NetButton");
            buttonSecurityManager.Lock("UmbrellaButton");
            buttonSecurityManager.Lock("MedpackButton");

            buttonSecurityManager.Lock("WaterPipeButton");
            buttonSecurityManager.Lock("FreezePipeButton");
            buttonSecurityManager.Lock("ShieldBubblePipeButton");
        }

        StartCoroutine(GameLoop());
    }

    public static void ResetStatics()
    {
        // --- Player & Citizen State ---
        Player.timesHit = 0;
        CitizenManager.rewardsCollected = 0;
        CitizenManager.possibleRewards = 0;
        CitizenManager.timesMidgeHit = 0;

        // --- Combat & Projectile Tracking ---
        Ball.numActiveBalls = 0;
        GenerateWater.waterShotCount = 0;
        WaterCollision.waterHitCount = 0;

        // --- Active Item Counters ---
        Umbrella.activeUmbrellas = new Stack<Umbrella>();
        Umbrella.mostActiveUmbrellasEver = 0;
        Net.activeNets = new List<Net>();
        Medpack.activeMedpacks = 0;

        // --- Pipe Systems ---
        WaterPipe.activeWaterPipes = 0;
        FreezePipe.activeFreezePipes = default;  // assuming int or struct
        ShieldBubblePipe.activeShieldBubblePipes = 0;

        // --- Cloud & Boss Systems ---
        CloudWeakSpawner.weakSpawnersActive = 0;
        CloudWeakSpot.weakSpotsActive = 0;

        // --- Defense / Shields ---
        ShieldBubble.numActive = 0;

        // --- Shop & Gadget UI ---
        GadgetPurchase.locationSelected = false;
        GadgetPurchase.waitingForLocation = false;
        GadgetPurchase.recntlyClickedGButton = null;
        GadgetPurchase.purchaseAction = null;
        GadgetPurchase.nextPipe = null;
        GadgetPurchase.attemptingPurchase = false;
        Popup.IsPopupOpen = false;

        // --- Event / Dialogue Systems ---
        Pigeon.showPopup = false;
        Pigeon.alreadyShowedPopup = false;
    }


    void Update()
    {
        if (checkSubWaveIsOver)
        {
            updateWaveIsOver();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Screen.fullScreen)
                Screen.fullScreen = false;
        }
    }

    private void updateWaveIsOver()
    {
        subWaveIsOver = Ball.numActiveBalls < 1;
    }

    private IEnumerator GameLoop()
    {
        //van appear
        CCPVan.SetActive(true);
        yield return CCPVan.GetComponent<CCPVan>().StartCoroutine("WaitAndMove");

        // move, reload, shoot tutorial
        SFXManager.Instance.PlayLoopingMusic("casualSong");
        if (tutorialEnabled)
        {
            // Set tutorial name to "player" for the first part
            Instantiate(tutorialPrefab, Vector3.zero, Quaternion.identity).GetComponent<Tutorial>().tutorialName = "player";
        }

        cameraManager.SwitchToWaveView();
        currentWave++;
        yield return new WaitUntil(() => !cinemachineBrain.IsBlending);
        yield return new WaitForSeconds(2f);

        //show title card
        yield return new WaitForSeconds(1f);
        SFXManager.Instance.PlaySFX("titleCard");
        titleCard.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        titleCard.SetActive(false);
        yield return new WaitForSeconds(1f);

        //Show wave info
        waveNumText.text = "Wave " + currentWave;
        waveDescriptionText.text = waveInfo.allWaves[currentWaveIndex].description;
        EnableTransitionText();
        yield return new WaitForSeconds(timeBetweenWave);

        while (currentWave <= maxWaves)
        {
            cameraManager.SwitchToGameView();

            yield return new WaitForSeconds(1f);
            DisableTransitionText();

            //Check for pigeon popup
            if (Pigeon.showPopup)
            {
                yield return StartCoroutine(WaitForPigeonPopup());
            }

            //Toggle UI on
            ToggleCitizenHealth();
            shopManager.ToggleCurrency();
            yield return new WaitUntil(() => !cinemachineBrain.IsBlending);

            shopManager.isShopToggleReady = true;
            shopManager.isBackgroundToggleReady = true;


            //Spawn wave
            yield return StartCoroutine(SpawnWave());

            //End wave
            currentWave++;

            //Enable scene views and popups
            ToggleCitizenHealth();
            shopManager.ToggleCurrency();
            shopManager.isShopToggleReady = false;
            shopManager.isBackgroundToggleReady = true;

            //Wave rewards
            if (unlockQueue.Count > 0)
            {
                Action popup = unlockQueue.Dequeue();
                popup();

                shopManager.newUnlock = true;
            }

            girlfriend.maxThanks = (int)(girlfriend.maxThanks + maxThanksIncrease); //increase midges possible rewards

            cameraManager.SwitchToCloudView();
            yield return new WaitUntil(() => !cinemachineBrain.IsBlending);
            yield return new WaitForSeconds(2f);

            if (currentWave <= maxWaves)
            {
                StartCoroutine(cloudMovement.ChangeCloudHeight(cloudHeightChange));

                yield return new WaitForSeconds(2f);
                cameraManager.SwitchToWaveView();
                yield return new WaitUntil(() => !cinemachineBrain.IsBlending);
                yield return new WaitForSeconds(2f);
                waveNumText.text = "Wave " + currentWave;
                waveDescriptionText.text = waveInfo.allWaves[currentWaveIndex].description;
                EnableTransitionText();

                yield return new WaitForSeconds(timeBetweenWave);
            }
        }

        Debug.Log("start");
        SFXManager.Instance.StopLoopingMusic();
        yield return new WaitForSeconds(2f);
        Debug.Log("continue");
        //Wave 11: Bossfight
        StartCoroutine(FlashFlood());
    }

    private IEnumerator WaitForPigeonPopup()
    {
        createPopup("???", "It seems well-hydrated...", "Stupid pigeon", pigeonImage, 5f);
        Popup.IsPopupOpen = true;
        Pigeon.showPopup = false;

        // Wait until the popup is no longer active
        while (Popup.IsPopupOpen)
        {
            yield return null;
        }
    }


    private IEnumerator SpawnWave()
    {
        currentSubWaveIndex = 0;
        Instantiate(spawnNotificationPrefab);
        yield return new WaitForSeconds(1f);

        //Check if the current wave is over
        while (currentSubWaveIndex < waveInfo.allWaves[currentWaveIndex].subWaves.Count)
        {
            subWaveIsOver = false;
            waveInfo.SpawnSubWave(currentWaveIndex, currentSubWaveIndex, randomSpawnPosition);

            // Wait for subwave to end
            checkSubWaveIsOver = true;
            yield return new WaitUntil(() => subWaveIsOver);
            checkSubWaveIsOver = false;

            yield return new WaitForSeconds(1f);
            girlfriend.GiveThanks();
            yield return new WaitForSeconds(2f);

            // gold tutorial
            if (tutorialEnabled)
            {
                shopManager.isShopToggleReady = false;
                // Set tutorial name to "gold" for gold tutorial
                GameObject pricetut = Instantiate(tutorialPrefab, Vector3.zero, Quaternion.identity);
                pricetut.GetComponent<Tutorial>().tutorialName = "gold";

                yield return new WaitUntil(() => pricetut == null);
                tutorialEnabled = false;
                shopManager.ToggleShop();
                shopManager.isShopToggleReady = true;
                yield return new WaitForSeconds(2f);
            }

            currentSubWaveIndex++;

            if (currentSubWaveIndex < waveInfo.allWaves[currentWaveIndex].subWaves.Count)
            {
                Instantiate(spawnNotificationPrefab);
                yield return new WaitForSeconds(1f);
            }
        }

        girlfriend.AttemptHeal();
        yield return new WaitForSeconds(1f);
        currentWaveIndex++;
    }


    private IEnumerator FlashFlood()
    {
        
        StartCoroutine(cloudMovement.ChangeCloudHeight(cloudHeightChange));

        SFXManager.Instance.PlayLoopingMusic("bossentrance");
        //this routine takes 7s
        yield return new WaitForSeconds(2f);
        cameraManager.SwitchToWaveView();
        yield return new WaitUntil(() => !cinemachineBrain.IsBlending);
        yield return new WaitForSeconds(2f);
        waveNumText.text = "Protect midge";
        waveDescriptionText.text = "";
        EnableTransitionText();

        SFXManager.Instance.PlaySFX("mad");
        yield return new WaitForSeconds(timeBetweenWave);
        //end of routine

        cameraManager.SwitchToGameView();
        StartCoroutine(cloudBoss.DeathByGlamour()); //Start bossfight

        //delay to allow for cinemachine to start belending
        yield return new WaitForSeconds(0.1f);

        //show UI
        DisableTransitionText();
        ToggleCitizenHealth();
        shopManager.ToggleCurrency();

        yield return new WaitUntil(() => !cinemachineBrain.IsBlending);

        shopManager.isShopToggleReady = true;
        shopManager.isBackgroundToggleReady = true;

        yield return new WaitForSeconds(1f);

        //Spawn boss wave
        //handle everything in CloudBoss script

        yield return null;
    }


    private void createPopup(string unitType, string unitDesc, string unitName, Sprite unitSprite, float spriteSizeMult, Sprite pipeSprite = null, Color pipeColor = default)
    {
        GameObject popup = Instantiate(popupPrefab, new Vector2(382f, 215f), Quaternion.identity);
        popup.GetComponent<Popup>().SetUnitInfo(unitType, unitDesc, unitName, unitSprite, spriteSizeMult, pipeSprite, pipeColor);
    }


    private void EnableTransitionText()
    {
        SFXManager.Instance.PlaySFX("defend");
        waveNumText.enabled = true;
        waveDescriptionText.enabled = true;
    }

    private void DisableTransitionText()
    {
        waveNumText.enabled = false;
        waveDescriptionText.enabled = false;
    }

    public void ToggleCitizenHealth()
    {
        citizenHealthIndicator.SetActive(!citizenHealthIndicator.activeSelf);
    }
}
