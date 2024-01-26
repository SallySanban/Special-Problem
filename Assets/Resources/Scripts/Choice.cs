using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Choice : Button
{
    int choiceIndex = 0;
    string baseChoiceId;

    public static int clickedChoice = 0;

    new void Start()
    {
        onClick.AddListener(OnClick);
    }

    public void Init(int choiceNumber, string choiceText, string id)
    {
        gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = choiceText;

        gameObject.SetActive(true);

        choiceIndex = choiceNumber;
        baseChoiceId = id;
    }

    public void OnClick()
    {
        ShowChoice.instance.ShowHideChoice(false);

        ActionManager.instance.insideChoice = true;
        clickedChoice = choiceIndex;

        string currentAction = baseChoiceId + "." + choiceIndex.ToString() + " - 1";
        ActionManager.instance.currentActionId = currentAction;

        ActionManager.instance.doAction(currentAction); //start with first choice id
    }
}
