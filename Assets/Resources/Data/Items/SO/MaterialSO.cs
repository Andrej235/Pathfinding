using Assets.Code.Items.Interfaces;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Material", menuName = "Item/Material")]
public class MaterialSO : ItemSO, IMaterial
{

}

[CustomEditor(typeof(ItemSO), editorForChildClasses: true)]
public class ItemSOEditor : Editor
{

}
