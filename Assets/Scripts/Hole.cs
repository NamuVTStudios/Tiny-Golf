using System.Collections;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem holeInParticles;

    private Collider2D holeCollider;

    void Awake()
    {
        holeCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.GetComponent<Ball>();
        if (ball == null) return;

        StartCoroutine(DisableHoleTemporarily());

        if (holeInParticles != null) holeInParticles.Play();
        GameManager.Instance.RegisterHoleCompletion(transform.position);
    }

    private IEnumerator DisableHoleTemporarily()
    {
        holeCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        holeCollider.enabled = true;
    }
}