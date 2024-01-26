using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public static Action[] actionList;

    public static Action[] getActions(TextAsset jsonFile)
    {
        Actions jsonActions = JsonUtility.FromJson<Actions>(jsonFile.text);

        actionList = jsonActions.actions;

        return actionList;
    }
}
