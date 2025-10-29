using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Umbrella : MonoBehaviour
{

    [SerializeField] private CitizenManager citizen;
    [SerializeField] private Rigidbody2D rb2d;
    private PurchaseUmbrella purchaseUmbrella;
    [SerializeField] private TextMeshProUGUI healthIndicator;

    public static int mostActiveUmbrellasEver = 0;
    public static Stack<Umbrella> activeUmbrellas = new Stack<Umbrella>();

    //Debug
    private Umbrella lastHighlighted = null;

    //Health
    private int health = default;
    [SerializeField] private int maxHealth = 3;

    //Flash
    [SerializeField] private float flashDuration = 0.15f;
    private Coroutine flashCoroutine;

    //Bounce
    [SerializeField] private float bounceForceX = 1f;
    [SerializeField] private float bounceForceY = 1f;
    private Coroutine deleteGroundedCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        citizen = FindObjectOfType<CitizenManager>();
        purchaseUmbrella = FindObjectOfType<PurchaseUmbrella>();

        if (activeUmbrellas.Count == 0)
        {
            transform.position = citizen.transform.position + new Vector3(0, -0.3f);
            FixedJoint2D fixedJoint = gameObject.AddComponent<FixedJoint2D>();
            fixedJoint.connectedBody = citizen.GetComponent<Rigidbody2D>();
        }
        else
        {
            //Increase health of lower umbrellas
            foreach (Umbrella umbrella in activeUmbrellas)
            {
                umbrella.health += 1;
            }

            transform.position = activeUmbrellas.Peek().transform.position + new Vector3(0, 0.95f);
            HingeJoint2D hinge = gameObject.AddComponent<HingeJoint2D>();
            hinge.connectedBody = activeUmbrellas.Peek().GetComponent<Rigidbody2D>();

            JointAngleLimits2D limits = hinge.limits;
            limits.min = -5f;
            limits.max = 5f;
            hinge.limits = limits;
            hinge.useLimits = true;

        }

        activeUmbrellas.Push(this);
        purchaseUmbrella.determineIsReady();

        if (activeUmbrellas.Count > mostActiveUmbrellasEver)
        {
            mostActiveUmbrellasEver = activeUmbrellas.Count;
        }

    }




    void Update()
    {
        //highlightTopUmbrella();
        healthIndicator.text = $"{health}";
        if(health < 0)
        {
            health = 0;
        }

    }



    //Highlight umbrellas debug
    void ResetHighlight(Umbrella umbrella)
    {
        SpriteRenderer sr = umbrella.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
        }
    }

    void DrawColliderOutline(Collider2D collider)
    {
        if (collider is CircleCollider2D circle)
        {
            Debug.DrawRay(circle.transform.position, Vector2.right * circle.radius, Color.red);
            Debug.DrawRay(circle.transform.position, Vector2.up * circle.radius, Color.red);
        }
    }

    void highlightTopUmbrella()
    {
        if (activeUmbrellas.Count > 0)
        {
            Umbrella topUmbrella = activeUmbrellas.Peek();

            // Reset the last highlighted umbrella if it's not the top anymore
            if (lastHighlighted != null && lastHighlighted != topUmbrella)
            {
                ResetHighlight(lastHighlighted);
            }

            // Highlight the top umbrella
            Collider2D collider = topUmbrella.GetComponent<Collider2D>();
            if (collider != null)
            {
                SpriteRenderer sr = topUmbrella.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.red;
                }

                DrawColliderOutline(collider);
            }

            lastHighlighted = topUmbrella;
        }
        else if (lastHighlighted != null)
        {
            ResetHighlight(lastHighlighted);
            lastHighlighted = null;
        }
    }


    private void popUnattatchedUmbrellas(GameObject col)
    {

        if (activeUmbrellas.ToArray().Contains(this))
        {
            while (activeUmbrellas.Count > 0)
            {
                Umbrella popped = activeUmbrellas.Pop();
                HingeJoint2D hinge = popped.GetComponent<HingeJoint2D>();
                if (hinge != null)
                {
                    Destroy(hinge);
                }
                if (popped == this)
                {
                    break;
                }
            }
        }

    }

    private IEnumerator FlashWhite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;

        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;

        flashCoroutine = null;
    }

    private IEnumerator TakeDamage(GameObject col)
    {
        health--;


        if (flashCoroutine == null)
        {
            flashCoroutine = StartCoroutine(FlashWhite());
        }

        if (health <= 0)
        {
            //yield return flashCoroutine;
            yield return new WaitForSeconds(flashDuration);
            popUnattatchedUmbrellas(col);
            Destroy(gameObject);
        }

    }


    private IEnumerator deleteGrounded(GameObject col)
    {
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(TakeDamage(col));
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //GameObject col = collision.gameObject;
        GameObject col = collision.GetContact(0).collider.gameObject;

        if (col.tag == "BallGuard")
        {
            Transform guardedBall = col.gameObject.transform.parent;
            guardedBall.GetComponent<Ball>().Bounce();

            StartCoroutine(TakeDamage(col));
        }
        if (col.tag == "Ball")
        {
            col.GetComponent<Ball>().Split();

            StartCoroutine(TakeDamage(col));
        }
        //if (col.tag == "SupportBall")
        //{
        //    col.GetComponent<SupportBall>().Bounce();
        //}

        if (col.gameObject.tag == "Wall")
        {
            if (col.gameObject.name == "Left_Apartment")
            {
                rb2d.AddForce(new Vector2(bounceForceX, bounceForceY), ForceMode2D.Impulse);
            }
            else 
            {
                rb2d.AddForce(new Vector2(-bounceForceX, bounceForceY), ForceMode2D.Impulse);
            }

            StartCoroutine(TakeDamage(col));
        }
        if (col.gameObject.tag == "Ground")
        {
            deleteGroundedCoroutine = StartCoroutine(deleteGrounded(col));
            rb2d.AddForce(new Vector2(0f, bounceForceY), ForceMode2D.Impulse);
            StartCoroutine(TakeDamage(col));
        }

    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject col = collision.gameObject;
        if (col.gameObject.tag == "Ground")
        {
            if (deleteGroundedCoroutine != null)
            {
                StopCoroutine(deleteGroundedCoroutine);
            }

        }
    }




}
