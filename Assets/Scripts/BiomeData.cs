using UnityEngine;

[System.Serializable]
public class BiomeData : MonoBehaviour {
     public Biome[]  biomes;
     //[HideInInspector]
     //public Color[] biomeMap;

     public void OnValidate() {
          Validate();
     }

     public void Validate() {
          biomes[0].biomeHeight = 0.0f;

          for(int i = 1; i < biomes.Length; i++) {
               if (biomes[i].biomeHeight < biomes[i - 1].biomeHeight)
                    biomes[i].biomeHeight = biomes[i - 1].biomeHeight;
          }

          foreach(Biome biome in biomes) {
               for(int i = 1; i < biome.biomeRegions.Length; i++) {
                    if (biome.biomeRegions[i].terrainHeight < biome.biomeRegions[i - 1].terrainHeight)
                         biome.biomeRegions[i].terrainHeight = biome.biomeRegions[i - 1].terrainHeight;
               }
          }
     }

     public Color[] GenerateBiomeMap(int width, int height, float [,] heightMap) {
          Color[] biomeMap = new Color[width * height];

          for(int y = 0; y < height; y++) {
               for(int x = 0; x < width; x++) {
                    for(int i = 0; i < biomes.Length; i++) {
                         if (heightMap[x,y] > biomes[i].biomeHeight)
                              biomeMap[y * height + x] = biomes[i].biomeColor;
                    }
               }
          }

          return biomeMap;
     }

     [System.Serializable]
     public struct Biome {
          public string biomeName;
          public Color biomeColor;
          [Range(0, 1)]
          public float biomeHeight;
          public TerrainType[] biomeRegions;
     }

     [System.Serializable]
     public struct TerrainType {
          [Range(0, 1)]
          public float terrainHeight;
          public Color terrainColor;
     }
}
