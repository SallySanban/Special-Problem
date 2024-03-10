using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Choice : Button
{
    int choiceIndex = 0;
    string baseChoiceId;
    string currentAction;

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

        ActionManager.instance.choiceActionId = 1;

        currentAction = baseChoiceId + "." + choiceIndex.ToString() + " - 1";

        if (ActionManager.instance.getActionFromId(currentAction) != null)
        {
            ActionManager.instance.insideChoice = true;

            ActionManager.instance.currentActionId = currentAction;
        }
        else
        {
            currentAction = (int.Parse(baseChoiceId) + 1).ToString();

            ActionManager.instance.mainActionId = currentAction;
        }

        ActionManager.instance.doAction(currentAction);
    }
}
