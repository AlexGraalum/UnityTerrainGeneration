using UnityEngine;

[System.Serializable]
public class World : MonoBehaviour {
     public enum DrawMode {HeightMap, BiomeHeightMap, BiomeMap, ColorMap};

     public WorldSettings worldSettings;
     [Space]
     public HeightMapData landMassHeightMapData;
     [Range(0, 1)]
     public float landMassMultiplier;
     public HeightMapData detailHeightMapData;
     [Space]
     public FalloffMap falloffMap;
     public bool useFalloffMap;
     float[,] worldHeightMap;
     [Space]
     public HeightMapData biomeHeightMapData;

     public bool autoUpdate;

     private void OnValidate() {
          worldSettings.Validate();
          transform.Find("BiomeData").GetComponent<BiomeData>().Validate();
          if (useFalloffMap) falloffMap.Validate();
     }

     public void GenerateHeightMaps() {
          //Place World at Center
          transform.position = Vector3.zero;

          //Initialize World Chunks Child Object
          if (!transform.Find("WorldChunks")) {
               GameObject child = new GameObject("WorldChunks");
               child.transform.parent = transform;
          }
          //Destroy All Chunk Child Objects
          while (transform.Find("WorldChunks").childCount > 0) {
               DestroyImmediate(transform.Find("WorldChunks").GetChild(0).gameObject);
          }

          //Calculate World Width and Height
          int worldWidth = (worldSettings.worldChunkWidth * (worldSettings.chunkSize + 1));
          int worldHeight = (worldSettings.worldChunkHeight * (worldSettings.chunkSize + 1));

          //Generate Landmass and Detail Heightmaps
          landMassHeightMapData.GenerateHeightMap(worldWidth, worldHeight, worldSettings.seed, landMassHeightMapData.offset);
          detailHeightMapData.GenerateHeightMap(worldWidth, worldHeight, worldSettings.seed, detailHeightMapData.offset);

          //Generate Falloff Map
          if(useFalloffMap) falloffMap.GenerateMap(worldWidth, worldHeight);

          //Generate Biome Heightmap
          biomeHeightMapData.GenerateHeightMap(worldWidth, worldHeight, worldSettings.seed, biomeHeightMapData.offset);

          //Compile Landmass and Detail into World Heightmap
          worldHeightMap = new float[worldWidth, worldHeight];
          for(int y = 0; y < worldHeight; y++) {
               for(int x = 0; x < worldWidth; x++) {
                    worldHeightMap[x, y] = landMassHeightMapData.heightMap[x, y];

                    if (useFalloffMap) {
                         worldHeightMap[x, y] *= falloffMap.heightMap[x, y];
                         biomeHeightMapData.heightMap[x, y] *= falloffMap.heightMap[x, y];
                    }

                    worldHeightMap[x, y] -= detailHeightMapData.heightMap[x, y];
               }
          }

          //transform.Find("BiomeData").GetComponent<BiomeData>().GenerateBiomeMap(worldWidth, worldHeight, biomeHeightMapData.heightMap);

          //Generate All Chunks
          int halfWidth = (worldSettings.worldChunkWidth - 1) / 2;
          int halfHeight = (worldSettings.worldChunkHeight - 1) / 2;
          //int halfChunk = worldSettings.chunkSize / 2;
          for(int y = -halfHeight; y <= halfHeight; y++) {
               for(int x = -halfWidth; x <= halfWidth; x++) {
                    //Create New Chunk
                    GameObject chunk = new GameObject("Chunk[" + x + "," + y + "]");
                    //Set Parent and Position
                    chunk.transform.parent = transform.Find("WorldChunks");
                    chunk.transform.position = new Vector3(x * worldSettings.chunkSize, 0.0f, -y * worldSettings.chunkSize);
                    //Add Chunk Component
                    chunk.AddComponent<WorldChunk>().GenerateChunk(new Vector2Int(x, y), worldSettings, worldHeightMap, biomeHeightMapData.heightMap, transform.Find("BiomeData").GetComponent<BiomeData>());
               }
          }
     }

     public void Randomize() {
          worldSettings.Randomize();
          landMassHeightMapData.Randomize();
          detailHeightMapData.Randomize();
     }

     [System.Serializable]
     public struct WorldSettings {
          public DrawMode drawMode;
          [Range(1, 32)]
          public int worldChunkWidth;
          [Range(1, 32)]
          public int worldChunkHeight;
          [Space]
          public int chunkSize;
          [Space]
          public int seed;
          public Vector2Int offset;
          [Space]
          [Range(1, 100)]
          public float heightMapMultiplier;
          public AnimationCurve heightMapCurve;

          public void Randomize() {
               seed = Random.Range(int.MinValue, int.MaxValue);
               offset = new Vector2Int(Random.Range(-100000, 100000), Random.Range(-100000, 100000));
          }

          public void Validate() {
               if (worldChunkWidth < 1) worldChunkWidth = 1;
               if (worldChunkWidth % 2 == 0) worldChunkWidth += 1;

               if (worldChunkHeight < 1) worldChunkHeight = 1;
               if (worldChunkHeight % 2 == 0) worldChunkHeight += 1;

               if (chunkSize < 1) chunkSize = 1;
          }
     }
}
