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
        ActionManager.instance.doAction(currentAction);
        ActionManager.instance.currentActionId = currentAction;
    }
}
