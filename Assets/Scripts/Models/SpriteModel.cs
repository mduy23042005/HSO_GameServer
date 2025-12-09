using UnityEngine;
using UnityEngine.U2D.Animation;

public enum ItemType
{
    Weapon,
    Helmet,
    Armor,
    LegArmor,
    Hair,
    // Item0 không đổi sprite library
    Gloves,
    Shoes,
    Ring,
    Necklace,
}

[System.Serializable]
public class ListItem0
{
    public int idItem0;
    public Sprite iconItem0;
    public ItemType typeItem0;

    public WeaponLibraries weapon;
    public HelmetLibraries helmet;
    public ArmorLibraries armor;
    public LegArmorLibraries legArmor;
    public HairLibraries hair;
}

[System.Serializable]
public class WeaponLibraries
{
    public int idWeapon;
    public SpriteLibraryAsset weaponBackLibraries;
    public SpriteLibraryAsset weaponFrontLibraries;
}

[System.Serializable]
public class HelmetLibraries
{
    public int idHelmet;
    public SpriteLibraryAsset helmetLibrariesAsset;
    public bool isHiddenHair = false;
}

[System.Serializable]
public class ArmorLibraries
{
    public int idArmor;
    public SpriteLibraryAsset armorLibrariesAsset;
}

[System.Serializable]
public class HeadLibraries
{
    public int idHead;
    public SpriteLibraryAsset headLibrariesAsset;
}

[System.Serializable]
public class LegArmorLibraries
{
    public int idLegArmor;
    public SpriteLibraryAsset legArmorLibrariesAsset;
}

[System.Serializable]
public class HairLibraries
{
    public SpriteLibraryAsset hairLibrariesAsset;
}