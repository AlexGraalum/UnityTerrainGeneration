using UnityEngine;

[System.Serializable]
public struct HeightMapData {
     [HideInInspector]
     public float[,] heightMap;
     [Range(1, 8)]
     public int octaves;
     public static float persistence = 0.555f;
     public static float lacunarity = 2.0f;
     [Range(0, 1000)]
     public float scale;
     public Vector2Int offset;

     public void Validate() {
          if (lacunarity < 1) lacunarity = 1;
          if (octaves < 1) octaves = 1;
          if (scale <= 0) scale = 0.001f;
     }

     public void Randomize() {
          offset = new Vector2Int(Random.Range(-100000, 100000), Random.Range(-100000, 100000));
     }

     public void GenerateHeightMap(int width, int height, int seed, Vector2Int extraOffset) {
          heightMap = new float[width, height];
          heightMap = Noise.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offset + extraOffset);
     }
}
