using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LogOutClickEvent { }

public class LogOutController : MonoBehaviour, IPointerClickHandler
{
    public void StartLogOut()
    {
        ObserverManager.Notify(new LogOutClickEvent());

        SceneManager.LoadScene("Main");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        StartLogOut();
    }
}