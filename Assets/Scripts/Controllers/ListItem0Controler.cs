using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ListItem0))]
public class ListItem0Drawer : PropertyDrawer
{
    const float PADDING_SMALL = 3f;
    const float PADDING_LARGE = 6f;
    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, property);

        // Foldout — lưu trạng thái vào property.isExpanded để giữ trạng thái khi re-draw
        Rect foldRect = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty idProp = property.FindPropertyRelative("idItem0");
        SerializedProperty iconProp = property.FindPropertyRelative("iconItem0");
        SerializedProperty type = property.FindPropertyRelative("typeItem0");

        property.isExpanded = EditorGUI.Foldout(foldRect, property.isExpanded, $"IDItem0: {idProp.intValue} TypeItem0: {((ItemType)type.enumValueIndex).ToString()}", true);

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        // Move rect down to start drawing fields
        pos.y += EditorGUIUtility.singleLineHeight + PADDING_SMALL;

        EditorGUI.indentLevel++;

        // idItem0
        Rect r = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(r, idProp);
        pos.y += EditorGUIUtility.singleLineHeight + PADDING_SMALL;

        // iconItem0
        r = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(r, iconProp);
        pos.y += EditorGUIUtility.singleLineHeight + PADDING_SMALL;

        // typeItem0 (enum)
        r = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(r, type);
        pos.y += EditorGUIUtility.singleLineHeight + PADDING_LARGE;

        // chọn tên field con theo enum
        string target =
            type.enumValueIndex == (int)ItemType.Weapon ? "weapon" :
            type.enumValueIndex == (int)ItemType.Helmet ? "helmet" :
            type.enumValueIndex == (int)ItemType.Armor ? "armor" :
            type.enumValueIndex == (int)ItemType.LegArmor ? "legArmor" :
            type.enumValueIndex == (int)ItemType.Hair ? "hair" :
            type.enumValueIndex == (int)ItemType.Gloves ? null :
            type.enumValueIndex == (int)ItemType.Shoes ? null :
            type.enumValueIndex == (int)ItemType.Ring ? null :
            type.enumValueIndex == (int)ItemType.Necklace ? null : null;

        SerializedProperty targetProp = property.FindPropertyRelative(target);

        if (targetProp != null)
        {
            // tính height cho trường con (nhiều dòng có thể)
            float targetHeight = EditorGUI.GetPropertyHeight(targetProp, true);
            r = new Rect(pos.x, pos.y, pos.width, targetHeight);
            EditorGUI.PropertyField(r, targetProp, true);
            pos.y += targetHeight + PADDING_SMALL;
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // luôn tính căn bản cho foldout
        float height = EditorGUIUtility.singleLineHeight + 4f;

        if (!property.isExpanded)
            return height;

        // cộng các field cố định
        height += (EditorGUIUtility.singleLineHeight + 3f) * 3; // id, icon, type (mỗi cái + spacing)

        // spacing extra trước target
        height += 6f;

        // cộng height của field target (nhiều dòng)
        // cần đọc type để biết tên target
        SerializedProperty type = property.FindPropertyRelative("typeItem0");
        string target =
            type.enumValueIndex == (int)ItemType.Weapon ? "weapon" :
            type.enumValueIndex == (int)ItemType.Helmet ? "helmet" :
            type.enumValueIndex == (int)ItemType.Armor ? "armor" :
            type.enumValueIndex == (int)ItemType.LegArmor ? "legArmor" : 
            type.enumValueIndex == (int)ItemType.Hair ? "hair" : null;

        SerializedProperty targetProp = property.FindPropertyRelative(target);
        if (targetProp != null)
        {
            height += EditorGUI.GetPropertyHeight(targetProp, true) + 3f;
        }
        else
        {
            height += EditorGUIUtility.singleLineHeight + 3f;
        }

        // một chút cushion
        height += 2f;
        return height;
    }
}
#endif

public class ListItem0Controler : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<ListItem0> listItem0Sprite;

    private void OnEnable()
    {
        GameManager.Instance.Register(this);
        RegisterDontDestroyOnLoad();
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Unregister(this);
        }
    }
    public void OnUpdate() { }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad() { }

    public List<ListItem0> GetListItem0()
    {
        return listItem0Sprite;
    }
}
