using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "Scriptable objects/Prop")]
public class PropSO : ScriptableObject
{
    public enum PropPlacementType
    {
        Center = 1,
        NextToTopWall = 2,
        NextToRightWall = 4,
        NextToBottomWall = 8,
        NextToLeftWall = 16,
        Corner = 32,
    }

    public GameObject propPrefab;
    public PropPlacementType placementType;

    public bool placeAsAGroup;
    public int groupMinCount;
    public int groupMaxCount;
}

[CustomEditor(typeof(PropSO))]
public class PropSOEditor : Editor
{
    private PropSO prop;

    private void Awake()
    {
        prop = (PropSO)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        prop.propPrefab = (GameObject)EditorGUILayout.ObjectField("", prop.propPrefab, typeof(GameObject), false);
        prop.placementType = (PropSO.PropPlacementType)EditorGUILayout.EnumFlagsField("Placement type: ", prop.placementType);

        prop.placeAsAGroup = EditorGUILayout.Toggle("Place as a group: ", prop.placeAsAGroup);
        if (prop.placeAsAGroup)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Minimum props in a single group: ", GUILayout.Width(225));
            prop.groupMinCount = EditorGUILayout.IntField(prop.groupMinCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum props in a single group: ", GUILayout.Width(225));
            prop.groupMaxCount = EditorGUILayout.IntField(prop.groupMaxCount);
            GUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(prop);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //SaveChanges();
    }
}