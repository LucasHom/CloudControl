using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUnitEffects : MonoBehaviour
{
    [SerializeField] float shakeDuration = 3.5f;  // Shake duration
    [SerializeField] float shakeMagnitude = 10f;  // Shake distance
    [SerializeField] float growAmount = 1.2f;   // How much the UI will grow
    [SerializeField] float growDuration = 0.5f; // Duration to grow
    [SerializeField] float shrinkDuration = 0.5f; // Duration to shrink back
    [SerializeField] float rotationAmount = -5f;  // Rotation in degrees

    [SerializeField] float idleGrowAmount = 1.1f;   // How much the UI will grow in idle
    [SerializeField] float idleGrowDuration = 0.8f; // Duration to grow
    [SerializeField] float idleShrinkDuration = 0.8f; // Duration to shrink back

    private RectTransform rectTransform;
    private Vector3 originalScale;

    private Vector2 originalPos;
    private bool isShaking = false;
    public bool isIdle = false; // Flag to check if the object is idle

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        StartPopShakeAnimation();
    }

    public void StartPopShakeAnimation()
    {
        if (!isShaking)
        {
            originalPos = rectTransform.anchoredPosition;
            StartCoroutine(PopShakeRoutine());
        }
    }

    private IEnumerator PopShakeRoutine()
    {
        isShaking = true;
        SFXManager.Instance.PlaySFX("drumroll");

        // Step 1: Shake
        yield return StartCoroutine(Shake());

        yield return new WaitForSecondsRealtime(1f); // Optional delay before pop-out

        // Step 2: Pop-Out (Scale + Rotate)
        yield return StartCoroutine(PopEffect());


        isShaking = false;
        isIdle = true;

        StartCoroutine(IdlePopEffect()); // Start the idle pop effect after the main animation
    }

    // Shake animation
    private IEnumerator Shake()
    {
        float elapsed = 0f;
        Vector2 originalPos = rectTransform.anchoredPosition;

        // This will control how often the shake moves (smaller shake increments)
        float shakeFrequency = 0.03f; // Increase this value for more frequent shakes

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            rectTransform.anchoredPosition = originalPos + new Vector2(offsetX, offsetY);

            // Update elapsed time, accounting for each shake iteration
            elapsed += shakeFrequency;

            // Wait for the next shake increment (this ensures consistent duration)
            yield return new WaitForSecondsRealtime(shakeFrequency);


        }

        // Reset to the original position after shaking ends
        rectTransform.anchoredPosition = originalPos;
    }

    private IEnumerator PopEffect()
    {
        Vector3 targetScale = originalScale * growAmount; // The target scale for the grow effect
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAmount); // Target rotation for the effect

        // Grow phase (with EaseIn)
        float elapsedTime = 0f;
        while (elapsedTime < growDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            // Apply EaseIn during the growing phase
            float t = elapsedTime / growDuration;
            float easedT = EaseIn(t);

            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, easedT);
            rectTransform.rotation = Quaternion.Lerp(Quaternion.identity, targetRotation, easedT); // Apply rotation

            yield return null;
        }
        rectTransform.localScale = targetScale; // Ensure it reaches the target scale
        rectTransform.rotation = targetRotation; // Ensure the target rotation is reached

        SFXManager.Instance.PlaySFX("reveal");
        yield return new WaitForSecondsRealtime(0.21f);

        // Shrink phase (with EaseOut)
        elapsedTime = 0f;
        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            // Apply EaseOut during the shrinking phase
            float t = elapsedTime / shrinkDuration;
            float easedT = EaseOut(t);

            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, easedT);
            rectTransform.rotation = Quaternion.Lerp(targetRotation, Quaternion.identity, easedT); // Reverse rotation

            yield return null;
        }
        rectTransform.localScale = originalScale; // Reset to original scale
        rectTransform.rotation = Quaternion.identity; // Reset to original rotation
    }

    private IEnumerator IdlePopEffect()
    {
        // Target scale for the grow effect
        Vector3 targetScale = originalScale * idleGrowAmount;

        // Continuous loop for the idle pop effect
        while (true)
        {
            // Grow phase (with EaseIn)
            float elapsedTime = 0f;
            while (elapsedTime < idleGrowDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                // Apply EaseIn during the growing phase
                float t = elapsedTime / idleGrowDuration;
                float easedT = EaseIn(t);

                rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, easedT);

                yield return null;
            }
            rectTransform.localScale = targetScale; // Ensure it reaches the target scale

            // Optional delay before shrinking back
            yield return new WaitForSecondsRealtime(0.21f);

            // Shrink phase (with EaseOut)
            elapsedTime = 0f;
            while (elapsedTime < idleShrinkDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                // Apply EaseOut during the shrinking phase
                float t = elapsedTime / idleShrinkDuration;
                float easedT = EaseOut(t);

                rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, easedT);

                yield return null;
            }
            rectTransform.localScale = originalScale; // Reset to original scale
        }
    }

    // EaseOut function (slow down at the end)
    private float EaseOut(float time)
    {
        return Mathf.Pow(time - 1f, 3) + 1f;
    }

    // EaseIn function (speed up at the start)
    private float EaseIn(float time)
    {
        return Mathf.Pow(time, 3);
    }


}
