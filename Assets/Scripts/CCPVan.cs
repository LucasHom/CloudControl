using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCPVan : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GenerateWater generateWater;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        generateWater.enabled = false;
        player.SetActive(false);
        transform.position = new Vector3(-15f, -1.69f, 0);

    }

    private IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MoveToTarget(new Vector3(0, -1.69f, 0), 3f));
        yield return new WaitForSeconds(0.7f);
        generateWater.enabled = true;
        player.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        yield return StartCoroutine(MoveToTarget(new Vector3(12.5f, -1.69f, 0), 2f));
        gameObject.SetActive(false);
    }

    private IEnumerator MoveToTarget(Vector3 target, float duration)
    {
        Vector3 start = transform.position; // store starting point once
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Ease in and out
            float easeT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(start, target, easeT);
            yield return null;
        }

        transform.position = target; // ensure it ends exactly
    }




}
