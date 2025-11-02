using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public Vector2 startForce;

    [SerializeField] private Rigidbody2D rb2d;

    [SerializeField] private GameObject nextBall;
    [SerializeField] private GameObject largePuddle;
    [SerializeField] private GameObject mediumPuddle;
    [SerializeField] private GameObject smallPuddle;
    [SerializeField] private float puddleHeight = -4.308f;
    [SerializeField] private float horizontalBarrierForce = 2f;
    [SerializeField] private Vector2 bounceForce = new Vector2(0f, 12f);

    //Track balls
    public static int numActiveBalls = 0;

    private WaveManager BallGenerator;

    private bool hasSplit = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d.AddForce(startForce, ForceMode2D.Impulse);

        numActiveBalls++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Gets name of prefab without "(Clone)" modifier
    private string getPureName(string oldName)
    {
        string newName = oldName;

        // Remove "(Clone)" if it exists
        if (newName.EndsWith("(Clone)"))
        {
            newName = newName.Substring(0, newName.Length - "(Clone)".Length);
        }
        return newName;
    }


    //Create puddle
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (getPureName(gameObject.name) == "ball_large" || getPureName(gameObject.name) == "ball_large_guarded")
            {
                GameObject large_puddle = Instantiate(largePuddle, new Vector3(rb2d.position.x, puddleHeight, 0f), Quaternion.identity);
                large_puddle.GetComponent<PuddleCycle>().createPuddleSplash(rb2d.velocity.y);
            }
            else if (getPureName(gameObject.name) == "ball_medium" || getPureName(gameObject.name) == "ball_medium_guarded")
            {
                GameObject medium_puddle = Instantiate(mediumPuddle, new Vector3(rb2d.position.x, puddleHeight, 0f), Quaternion.identity);
                medium_puddle.GetComponent<PuddleCycle>().createPuddleSplash(rb2d.velocity.y);
            }
            else if (getPureName(gameObject.name) == "ball_small" || getPureName(gameObject.name) == "ball_small_guarded")
            {
                GameObject small_puddle = Instantiate(smallPuddle, new Vector3(rb2d.position.x, puddleHeight, 0f), Quaternion.identity);
                small_puddle.GetComponent<PuddleCycle>().createPuddleSplash(rb2d.velocity.y);
            }
            else 
            {
                Debug.LogError("Ball name cannot be found, cannot create puddle");
            }
        }
        else if (col.gameObject.tag == "Player")
        {
            if (!col.gameObject.GetComponent<Player>().playerIsFrozen)
            {
                //Debug.Log("Started freeze routine");
                col.gameObject.GetComponent<Player>().startFreezePlayerCoroutine();
                if (rb2d.velocity.y < 0f && Mathf.Abs(rb2d.velocity.y) > 5.5f)
                {
                    rb2d.AddForce(new Vector2(0f, 15f), ForceMode2D.Impulse);
                    //Debug.Log("Bounced high");
                }
                else if (rb2d.velocity.y < 0f)
                {
                    rb2d.AddForce(new Vector2(0f, 8f), ForceMode2D.Impulse);
                    //Debug.Log("Bounced medium");
                }
                else
                {
                    rb2d.AddForce(new Vector2(0f, 2.5f), ForceMode2D.Impulse);
                    //Debug.Log("Bounced low");
                }
                
            }
            
        }
        else if (col.gameObject.tag == "Citizen")
        {
            if (!col.gameObject.GetComponent<CitizenManager>().citizenIsFrozen)
            {
                //Debug.Log("Started citizen freeze routine");
                CitizenManager cman = col.gameObject.GetComponent<CitizenManager>();
                cman.setCitizenHealth(cman.getCitizenHealth() - 1);
                if (cman.getCitizenHealth() <= 0)
                {
                    cman.EndGame();
                }
                else
                {
                    cman.startFreezeCitizenCoroutine();
                }


                if (rb2d.velocity.y < 0f && Mathf.Abs(rb2d.velocity.y) > 5.5f)
                {
                    rb2d.AddForce(new Vector2(0f, 15f), ForceMode2D.Impulse);
                }
                else if (rb2d.velocity.y < 0f)
                {
                    rb2d.AddForce(new Vector2(0f, 8f), ForceMode2D.Impulse);
                }
                else
                {
                    rb2d.AddForce(new Vector2(0f, 2.5f), ForceMode2D.Impulse);
                }
            }
        }
    }

    public void Split()
    {
        if (hasSplit) return;
        hasSplit = true;


        if (nextBall != null)
        {
            SFXManager.Instance.PlaySFX("waterHit");
            GameObject ball01 = Instantiate(nextBall, rb2d.position + Vector2.right / 4f, Quaternion.identity);
            GameObject ball02 = Instantiate(nextBall, rb2d.position + Vector2.left / 4f, Quaternion.identity);

            if (nextBall.tag == "Ball")
            {
                numActiveBalls -= 1;
                ball01.GetComponent<Ball>().startForce = new Vector2(2f, 5f);
                ball02.GetComponent<Ball>().startForce = new Vector2(-2f, 5f);
            }
            if (nextBall.tag == "SupportBall")
            {
                numActiveBalls -= 1;
                ball01.GetComponent<SupportBall>().supportStartForce = new Vector2(2f, 7f);
                ball02.GetComponent<SupportBall>().supportStartForce = new Vector2(-2f, 7f);
            }

        }

        Destroy(gameObject);
    }


    //Freeze for gusts
    public void StartFreeze()
    {
        StartCoroutine(Freeze());
    }

    private IEnumerator Freeze()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(180f/255f, 1f, 1f);

        while (rb2d.velocity.magnitude > 0.1f)
        {
            rb2d.gravityScale = 0f;
            rb2d.velocity *= 0.8f;

            yield return new WaitForSeconds(0.01f);
        }
        rb2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.1f);

        sr.color = Color.white;
        rb2d.gravityScale = 1f;
    }


    public void SlowDown()
    {
        rb2d.velocity = new Vector2(0f, 0f);
        rb2d.AddForce(new Vector3(Random.Range(-horizontalBarrierForce, horizontalBarrierForce), 0f, 0f), ForceMode2D.Impulse);
    }

    public void Bounce()
    {
        rb2d.AddForce(bounceForce, ForceMode2D.Impulse);
    }

}
