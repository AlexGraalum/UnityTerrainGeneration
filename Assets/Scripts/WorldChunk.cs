using UnityEngine;

[System.Serializable]
public class WorldChunk : MonoBehaviour{
     public float[,] chunkHeightMap;
     public float[,] chunkBiomeHeightMap;
     
     public Color[] chunkBiomeMap;
     public Color[] chunkColorMap;

     ChunkMeshData chunkMeshData;
     GameObject meshObject;

     public Vector2 coord;

     public void GenerateChunk(Vector2Int coord, World.WorldSettings worldSettings, float[,] worldHeightMap, float[,] worldBiomeHeightMap, BiomeData biomeData) {
          //Initialize
          this.coord = coord;

          chunkHeightMap = new float[worldSettings.chunkSize + 1, worldSettings.chunkSize + 1];
          chunkBiomeHeightMap = new float[worldSettings.chunkSize + 1, worldSettings.chunkSize + 1];

          chunkBiomeMap = new Color[(worldSettings.chunkSize + 1) * (worldSettings.chunkSize + 1)];
          chunkColorMap = new Color[(worldSettings.chunkSize + 1) * (worldSettings.chunkSize + 1)];

          chunkMeshData = new ChunkMeshData(worldSettings.chunkSize + 1);

          //Get Chunk Offset
          int chunkOffsetX = (coord.x + (int)(worldSettings.worldChunkWidth/ 2.0f)) * worldSettings.chunkSize;
          int chunkOffsetY = (coord.y + (int)(worldSettings.worldChunkHeight/ 2.0f)) * worldSettings.chunkSize;

          //Get Mesh TopX and TopZ
          float topX = (worldSettings.chunkSize - 1) / -2.0f;
          float topZ = (worldSettings.chunkSize - 1) / 2.0f;

          //Generate Chunk Heightmap and Mesh
          for (int y = 0; y <= worldSettings.chunkSize; y++) {
               int offsetY = y + chunkOffsetY;

               for (int x = 0; x <= worldSettings.chunkSize; x++) {
                    int offsetX = x + chunkOffsetX;

                    chunkHeightMap[x, y] = worldHeightMap[offsetX, offsetY];
                    chunkBiomeHeightMap[x, y] = worldBiomeHeightMap[offsetX, offsetY];

                    int i = (y * (worldSettings.chunkSize + 1)) + x;

                    float h = worldSettings.heightMapCurve.Evaluate(chunkHeightMap[x, y]) * worldSettings.heightMapMultiplier;

                    chunkMeshData.vertices[i] = new Vector3(topX + x, h, topZ - y);
                    chunkMeshData.uvs[i] = new Vector2(x / (float)(worldSettings.chunkSize + 1), y / (float)(worldSettings.chunkSize + 1));

                    if (x < worldSettings.chunkSize && y < worldSettings.chunkSize) {
                         chunkMeshData.AddTriangle(i, i + worldSettings.chunkSize + 2, i + worldSettings.chunkSize + 1);
                         chunkMeshData.AddTriangle(i + worldSettings.chunkSize + 2, i, i + 1);
                    }
               }
          }

          chunkBiomeMap = biomeData.GenerateBiomeMap(worldSettings.chunkSize + 1, worldSettings.chunkSize + 1, chunkBiomeHeightMap);
          GenerateColorMap(worldSettings, biomeData);

          //Create ChunkMesh and Texture
          meshObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer));

          meshObject.GetComponent<MeshFilter>().sharedMesh = chunkMeshData.CreateMesh();
          meshObject.GetComponent<MeshCollider>().sharedMesh = meshObject.GetComponent<MeshFilter>().sharedMesh;

          meshObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(Resources.Load("Materials/LandMassMaterial", typeof(Material)) as Material);
          meshObject.GetComponent<MeshRenderer>().sharedMaterial.shader = Resources.Load("Shaders/LandMassShader", typeof(Shader)) as Shader;

          //Draw Selected Texture
          switch (worldSettings.drawMode) {
               case World.DrawMode.HeightMap:
                    meshObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MapTexture", Texture.TextureFromHeightMap(chunkHeightMap));
                    break;
               case World.DrawMode.BiomeHeightMap:
                    meshObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MapTexture", Texture.TextureFromHeightMap(chunkBiomeHeightMap));
                    break;
               case World.DrawMode.BiomeMap:
                    meshObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MapTexture", Texture.TextureFromColorMap(chunkBiomeMap, worldSettings.chunkSize + 1));
                    break;
               case World.DrawMode.ColorMap:
                    meshObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MapTexture", Texture.TextureFromColorMap(chunkColorMap, worldSettings.chunkSize + 1));
                    break;
          }

          //Set Transform and Parent
          meshObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
          meshObject.transform.parent = transform;
     }

     private void GenerateColorMap(World.WorldSettings worldSettings, BiomeData biomeData) {
          for (int y = 0; y <= worldSettings.chunkSize; y++) {
               for(int x = 0; x <= worldSettings.chunkSize; x++) {
                    float currHeight = chunkHeightMap[x, y];

                    if (currHeight == 0.0f) {
                         chunkColorMap[y * (worldSettings.chunkSize + 1) + x] = Color.blue;
                    } else {
                         foreach(BiomeData.Biome b in biomeData.biomes) {
                              if(chunkBiomeMap[y * (worldSettings.chunkSize + 1) + x] == b.biomeColor) {
                                   foreach(BiomeData.TerrainType r in b.biomeRegions) {
                                        if(r.terrainHeight <= currHeight) {
                                             chunkColorMap[y * (worldSettings.chunkSize + 1) + x] = r.terrainColor;
                                        } else {
                                             break;
                                        }
                                   }
                              }
                         }
                    }
               }
          }
     }

     [System.Serializable]
     public class ChunkMeshData {
          public Vector3[] vertices;
          public int[] triangles;
          public Vector2[] uvs;

          int index;

          public ChunkMeshData(int chunkSize) {
               vertices = new Vector3[chunkSize * chunkSize];
               triangles = new int[(chunkSize - 0) * (chunkSize - 0) * 6];
               uvs = new Vector2[chunkSize * chunkSize];
          }

          public void AddTriangle(int a, int b, int c) {
               triangles[index] = a;
               triangles[index + 1] = b;
               triangles[index + 2] = c;

               index += 3;
          }

          public Mesh CreateMesh() {
               Mesh mesh = new Mesh();

               mesh.vertices = vertices;
               mesh.triangles = triangles;
               mesh.uv = uvs;
               mesh.RecalculateNormals();

               return mesh;
          }
     }
}
