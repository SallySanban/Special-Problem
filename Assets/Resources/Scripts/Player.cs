using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static string playerName = "Player";
    public static int choicesIncorrect = 5; //everyone starts with 5 (total number of choices)

    public void readInputField(string input)
    {
        playerName = input;

        Debug.Log(playerName);
    }
}
