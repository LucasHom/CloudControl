using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
    public static int waterHitCount = 0;    

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "Ball")
        {
            waterHitCount++;
            col.GetComponent<Ball>().Split();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "SupportBall")
        {
            waterHitCount++;
            col.GetComponent<SupportBall>().Grow();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "WeakSpot")
        {
            waterHitCount++;
            col.GetComponent<CloudWeakSpot>().HitByWater();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "WeakSpawner")
        {
            waterHitCount++;
            col.GetComponent<CloudWeakSpawner>().HitByWater();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "BallGuard")
        {
            waterHitCount++;
            Transform guardedBall = col.gameObject.transform.parent;
            guardedBall.GetComponent<HitGuard>().ActivateHitGuardPS();
            Destroy(transform.parent.gameObject);
        }
        else if (col.gameObject.tag == "Wall")
        {
            //Special wall particlees?
            Destroy(transform.parent.gameObject);
        }
        else if (col.gameObject.tag == "Medpack")
        {
            waterHitCount++;
            col.GetComponent<Medpack>().Bounce();
            Destroy(transform.parent.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
