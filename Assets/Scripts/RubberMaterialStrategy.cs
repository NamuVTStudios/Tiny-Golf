using UnityEngine;

public class RubberMaterialStrategy : IBallMaterialStrategy
{
    public string MaterialName => "Rubber Ball";

    public void ApplyPhysicalProperties(Rigidbody2D rb, CircleCollider2D collider, SpriteRenderer renderer, GameSettings settings)
    {
        rb.linearDamping = 0.8f;
        collider.sharedMaterial = new PhysicsMaterial2D { bounciness = 0.95f, friction = 0.4f };
    }
}
