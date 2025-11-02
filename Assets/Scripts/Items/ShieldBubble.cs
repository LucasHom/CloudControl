using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShieldBubble : MonoBehaviour
{

    [SerializeField] private float growDuration = 0.25f;
    [SerializeField] private float shrinkDuration = 0.1f;

    [SerializeField] Sprite activeSprite;


    [SerializeField] private Rigidbody2D rb2d;

    private bool isPopping = false;

    private Collider2D col2d;

    public static int numActive = 0;

    // Start is called before the first frame update
    void Start()
    {
        numActive++;
        SFXManager.Instance.PlaySFX("shoot");
        transform.localScale = new Vector2(0.2f, 0.2f);
        col2d = GetComponent<Collider2D>();
        col2d.isTrigger = true;
        rb2d.bodyType = RigidbodyType2D.Dynamic;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //GameObject col = collision.gameObject;

        GameObject col = collision.GetContact(0).collider.gameObject;
        if (col.tag == "BallGuard")
        {
            if (!isPopping)
            {
                isPopping = true;
                StartCoroutine(PopDestroySelf());
            }
        }
        if (col.tag == "Ball")
        {
            col.GetComponent<Ball>().Split();

            if (!isPopping)
            {
                isPopping = true;
                StartCoroutine(PopDestroySelf());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject col = collision.gameObject;
        if (col.tag == "Ground")
        {
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            SpriteRenderer sp = GetComponent<SpriteRenderer>();
            sp.sortingLayerName = "Default";
            sp.sprite = activeSprite;

            //Grow
            SFXManager.Instance.PlaySFX("shield");
            StartCoroutine(GrowEffect());
            col2d.isTrigger = false;
        }
    }

    private IEnumerator PopDestroySelf()
    {
        numActive--;
        yield return PopEffect();
        Destroy(gameObject);
    }

    private IEnumerator GrowEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 shrunkScale = transform.localScale - new Vector3(0.1f, 0.1f);
        Vector3 targetScale = new Vector3(2f, 2f, 1f);

        // Shrink phase
        float elapsedTime = 0f;
        while (elapsedTime < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, shrunkScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.zero;

        // Grow phase
        elapsedTime = 0f;
        while (elapsedTime < growDuration)
        {
            transform.localScale = Vector3.Lerp(shrunkScale, targetScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;


    }


    private IEnumerator PopEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(2.4f, 2.4f, 1f);

        // Grow phase
        float elapsedTime = 0f;
        while (elapsedTime < 0.1f)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        // Shrink phase
        elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.zero;
    }
}
