using UnityEngine;

public class StandardMaterialStrategy : IBallMaterialStrategy
{
    public string MaterialName => "Standard Ball";

    public void ApplyPhysicalProperties(Rigidbody2D rb, CircleCollider2D collider, SpriteRenderer renderer, GameSettings settings)
    {
        rb.linearDamping = 1.5f;
        collider.sharedMaterial = new PhysicsMaterial2D { bounciness = 0.6f, friction = 0.4f };
    }
}