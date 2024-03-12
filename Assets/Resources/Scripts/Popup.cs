using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class Popup : MonoBehaviour
{
    public static Popup instance;

    [SerializeField] TextMeshProUGUI clickedText;
    [SerializeField] new Camera camera;
    [SerializeField] Image popupBackground;
    [SerializeField] Image popup;
    [SerializeField] Button okButton;
    [SerializeField] TextMeshProUGUI tipText;

    public bool popupOnScreen = false;
    float targetOpacity;

    private Dictionary<int, string> tips = new Dictionary<int, string>();

    private void Awake()
    {
        instance = this;

        popupOnScreen = false;

        popupBackground.gameObject.SetActive(false);

        okButton.onClick.AddListener(HidePopup);
    }

    private void Start()
    {
        tips = JSONReader.getTips(Resources.Load<TextAsset>("JSON/Tips"));

        targetOpacity = popupBackground.color.a;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var wordIndex = TMP_TextUtilities.FindIntersectingLink(clickedText, Input.mousePosition, camera);

            if (wordIndex != -1)
            {
                popupOnScreen = true;

                popupBackground.gameObject.SetActive(true);
                popup.gameObject.SetActive(true);

                popupBackground.color = new Color(0, 0, 0, 0);
                popup.color = new Color(1, 1, 1, 0);

                tipText.text = tips[int.Parse(clickedText.textInfo.linkInfo[wordIndex].GetLinkID())];

                StartCoroutine(Fade.FadeMethod(popupBackground, true, fadeOpacity: targetOpacity));
                StartCoroutine(Fade.FadeMethod(popup, true));
            }
        }
    }

    private void HidePopup()
    {
        StartCoroutine(Fade.FadeMethod(popup, false));
        StartCoroutine(Fade.FadeMethod(popupBackground, false, fadeOpacity: targetOpacity));

        popupOnScreen = false;
    }
}
