using UnityEngine;

public class Texture : MonoBehaviour{
     public static Texture2D TextureFromHeightMap(float[,] heightMap) {
          int height = heightMap.GetLength(0);
          int width = heightMap.GetLength(1);

          Color[] colorMap = new Color[width * height];
          for(int y = 0; y < height; y++) {
               for(int x = 0; x < width; x++) {
                    colorMap[y * height + x] = Color.Lerp(Color.black, Color.white, heightMap[x,y]);
               }
          }
          return TextureFromColorMap(colorMap, width, height);
     }

     public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height) {
          Texture2D texture = new Texture2D(width, height);

          texture.filterMode = FilterMode.Point;
          texture.wrapMode = TextureWrapMode.Clamp;

          texture.SetPixels(colorMap);
          texture.Apply();

          return texture;
     }
}
