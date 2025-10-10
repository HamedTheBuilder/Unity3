using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 5f;
    public float jumpForce = 5f;

    // ≈÷«›… „ €Ì—«  «·√‰Ì„Ì‘‰
    public Animator animator;

    private bool isGrounded = true;
    private Vector3 movement;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // «·Õ’Ê· ⁄·Ï „ﬂÊ‰ «·√‰Ì„Ì —  ·ﬁ«∆Ì«
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Movement
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");
        movement = new Vector3(Horizontal, 0, Vertical);

        // «· Õﬂ„ ›Ì «·√‰Ì„Ì‘‰
        HandleAnimation(Horizontal, Vertical);

        // «·Õ—ﬂ…
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
        }
    }

    // œ«·… ·· Õﬂ„ ›Ì «·√‰Ì„Ì‘‰
    void HandleAnimation(float horizontal, float vertical)
    {
        // «· Õﬁﬁ ≈–« ﬂ«‰ «··«⁄» Ì Õ—ﬂ
        bool isMoving = (horizontal != 0f || vertical != 0f);

        //  ⁄ÌÌ‰ »«—«„Ì —«  «·√‰Ì„Ì‘‰
        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);

        // ≈–« ﬂ«‰ Ì Õ—ﬂ° ‰Õœœ « Ã«Â «·Õ—ﬂ… ··√‰Ì„Ì‘‰
        if (isMoving)
        {
            animator.SetFloat("Speed", movement.magnitude);
        }
    }

    // Check if player is on ground
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }
    }

    // ≈÷«›… ·· Õﬁﬁ „‰ «·Œ—ÊÃ „‰ «·√—÷ («Œ Ì«—Ì)
    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}