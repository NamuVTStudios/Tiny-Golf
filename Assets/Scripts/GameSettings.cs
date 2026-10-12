using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Golf/Game Settings")]
public class GameSettings : ScriptableObject
{
    public int playerCount;             
    public int totalHoles;              
    public int maxStrokesPerHole;       
    public float turnTimeLimit;         
    public float matchTimeLimit = 300f;
    public string[] playerNames;
    
    // CORRECCIÓN: Declaración pura sin tamaños fijos ni variables en la cabecera
    public int[] selectedPlayerSpriteIndices; 
    
    [Header("Visual Asset Libraries")]
    public Sprite[] availableBallSprites;    
    public Sprite[] availablePlayerSprites;
}