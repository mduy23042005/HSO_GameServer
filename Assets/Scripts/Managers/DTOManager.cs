using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Stand = 0,
    Move = 1,
    Attack = 2,
    Injured = 3,
    Die = 4,
}
public enum Direction
{
    Front = 0,
    Back = 1,
    Left = 2,
    Right = 3,
}
public enum Category
{
    Stand = 0,
    Move = 1,
    Atk = 2,
    Injured = 3,
    Die = 4,
}
public enum Label
{
    StandFrontFrame0 = 4,
    StandFrontFrame1 = 5,
    StandBackFrame0 = 6,
    StandBackFrame1 = 7,
    StandLeftFrame0 = 8,
    StandLeftFrame1 = 9,
    StandRightFrame0 = 10,
    StandRightFrame1 = 11,

    MoveFrontFrame0 = 12,
    MoveFrontFrame1 = 13,
    MoveBackFrame0 = 14,
    MoveBackFrame1 = 15,
    MoveLeftFrame0 = 16,
    MoveLeftFrame1 = 17,
    MoveRightFrame0 = 18,
    MoveRightFrame1 = 19,

    AtkFrontFrame0 = 20,
    AtkFrontFrame1 = 21,
    AtkBackFrame0 = 22,
    AtkBackFrame1 = 23,
    AtkLeftFrame0 = 24,
    AtkLeftFrame1 = 25,
    AtkRightFrame0 = 26,
    AtkRightFrame1 = 27,

    InjuredFrontFrame0 = 28,
    InjuredFrontFrame1 = 29,
    InjuredBackFrame0 = 30,
    InjuredBackFrame1 = 31,
    InjuredLeftFrame0 = 32,
    InjuredLeftFrame1 = 33,
    InjuredRightFrame0 = 34,
    InjuredRightFrame1 = 35,

    DieFrame0 = 36
}
public class PositionData
{
    public float x;
    public float y;
    public float z;
}
public class RotationData
{
    public float x;
    public float y;
    public float z;
}
public class ScaleData
{
    public float x;
    public float y;
    public float z;
}
public class ColorData
{
    public float r;
    public float g;
    public float b;
    public float a;
}
public class PartBodyData
{
    public Category category;
    public Label label;
    public PositionData positionData;
    public RotationData rotationData;
    public ScaleData scaleData;
    public ColorData colorData;
}
public class PlayerStateData
{
    public State stateData;
    public Direction directionData;
    public List<PartBodyData> partBodyTransforms;
}
public class PlayerTransformData
{
    public PositionData positionData;
    public ScaleData scaleData;
}
public class PlayerData
{
    public int idAccount;
    public string nameChar;
    public int level;
    public int idSchool;
    public int hair;
    public int weapon;
    public int helmet;
    public int armor;
    public int legArmor;
    public int gloves;
    public int shoes;
    public int ring1;
    public int ring2;
    public int necklace;
    public int medal;
    public int cloak;
    public int wing;
    public int skinWing;
    public int mounts;
    public int pet;
    public int skin;
    public int maxHP;
    public int maxMP;
    public int hp;
    public int mp;
    public TileType currentTile;
}
public class PlayerSyncData
{
    public PlayerData playerData;
    public PlayerTransformData playerTransformData;
    public PlayerStateData playerStateData;
}

public class OtherPlayerSyncData
{
    public PlayerData otherPlayerData;
    public PlayerTransformData otherPlayerTransformData;
    public PlayerStateData otherPlayerStateData;
}
public class SyncOtherPlayersResultPacket
{
    public EnumCmdCode cmd;
    public List<OtherPlayerSyncData> otherPlayersData;
}

public class OtherPlayer
{
    public GameObject otherPlayerObject;
    public PlayerData otherPlayerData;
    public SyncSpriteController syncSpriteController;
}

public class EquipmentData
{
    public int id;
    public int idItem0_1;
    public string nameItem0_1;
    public int category;
    public string slotName;
    public List<Item0_Attribute> item0_Attributes;
    public List<Attribute> nameAttributes;
}
public class EquipmentResultPacket
{
    public EnumCmdCode cmd;
    public List<EquipmentData> equipmentData;
}

public class InventoryItem0Data
{
    public int id;
    public int idItem0;
    public string nameItem0;
    public string typeItem0;
    public int category;
    public int idSchool;
    public int level;
    public List<Item0_Attribute> item0_Attributes;
    public List<Attribute> nameAttributes;
}
public class InventoryItem1Data
{
    public int id;
    public int idItem1;
    public string nameItem1;
    public string typeItem1;
    public int level;
    public List<Item1_Attribute> item1_Attributes;
    public List<Attribute> nameAttributes;
}
public class InventoryItem2Data
{
    public int id;
    public int idItem2;
    public string nameItem2;
    public int level;
    public int quality;
}
public class InventoryItem3Data
{
    public int id;
    public int idItem3;
    public string nameItem3;
    public int level;
    public string details;
    public int quality;
}
public class InventoryItem4Data
{
    public int id;
    public int idItem4;
    public string nameItem4;
    public int level;
    public string details;
    public int quality;
}
public class InventoryResultPacket
{
    public EnumCmdCode cmd;
    public List<InventoryItem0Data> inventoryItem0Data;
    public List<InventoryItem1Data> inventoryItem1Data;
    public List<InventoryItem2Data> inventoryItem2Data;
    public List<InventoryItem3Data> inventoryItem3Data;
    public List<InventoryItem4Data> inventoryItem4Data;
}

public class LogInRequestPacket
{
    public EnumCmdCode cmd;
    public string username;
    public string password;
}
public class LogInResultPacket
{
    public EnumCmdCode cmd;
    public bool success;
    public int idAccount;
    public int idSchool;
    public string nameChar;
    public int hair;
    public int level;
    public int maxHP;
    public int maxMP;
    public int hp;
    public int mp;
    public string message;
}

public class Injured
{
    public GameObject attackObject;
    public int damage;
    public GameObject injuredObject;
    public Vector2 splatterBloodPosition;
}

public class Die
{
    public int id;
}