using UnityEngine;
using UnityEngine.EventSystems;

public class ShotController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Ball targetBall;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TurnController turnController;

    [Header("Force Settings")]
    [SerializeField] private float forceMultiplier = 5f;
    [SerializeField] private float maxForce = 15f;

    private Vector2 initialMousePosition;
    private Vector2 currentMousePosition;
    private bool isDragging = false;

    void Update()
    {
        // 1. Validar que la bola exista y se encuentre estática
        if (targetBall == null || !targetBall.IsStopped()) return;

        // 2. Inicio del arrastre (Click)
        if (Input.GetMouseButtonDown(0))
        {
            // BLINDAJE CRÍTICO: Si el mouse está haciendo clic sobre un botón de la UI, ignoramos el tiro
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            initialMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        // 3. Arrastre en ejecución
        if (Input.GetMouseButton(0) && isDragging)
        {
            currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        // 4. Fin del arrastre (Liberar Click) - Disparar
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            CalculateAndShoot();
        }
    }

    private void CalculateAndShoot()
    {
        // 1. Calcular la dirección inversa al arrastre (estilo catapulta)
        Vector2 shotDirection = initialMousePosition - currentMousePosition;

        // 2. Calcular la magnitud relativa según la distancia y aplicar límites (Clamp)
        float forceMagnitude = shotDirection.magnitude * forceMultiplier;
        forceMagnitude = Mathf.Clamp(forceMagnitude, 0f, maxForce);

        // Si el arrastre fue insignificante, no lo contamos como tiro gastado
        if (forceMagnitude < 0.2f) return;

        // 3. COMBINAR EN UN SOLO ARGUMENTO VECTOR2: Multiplicamos la dirección por la potencia
        Vector2 finalCombinedForce = shotDirection.normalized * forceMagnitude;

        // CORRECCIÓN MAGISTRAL: Le pasamos un único Vector2 a la bola
        targetBall.ApplyForce(finalCombinedForce);
		GameManager.Instance.OnShotTaken();

		// 4. Registrar el golpe en el TurnController leyendo los settings del GameManager
		GameSettings settings = GameManager.Instance.GetGameSettings();
        bool outOfStrokes = turnController.RegisterStroke(settings.maxStrokesPerHole);

        // 5. Refrescar la UI en pantalla en tiempo real
        GameManager.Instance.UpdateGameplayUI();

        // 6. Si usó su último tiro, levantar la bandera de descalificación por hoyo
        if (outOfStrokes)
        {
            GameManager.Instance.NotifyPlayerMaxStrokesReached();
        }
    }

    private void OnDisable() { isDragging = false; }
}