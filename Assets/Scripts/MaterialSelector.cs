using UnityEngine;

public class MaterialSelector : MonoBehaviour
{
    [SerializeField] private Ball gameplayBall;
    private int currentSelectionIndex = 0;

    // Vincular este método al OnClick() de tu botón de la interfaz
    public void ToggleNextBallMaterial()
    {
        // Solo permitimos cambiar el material si la bola está quieta en el suelo
        if (!gameplayBall.IsStopped()) return;

        currentSelectionIndex = (currentSelectionIndex + 1) % 3;

        switch (currentSelectionIndex)
        {
            case 0:
                gameplayBall.ChangeMaterial(new StandardMaterialStrategy());
                break;
            case 1:
                gameplayBall.ChangeMaterial(new RubberMaterialStrategy());
                break;
            case 2:
                gameplayBall.ChangeMaterial(new StoneMaterialStrategy());
                break;
        }
    }
}