using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class AskName : MonoBehaviour
{
    public static AskName instance;

    [SerializeField] Image inputField;
    [SerializeField] TMP_InputField inputText;
    [SerializeField] TextMeshProUGUI placeholderText;

    public bool inputFieldOnScreen;

    private void Awake()
    {
        instance = this;

        ShowHideInputField(false);
    }

    public void ShowHideInputField(bool show)
    {
        if (show)
        {
            if(!inputFieldOnScreen)
            {
                inputField.gameObject.SetActive(true);
                inputField.color = new Color(1, 1, 1, 0);

                StartCoroutine(Fade.FadeMethod(inputField, true));
                inputFieldOnScreen = true;
            }
        }
        else
        {
            inputField.gameObject.SetActive(false);

            inputFieldOnScreen = false;
        }
    }

    public void ReadInputField(string input)
    {
        if(input == "")
        {
            inputText.text = "";
            placeholderText.text = "Name cannot be blank";

            return;
        }
        
        if(!Regex.IsMatch(input, "^[a-zA-Z0-9]*$"))
        {
            inputText.text = "";
            placeholderText.text = "Alphanumeric only";

            return;
        }

        if (input.Length > 30)
        {
            inputText.text = "";
            placeholderText.text = "30 characters only";

            return;
        }

        PlayerData.playerName = input;

        Debug.Log(PlayerData.playerName);

        ShowHideInputField(false);

        ActionManager.instance.doAction(ActionManager.instance.getNextAction(ActionManager.instance.currentActionId, ActionManager.instance.choiceActionId));
    }

    public void AskNameMethod()
    {
        ShowHideInputField(true);
    }
}
