using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Net : MonoBehaviour
{
    private Coroutine slowDownRoutine;
    [SerializeField] private TextMeshProUGUI netDurabilityText;
    [SerializeField] int maxDurbility = 20;
    public int durability;


    //Spawning
    private float minSpawnX = -6.8f;
    private float maxSpawnX = 6.8f;
    private float minSpawnY = -1.5f;
    private float maxSpawnY = 0.0f;
    private Vector2 spawnPosition = default;
    private int attempts = 0;
    private bool positionFound = false;
    private Vector2 boxSize = new Vector2(1.5f, 1.5f);


    //Destroy
    [SerializeField] private float growAmount = 1.2f;
    [SerializeField] private float growDuration = 0.1f;
    [SerializeField] private float shrinkDuration = 0.2f;

    private LayerMask netLayer;




    public static List<Net> activeNets = new List<Net>();

    // Start is called before the first frame update
    void Start()
    {
        netLayer = LayerMask.GetMask("Net");
        PurchaseNet purchaseNet = FindObjectOfType<PurchaseNet>();

        durability = maxDurbility;
        CreateSpawnPosition();
        activeNets.Add(this);

        purchaseNet.determineIsReady();
    }

    // Update is called once per frame
    void Update()
    {
        netDurabilityText.text = $"{durability}";
        if (durability <= 0)
        {
            StartCoroutine(PopDestroySelf());

        }
    }

    private IEnumerator PopDestroySelf()
    {
        Debug.Log("Popping net");
        SFXManager.Instance.PlaySFX("whack");
        yield return PopEffect();
        activeNets.Remove(this);
        Destroy(gameObject);
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

    private void CreateSpawnPosition()
    {
        do
        {
            spawnPosition = new Vector2(Random.Range(minSpawnX, maxSpawnX), Random.Range(minSpawnY, maxSpawnY));

            Collider2D hit = Physics2D.OverlapBox(spawnPosition, boxSize, 0f, netLayer);

            if (hit == null)
            {
                //Debug.Log("position found");
                positionFound = true;
            }
            else
            {
                //Debug.Log("hit other net");
            }
            attempts++;
        }
        while (!positionFound && attempts < 100);

        if (positionFound)
        {
            transform.position = spawnPosition;
        }
        else
        {
            Debug.LogWarning("Couldn't find a valid spawn position after 100 attempts.");
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (spawnPosition != default)
        {
            Gizmos.DrawWireCube(spawnPosition, boxSize);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        GameObject col = collision.gameObject;

        if (col.tag == "SupportBall")
        {
            SupportBall supportBall = col.GetComponent<SupportBall>();
            if (supportBall.isMaxSize == false)
            {
                slowDownRoutine = StartCoroutine(supportBall.StickToNet(gameObject));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject col = collision.gameObject;
        if (col.tag == "SupportBall")
        {
            Rigidbody2D ballrb2d = col.GetComponent<Rigidbody2D>();
            ballrb2d.gravityScale = 1f;
            if (slowDownRoutine != null)
            {
                StopCoroutine(slowDownRoutine);
                slowDownRoutine = null;
            }
        }
    }
}
