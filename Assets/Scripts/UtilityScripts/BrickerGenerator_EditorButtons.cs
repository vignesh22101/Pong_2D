using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(BrickGenerator))]
public class BrickerGenerator_EditorButtons : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BrickGenerator brickGenerator = (BrickGenerator)target;

        if (GUILayout.Button("Generate Bricks"))
        {
            brickGenerator.GenerateBricks();
        }
    }
}
#endif