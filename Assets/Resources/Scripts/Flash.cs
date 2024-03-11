using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{
    public static Flash instance;

    [SerializeField] Image flashScreen;

    private void Awake()
    {
        instance = this;

        flashScreen.gameObject.SetActive(false);
    }

    public IEnumerator FlashMethod()
    {
        flashScreen.gameObject.SetActive(true);
        flashScreen.color = new Color(1, 1, 1, 0);

        float flashDuration = 0.5f;
        float flashTime = flashDuration / 2;

        for (float i = 0; i <= flashTime; i += Time.deltaTime)
        {
            Color newColor = flashScreen.color;
            newColor.a = Mathf.Lerp(0, 1, i / flashTime);
            flashScreen.color = newColor;
            yield return null;
        }

        for (float i = 0; i <= flashTime; i += Time.deltaTime)
        {
            Color newColor = flashScreen.color;
            newColor.a = Mathf.Lerp(1, 0, i / flashTime);
            flashScreen.color = newColor;
            yield return null;
        }

        yield return new WaitForSeconds(flashDuration);

        flashScreen.gameObject.SetActive(false);
    }
}
