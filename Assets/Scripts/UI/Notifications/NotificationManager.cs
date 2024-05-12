using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textOutput;
    [SerializeField] private GameObject notificationUI;
    //private readonly WaitForSeconds popupDuration = new (1);

    private Coroutine curCoroutine;

    //private bool isActive;

    public void Notify(string text, float showTime = 3)
    {
/*        if (isActive)
            return;*/
        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }
        
        textOutput.SetText(text);
        notificationUI.SetActive(true);
        //isActive = true;
        StartCoroutine(DeactivateNotification(showTime));
    }

    private IEnumerator DeactivateNotification(float duration)
    {
        yield return new WaitForSeconds(duration);
        notificationUI.SetActive(false);
        //isActive = false;
    }
}