using UnityEngine;

public class MapBounds : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el objeto que se escapó del colisionador contenedor es la bola
        Ball ball = collision.GetComponent<Ball>();
        if (ball != null)
        {
            Debug.LogWarning("MapBounds: ¡La bola se ha salido de los límites del mapa!");
            
            // Le ordenamos al GameManager que active el protocolo de reubicación
            GameManager.Instance.NotifyBallOutOfBounds();
        }
    }
}