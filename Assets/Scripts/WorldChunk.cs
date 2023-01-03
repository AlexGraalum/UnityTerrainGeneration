using UnityEngine;

[System.Serializable]
public class WorldChunk : MonoBehaviour{
     //Vector2Int chunkOffset;
     public float[,] chunkHeightMap;

     GameObject meshObject;

     public void GenerateChunk(Vector2Int coord, World.WorldSettings worldSettings, float[,] worldHeightMap) {
          //Initialize
          Vector2 offset = (Vector2)coord * (worldSettings.chunkSize);
          chunkHeightMap = new float[worldSettings.chunkSize, worldSettings.chunkSize];

          //Get Chunk Offset
          int chunkOffsetX = (coord.x + (int)(worldSettings.worldChunkWidth/ 2.0f)) * worldSettings.chunkSize;
          int chunkOffsetY = (coord.y + (int)(worldSettings.worldChunkHeight/ 2.0f)) * worldSettings.chunkSize;

          //Generate Chunk Heightmap
          for (int y = 0; y < worldSettings.chunkSize; y++) {
               for (int x = 0; x < worldSettings.chunkSize; x++) {
                    chunkHeightMap[x, y] = worldHeightMap[x + chunkOffsetX, y + chunkOffsetY];
               }
          }

          //Create Plane Primitive
          meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
          meshObject.transform.parent = transform;
          meshObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(Resources.Load("Materials/LandMassMaterial", typeof(Material)) as Material);
          meshObject.GetComponent<MeshRenderer>().sharedMaterial.shader = Resources.Load("Shaders/LandMassShader", typeof(Shader)) as Shader;
          meshObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MapTexture", Texture.TextureFromHeightMap(chunkHeightMap));

          //Position World Chunk
          transform.Translate(new Vector3(-offset.x, 0, -coord.y * worldSettings.chunkSize));
          transform.localScale = new Vector3(worldSettings.chunkSize / 10.0f,
                                             1.0f,
                                             worldSettings.chunkSize / 10.0f);
     }
}
