using UnityEngine;
using UnityEngine.U2D.Animation;

public enum Item0Type
{
    Weapon,
    Helmet,
    Armor,
    LegArmor,
    // Item0 không đổi sprite library
    Gloves,
    Shoes,
    Ring,
    Necklace,
}
public enum Item1Type
{
    Medal,
    Cloak,
    Wing,
    SkinWing,
    Skin,
    Mounts,
    Pet
}

[System.Serializable]
public class ListItem0
{
    public int idItem0;
    public Sprite iconItem0;
    public Item0Type typeItem0;

    public WeaponLibraries weapon;
    public HelmetLibraries helmet;
    public ArmorLibraries armor;
    public LegArmorLibraries legArmor;
}
[System.Serializable]
public class ListItem1
{
    public int idItem1;
    public Sprite iconItem1;
    public Item1Type typeItem1;

    public WingLibraries wing;
}
[System.Serializable]
public class ListItem3
{
    public int idItem3;
    public Sprite iconItem3;
}

//Item0
[System.Serializable]
public class WeaponLibraries
{
    public SpriteLibraryAsset weaponBackLibraries;
    public SpriteLibraryAsset weaponFrontLibraries;
}

[System.Serializable]
public class HelmetLibraries
{
    public SpriteLibraryAsset helmetLibrariesAsset;
    public bool isHiddenHair = false;
}

[System.Serializable]
public class ArmorLibraries
{
    public SpriteLibraryAsset armorLibrariesAsset;
}

[System.Serializable]
public class HeadLibraries
{
    public SpriteLibraryAsset headLibrariesAsset;
}

[System.Serializable]
public class LegArmorLibraries
{
    public SpriteLibraryAsset legArmorLibrariesAsset;
}

[System.Serializable]
public class HairLibraries
{
    public SpriteLibraryAsset hairLibrariesAsset;
}

//Item1
[System.Serializable]
public class MedalLibraries
{
    public SpriteLibraryAsset medalLibrariesAsset;
}

[System.Serializable]
public class WingLibraries
{
    public SpriteLibraryAsset wingLibrariesAsset;
}
