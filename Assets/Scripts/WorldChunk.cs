using UnityEngine;

[System.Serializable]
public class WorldChunk : MonoBehaviour{
     public float[,] chunkHeightMap;

     ChunkMeshData chunkMeshData;
     GameObject meshObject;

     public Vector2 coord;

     public void GenerateChunk(Vector2Int coord, World.WorldSettings worldSettings, float[,] worldHeightMap) {
          //Initialize
          this.coord = coord;
          chunkHeightMap = new float[worldSettings.chunkSize + 1, worldSettings.chunkSize + 1];
          chunkMeshData = new ChunkMeshData(worldSettings.chunkSize + 1);

          //Get Chunk Offset
          int chunkOffsetX = (coord.x + (int)(worldSettings.worldChunkWidth/ 2.0f)) * worldSettings.chunkSize;
          int chunkOffsetY = (coord.y + (int)(worldSettings.worldChunkHeight/ 2.0f)) * worldSettings.chunkSize;

          //Get Mesh TopX and TopZ
          float topX = (worldSettings.chunkSize - 1) / -2.0f;
          float topZ = (worldSettings.chunkSize - 1) / 2.0f;

          //Generate Chunk Heightmap and Mesh
          for (int y = 0; y <= worldSettings.chunkSize; y++) {
               for (int x = 0; x <= worldSettings.chunkSize; x++) {

                    chunkHeightMap[x, y] = worldHeightMap[x + chunkOffsetX, y + chunkOffsetY];

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

          //Create ChunkMesh and Texture
          meshObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer));
          meshObject.GetComponent<MeshFilter>().sharedMesh = chunkMeshData.CreateMesh();
          meshObject.GetComponent<MeshCollider>().sharedMesh = meshObject.GetComponent<MeshFilter>().sharedMesh;
          meshObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(Resources.Load("Materials/LandMassMaterial", typeof(Material)) as Material);
          meshObject.GetComponent<MeshRenderer>().sharedMaterial.shader = Resources.Load("Shaders/LandMassShader", typeof(Shader)) as Shader;
          meshObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MapTexture", Texture.TextureFromHeightMap(chunkHeightMap));
          meshObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
          meshObject.transform.parent = transform;
          
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
