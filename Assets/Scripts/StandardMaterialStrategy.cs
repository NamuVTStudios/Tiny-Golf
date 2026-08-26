using UnityEngine;

public class StandardMaterialStrategy : IBallMaterialStrategy
{
    public string MaterialName => "Standard Ball";

    public void ApplyPhysicalProperties(Rigidbody2D rb, CircleCollider2D collider, SpriteRenderer renderer, GameSettings settings)
    {
        rb.linearDamping = 1.5f;
        if (collider.sharedMaterial != null) collider.sharedMaterial.bounciness = 0.6f;

        // Inyectamos la primera pelota del Spritesheet (Índice 0)
        if (renderer != null && settings.availableBallSprites.Length > 0)
        {
            renderer.sprite = settings.availableBallSprites[0];
        }
    }
}
