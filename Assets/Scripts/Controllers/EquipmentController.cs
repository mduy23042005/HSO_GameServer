using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentController : MonoBehaviour, IUpdatable
{
    [Header("Danh sách các ô hành trang")]
    [SerializeField] private List<Image> equipmentSlots;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private List<Image> ring1Ring2Menu;

    private APIManager api;
    private List<Account_Equipment> equipment;

    private void Awake()
    {
        api = Object.FindFirstObjectByType<APIManager>();
    }
    
    public void OnUpdate() { }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _ = ReadDatabase();
    }

    // Đọc dữ liệu từ database và hiển thị vào Equipment Slots
    private async Task ReadDatabase()
    {
        int idAccount = LogInController.GetIDAccount() ?? 0;

        try
        {
            string urlItems = $"{api.GetApiUrl()}/api/account/{idAccount}/equipment?idAccount={idAccount}";
            HttpResponseMessage res = await api.GetHttpClient().GetAsync(urlItems);
            string json = await res.Content.ReadAsStringAsync();
            equipment = JsonConvert.DeserializeObject<List<Account_Equipment>>(json);

            for (int i = 0; i < equipmentSlots.Count && i < equipment.Count; i++)
            {
                if (equipment[i].IDItem0_1 == 0)
                {
                    equipmentSlots[i].sprite = inventoryController.GetItemDefault(i + 1);
                }
                else
                {
                    equipmentSlots[i].sprite = inventoryController.GetItem0(equipment[i].IDItem0_1);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Lỗi khi lấy equipment: " + ex.Message);
        }
    }
    public async void Ring1Ring2Info()
    {
        int idAccount = LogInController.GetIDAccount() ?? 0;

        try
        {
            string url = $"{api.GetApiUrl()}/api/account/{idAccount}/equipment?idAccount={idAccount}";
            HttpResponseMessage res = await api.GetHttpClient().GetAsync(url);
            string json = await res.Content.ReadAsStringAsync();
            equipment = JsonConvert.DeserializeObject<List<Account_Equipment>>(json);

            int[] ringIndex = { 6, 7 };

            for (int uiIndex = 0; uiIndex < 2; uiIndex++)
            {
                int equipSlot = ringIndex[uiIndex];
                int itemId = equipment[equipSlot].IDItem0_1;

                if (itemId == 0)
                {
                    ring1Ring2Menu[uiIndex].sprite = inventoryController.GetItemDefault(equipSlot + 1);
                }
                else
                {
                    ring1Ring2Menu[uiIndex].sprite = inventoryController.GetItem0(itemId);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Lỗi Ring1Ring2Info(): " + ex.Message);
        }
    }

    public void RefreshEquipmentUI()
    {
        _ = ReadDatabase(); // Gọi lại logic load item từ database
    }
    public List<Account_Equipment> GetEquipmentSlotsArray()
    {
        return equipment;
    }
}
