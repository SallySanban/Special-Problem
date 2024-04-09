using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHideCharacter : MonoBehaviour
{
    public static ShowHideCharacter instance;

    [SerializeField] public Image character;
    [SerializeField] public Image characterEmotion;

    private void Awake()
    {
        instance = this;

        character.gameObject.SetActive(false);
    }

    public void ShowHideCharacterMethod(string filename, string emotion, bool show)
    {
        character.sprite = Resources.Load<Sprite>("Art/" + filename);
        characterEmotion.sprite = Resources.Load<Sprite>("Art/" + emotion);

        if (show)
        {
            character.gameObject.SetActive(true);
            characterEmotion.gameObject.SetActive(true);

            character.color = new Color(1, 1, 1, 0);
            characterEmotion.color = new Color(1, 1, 1, 0);

            StartCoroutine(Fade.FadeMethod(character, true));
            StartCoroutine(Fade.FadeMethod(characterEmotion, true));
        }
        else
        {
            StartCoroutine(Fade.FadeMethod(character, false));
            StartCoroutine(Fade.FadeMethod(characterEmotion, false));
        }
    }
}
