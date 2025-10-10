using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation References")]
    public Animator animator;
    public Transform playerModel;
    public CapsuleCollider playerCollider;

    [Header("Collider Settings")]
    public float normalColliderHeight = 2f;
    public float normalColliderCenter = 1f;
    public float crouchColliderHeight = 1f;
    public float crouchColliderCenter = 0.5f;

    // Animation Hashes
    private readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int IsCrouchingHash = Animator.StringToHash("IsCrouching");
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private readonly int DoubleJumpHash = Animator.StringToHash("DoubleJump");
    private readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");

    private bool isCrouching = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider>();

        SetupNormalCollider();
    }

    // ██████████████████████████████████████████████████████████████████████████████
    // الحركة والمشي
    // ██████████████████████████████████████████████████████████████████████████████

    public void SetMovement(float moveSpeed, Vector3 moveDirection, bool isMoving)
    {
        if (animator == null) return;

        animator.SetBool(IsWalkingHash, isMoving);
        animator.SetFloat(MoveSpeedHash, moveSpeed);

        if (isMoving && moveDirection != Vector3.zero && playerModel != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    // ██████████████████████████████████████████████████████████████████████████████
    // الانحناء (Crouch)
    // ██████████████████████████████████████████████████████████████████████████████

    public void SetCrouching(bool crouch)
    {
        if (animator == null) return;

        isCrouching = crouch;
        animator.SetBool(IsCrouchingHash, crouch);

        if (crouch)
        {
            SetupCrouchCollider();
        }
        else
        {
            SetupNormalCollider();
        }
    }

    void SetupNormalCollider()
    {
        if (playerCollider != null)
        {
            playerCollider.height = normalColliderHeight;
            playerCollider.center = new Vector3(0, normalColliderCenter, 0);
        }
    }

    void SetupCrouchCollider()
    {
        if (playerCollider != null)
        {
            playerCollider.height = crouchColliderHeight;
            playerCollider.center = new Vector3(0, crouchColliderCenter, 0);
        }
    }

    // ██████████████████████████████████████████████████████████████████████████████
    // القفز (Jump & Double Jump)
    // ██████████████████████████████████████████████████████████████████████████████

    public void Jump(int jumpCount)
    {
        if (animator == null) return;

        // إعادة تعيين جميع triggers أولاً
        ResetJumpTriggers();

        // تشغيل أنيميشن القفز المناسبة
        if (jumpCount == 1)
        {
            animator.SetTrigger(JumpHash);
            Debug.Log("Playing First Jump Animation");
        }
        else if (jumpCount == 2)
        {
            animator.SetTrigger(DoubleJumpHash);
            Debug.Log("Playing Double Jump Animation");
        }
    }

    public void Land()
    {
        if (animator == null) return;

        animator.SetBool(IsGroundedHash, true);
        ResetJumpTriggers(); // إعادة تعيين عند الهبوط
    }

    public void SetGrounded(bool grounded)
    {
        if (animator == null) return;

        animator.SetBool(IsGroundedHash, grounded);
    }

    // ██████████████████████████████████████████████████████████████████████████████
    // الهجوم (Attack)
    // ██████████████████████████████████████████████████████████████████████████████

    public void Attack()
    {
        if (animator == null) return;

        animator.SetTrigger(AttackHash);
    }

    // ██████████████████████████████████████████████████████████████████████████████
    // دوال مساعدة
    // ██████████████████████████████████████████████████████████████████████████████

    void ResetJumpTriggers()
    {
        if (animator == null) return;

        animator.ResetTrigger(JumpHash);
        animator.ResetTrigger(DoubleJumpHash);
    }

    public void ResetAllTriggers()
    {
        if (animator == null) return;

        ResetJumpTriggers();
        animator.ResetTrigger(AttackHash);
    }
}