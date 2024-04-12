using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CombatChoice : Button
{
    private bool isCorrectChoice = false;

    new void Start()
    {
        onClick.AddListener(OnClick);
    }

    public void Init(string choiceText, bool correctChoice)
    {
        gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = choiceText;

        gameObject.SetActive(true);

        isCorrectChoice = correctChoice;
    }

    public void OnClick()
    {
        foreach(CombatChoice choice in CombatPromptsManager.instance.listOfCombatChoices)
        {
            Destroy(choice.gameObject);
        }

        CombatPromptsManager.instance.listOfCombatChoices.Clear();

        CombatPromptsManager.instance.waitText.gameObject.SetActive(true);

        if (isCorrectChoice)
        {
            if(NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out PlayerController playerController))
            {
                playerController.UpdatePickedChoiceServerRpc(1);
            }
        }
        else
        {
            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out PlayerController playerController))
            {
                playerController.UpdatePickedChoiceServerRpc(0);
            }
        }
    }
}
