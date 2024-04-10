using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingImage : MonoBehaviour
{
    [SerializeField] public Image image;

    private float blinkInterval = 1f;
    private float fadeDuration = 1f;

    private void Start()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
    }

    private void OnEnable()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

        StartCoroutine(Blink());
    }

    public IEnumerator Blink()
    {
        while (Fade.currentlyFading || Talk.instance.textTypewriter.isBuilding)
        {
            yield return null;
        }

        while (true)
        {
            yield return BlinkIn();
            yield return new WaitForSeconds(blinkInterval / 2f);
            yield return BlinkOut();
            yield return new WaitForSeconds(blinkInterval / 2f);
        }
    }

    private IEnumerator BlinkOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
    }

    private IEnumerator BlinkIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
    }
}
