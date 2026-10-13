using System.Collections.Generic;
using UnityEngine;

public static class MatchSession
{
	public static readonly string[] LevelScenes = { "Level1", "Level2", "Level3" };
	public static int CurrentLevel;
	public static Dictionary<int, int> Scores = new Dictionary<int, int>();
	public static Dictionary<int, int> Materials = new Dictionary<int, int>(); // id jugador -> 0 estándar, 1 goma, 2 piedra
	public static float MatchTimeRemaining;

	private static int playerCount;
	private static float totalMatchTime;

	public static void Reset(int players, float matchTime)
	{
		playerCount = players;
		totalMatchTime = matchTime;
		CurrentLevel = 0;
		MatchTimeRemaining = matchTime;
		Scores.Clear();
		Materials.Clear();
		for (int i = 0; i < players; i++) Scores[i] = 0;
	}

	public static void Restart()
	{
		Reset(playerCount, totalMatchTime);
	}

	public static int GetMaterial(int playerId)
	{
		return Materials.TryGetValue(playerId, out int m) ? m : 0;
	}

	public static void SetMaterial(int playerId, int index)
	{
		Materials[playerId] = index;
	}
}