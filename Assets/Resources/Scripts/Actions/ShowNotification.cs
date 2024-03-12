using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowNotification : MonoBehaviour
{
    public static ShowNotification instance;

    [SerializeField] Image notification;
    [SerializeField] TextMeshProUGUI notificationText;

    private void Awake()
    {
        instance = this;

        notification.gameObject.SetActive(false);
    }

    public IEnumerator ShowNotificationMethod(string type)
    {
        notification.gameObject.SetActive(true);
        notification.color = new Color(1, 1, 1, 0);

        notificationText.text = "";

        string textColor = "";

        if(type == "blue")
        {
            textColor = "#0b109c";
        }
        else
        {
            textColor = "#cf1006";
        }

        yield return new WaitForSeconds(0.5f);

        notificationText.text = "<b>Mental Health Tip!</b>" + "\n" + "Click on the <color=" + textColor + "><b>" + type + "</b></color> line!";

        StartCoroutine(Fade.FadeMethod(notification, true));

        yield return new WaitForSeconds(3f);

        StartCoroutine(Fade.FadeMethod(notification, false));
    }
}
