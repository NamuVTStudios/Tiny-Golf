using System;
using UnityEngine; // <--- Asegúrate de tener esta librería para usar Vector2

[Serializable]
public class PlayerData
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int CurrentHoleStrokes { get; set; }
    public int TotalScore { get; set; }
    
    // NUEVO: Almacena las coordenadas en el mapa de dónde quedó este jugador
    public Vector2 LastSavedPosition { get; set; }

    public PlayerData(int id, string name)
    {
        Id = id;
        Name = name;
        CurrentHoleStrokes = 0;
        TotalScore = 0;
        LastSavedPosition = Vector2.zero; // Inicializa por defecto
    }
}
