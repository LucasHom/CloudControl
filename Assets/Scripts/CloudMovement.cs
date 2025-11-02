using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [SerializeField] public float startingCloudHeight = 10f;
    [SerializeField] private float moveDuration = 1f;

    //Pigeon
    private PigeonManager pigeonManager;
    [SerializeField] private WaveManager waveManager;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0f, startingCloudHeight, 0f);
        pigeonManager = GetComponent<PigeonManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator ChangeCloudHeight(float height)
    {
        SFXManager.Instance.PlaySFX("rise");
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        if (waveManager.currentWave == 10)
        {
            StartCoroutine(pigeonManager.SpawnColumbidae());
        }
        else if (waveManager.currentWave < 10)
        {
            pigeonManager.SpawnPigeons();
        }


    }



    public void RandomizeColliderOffset(float range = 0.3f)
    {
        Collider2D col2d = GetComponent<Collider2D>();

        if (col2d != null)
        {
            Vector2 originalOffset = new Vector2(0.52f, 0f);
            float randomX = Random.Range(-range, range);
            col2d.offset = new Vector2(randomX, originalOffset.y);
        }
    }

    public float getHeight()
    {
        return transform.position.y;
    }
}