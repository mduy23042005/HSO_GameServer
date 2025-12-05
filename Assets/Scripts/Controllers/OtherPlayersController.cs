using UnityEngine;

class OtherPlayersController : MonoBehaviour
{
    [SerializeField] private GameObject otherPlayers;
    private GameObject chienBinh;
    private GameObject satThu;
    private GameObject phapSu;
    //private GameObject XaThu;

    private void Awake()
    {
        chienBinh = otherPlayers.transform.Find("ChienBinh").gameObject;
        satThu = otherPlayers.transform.Find("SatThu").gameObject;
        phapSu = otherPlayers.transform.Find("PhapSu").gameObject;
        //xaThu = otherPlayers.transform.Find("XaThu").gameObject;

        SyncModels otherPlayerData = SyncManager.Instance.GetPlayerData();
        switch (otherPlayerData.school)
        {
            case 1:
                chienBinh.SetActive(true);
                Destroy(satThu);
                Destroy(phapSu);
                //Destroy(xaThu);
                break;
            case 2:
                Destroy(chienBinh);
                satThu.SetActive(true);
                Destroy(phapSu);
                //Destroy(xaThu);
                break;
            case 3:
                Destroy(chienBinh);
                Destroy(satThu);
                phapSu.SetActive(true);
                //Destroy(xaThu);
                break;
        }
    }
}