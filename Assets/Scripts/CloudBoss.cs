using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBoss : MonoBehaviour
{
    //Entrance
    [SerializeField] private float entranceHeight = 2.5f;

    //Sludge spawning
    [SerializeField] private GameObject largeBallPrefab;
    [SerializeField] private GameObject mediumBallPrefab;
    [SerializeField] private GameObject smallBallPrefab;
    [SerializeField] private GameObject largeBallGuardedPrefab;
    [SerializeField] private GameObject mediumBallGuardedPrefab;
    [SerializeField] private GameObject smallBallGuardedPrefab;
    //[SerializeField] private GameObject supportBallPrefab;

    [SerializeField] private GameObject sewageExplosion;

    private GameObject[] normalSludge;
    private GameObject[] guardedSludge;
    private GameObject[] allSludge;

    //Weak spot spawning
    [SerializeField] private GameObject weakSpotPrefab;
    [SerializeField] private GameObject weakSpawnPrefab;


    //Misc
    [SerializeField] private PigeonManager pigeonManager;
    [SerializeField] private CitizenManager citizenManager;
    [SerializeField] private GameObject endScreen;

    [SerializeField] private GameObject waterPellet;

    // Start is called before the first frame update
    void Start()
    {
        normalSludge = new GameObject[] { largeBallPrefab, mediumBallPrefab, smallBallPrefab };
        guardedSludge = new GameObject[] { largeBallGuardedPrefab, mediumBallGuardedPrefab, smallBallGuardedPrefab };
        allSludge = new GameObject[] { largeBallPrefab, mediumBallPrefab, smallBallPrefab,
                                      largeBallGuardedPrefab, mediumBallGuardedPrefab, smallBallGuardedPrefab };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DeathByGlamour()
    {
        
        yield return StartCoroutine(DRAMATICEntrance());

        yield return StartCoroutine(PhaseOne());
        for (int i = 0; i < 3; i++)
        {
            citizenManager.GiveThanks();
            yield return new WaitForSeconds(0.8f);
        }

        yield return StartCoroutine(TakeDamage()); //break

        yield return StartCoroutine(PhaseTwo());
        for (int i = 0; i < 3; i++)
        {
            citizenManager.GiveThanks();
            yield return new WaitForSeconds(0.8f);
        }

        yield return StartCoroutine(TakeDamage()); //break

        yield return StartCoroutine(PhaseThree());

        //turn off shop toggle

        yield return StartCoroutine(CloudDie());

        //Endgame screen
        endScreen.GetComponent<EndScreen>().didWin = true;
        endScreen.SetActive(true);
    }

    private IEnumerator DRAMATICEntrance()
    {
        StartCoroutine(Move(transform.position, new Vector3(transform.position.x, entranceHeight, transform.position.z), 2f));
        for (int i = 0; i < 8; i++)
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("normal", -0.8f, randomPosition);
            yield return new WaitForSeconds(0.12f);
        }
        yield return new WaitUntil(() => transform.position.y == entranceHeight);

    }

    private IEnumerator PhaseOne()
    {
        yield return StartCoroutine(SpawnWeakSpots(5));
        yield return new WaitForSeconds(10f);

        //Create weak spots then have this spawning until weak spots gone
        while (CloudWeakSpot.weakSpotsActive > 0) //while weak spots exist
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("normal", 5f, randomPosition);
            yield return new WaitForSeconds(3f);
        }

        Debug.Log("Phase1 over!");
    }

    private IEnumerator PhaseTwo()
    {
        yield return StartCoroutine(Move(transform.position, new Vector3(transform.position.x, 4.5f, transform.position.z), 1.5f));


        //Could do this:   but while is better for polling runtime for 1 sec instead of 1 frame
        //yield return new WaitWhile(() => CloudWeakSpawner.weakSpawnersActive > 0);
        yield return StartCoroutine(SpawnWeakSpawners(1));

        while (CloudWeakSpawner.weakSpawnersActive > 0) //while weak spawners exist
        {
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(SpawnWeakSpawners(2));

        while (CloudWeakSpawner.weakSpawnersActive > 0) //while weak spawners exist
        {
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(SpawnWeakSpawners(2));

        while (CloudWeakSpawner.weakSpawnersActive > 0) //while weak spawners exist
        {
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(SpawnWeakSpawners(3));

        while (CloudWeakSpawner.weakSpawnersActive > 0) //while weak spawners exist
        {
            yield return new WaitForSeconds(1f);
        }

    }

    private IEnumerator PhaseThree()
    {
        //subphase 1
        for (int i = 0; i < 6; i++)
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("normal", 5f, randomPosition);
            yield return new WaitForSeconds(0.12f);
        }
        
        yield return StartCoroutine(Move(transform.position, new Vector3(transform.position.x, 2.5f, transform.position.z), .4f));

        yield return StartCoroutine(SpawnWeakSpots(2));

        yield return new WaitForSeconds(5f);

        yield return StartCoroutine(SpawnWeakSpawners(1));

        while (CloudWeakSpawner.weakSpawnersActive > 0 || CloudWeakSpot.weakSpotsActive > 0) //while weak spawners exist
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("normal", 5f, randomPosition);
            yield return new WaitForSeconds(4f);
        }

        //subphase2

        for (int i = 0; i < 3; i++)
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("guarded", 7f, randomPosition);
            yield return new WaitForSeconds(0.12f);
        }

        yield return StartCoroutine(Move(transform.position, new Vector3(transform.position.x, 4.5f, transform.position.z), 0.4f));


        yield return new WaitForSeconds(3f);

        for (int i = 0; i < 2; i++)
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("guarded", 5f, randomPosition);
            yield return new WaitForSeconds(0.12f);
        }

        yield return StartCoroutine(Move(transform.position, new Vector3(transform.position.x, 2.5f, transform.position.z), .4f));

        yield return StartCoroutine(SpawnWeakSpots(3));

        yield return new WaitForSeconds(7f);

        yield return StartCoroutine(SpawnWeakSpawners(2));

        while (CloudWeakSpawner.weakSpawnersActive > 0 || CloudWeakSpot.weakSpotsActive > 0) //while weak spawners exist
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("normal", 5f, randomPosition);
            yield return new WaitForSeconds(4f);
        }

        //subphase3
        for (int i = 0; i < 8; i++)
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("all", 7f, randomPosition);
            yield return new WaitForSeconds(0.12f);
        }

        yield return StartCoroutine(Move(transform.position, new Vector3(transform.position.x, 4.5f, transform.position.z), 0.4f));

        yield return new WaitForSeconds(5f);

        yield return StartCoroutine(Move(transform.position, new Vector3(transform.position.x, 2.5f, transform.position.z), .4f));

        yield return StartCoroutine(SpawnWeakSpots(10));

        while (CloudWeakSpot.weakSpotsActive > 0) //while weak spots exist
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
            SpawnSludge("normal", 5f, randomPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator TakeDamage()
    {
        StartCoroutine(pigeonManager.SpawnColumbidae());
        for (int i = 0; i < 60; i++)
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-6.5f, 6.5f), transform.position.y + Random.Range(-1.5f, 1), transform.position.z);
            Instantiate(sewageExplosion, randomPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.08f);
        }
        yield return new WaitForSeconds(2f);
    }


    private IEnumerator CloudDie()
    {
        StartCoroutine(FadeOut(GetComponent<SpriteRenderer>(), 5f)); // fade now lasts 8 seconds
        StartCoroutine(Rain());
        for (int i = 0; i < 140; i++)
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-6.5f, 6.5f), transform.position.y + Random.Range(-1.5f, 1), transform.position.z);
            Instantiate(sewageExplosion, randomPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.04f);
        }

        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(citizenManager.ShowEcstatic());

    }

    // Testing some new butt
    /// <summary>
    /// Spawns a random sludge ball from the given list at a specified position.
    /// </summary>
    /// <param name="type">The array of ball prefabs to choose from: normal, guarded, all</param>
    public void SpawnSludge(string type = "all", float spawnUpForce = 0f, Vector3 spawnPosition = default)
    {
        GameObject sludge = null;

        Instantiate(sewageExplosion, spawnPosition, Quaternion.identity);

        if (type == "normal")
        {
            sludge = Instantiate(normalSludge[Random.Range(0, normalSludge.Length)], spawnPosition, Quaternion.identity);
        }
        else if (type == "guarded")
        {
            sludge = Instantiate(guardedSludge[Random.Range(0, guardedSludge.Length)], spawnPosition, Quaternion.identity);
        }
        else if (type == "all")
        {
            sludge = Instantiate(allSludge[Random.Range(0, allSludge.Length)], spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Invalid sludge type specified for SpawnSludge method.");
        }

        if (sludge != null)
        {
            sludge.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-0.4f, 0.4f), spawnUpForce), ForceMode2D.Impulse);
        }
    }

    private IEnumerator Move(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator Rain()
    {
        for (int i = 0; i < 300; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-7.5f, 7.5f), 6f, transform.position.z);
            Instantiate(waterPellet, randomPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.015f);
        }
    }

    private IEnumerator FadeOut(SpriteRenderer sprite, float duration = 5f)
    {
        yield return new WaitForSeconds(1f); // optional delay
        Color originalColor = sprite.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // ease-out: slow at start, fast at end
            float easedT = Mathf.Pow(t, 6f); // change 2f to higher for stronger acceleration

            float alpha = Mathf.Lerp(1f, 0f, easedT);
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // ensure fully transparent at the end
        sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }


    private IEnumerator SpawnWeakSpawners(int amount)
    {
        float minX = -6f;
        float maxX = 6f;

        for (int i = 0; i < amount; i++)
        {
            // Compute evenly spaced x using interpolation
            float interpVal = (amount == 1) ? 0.5f : (float)i / (amount - 1);
            float x = Mathf.Lerp(minX, maxX, interpVal);

            Vector3 randomPosition = new Vector3(
                x,
                transform.position.y - 2f,
                transform.position.z
            );

            GameObject sludge = Instantiate(weakSpawnPrefab, randomPosition, Quaternion.identity);
            sludge.GetComponent<CloudWeakSpawner>().cloudBoss = this;

            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator SpawnWeakSpots(int amount)
    {
        float minX = -5f;
        float maxX = 5f;

        for (int i = 0; i < amount; i++)
        {
            // Compute evenly spaced x using interpolation
            float interpVal = (amount == 1) ? 0.5f : (float)i / (amount - 1);
            float x = Mathf.Lerp(minX, maxX, interpVal);

            Vector3 randomPosition = new Vector3(
                x,
                transform.position.y - 1.2f,
                transform.position.z
            );

            GameObject sludge = Instantiate(weakSpotPrefab, randomPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
