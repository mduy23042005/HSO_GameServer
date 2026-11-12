using UnityEngine;

public class ObjectTypeController : MonoBehaviour
{
    [SerializeField] private ObjectType objectType = ObjectType.None;

    [SerializeField, HideInInspector] private int id;

    public ObjectType GetObjectType()
    {
        return objectType;
    }

    public int GetID()
    {
        return id;
    }

    public void SetID(int newID)
    {
        id = newID;
    }

    public bool HasId()
    {
        return objectType == ObjectType.DroppedItem;
    }
}