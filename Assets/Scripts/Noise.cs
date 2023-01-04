using UnityEngine;

public class Noise : MonoBehaviour {
     public static float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset) {
          float[,] noiseMap = new float[width, height];

          System.Random rand = new System.Random(seed);
          Vector2[] octaveOffsets = new Vector2[octaves];

          float maxHeight = 0;
          float amplitude = 1;

          for(int i = 0; i < octaves; i++) {
               float offsetX = rand.Next(-100000, 100000) + offset.x;
               float offsetY = rand.Next(-100000, 100000) + offset.y;
               octaveOffsets[i] = new Vector2(offsetX, offsetY);

               maxHeight += amplitude;
               amplitude *= persistence;
          }

          if (scale <= 0) scale = 0.0001f;

          float halfWidth = width / 2.0f;
          float halfHeight = height / 2.0f;

          for(int y = 0; y < height; y++) {
               for(int x = 0; x < width; x++) {
                    amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for(int i = 0; i < octaves; i++) {
                         float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                         float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                         float pVal = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                         noiseHeight += pVal * amplitude;

                         amplitude *= persistence;
                         frequency *= lacunarity;
                    }

                    noiseMap[x, y] = noiseHeight;
               }
          }

          for(int y = 0; y < height; y++) {
               for(int x = 0; x < width; x++) {
                    noiseMap[x, y] = Mathf.Clamp((noiseMap[x, y] + 1) / maxHeight, 0, int.MaxValue);
               }
          }

          return noiseMap;
     }
}
