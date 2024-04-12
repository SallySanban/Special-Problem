using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI textButton;

    private string originalText;

    private void Start()
    {
        originalText = textButton.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textButton.text = "<u>" + originalText + "</u>";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textButton.text = originalText;
    }
}
