using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talk : MonoBehaviour
{
    public static Talk instance;

    public TextTypewriter textTypewriter;

    private bool textBoxOnScreen;
    private bool nameBoxOnScreen;

    [SerializeField] GameObject nameBox;
    [SerializeField] GameObject textBox;

    Image textBoxImage;
    Image nameBoxImage;

    private void Awake()
    {
        instance = this;

        textBoxImage = textBox.GetComponent<Image>();
        nameBoxImage = nameBox.GetComponent<Image>();

        ShowHideTextbox(false);
        ShowHideNamebox(false);

        textBoxOnScreen = false;
        nameBoxOnScreen = false;

        textTypewriter = new TextTypewriter(textBox.GetComponentInChildren<TMPro.TextMeshProUGUI>());
    }

    public void ShowHideTextbox(bool show)
    {
        if (show)
        {
            if (!textBoxOnScreen)
            {
                textBox.SetActive(true);
                textBoxImage.color = new Color(1, 1, 1, 0);

                StartCoroutine(Fade.FadeMethod(textBoxImage, true));
                textBoxOnScreen = true;
            }
        }
        else
        {
            textBox.SetActive(false);

            textBoxOnScreen = false;
        }
    }

    public void ShowHideNamebox(bool show)
    {
        if (show)
        {
            if (!nameBoxOnScreen)
            {
                nameBox.SetActive(true);
                nameBoxImage.color = new Color(1, 1, 1, 0);

                StartCoroutine(Fade.FadeMethod(nameBox.GetComponent<Image>(), true));
                nameBoxOnScreen = true;
            }
        }
        else
        {
            nameBox.SetActive(false);

            nameBoxOnScreen = false;
        }
    }

    public void TalkMethod(string dialogue, string name)
    {

        if (!(name == ""))
        {
            ShowHideNamebox(true);

            nameBox.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;
        }
        else
        {
            nameBox.SetActive(false);
        }

        textTypewriter.textType = TextTypewriter.TextType.typewriter;

        string line;

        if (dialogue.Contains("[Name]"))
        {
            line = dialogue.Replace("[Name]", Player.playerName);
        }
        else
        {
            line = dialogue;
        }

        ShowHideTextbox(true);
        textTypewriter.Build(line);
    }
}
