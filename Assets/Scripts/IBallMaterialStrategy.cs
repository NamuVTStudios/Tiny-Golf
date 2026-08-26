using UnityEngine;

public interface IBallMaterialStrategy
{
    string MaterialName { get; }
    // Añadimos el SpriteRenderer como parámetro obligatorio
    void ApplyPhysicalProperties(Rigidbody2D rb, CircleCollider2D collider, SpriteRenderer renderer, GameSettings settings);
}