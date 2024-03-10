using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action
{
    public string id = "";
    public string action = "";
    public string name = "";
    public string value = "";
    public List<string> choices;
    public bool playNextAction;
}
