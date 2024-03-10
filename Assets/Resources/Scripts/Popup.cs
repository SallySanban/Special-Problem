using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Popup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] new Camera camera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var wordIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, camera);

            if (wordIndex != -1)
            {
                Debug.Log("0");
            }
        }
    }
}
