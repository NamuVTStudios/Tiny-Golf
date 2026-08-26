using UnityEngine;

public class RubberMaterialStrategy : IBallMaterialStrategy
{
    public string MaterialName => "Rubber Ball";

    public void ApplyPhysicalProperties(Rigidbody2D rb, CircleCollider2D collider, SpriteRenderer renderer, GameSettings settings)
    {
        rb.linearDamping = 0.8f;
        if (collider.sharedMaterial != null) collider.sharedMaterial.bounciness = 0.95f;

        // Inyectamos la segunda pelota del Spritesheet (Índice 1)
        if (renderer != null && settings.availableBallSprites.Length > 1)
        {
            renderer.sprite = settings.availableBallSprites[1];
        }
    }
}
