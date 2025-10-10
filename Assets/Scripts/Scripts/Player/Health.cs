using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
   
    private bool dead;

   // private Animator anim;
    private void Awake()
    {
        currentHealth = startingHealth;

        //  ÷Ì› «·«‰Ì„Ì‘‰
        // anim = GetComponent<Animator>();
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        if (currentHealth > 0)

        {
            // «÷«›… «‰Ì„Ì‘‰ «·œ„Ã
            // anim.SetTrigger("hurt");
        }
        else
        {
            if (!dead)
            {
                // anim.SetTrigger("die");

                // Â‰« ‰÷Ì› ﬂÊœ «··«⁄» Ì⁄‰Ì »œ«· char movement
                GetComponent<BetterWallJump3D>().enabled = false;
                dead = true;
            }
        }
    }

    public void AddHealth (float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);

    }

}