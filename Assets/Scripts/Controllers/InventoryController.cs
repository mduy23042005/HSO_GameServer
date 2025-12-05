using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    #region Sprite list variable
    [Header("Item Default")]
    [SerializeField] private List<Sprite> itemDefault;
    [Header("Item0")]
    [SerializeField] private List<Sprite> item0;
    [Header("Item1")]
    [SerializeField] private List<Sprite> item1;
    [Header("Item2")]
    [SerializeField] private List<Sprite> item2;
    [Header("Item3")]
    [SerializeField] private List<Sprite> item3;
    [Header("Item4")]
    [SerializeField] private List<Sprite> item4;
    #endregion

    [Header("Các ô hành trang")]
    [SerializeField] private List<Image> inventorySlots;
    [SerializeField] private EquipmentController equipmentController;
    [SerializeField] private List<SpriteController> spriteController;
    [SerializeField] private GameObject itemInfo;
    [SerializeField] private GameObject ring1Ring2Menu;

    private Dictionary<int, Sprite> itemDefaultMap;
    private Dictionary<int, Sprite> item0Map;
    private Dictionary<int, Sprite> item1Map;
    private Dictionary<int, Sprite> item2Map;
    private Dictionary<int, Sprite> item3Map;
    private Dictionary<int, Sprite> item4Map;

    private APIManager api;
    private List<Account_Item0> inventoryItem0;
    private int idItem0 = 0;
    private int inventorySlot = 0;
    private List<Account_Equipment> equipmentSlot;
    private TMP_Text infoText;
    private string ringSlot = null;

    private void Awake()
    {
        itemDefaultMap = ConvertListToMap(itemDefault);
        item0Map = ConvertListToMap(item0);
        item1Map = ConvertListToMap(item1);
        item2Map = ConvertListToMap(item2);
        item3Map = ConvertListToMap(item3);
        item4Map = ConvertListToMap(item4);
        api = Object.FindFirstObjectByType<APIManager>();
        infoText = itemInfo.GetComponent<TMP_Text>();
    }
    private void Start()
    {
        itemInfo.SetActive(false);
        ring1Ring2Menu.SetActive(false);
        _ = ReadDatabase();
    }
    private Dictionary<int, Sprite> ConvertListToMap(List<Sprite> list)
    {
        var map = new Dictionary<int, Sprite>();
        for (int i = 0; i < list.Count; i++)
        {
            map[i + 1] = list[i];
        }
        return map;
    }

    public Sprite GetItemDefault(int id)
    {
        return itemDefaultMap.TryGetValue(id, out var sprite) ? sprite : null;
    }
    public Sprite GetItem0(int id)
    {
        if (id < 0) return null;
        return item0Map.TryGetValue(id, out var sprite) ? sprite : null;
    }
    public Sprite GetItem1(int id)
    {
        if (id < 0) return null;
        return item1Map.TryGetValue(id, out var sprite) ? sprite : null;
    }
    public Sprite GetItem2(int id)
    {
        if (id < 0) return null;
        return item2Map.TryGetValue(id, out var sprite) ? sprite : null;
    }
    public Sprite GetItem3(int id)
    {
        if (id < 0) return null;
        return item3Map.TryGetValue(id, out var sprite) ? sprite : null;
    }
    public Sprite GetItem4(int id)
    {
        if (id < 0) return null;
        return item4Map.TryGetValue(id, out var sprite) ? sprite : null;
    }

    // Đọc dữ liệu từ database và hiển thị vào Inventory Slots
    private async Task ReadDatabase()
    {
        int idAccount = LogInController.GetIDAccount() ?? 0;

        try
        {
            string urlItems = $"{api.GetApiUrl()}/api/account/{idAccount}/inventory";
            HttpResponseMessage res = await api.GetHttpClient().GetAsync(urlItems);
            string json = await res.Content.ReadAsStringAsync();
            inventoryItem0 = JsonConvert.DeserializeObject<List<Account_Item0>>(json);

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (i < inventoryItem0.Count && inventoryItem0[i].IDItem0 != 0)
                {
                    inventorySlots[i].sprite = GetItem0(inventoryItem0[i].IDItem0);
                    inventorySlots[i].color = Color.white;
                }
                else
                {
                    inventorySlots[i].sprite = null;
                    inventorySlots[i].color = new Color(0, 0, 0, 0);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Lỗi khi lấy inventory: " + ex.Message);
        }
    }

    // Đọc attribute của item trong equipment
    private async Task ReadAttributeInEquipment(int idSlot)
    {
        int idAccount = LogInController.GetIDAccount() ?? 0;
        equipmentSlot = equipmentController.GetEquipmentSlotsArray();

        idItem0 = equipmentSlot[idSlot].IDItem0_1;

        if (idItem0 == 0)
        {
            Debug.Log("Slot này chưa có item");
            return;
        }

        try
        {
            string urlListAttributes = $"{api.GetApiUrl()}/api/account/{idAccount}/equipItem/{idItem0}/listAttributes?idItem={idItem0}";
            HttpResponseMessage res = await api.GetHttpClient().GetAsync(urlListAttributes);
            string json = await res.Content.ReadAsStringAsync();
            List<Item0_Attribute> listIDAttributeEquip = JsonConvert.DeserializeObject<List<Item0_Attribute>>(json);

            itemInfo.SetActive(true);
            infoText.text = "";

            foreach (var r in listIDAttributeEquip)
            {
                string urlNameAttribute = $"{api.GetApiUrl()}/api/account/{idAccount}/equipItem/{idItem0}/listAttributes/{r.IDAttribute}?idAttribute={r.IDAttribute}";
                res = await api.GetHttpClient().GetAsync(urlNameAttribute);
                json = await res.Content.ReadAsStringAsync();
                string attributeEquip = JsonConvert.DeserializeObject<string>(json);

                infoText.text += $"{r.Value} {attributeEquip} \n";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Lỗi khi lấy attribute: " + ex.Message);
            return;
        }
    }
    public void ClickReadAttributeInEquipment(int idSlot)
    {
        _ = ReadAttributeInEquipment(idSlot);
    }

    // Đọc attribute của item trong inventory
    private async Task ReadAttributeInInventory(int idSlot)
    {
        int idAccount = LogInController.GetIDAccount() ?? 0;

        int categoryItem = 0;

        inventorySlot = idSlot;

        if (idSlot >= 0 && idSlot < inventoryItem0.Count) // nếu id hợp lệ thì lấy idItem từ inventory
        {
            idItem0 = inventoryItem0[idSlot].IDItem0;
            categoryItem = inventoryItem0[idSlot].Category;
        }

        if (idItem0 == 0 || idSlot >= inventoryItem0.Count) // Slot trống hoặc ngoài phạm vi
        {
            Debug.Log("Slot này chưa có item");
            return;
        }

        if (idItem0 == 0)
        {
            Debug.Log("Slot này chưa có item");
            return;
        }

        try
        {
            string urlListAttributes = $"{api.GetApiUrl()}/api/account/{idAccount}/inventoryItem/{idItem0}/listAttributes?idItem={idItem0}";
            HttpResponseMessage res = await api.GetHttpClient().GetAsync(urlListAttributes);
            string json = await res.Content.ReadAsStringAsync();
            List<Item0_Attribute> listAttributes = JsonConvert.DeserializeObject<List<Item0_Attribute>>(json);

            itemInfo.SetActive(true);
            infoText.text = "";

            foreach (var attr in listAttributes)
            {
                string urlName = $"{api.GetApiUrl()}/api/account/{idAccount}/inventoryItem/{idItem0}/listAttributes/{attr.IDAttribute}?idAttribute={attr.IDAttribute}";
                res = await api.GetHttpClient().GetAsync(urlName);
                json = await res.Content.ReadAsStringAsync();
                string attributeName = JsonConvert.DeserializeObject<string>(json);

                infoText.text += $"{attr.Value} {attributeName}\n";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Lỗi khi lấy attribute: " + ex.Message);
            return;
        }
    }
    public void ClickReadAttributeInInventory(int idSlot)
    {
        inventorySlot = idSlot;
        _ = ReadAttributeInInventory(idSlot);
    }

    // Trang bị item từ inventory vào equipment
    private async Task EquipItem0()
    {
        int idAccount = LogInController.GetIDAccount() ?? 0;
        int idSchool = LogInController.GetIDSchool();

        if (idItem0 == 0)
        {
            Debug.Log("Chọn Item đi");
            return;
        }

        try
        {
            string urlItemToEquip = $"{api.GetApiUrl()}/api/account/{idAccount}/inventoryItem0/{idItem0}?idAccount={idAccount}&idItem0={idItem0}";
            HttpResponseMessage res = await api.GetHttpClient().GetAsync(urlItemToEquip);
            string json = await res.Content.ReadAsStringAsync();
            Account_Item0 itemToEquip = JsonConvert.DeserializeObject<Account_Item0>(json);

            string urlTypeItem = $"{api.GetApiUrl()}/api/account/{idAccount}/inventoryItem0/{itemToEquip.IDItem0}/typeItem0?idItem={itemToEquip.IDItem0}";
            res = await api.GetHttpClient().GetAsync(urlTypeItem);
            json = await res.Content.ReadAsStringAsync();
            Item0 typeItem0 = JsonConvert.DeserializeObject<Item0>(json);

            // Xử lý trang bị nhẫn
            // Nếu chưa có nhẫn nào được trang bị thì hiện menu chọn nhẫn 1 hoặc nhẫn 2
            if (typeItem0.TypeItem0.Equals("Ring") && ringSlot == null)
            {
                ring1Ring2Menu.SetActive(true);
                equipmentController.Ring1Ring2Info();
                return;
            }
            // Nếu đã chọn slot nhẫn để trang bị rồi thì gán vào slot đó
            if (typeItem0.TypeItem0.Equals("Ring") && ringSlot != null)
            {
                typeItem0.TypeItem0 = ringSlot;
            }

            string urlSlotData = $"{api.GetApiUrl()}/api/account/{idAccount}/equipItem/{itemToEquip.IDItem0}/slotData?idAccount={idAccount}&slotName={typeItem0.TypeItem0}";
            res = await api.GetHttpClient().GetAsync(urlSlotData);
            json = await res.Content.ReadAsStringAsync();
            Account_Equipment slotData = JsonConvert.DeserializeObject<Account_Equipment>(json);

            if (typeItem0.IDSchool != 0 && typeItem0.IDSchool != idSchool)
            {
                Debug.LogWarning("Item này không thuộc phái của nhân vật, không thể trang bị!");
                return;
            }

            await api.PostAsync($"api/account/inventory/return?idAccount={idAccount}&idItem0={slotData.IDItem0_1}&category={slotData.Category}&inventorySlot={inventorySlot}");

            await api.PostAsync($"api/account/equipment/equip?idAccount={idAccount}&slotName={typeItem0.TypeItem0}&idItem0={itemToEquip.IDItem0}&category={itemToEquip.Category}");

            equipmentController.RefreshEquipmentUI();
            _ = ReadDatabase();

            switch (idSchool)
            {
                case 1: // Chiến binh
                    spriteController[0].RefreshCharacterSprite();
                    spriteController[1].RefreshCharacterSprite();
                    break;
                case 2: // Sát thủ
                    spriteController[2].RefreshCharacterSprite();
                    spriteController[3].RefreshCharacterSprite();
                    break;
                case 3: // Pháp sư
                    spriteController[4].RefreshCharacterSprite();
                    spriteController[5].RefreshCharacterSprite();
                    break;
                case 4: // Xạ thủ
                    spriteController[6].RefreshCharacterSprite();
                    spriteController[7].RefreshCharacterSprite();
                    break;
            }

            ringSlot = null;
            infoText.text = "";
            itemInfo.SetActive(false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Lỗi khi trang bị item: " + ex.Message);
            return;
        }
    }
    public void ClickEquipItem0()
    {
        _ = EquipItem0();
    }
    public void ClickRing1Choice()
    {
        ring1Ring2Menu.SetActive(false);
        ringSlot = "Ring1";

        _ = EquipItem0();
    }
    public void ClickRing2Choice()
    {
        ring1Ring2Menu.SetActive(false);
        ringSlot = "Ring2";

        _ = EquipItem0();
    }
}
