using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHideCharacter : MonoBehaviour
{
    public static ShowHideCharacter instance;

    [SerializeField] Image character;

    bool characterOnScreen;

    private void Awake()
    {
        instance = this;

        character.gameObject.SetActive(false);
    }

    public void ShowHideCharacterMethod(string filename, bool show)
    {
        character.sprite = Resources.Load<Sprite>("Art/" + filename);

        if (show)
        {
            character.gameObject.SetActive(true);
            character.color = new Color(1, 1, 1, 0);

            StartCoroutine(Fade.FadeMethod(character, true));
        }
        else
        {
            StartCoroutine(Fade.FadeMethod(character, false));
        }
    }
}
