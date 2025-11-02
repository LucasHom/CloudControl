using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Tracking
    public static int timesHit = 0;


    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D bc2d;
    [SerializeField] private ParticleSystem turnDust;
    [SerializeField] private ParticleSystem movePS;

    [SerializeField] public bool playerHealthy = true;
    [SerializeField] public bool playerIsFrozen = false;
    [SerializeField] public bool isReloading = false;
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float flashInterval = 0.2f;
    [SerializeField] private float freezeTime = 1.5f;
    [SerializeField] private Color flashColor = Color.white;

    //Puddles
    [SerializeField] public float puddleBuffer = 2f;
    private float appliedPuddleBuffer = 0f;

    //Update based on real layer
    private int playerLayer = 6;
    private int ballLayer = 9;
    private int ballGuardLayer = 11;

    //Movement
    [SerializeField] public float playerSpeed = 6f;
    [SerializeField] public float accelerationRate = 8f;
    private float currentHorizontalInput;
    private float horizontalMaxInput = 1f;
    private float movement = 0f;
    [SerializeField] public bool isFacingRight = true;

    //Shooting
    //[SerializeField] private float upZoneAngleStart = -60f;
    //[SerializeField] private float upZoneAngleEnd = 60f;

    void Start()
    {
        //originalColor = spriteRenderer.color;
    }

    public void applyPuddleBuffer(bool isSlowed)
    {
        if (isSlowed)
        {
            appliedPuddleBuffer = puddleBuffer;
        }
        else
        {
            appliedPuddleBuffer = 0f;
        }
    }

    public void getPlayerMovement()
    {
        if (!playerHealthy)
        {
            spriteRenderer.color = flashColor;
            movement = 0f;
            currentHorizontalInput = 0f;

            if (!isInvincible)
            {
                StartCoroutine(InvincibilityPeriod());
                StartCoroutine(FlashDuringInvincibility());
            }
            
        }
        else if (playerHealthy && !isReloading && !playerIsFrozen)
        {

            spriteRenderer.color = new Color(255f, 255f, 255f);

            if (Input.GetKey(KeyCode.A))
            {
                currentHorizontalInput = Mathf.MoveTowards(currentHorizontalInput, -horizontalMaxInput, accelerationRate * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                currentHorizontalInput = Mathf.MoveTowards(currentHorizontalInput, horizontalMaxInput, accelerationRate * Time.deltaTime);
            }
            else
            {
                currentHorizontalInput = Mathf.MoveTowards(currentHorizontalInput, 0, accelerationRate * Time.deltaTime);
            }
            movement = currentHorizontalInput * (playerSpeed - appliedPuddleBuffer);

        }
        else
        {
            //means player is healthy and player is reloading
            spriteRenderer.color = new Color(255f, 255f, 255f);
            movement = 0f;
            currentHorizontalInput = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //Gather input
        if (movement > 0 && !isFacingRight)
        {
            flipPlayer();
        }
        else if (movement < 0 && isFacingRight)
        {
            flipPlayer();
        }
        getPlayerMovement();

        rb2d.velocity = new Vector2(movement, 0f);

        if (rb2d.velocity.sqrMagnitude > 0f && movePS.isPlaying == false)
        {
            movePS.Play();
        }
        else if (rb2d.velocity.sqrMagnitude == 0f && movePS.isPlaying == true)
        {
            movePS.Stop();
        }
    }

    public void startFreezePlayerCoroutine()
    {
        StartCoroutine(freezePlayer());
    }
    public IEnumerator freezePlayer()
    {
        SFXManager.Instance.PlaySFX("playerHit");
        timesHit++;
        if (playerIsFrozen) yield break;

        //Stop movement and shooting
        playerIsFrozen = true;
        invincibilityDuration = freezeTime + 1.2f;
        playerHealthy = false;
        yield return new WaitForSeconds(freezeTime);

        //Start movement and shooting
        playerHealthy = true;
        playerIsFrozen = false;

    }
    private IEnumerator FlashDuringInvincibility()
    {
        float elapsedTime = 0f;

        while (elapsedTime < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            //toggleTransparency();
            yield return new WaitForSeconds(flashInterval);   
            elapsedTime += flashInterval;
        }

        // Ensure player is visible at the end
        spriteRenderer.enabled = true;
    }

    private IEnumerator InvincibilityPeriod()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, ballLayer, true);
        Physics2D.IgnoreLayerCollision(playerLayer, ballGuardLayer, true);
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        Physics2D.IgnoreLayerCollision(playerLayer, ballLayer, false);
        Physics2D.IgnoreLayerCollision(playerLayer, ballGuardLayer, false);
        isInvincible = false;
    }

    private void flipPlayer()
    {
        createTurnDust();
        isFacingRight = !isFacingRight;
        //Uncomment if I want to actually flip the player, unfortunately also flips reloadCanvas
        //Vector3 scale = transform.localScale;
        //scale.x *= -1;
        //transform.localScale = scale;
    }

    private void createTurnDust()
    {
        turnDust.Play();
    }

}
