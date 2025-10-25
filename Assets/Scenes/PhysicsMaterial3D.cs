using UnityEngine;

public class PhysicsMaterial3D : MonoBehaviour
{
    [Header("≈⁄œ«œ«  Physics Material")]
    public float friction = 0f;
    public float bounciness = 0f;

    private PhysicsMaterial physicsMat;
    private Collider col;

    void Start()
    {
        // ≈‰‘«¡ Physics Material
        physicsMat = new PhysicsMaterial();
        physicsMat.name = "PlayerPhysics3D";

        //  ÿ»Ìﬁ «·≈⁄œ«œ« 
        physicsMat.dynamicFriction = friction;
        physicsMat.staticFriction = friction;
        physicsMat.bounciness = bounciness;
        physicsMat.frictionCombine = PhysicsMaterialCombine.Multiply;
        physicsMat.bounceCombine = PhysicsMaterialCombine.Average;

        //  ÿ»Ìﬁ «·„«œ… ⁄·Ï «·ﬂÊ·Ìœ—
        col = GetComponent<Collider>();
        if (col != null)
        {
            col.material = physicsMat;
        }
    }

    void Update()
    {
        //  ÕœÌÀ «·≈⁄œ«œ«  ≈–«  €Ì—  ›Ì «·‹ Inspector
        if (physicsMat != null)
        {
            physicsMat.dynamicFriction = friction;
            physicsMat.staticFriction = friction;
            physicsMat.bounciness = bounciness;
        }
    }
}