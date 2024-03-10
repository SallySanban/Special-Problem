using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatChoice : MonoBehaviour
{
    public static RepeatChoice instance;

    private void Awake()
    {
        instance = this;
    }

    public void RepeatChoiceMethod(string idToRepeat)
    {
        string currentAction = idToRepeat;

        ActionManager.instance.currentActionId = currentAction;
        ActionManager.instance.choiceActionId = 1;

        ActionManager.instance.doAction(currentAction);
    }
}
