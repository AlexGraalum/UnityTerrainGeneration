using UnityEngine;

public class WorldChunk : MonoBehaviour{
     Vector2Int chunkOffset;
     public float[,] chunkHeightMap;

     GameObject meshObject;

     public WorldChunk() {}

     public void GenerateChunk(Vector2Int coord, World.WorldSettings worldSettings, float[,] worldHeightMap) {
          //Initialize
          Vector2 offset = (Vector2)coord * (worldSettings.chunkSize);
          chunkHeightMap = new float[worldSettings.chunkSize, worldSettings.chunkSize];

          //Get Chunk Offset
          chunkOffset = new Vector2Int();
          chunkOffset.x = (coord.x + (int)(worldSettings.worldChunkWidth/ 2.0f)) * worldSettings.chunkSize;
          chunkOffset.y = (coord.y + (int)(worldSettings.worldChunkHeight/ 2.0f)) * worldSettings.chunkSize;

          //Generate Chunk Heightmap
          for (int y = 0; y < worldSettings.chunkSize; y++) {
               for (int x = 0; x < worldSettings.chunkSize; x++) {
                    //chunkHeightMap[x, y] = worldHeightMap[x + chunkOffset.x, y + chunkOffset.y];
               }
          }

          //Create Plane Primitive
          meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
          meshObject.transform.parent = transform;

          //Position World Chunk
          transform.Translate(new Vector3(offset.x, 0, -coord.y * worldSettings.chunkSize));
          transform.localScale = new Vector3(worldSettings.chunkSize / 10.0f,
                                             1.0f,
                                             worldSettings.chunkSize / 10.0f);
     }
}
