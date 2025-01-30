using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float pushSpeed;
    [SerializeField] float AttractSpeed;
    [SerializeField] float LaunchTime;
    [SerializeField] int jumpCount = 1;

    [Tooltip("Tags")]
    [SerializeField] string GroundTag;
    [SerializeField] string BouncerTag;
    [SerializeField] string AttractorTag;
    [SerializeField] string ExtraJumpGem;


    bool isTouchingGround;
    private float directionX;

    bool canMoveLR = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value) 
    {
        float v = value.Get<float>();
        Debug.Log(v);
        directionX = v;
    }

    private void Move()
    {
        if (!canMoveLR) return;
        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
    }

    void OnJump() 
    {
        if (!isTouchingGround && jumpCount < 1) return;
        Jump();
    }

    void Jump() 
    {
        Debug.Log("boing");
        rb.linearVelocityY = 0;
        rb.AddForce(new Vector2(0, jumpForce));
        jumpCount--;
    }

    private void FixedUpdate()
    {
        Move();
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if(collision.collider.IsTouchingLayers(6))
    //        isTouchingGround = true;
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.collider.IsTouchingLayers(6))
    //        isTouchingGround = false;
    //}

    private void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++) 
        {
            if (Vector2.Angle(collision.GetContact(i).normal, Vector2.up)< 45f/*&& collision.GetContact(i).otherCollider.CompareTag(GroundTag)*/)
            {
                isTouchingGround = true;
                if (jumpCount < 1) jumpCount = 1;
            }
        }

        //collision.collider.IsTouchingLayers(6);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GroundTag))
        {
            isTouchingGround = false;
            jumpCount--;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag(BouncerTag))
        {
            Vector2 v = -(collision.transform.position-this.transform.position).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(v * pushSpeed);
            PauseMove(LaunchTime);
        }
        if (collision.gameObject.CompareTag(AttractorTag))
        {
            Vector2 v = collision.transform.position - this.transform.position;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(v * AttractSpeed);
            PauseMove(LaunchTime);
        }
        if (collision.gameObject.CompareTag(ExtraJumpGem)) 
        {
            Destroy(collision.gameObject);
            jumpCount++;
        }
        
    }

    IEnumerator PauseMove(float time) 
    {
        canMoveLR = false;
        yield return new WaitForSeconds(time);
        canMoveLR = true;

    }
}
