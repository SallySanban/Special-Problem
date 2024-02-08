using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance;

    Action[] actionList;

    public string currentActionId = "1";
    string mainActionId = "1";
    int choiceActionId = 2;
    public bool insideChoice = false;

    void Start()
    {
        instance = this;

        actionList = JSONReader.getActions(Resources.Load<TextAsset>("JSON/Trial"));

        doAction(mainActionId); //start with the 1st id
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && !AskName.instance.inputFieldOnScreen && !ShowChoice.instance.choicesOnScreen && !Talk.instance.textTypewriter.isBuilding && !Fade.currentlyFading)
        {
            if (ShowText.instance.textOnScreen)
            {
                ShowText.instance.ShowHideText(false);
            }

            doAction(getNextAction(currentActionId, choiceActionId));
        }
        else if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && !AskName.instance.inputFieldOnScreen && !ShowChoice.instance.choicesOnScreen && Talk.instance.textTypewriter.isBuilding)
        {
            Talk.instance.textTypewriter.ForceComplete();
        }
    }

    public void doAction(string choiceId)
    {
        Action actionToPlay = getActionFromId(choiceId);

        Debug.Log(actionToPlay.action);

        switch (actionToPlay.action)
        {
            case "Talk":
                Talk.instance.TalkMethod(actionToPlay.value, actionToPlay.name);
                break;
            case "Change Background":
                ChangeBackground.instance.ChangeBackgroundMethod(actionToPlay.value);
                break;
            case "Ask Name":
                AskName.instance.AskNameMethod();
                break;
            case "Show Text":
                ShowText.instance.ShowTextMethod(actionToPlay.value);
                break;
            case "Show Character":
                ShowHideCharacter.instance.ShowHideCharacterMethod(actionToPlay.value, true);
                doAction(getNextAction(currentActionId, choiceActionId));
                break;
            case "Hide Character":
                ShowHideCharacter.instance.ShowHideCharacterMethod(actionToPlay.value, false);
                doAction(getNextAction(currentActionId, choiceActionId));
                break;
            case "Character Shake":
                CharacterShake.instance.CharacterShakeMethod();
                break;
            case "Show Choice":
                ShowChoice.instance.ShowChoiceMethod(actionToPlay.choices, actionToPlay.id);
                break;
            case "Repeat Choice":
                RepeatChoice.instance.RepeatChoiceMethod(actionToPlay.value);
                break;
            case "Show Heart":
                ShowHeart.instance.ShowHeartMethod();
                doAction(getNextAction(currentActionId, choiceActionId));
                break;
            case "Change Scene":
                ChangeScene.instance.ChangeSceneMethod(actionToPlay.value);
                break;
        }
    }

    private Action getActionFromId(string choiceId)
    {
        foreach (Action action in actionList)
        {
            if (action.id == choiceId)
            {
                return action;
            }
        }

        return null;
    }

    private string getNextAction(string currentId, int currentChoiceId)
    {
        if(insideChoice)
        {
            string currentChoice = currentId.Substring(0, (currentId.Length - 1)) + currentChoiceId.ToString();

            //check where the choice ends
            string nextActionId = currentId.Substring(0, (currentId.Length - 1)) + (currentChoiceId + 1).ToString();
            Action nextAction = getActionFromId(nextActionId);

            if (nextAction == null)
            {
                insideChoice = false;

                choiceActionId = 2;
            }
            else
            {
                choiceActionId++;
            }

            currentActionId = currentChoice;

            return currentActionId;
        }
        else
        {
            mainActionId = (int.Parse(mainActionId) + 1).ToString();
            currentActionId = mainActionId;

            return mainActionId;
        }
    }
}
