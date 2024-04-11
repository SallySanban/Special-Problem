using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowChoice : MonoBehaviour
{
    public static ShowChoice instance;

    [SerializeField] Choice choiceTemplate;
    [SerializeField] GameObject choicesGroup;

    public bool choicesOnScreen;

    private List<Choice> listOfChoices = new List<Choice>();

    private void Awake()
    {
        instance = this;
    }

    public void ShowHideChoice(bool show)
    {
        if (show)
        {
            if (!choicesOnScreen)
            {
                choicesGroup.SetActive(true);

                choicesOnScreen = true;
            }
        }
        else
        {
            choicesGroup.SetActive(false);

            choicesOnScreen = false;

            foreach(Choice choice in listOfChoices)
            {
                Destroy(choice.gameObject);
            }

            listOfChoices.Clear();
        }
    }
    public void ShowChoiceMethod(List<string> choices, string id)
    {
        ShowHideChoice(true);

        int choiceIndex = 1;

        foreach (string choiceText in choices)
        {
            Choice c = Instantiate(choiceTemplate, choicesGroup.transform);

            c.Init(choiceIndex, choiceText, id);

            listOfChoices.Add(c);

            choiceIndex++;
        }
    }
}
