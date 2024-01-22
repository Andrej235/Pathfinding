using UnityEditor;
using UnityEngine;

public class CustomSceneView : SceneView
{
    [MenuItem("A/Custom tile map scene view")]
    static void ShowWindow()
    {
        var window = (CustomSceneView)GetWindow(typeof(CustomSceneView));
        window.titleContent = new GUIContent("MyScene");
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                Debug.Log("MouseDown");
                break;
        }
    }
}
