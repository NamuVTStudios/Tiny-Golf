using UnityEngine;

public class StoneMaterialStrategy : IBallMaterialStrategy
{
    public string MaterialName => "Stone Ball";

    public void ApplyPhysicalProperties(Rigidbody2D rb, CircleCollider2D collider, SpriteRenderer renderer, GameSettings settings)
    {
        rb.linearDamping = 3.5f;
        if (collider.sharedMaterial != null) collider.sharedMaterial.bounciness = 0.1f;

        // Inyectamos la tercera pelota del Spritesheet (Índice 2)
        if (renderer != null && settings.availableBallSprites.Length > 2)
        {
            renderer.sprite = settings.availableBallSprites[2];
        }
    }
}
