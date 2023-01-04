using UnityEngine;

[System.Serializable]
public struct FalloffMap{
     [Range(0,1)]
     public float innerRadius;
     [Range(0,1)]
     public float outerRadius;
     public float scale;
     [Space]
     public Vector2 offset;
     public AnimationCurve falloffFallOff;
     public float[,] heightMap;

     public void GenerateMap(int width, int height) {
          heightMap = new float[width, height];
          for (int y = 0; y < height; y++) {
               for(int x = 0; x < width; x++) {
                    float d = Vector2.Distance(new Vector2(x,y), offset);
                    if (d > (outerRadius * scale)) {
                         heightMap[x, y] = 0.0f;
                    } else if (d < (innerRadius * scale)) {
                         heightMap[x, y] = 1.0f;
                    } else {
                         float val = (d - (outerRadius * scale)) / ((innerRadius - outerRadius) * scale);
                         heightMap[x, y] = falloffFallOff.Evaluate(val);
                         //heightMap[x, y] = Mathf.Lerp(outerRadius * scale, innerRadius * scale, d / scale);
                         //heightMap[x, y] = (d - (outerRadius * scale)) / ((innerRadius * scale) - d / scale);
                         //heightMap[x, y] = 0.5f;
                    }
               }
          }
     }

     public void Validate() {
          if (innerRadius > outerRadius || outerRadius < innerRadius) innerRadius = outerRadius;
     }
}
