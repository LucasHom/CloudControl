using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    //tutorial name is either player or gold
    public string tutorialName = "player";

    //Welcome
    [SerializeField] private GameObject welcome;
    [SerializeField] private GameObject welcome_anykey;

    //Part 1: Move, Reload, Shoot
    [SerializeField] private GameObject part1;
    private bool moveda = false;
    private bool movedd = false;
    private bool shot = false;
    private bool reloaded = false;
    [SerializeField] private Image aimage;
    [SerializeField] private Image dimage;
    [SerializeField] private Image m0image;
    [SerializeField] private Image rimage;
    [SerializeField] private Image simage;

    //Part 2: Burst
    [SerializeField] private GameObject part2;
    private bool bursted = false;
    [SerializeField] private Image m1image;

    //Part 3: Goal
    [SerializeField] private GameObject part3;
    [SerializeField] private GameObject anykey;

    //Part 4: Price
    [SerializeField] private GameObject part4;
    [SerializeField] private Image bimage;

    // Animation
    [SerializeField] private Animator buttonAnimator_A;
    [SerializeField] private Animator buttonAnimator_D;
    [SerializeField] private Animator buttonAnimator_R;
    [SerializeField] private Animator buttonAnimator_S;
    [SerializeField] private Animator buttonAnimator_B;
    [SerializeField] private Animator buttonAnimator_mouse0;
    [SerializeField] private Animator buttonAnimator_mouse1;

    private void Awake()
    {
        welcome.SetActive(false);
        part1.SetActive(false);
        part2.SetActive(false);
        part3.SetActive(false);
        part4.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (tutorialName == "player")
        {
            StartCoroutine(confirmPartOneTwoThree());
        }
        else if (tutorialName == "gold")
        {
            StartCoroutine(confirmPartFour());
        }
        else
        {
            Debug.LogError("Invalid tutorial name: " + tutorialName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator confirmPartOneTwoThree()
    {
        //Pause the game
        Time.timeScale = 0f;
        yield return StartCoroutine(confirmWelcome());
        yield return StartCoroutine(confirmBasics());
        yield return StartCoroutine(confirmSpecial());
        yield return StartCoroutine(confirmGoal());
        //Resume
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    //welcome
    private IEnumerator confirmWelcome()
    {
        welcome.SetActive(true);
        welcome_anykey.SetActive(false);
        //delay to not accidentaly skip
        yield return new WaitForSecondsRealtime(1.5f);
        welcome_anykey.SetActive(true);
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        SFXManager.Instance.PlaySFX("select");
        welcome.SetActive(false);
    }

    //move, reload, shoot
    private IEnumerator confirmBasics()
    {
        part1.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        while (!moveda || !movedd || !shot || !reloaded)
        {
            if (Input.GetKeyDown(KeyCode.A) && !moveda)
            {
                SFXManager.Instance.PlaySFX("select");
                moveda = true;
                aimage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                stopButton(buttonAnimator_A);

            }
            if (Input.GetKeyDown(KeyCode.D) && !movedd)
            {
                SFXManager.Instance.PlaySFX("select");
                movedd = true;
                dimage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                stopButton(buttonAnimator_D);
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && !shot)
            {
                SFXManager.Instance.PlaySFX("select");
                shot = true;
                m0image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                stopButton(buttonAnimator_mouse0);
            }
            //if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.S) && !reloaded)
            if (Input.GetKeyDown(KeyCode.S) && !reloaded)
            {
                SFXManager.Instance.PlaySFX("select");
                reloaded = true;
                rimage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                simage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                stopButton(buttonAnimator_S);
                //stopButton(buttonAnimator_R);
            }
            yield return null;
        }
        yield return new WaitForSecondsRealtime(0.8f);
        part1.SetActive(false);
    }

    //burst
    private IEnumerator confirmSpecial()
    {
        part2.SetActive(true);
        while (!bursted)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && !bursted)
            {
                SFXManager.Instance.PlaySFX("select");
                bursted = true;
                m1image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                stopButton(buttonAnimator_mouse1);
            }
            yield return null;
        }
        yield return new WaitForSecondsRealtime(0.8f);
        part2.SetActive(false);
    }

    //goal
    private IEnumerator confirmGoal()
    {
        part3.SetActive(true);
        anykey.SetActive(false);
        //delay to not accidentaly skip
        yield return new WaitForSecondsRealtime(1.5f);
        anykey.SetActive(true);
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        SFXManager.Instance.PlaySFX("select");
        part3.SetActive(false);
    }

    private IEnumerator confirmPartFour()
    {
        //Pause the game
        Time.timeScale = 0f;
        yield return StartCoroutine(confirmPrice());
        //Resume
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    //price and shop
    private IEnumerator confirmPrice()
    {
        part4.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.B))
        {
            yield return null;
        }
        SFXManager.Instance.PlaySFX("select");
        bimage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        stopButton(buttonAnimator_B);
        yield return new WaitForSecondsRealtime(0.8f);
        part4.SetActive(false);
    }

    private void stopButton(Animator buttonAnimator)
    {
        buttonAnimator.Play(0, 0, 0.5f); // Layer 0, time = 0%
        buttonAnimator.Update(0f); // Force immediate update
        buttonAnimator.speed = 0f;
    }
}
