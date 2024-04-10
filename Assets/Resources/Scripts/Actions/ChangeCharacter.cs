using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCharacter : MonoBehaviour
{
    public static ChangeCharacter instance;
    Image currentCharacter;
    Image currentEmotion;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeCharacterMethod(string filename, string emotion)
    {
        currentCharacter = ShowHideCharacter.instance.character;

        StartCoroutine(SwitchCharacter(filename, emotion));
    }

    public void ChangeEmotion(string emotion)
    {
        currentEmotion = ShowHideCharacter.instance.characterEmotion;

        currentEmotion.color = new Color(1, 1, 1, 0);

        currentEmotion.sprite = Resources.Load<Sprite>("Art/" + emotion);

        StartCoroutine(Fade.FadeMethod(currentEmotion, true));
    }

    IEnumerator SwitchCharacter(string filename, string emotion)
    {
        yield return StartCoroutine(Fade.FadeMethod(currentCharacter, false));

        yield return new WaitForSeconds(0.5f);

        ShowHideCharacter.instance.ShowHideCharacterMethod(filename, emotion, true);
    }


}
