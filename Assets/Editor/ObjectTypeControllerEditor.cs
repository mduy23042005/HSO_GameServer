using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectTypeController))]
public class ObjectTypeControllerEditor : Editor
{
    private SerializedProperty objectTypeProp;
    private SerializedProperty idProp;

    private void OnEnable()
    {
        objectTypeProp = serializedObject.FindProperty("objectType");
        idProp = serializedObject.FindProperty("id");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(objectTypeProp);

        // Chỉ hiện id khi chọn DroppedItem
        if ((ObjectType)objectTypeProp.enumValueIndex == ObjectType.DroppedItem)
        {
            EditorGUILayout.PropertyField(idProp, new GUIContent("Unique ID"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}