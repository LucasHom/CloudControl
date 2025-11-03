using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeon : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] GameObject sewageExplosionPrefab;
    private Rigidbody2D rb2d;
    [SerializeField] Sprite deadSprite;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool alive = true;

    //Explode self
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private float force = 5f;
    [SerializeField] private float spread = 0.2f;
    [SerializeField] private float delayBetweenShots = 0.05f;

    //Destroy
    [SerializeField] private float growAmount = 1.2f;
    [SerializeField] private float growDuration = 0.1f;
    [SerializeField] private float shrinkDuration = 0.2f;

    private CloudMovement cloudMovement;

    //For popup
    public static bool showPopup = false;
    public static bool alreadyShowedPopup = false;

    void Start()
    {
        cloudMovement = FindObjectOfType<CloudMovement>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezePositionY;

        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject col = collision.gameObject;
        if (col.name == "Cloud")
        {
            if (!alreadyShowedPopup)
            {
                alreadyShowedPopup = true;
                showPopup = true;
            }
            Instantiate(sewageExplosionPrefab, transform.position, Quaternion.identity);

            //Change movement
            rb2d.constraints = RigidbodyConstraints2D.None;
            alive = false;
            Collider2D col2d = GetComponent<Collider2D>();
            col2d.isTrigger = false;

            //Change appearance
            animator.enabled = false;
            spriteRenderer.sprite = deadSprite;
            cloudMovement.RandomizeColliderOffset();
            StartCoroutine(ChangeToGameLayer());
            
        }
        if (col.name == "StopBarrierRight")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject col = collision.gameObject;
        if (col.tag == "Ball")
        {
            StartCoroutine(OutWithABang());
        }
    }

    private IEnumerator ChangeToGameLayer()
    {
        yield return new WaitForSeconds(1.5f);
        spriteRenderer.sortingLayerName = "Item";
        yield return null;
    }

    private IEnumerator OutWithABang()
    {
        yield return StartCoroutine(ShootWater());
        yield return StartCoroutine(PopEffect());
        Destroy(gameObject);
    }


    private IEnumerator ShootWater()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject water = Instantiate(waterPrefab, transform.position + new Vector3(0f, 0.15f), Quaternion.identity);

            Rigidbody2D rb = water.GetComponent<Rigidbody2D>();
            Vector2 direction = new Vector2(Random.Range(-spread, spread), 1).normalized;

            rb.AddForce(direction * force, ForceMode2D.Impulse);

            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    private IEnumerator PopEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * growAmount;

        // Grow phase
        float elapsedTime = 0f;
        while (elapsedTime < growDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        // Shrink phase
        elapsedTime = 0f;
        while (elapsedTime < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.zero;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        float fadeDuration = 0.4f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        spriteRenderer.color = Color.white;
    }
}
