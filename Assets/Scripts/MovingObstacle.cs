using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingObstacle : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float speed = 2f;
	[SerializeField] private bool startMovingUp = true;

	private Rigidbody2D rb;
	private float direction;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.useFullKinematicContacts = true; // necesario para detectar choques con paredes estáticas
		direction = startMovingUp ? 1f : -1f;
	}

	private void FixedUpdate()
	{
		rb.linearVelocity = new Vector2(0f, direction * speed);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Contra la bola no invierte: ella rebota, el obstáculo sigue su camino
		if (collision.collider.GetComponent<Ball>() != null) return;

		// Si el choque fue por arriba, baja; si fue por abajo, sube
		float contactY = collision.GetContact(0).point.y;
		direction = contactY > rb.position.y ? -1f : 1f;
	}
}