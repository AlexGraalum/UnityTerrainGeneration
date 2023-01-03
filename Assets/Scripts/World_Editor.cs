using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class World_Editor : Editor{
     public override void OnInspectorGUI() {
          World world = (World)target;

          if (DrawDefaultInspector()) {
               if (world.autoUpdate) {
                    world.GenerateHeightMaps();
               }
          }

          if (!world.autoUpdate && GUILayout.Button("Update")) {
               world.GenerateHeightMaps();
          }

          if (GUILayout.Button("Randomize")) {
               world.Randomize();
          }
     }
}
