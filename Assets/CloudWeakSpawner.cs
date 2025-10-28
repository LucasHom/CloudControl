using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudWeakSpawner : MonoBehaviour
{
    public static int weakSpawnersActive = 0;
    private int health = 10;

    //Destroy
    [SerializeField] private float growAmount = 1.2f;
    [SerializeField] private float growDuration = 0.1f;
    [SerializeField] private float shrinkDuration = 0.2f;

    //Flash
    [SerializeField] private float flashDuration = 0.15f;
    private Coroutine flashCoroutine;
    private Coroutine popCoroutine = null;

    public CloudBoss cloudBoss;

    // Start is called before the first frame update
    void Start()
    {
        weakSpawnersActive++;
        StartCoroutine(ShootSludge());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HitByWater()
    {
        health--;
        if (flashCoroutine == null)
        {
            flashCoroutine = StartCoroutine(FlashWhite());
        }
        if (health <= 0 && popCoroutine == null)
        {
            popCoroutine = StartCoroutine(PopEffect());

        }
    }

    private IEnumerator ShootSludge()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            //move to new location really fast glide there and ease into it
            float prevX = transform.position.x;
            float targetX;
            Vector3 startPos = transform.position;
            do
            {
                targetX = Random.Range(-6f, 6f);
            }
            while (Mathf.Abs(targetX - prevX) < 1.5f);
            Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);

            float glideTime = 0.4f;  // how long the glide lasts
            float elapsed = 0f;

            // smooth glide (ease in/out)
            while (elapsed < glideTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / glideTime;
                // use SmoothStep for a nice easing curve
                t = Mathf.SmoothStep(0, 1, t);
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;  // wait a frame
            }

            // optional short pause after arriving
            yield return new WaitForSeconds(0.1f);


            cloudBoss.SpawnSludge("normal", -4f, transform.position);
            yield return new WaitForSeconds(0.4f);
        }
        yield return new WaitForSeconds(3f);

        StartCoroutine(ShootSludge());
    }

    private IEnumerator FlashWhite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;

        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;

        flashCoroutine = null;
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

        weakSpawnersActive--;
        Destroy(gameObject);
    }
}
