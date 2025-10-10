using System.Collections;
using UnityEngine;

public class PlayerMovementmain : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 7f;
    public float doubleJumpForce = 5f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("References")]
    public Animator animator;
    public CharacterController controller;
    public Transform cameraTransform;

    private Vector3 velocity;
    private bool isGrounded;
    private bool canDoubleJump = false;
    private bool isCrouching = false;
    private float currentSpeed;
    private float gravity = -9.81f;

    void Update()
    {
        // «· Õﬁﬁ „‰ «·√—÷
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            canDoubleJump = true;
        }

        // «·Õ—ﬂ…
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // «· Õﬂ„ ›Ì «·”—⁄… („‘Ì/—ﬂ÷/«‰Õ‰«¡)
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            currentSpeed = sprintSpeed;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        // «·«‰Õ‰«¡
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouch();
        }

        // «·ﬁ›“
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump(jumpForce);
                animator.SetTrigger("Jump");
            }
            else if (canDoubleJump)
            {
                Jump(doubleJumpForce);
                canDoubleJump = false;
                animator.SetTrigger("DoubleJump");
            }
        }

        // «·Ã«–»Ì…
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // «·√‰Ì„Ì‘‰
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("Speed", move.magnitude * currentSpeed);
    }

    void Jump(float force)
    {
        velocity.y = Mathf.Sqrt(force * -2f * gravity);
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            controller.height = 1f;
            animator.SetTrigger("Crouch");
        }
        else
        {
            controller.height = 2f;
        }
    }

    public void TakeKnockback(Vector3 direction, float force)
    {
        velocity = direction * force;
    }
}