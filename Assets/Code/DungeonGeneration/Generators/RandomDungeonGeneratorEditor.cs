using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    private AbstractDungeonGenerator generator;
    private void Awake() => generator = target as AbstractDungeonGenerator;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(5);

        if (GUILayout.Button("Create dungeon"))
            generator.GenerateDungeon();
    }
}
