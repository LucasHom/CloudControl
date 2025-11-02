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
            SFXManager.Instance.PlaySFX("waterHit");
            waterHitCount++;
            col.GetComponent<SupportBall>().Grow();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "WeakSpot")
        {
            SFXManager.Instance.PlaySFX("waterHit");
            waterHitCount++;
            col.GetComponent<CloudWeakSpot>().HitByWater();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "WeakSpawner")
        {
            SFXManager.Instance.PlaySFX("waterHit");
            waterHitCount++;
            col.GetComponent<CloudWeakSpawner>().HitByWater();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "BallGuard")
        {
            SFXManager.Instance.PlaySFX("blocked");
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
            SFXManager.Instance.PlaySFX("heal");
            waterHitCount++;
            col.GetComponent<Medpack>().Bounce();
            Destroy(transform.parent.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SFXManager.Instance.PlaySFX("shoot");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
