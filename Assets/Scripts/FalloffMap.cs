using UnityEngine;

[System.Serializable]
public struct FalloffMap{
     [Range(0,1)]
     public float innerRadius;
     [Range(0,1)]
     public float outerRadius;
     //public float scale;
     [Space]
     public Vector2 offset;
     public AnimationCurve falloffFallOff;
     public float[,] heightMap;

     public void GenerateMap(int width, int height) {
          heightMap = new float[width, height];

          float minSize = width < height ? width / 2.0f : height / 2.0f;
          for (int y = 0; y < height; y++) {
               for(int x = 0; x < width; x++) {

                    Vector2 center = new Vector2(width/2.0f, height/2.0f);
                    float d = Vector2.Distance(new Vector2(x,y), center + offset);
                    if (d > (outerRadius * minSize)) {
                         heightMap[x, y] = 0.0f;
                    } else if (d < (innerRadius * minSize)) {
                         heightMap[x, y] = 1.0f;
                    } else {
                         float val = (d - (outerRadius * minSize)) / ((innerRadius - outerRadius) * minSize);
                         heightMap[x, y] = falloffFallOff.Evaluate(val);
                    }
               }
          }
     }

     public void Validate() {
          if (innerRadius > outerRadius || outerRadius < innerRadius) innerRadius = outerRadius;
     }
}
