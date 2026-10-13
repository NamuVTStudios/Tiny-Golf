using UnityEngine;

public interface IBallMaterialStrategy
{
	string MaterialName { get; }
	void ApplyPhysicalProperties(Rigidbody2D rb, CircleCollider2D collider, SpriteRenderer renderer, GameSettings settings);

	// Sprite fijo del material. Devuelve null si el material usa el sprite elegido por el jugador.
	Sprite GetOverrideSprite(GameSettings settings);
}