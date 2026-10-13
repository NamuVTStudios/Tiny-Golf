using UnityEngine;

public class StoneMaterialStrategy : IBallMaterialStrategy
{
    public string MaterialName => "Stone Ball";

    public void ApplyPhysicalProperties(Rigidbody2D rb, CircleCollider2D collider, SpriteRenderer renderer, GameSettings settings)
    {
        rb.linearDamping = 3.5f;
        collider.sharedMaterial = new PhysicsMaterial2D { bounciness = 0.1f, friction = 0.6f };
    }

	public Sprite GetOverrideSprite(GameSettings settings) => settings != null ? settings.stoneBallSprite : null;
}