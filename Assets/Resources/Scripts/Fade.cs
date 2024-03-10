using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fade : MonoBehaviour
{
    public static bool currentlyFading = false;
    static float time = 0.01f;

    public static IEnumerator FadeMethod(Image image, bool fadeIn)
    {
        if (!fadeIn)
        {
            currentlyFading = true;

            for (float i = 1; i >= 0; i -= time)
            {
                image.color = new Color(1, 1, 1, i);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            image.gameObject.SetActive(false);

            currentlyFading = false;
        }
        else
        {
            currentlyFading = true;

            for (float i = 0; i <= 1; i += time)
            {
                image.color = new Color(1, 1, 1, i);
                yield return null;
            }

            currentlyFading = false;
        }
    }

    public static IEnumerator FadeMethod(TextMeshProUGUI text, bool fadeIn)
    {
        if (!fadeIn)
        {
            for (float i = 1; i >= 0; i -= time)
            {
                text.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        else
        {
            for (float i = 0; i <= 1; i += time)
            {
                text.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}
