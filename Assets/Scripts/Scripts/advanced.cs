using UnityEngine;
using UnityEngine.UI;

public class BetterWallJump3D : MonoBehaviour
{
    [Header("حركة اللاعب")]
    private float horizontal;
    public float speed = 20f;
    public float jumpingPower = 20f;
    private bool isFacingRight = true;

    [Header("نظام القفزات الإضافية")]
    private int extraJumps = 0;
    public int maxExtraJumps = 1; // تمت الإضافة هنا
    public Text extraJumpsText;

    [Header("التحكم في الجاذبية")]
    public float jumpGravity = 2f;      // جاذبية خفيفة أثناء الصعود
    public float fallGravity = 3f;        // جاذبية قوية أثناء الهبوط

    [Header("مراجع")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        HandleJumping();
        HandleGravity();
        FlipCharacter();
        UpdateUI();
    }

    private void HandleGravity()
    {
        if (!IsGrounded())
        {
            float currentGravity = (rb.linearVelocity.y > 0) ? jumpGravity : fallGravity;
            rb.linearVelocity += Physics.gravity * (currentGravity - 1) * Time.deltaTime;
        }
    }

    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded())
            {
                PerformJump();
            }
            else if (extraJumps > 0)
            {
                extraJumps--;
                PerformJump();
            }
        }
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpingPower, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(horizontal * speed, rb.linearVelocity.y, 0f);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
    }

    private void FlipCharacter()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void AddExtraJumps(int jumpsToAdd)
    {
        extraJumps += jumpsToAdd;
        // تمت الإضافة هنا لمنع تجاوز الحد الأقصى
        if (extraJumps > maxExtraJumps)
            extraJumps = maxExtraJumps;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (extraJumpsText != null)
            extraJumpsText.text = $"Extra Jumps: {extraJumps}";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
    }
}