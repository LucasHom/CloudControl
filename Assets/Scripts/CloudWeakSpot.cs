using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudWeakSpot : MonoBehaviour
{
    public static int weakSpotsActive = 0;
    private int health = 10;

    //Destroy
    [SerializeField] private float growAmount = 1.2f;
    [SerializeField] private float growDuration = 0.1f;
    [SerializeField] private float shrinkDuration = 0.2f;

    //Flash
    [SerializeField] private float flashDuration = 0.15f;
    private Coroutine flashCoroutine;
    private Coroutine popCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        weakSpotsActive++;
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
            SFXManager.Instance.PlaySFX("break");
            popCoroutine = StartCoroutine(PopEffect());
            
        }
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

        weakSpotsActive--;
        Destroy(gameObject);
    }
}
