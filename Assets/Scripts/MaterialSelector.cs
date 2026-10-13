using UnityEngine;

public class MaterialSelector : MonoBehaviour
{
	[SerializeField] private Ball gameplayBall;

	// Vincular al OnClick() del botón de cambiar bola
	public void ToggleNextBallMaterial()
	{
		// Solo se puede cambiar con la bola quieta y mientras es el turno de un jugador
		if (gameplayBall == null || !gameplayBall.IsStopped()) return;
		if (GameManager.Instance == null || !GameManager.Instance.CanPlayerShoot) return;

		GameManager.Instance.CycleCurrentPlayerMaterial();
	}
}