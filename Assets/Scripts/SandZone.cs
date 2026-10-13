using UnityEngine;

public class SandZone : MonoBehaviour
{
	[Header("Sand Settings")]
	[Tooltip("Cuanto mayor, más frena la arena. 3 = suave, 5 = media, 8 = muy pesada")]
	[SerializeField] private float dragStrength = 5f;

	// Se ejecuta en cada paso de física mientras la bola está dentro de la zona
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.GetComponent<Ball>() == null) return;

		Rigidbody2D rb = other.attachedRigidbody;
		if (rb == null) return;

		// Frenado proporcional a la velocidad, independiente de los FPS
		rb.linearVelocity /= 1f + dragStrength * Time.fixedDeltaTime;
	}
}