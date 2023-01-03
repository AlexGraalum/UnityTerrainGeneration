using UnityEngine;

public class Texture : MonoBehaviour{
     public static Texture2D TextureFromHeightMap(float[,] heightMap) {
          int chunkSize = heightMap.GetLength(0);

          Color[] colorMap = new Color[chunkSize * chunkSize];
          for(int y = 0; y < chunkSize; y++) {
               for(int x = 0; x < chunkSize; x++) {
                    colorMap[y * chunkSize + x] = Color.Lerp(Color.black, Color.white, heightMap[x,y]);
               }
          }
          return TextureFromColorMap(colorMap, chunkSize);
     }

     public static Texture2D TextureFromColorMap(Color[] colorMap, int chunkSize) {
          Texture2D texture = new Texture2D(chunkSize, chunkSize);

          texture.filterMode = FilterMode.Point;
          texture.wrapMode = TextureWrapMode.Clamp;

          texture.SetPixels(colorMap);
          texture.Apply();

          return texture;
     }
}
