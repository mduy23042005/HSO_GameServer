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

        property.isExpanded = EditorGUI.Foldout(foldRect, property.isExpanded, $"IDItem0: {idProp.intValue} TypeItem0: {((Item0Type)type.enumValueIndex).ToString()}", true);

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
            type.enumValueIndex == (int)Item0Type.Weapon ? "weapon" :
            type.enumValueIndex == (int)Item0Type.Helmet ? "helmet" :
            type.enumValueIndex == (int)Item0Type.Armor ? "armor" :
            type.enumValueIndex == (int)Item0Type.LegArmor ? "legArmor" :
            type.enumValueIndex == (int)Item0Type.Gloves ? null :
            type.enumValueIndex == (int)Item0Type.Shoes ? null :
            type.enumValueIndex == (int)Item0Type.Ring ? null :
            type.enumValueIndex == (int)Item0Type.Necklace ? null : null;

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
            type.enumValueIndex == (int)Item0Type.Weapon ? "weapon" :
            type.enumValueIndex == (int)Item0Type.Helmet ? "helmet" :
            type.enumValueIndex == (int)Item0Type.Armor ? "armor" :
            type.enumValueIndex == (int)Item0Type.LegArmor ? "legArmor" : null;

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

public class ItemController : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<ListItem0> listItem0Sprite;
    [SerializeField] private List<ListItem3> listItem3Sprite;
    [SerializeField] private List<Sprite> listDefaultItemSprite;
    [SerializeField] private List<HairLibraries> listMaleHairLibraries;
    [SerializeField] private List<HairLibraries> listFemaleHairLibraries;

    private Dictionary<int, ListItem0> mapItem0Sprite;
    private Dictionary<int, HairLibraries> mapMaleHairLibraries;
    private Dictionary<int, HairLibraries> mapFemaleHairLibraries;
    public static ItemController Instance;

    private void Awake()
    {
        mapItem0Sprite = ConvertListToMap(listItem0Sprite);
        mapMaleHairLibraries = ConvertListToMap(listMaleHairLibraries);
        mapFemaleHairLibraries = ConvertListToMap(listFemaleHairLibraries);
        Instance = this;
    }
    private Dictionary<int, ListItem0> ConvertListToMap(List<ListItem0> list)
    {
        Dictionary<int, ListItem0> map = new Dictionary<int, ListItem0>();
        if (list == null)
            return map;

        foreach (var item in list)
        {
            if (!map.ContainsKey(item.idItem0)) // tránh duplicate key
            {
                map.Add(item.idItem0, item);
            }
        }

        return map;
    }
    private Dictionary<int, HairLibraries> ConvertListToMap(List<HairLibraries> list)
    {
        Dictionary<int, HairLibraries> map = new Dictionary<int, HairLibraries>();
        if (list == null)
            return map;

        for (int i = 0; i < list.Count; i++)
        {
            map.Add(i, list[i]);
        }

        return map;
    }

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
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public ListItem0 GetItem0(int id)
    {
        if (mapItem0Sprite != null && mapItem0Sprite.TryGetValue(id, out var item))
        {
            return item;
        }
        return null;
    }
    public HairLibraries GetMaleHairLibrary(int id)
    {
        if (mapMaleHairLibraries != null && mapMaleHairLibraries.TryGetValue(id, out var item))
        {
            return item;
        }
        return null;
    }
    public HairLibraries GetFemaleHairLibrary(int id)
    {
        if (mapFemaleHairLibraries != null && mapFemaleHairLibraries.TryGetValue(id, out var item))
        {
            return item;
        }
        return null;
    }
    public Sprite GetDefaultItemSprite(int id) 
    {
        return listDefaultItemSprite[id];
    }
}
