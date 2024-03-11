using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance;

    Action[] actionList;

    public string currentActionId;
    public string mainActionId;
    public int choiceActionId;
    public bool insideChoice;

    void Start()
    {
        instance = this;

        currentActionId = "1";
        mainActionId = "1";
        choiceActionId = 1;
        insideChoice = false;

        if (EndScene.instance.endScene)
        {
            actionList = JSONReader.getActions(Resources.Load<TextAsset>("JSON/End"));
        }
        else
        {
            actionList = JSONReader.getActions(Resources.Load<TextAsset>("JSON/Trial"));
        }

        doAction(mainActionId); //start with the 1st id
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && !AskName.instance.inputFieldOnScreen && !ShowChoice.instance.choicesOnScreen && !Talk.instance.textTypewriter.isBuilding && !Fade.currentlyFading && !Flash.instance.currentlyFlashing)
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
                ShowHideCharacter.instance.ShowHideCharacterMethod(actionToPlay.value, actionToPlay.emotion, true);
                break;
            case "Hide Character":
                ShowHideCharacter.instance.ShowHideCharacterMethod(actionToPlay.value, actionToPlay.emotion, false);
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
                break;
            case "Change Scene":
                ChangeScene.instance.ChangeSceneMethod(actionToPlay.value);
                break;
            case "Play Sound Loop":
                AudioManager.instance.playSoundLoop(actionToPlay.value);
                break;
            case "Play Sound Effect":
                AudioManager.instance.playSoundEffect(actionToPlay.value);
                break;
            case "Stop Sound":
                AudioManager.instance.stopSoundLoop();
                break;
            case "Hide Textbox":
                Talk.instance.ShowHideTextbox(false);
                Talk.instance.ShowHideNamebox(false);
                break;
            case "Change Character":
                ChangeCharacter.instance.ChangeCharacterMethod(actionToPlay.value, actionToPlay.emotion);
                break;
            case "Change Emotion":
                ChangeCharacter.instance.ChangeEmotion(actionToPlay.emotion);
                break;
            case "Show Prop":
                ShowHideProp.instance.ShowHidePropMethod(actionToPlay.value, true);
                break;
            case "Hide Prop":
                ShowHideProp.instance.ShowHidePropMethod(actionToPlay.value, false);
                break;
            case "Flash Screen":
                StartCoroutine(Flash.instance.FlashMethod());
                break;
        }

        if(actionToPlay.playNextAction == true)
        {
            doAction(getNextAction(currentActionId, choiceActionId));
        }
    }

    public Action getActionFromId(string choiceId)
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
            //check where the choice ends
            string nextActionId = currentId.Substring(0, (currentId.Length - 1)) + (currentChoiceId + 1).ToString();
            Action nextAction = getActionFromId(nextActionId);

            if (nextAction == null)
            {
                insideChoice = false;

                choiceActionId = 1;
            }
            else
            {
                choiceActionId++;

                currentActionId = nextActionId;
            }
        }
        
        if(!insideChoice)
        {
            mainActionId = (int.Parse(mainActionId) + 1).ToString();
            currentActionId = mainActionId;
        }

        return currentActionId;
    }
}
