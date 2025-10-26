using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonManager : MonoBehaviour
{

    [SerializeField] GameObject pigeonPrefab;
    private CloudMovement cloudMovement; 

    // Start is called before the first frame update
    void Start()
    {
        cloudMovement = FindObjectOfType<CloudMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Debug.Log("Pressed p");
        //    SpawnPigeons();
        //}
    }

    public void SpawnPigeons()
    {
        int countToSpawn = GetWeightedSpawnCount();

        for (int i = 0; i < countToSpawn; i++)
        {
            Instantiate(pigeonPrefab, GetRandomCloudPosition(), Quaternion.identity);
        }
    }

    public IEnumerator SpawnColumbidae()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnPigeons();
            yield return new WaitForSeconds(0.7f);
        }
    }

    private Vector2 GetRandomCloudPosition(float range = 2f)
    {
        float randomX = Random.Range(-range - 0.2f, range);
        float randomY = Random.Range(-range, range);
        return new Vector2(-16f + randomX, cloudMovement.getHeight() - randomY);
    }

    private int GetWeightedSpawnCount()
    {
        float rand = Random.value;

        if (rand < 0.2f) return 0;      // 20% chance
        else if (rand < 0.7f) return 1; // 50% chance
        else if (rand < 0.9f) return 2; //20% chance
        else return 3;                 // 10% chance
    }
}
