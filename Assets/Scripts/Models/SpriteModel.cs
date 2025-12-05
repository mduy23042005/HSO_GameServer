using UnityEngine.U2D.Animation;

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