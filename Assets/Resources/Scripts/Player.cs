using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static string playerName;
    public static int choicesIncorrect = 5; //everyone starts with 5 (total number of choices)

    public void readInputField(string input)
    {
        if(input == "")
        {
            playerName = "Player";
        }
        else
        {
            playerName = input;
        }

        Debug.Log(playerName);
    }
}
