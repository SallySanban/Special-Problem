using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fade : MonoBehaviour
{
    public static bool currentlyFading = false;

    public static IEnumerator FadeMethod(Image image, bool fadeIn, float fadeOpacity = 1, float time = 0.02f)
    {
        float originalR = image.color.r;
        float originalG = image.color.g;
        float originalB = image.color.b;

        if (!fadeIn)
        {
            currentlyFading = true;

            for (float i = fadeOpacity; i >= 0; i -= time)
            {
                image.color = new Color(originalR, originalG, originalB, i);
                yield return null;
            }

            yield return new WaitForSeconds(0.01f);
            image.gameObject.SetActive(false);

            currentlyFading = false;
        }
        else
        {
            currentlyFading = true;

            for (float i = 0; i <= fadeOpacity; i += time)
            {
                image.color = new Color(originalR, originalG, originalB, i);
                yield return null;
            }

            currentlyFading = false;
        }
    }

    public static IEnumerator FadeMethod(TextMeshProUGUI text, bool fadeIn, float fadeOpacity = 1, float time = 0.02f)
    {
        if (!fadeIn)
        {
            for (float i = fadeOpacity; i >= 0; i -= time)
            {
                text.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        else
        {
            for (float i = 0; i <= fadeOpacity; i += time)
            {
                text.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}
