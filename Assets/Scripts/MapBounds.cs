using UnityEngine;

public class MapBounds : MonoBehaviour
{
	private Collider2D boundsCollider;

	private void Awake()
	{
		boundsCollider = GetComponent<Collider2D>();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Ball ball = collision.GetComponent<Ball>();
		if (ball == null) return;

		// Falso positivo: el collider de la bola se apagó, pero la bola sigue dentro del mapa
		if (boundsCollider.OverlapPoint(ball.transform.position)) return;

		Debug.LogWarning("MapBounds: ¡La bola se ha salido de los límites del mapa!");
		GameManager.Instance.NotifyBallOutOfBounds();
	}
}