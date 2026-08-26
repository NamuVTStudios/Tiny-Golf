using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Ball : MonoBehaviour
{
    public event Action OnBallStopped;

    [Header("Settings Injector")]
    [SerializeField] private GameSettings gameSettings; 

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shotSound;    
    [SerializeField] private AudioClip holeInSound;  

    private Rigidbody2D rb;
    private CircleCollider2D ballCollider;
    private SpriteRenderer ballRenderer; 
    private TrailRenderer ballTrail; 
    private AudioSource audioSource; 
    private bool isMoving = false;
    
    private IBallMaterialStrategy currentMaterialStrategy;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<CircleCollider2D>();
        ballRenderer = GetComponent<SpriteRenderer>(); 
        ballTrail = GetComponent<TrailRenderer>(); 
        audioSource = GetComponent<AudioSource>(); 

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; 

        ChangeMaterial(new StandardMaterialStrategy());
    }

    public void ChangeMaterial(IBallMaterialStrategy newMaterial)
    {
        currentMaterialStrategy = newMaterial;
        currentMaterialStrategy.ApplyPhysicalProperties(rb, ballCollider, ballRenderer, gameSettings);
    }

    // NUEVO MÉTODO DE CONTROL: Apaga o enciende gráficos y colisiones de la bola de forma segura
    public void SetBallState(bool visible)
    {
        if (ballRenderer != null) ballRenderer.enabled = visible;
        if (ballCollider != null) ballCollider.enabled = visible;
        
        if (!visible)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            isMoving = false;
        }
    }

    public void ApplyForce(Vector2 forceDirection)
    {
        ClearTrail();
        rb.AddForce(forceDirection, ForceMode2D.Impulse);
        isMoving = true;

        if (audioSource != null && shotSound != null)
        {
            audioSource.PlayOneShot(shotSound);
        }
    }

    public void PlayHoleInSound()
    {
        if (audioSource != null && holeInSound != null)
        {
            audioSource.PlayOneShot(holeInSound);
        }
    }

    void FixedUpdate()
    {
        // Solo procesamos el frenado si la bola está activa físicamente en el mapa
        if (isMoving && ballCollider.enabled && rb.linearVelocity.magnitude < 0.05f)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            isMoving = false;
            OnBallStopped?.Invoke();
        }
    }

    public bool IsStopped()
    {
        return !isMoving && rb.linearVelocity.magnitude < 0.05f;
    }

    public void ClearTrail()
    {
        if (ballTrail != null) ballTrail.Clear();
    }
}