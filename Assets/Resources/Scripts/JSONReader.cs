using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public static Action[] actionList;
    public static Tip[] tipList;

    public static Action[] getActions(TextAsset jsonFile)
    {
        Actions jsonActions = JsonUtility.FromJson<Actions>(jsonFile.text);

        actionList = jsonActions.actions;

        return actionList;
    }

    public static Dictionary<int, string> getTips(TextAsset jsonFile)
    {
        Dictionary<int, string> tipList = new Dictionary<int, string>();

        Tips jsonTips = JsonUtility.FromJson<Tips>(jsonFile.text);

        for(int i = 0; i < jsonTips.tips.Length; i++)
        {
            tipList.Add(jsonTips.tips[i].id, jsonTips.tips[i].tip);
        }

        return tipList;
    }
}
