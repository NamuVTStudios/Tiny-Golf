using System.Collections;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private Collider2D holeCollider;

    void Awake()
    {
        holeCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.GetComponent<Ball>();
        
        if (ball != null)
        {
            Debug.Log("Hole: ¡La bola ha ingresado al hoyo!");
            
            // Deshabilitamos el colisionador del hoyo temporalmente para evitar el doble registro
            StartCoroutine(DisableHoleTemporarily());

            // Le avisamos al GameManager que la bola llegó a la meta
            GameManager.Instance.RegisterHoleCompletion();
        }
    }

    private IEnumerator DisableHoleTemporarily()
    {
        holeCollider.enabled = false;
        
        // Esperamos una décima de segundo (tiempo suficiente para que la bola se teletransporte lejos)
        yield return new WaitForSeconds(0.1f);
        
        holeCollider.enabled = true;
    }
}