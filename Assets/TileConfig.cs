using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileConfig : MonoBehaviour
{
    [SerializeField]
    private Vector2 dimentions;

    private static Vector2 tilesize;

    [SerializeField]
    private GameObject outlinePrefab;

    private static GameObject OutlinePrefab;

    [SerializeField]
    private float basicGemSpawnChance = 90f;

    private static float BasicGemSpawnChance;
    private void Awake()
    {
        tilesize = dimentions;
        OutlinePrefab = outlinePrefab;
        BasicGemSpawnChance = basicGemSpawnChance;
    }

    public static Vector2 GetTileSize() { return tilesize; }

    public static GameObject GetOutline() { return OutlinePrefab; }

    public static float GetSpawnChance() { return BasicGemSpawnChance; }
}
