using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static string playerName;
    public static int choicesIncorrect = 3; //everyone starts with 3 (total number of choices)

    public void readInputField(string input)
    {
        playerName = input;

        Debug.Log(playerName);
    }
}
