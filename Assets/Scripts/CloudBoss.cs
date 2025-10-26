using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBoss : MonoBehaviour
{

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
        yield return StartCoroutine(PhaseTwo());
        yield return StartCoroutine(PhaseThree());
    }

    private IEnumerator DRAMATICEntrance()
    {
        StartCoroutine(FloatDown());
        for (int i = 0; i < 8; i++)
        {
            SpawnSludge("normal", -0.8f);
            yield return new WaitForSeconds(0.12f);
        }
    }

    private IEnumerator PhaseOne()
    {
        yield return new WaitForSeconds(10f);

        //Create weak spots then have this spawning until weak spots gone
        for (int i = 0; i < 20; i++)
        {
            SpawnSludge("normal", 5f);
            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator PhaseTwo()
    {
        yield return null;
    }

    private IEnumerator PhaseThree()
    {
        yield return null;
    }

    // Testing some new butt
    /// <summary>
    /// Spawns a random sludge ball from the given list at a specified position.
    /// </summary>
    /// <param name="type">The array of ball prefabs to choose from.</param>
    private void SpawnSludge(string type = "all", float spawnUpForce = 0f)
    {
        GameObject sludge = null;
        Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-1, 0), transform.position.z);
        Instantiate(sewageExplosion, randomPosition, Quaternion.identity);

        if (type == "normal")
        {
            sludge = Instantiate(normalSludge[Random.Range(0, normalSludge.Length)], randomPosition, Quaternion.identity);
        }
        else if (type == "guarded")
        {
            sludge = Instantiate(guardedSludge[Random.Range(0, guardedSludge.Length)], randomPosition, Quaternion.identity);
        }
        else if (type == "all")
        {
            sludge = Instantiate(allSludge[Random.Range(0, allSludge.Length)], randomPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Invalid sludge type specified for SpawnSludge method.");
        }

        if (sludge != null)
        {
            Debug.Log("Spawned sludge at position: " + randomPosition);
            sludge.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-0.4f, 0.4f), spawnUpForce), ForceMode2D.Impulse);
        }
    }

    private IEnumerator FloatDown()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, 2.5f, transform.position.z);
        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
