using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "Ball")
        {
            col.GetComponent<Ball>().Split();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "SupportBall")
        {
            col.GetComponent<SupportBall>().Grow();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "WeakSpot")
        {
            col.GetComponent<CloudWeakSpot>().HitByWater();
            Destroy(transform.parent.gameObject);
        }
        else if (col.tag == "BallGuard")
        {
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
