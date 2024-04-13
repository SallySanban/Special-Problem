using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CombatPromptsManager : NetworkBehaviour
{
    public static CombatPromptsManager instance;

    [SerializeField] GameObject combatPromptsScreen;
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] CombatChoice combatChoiceTemplate;
    [SerializeField] public GameObject combatChoiceGroup;
    [SerializeField] public TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI counter;

    public List<CombatChoice> listOfCombatChoices = new List<CombatChoice>();

    private CombatPrompt[] combatPrompts;

    public bool combatPromptOnScreen = false;

    private int numPlayers = 0;

    public bool allPlayersAnswered;

    private void Awake()
    {
        instance = this;

        combatPrompts = JSONReader.getCombatPrompts(Resources.Load<TextAsset>("JSON/CombatPrompts"));
    }

    [ClientRpc]
    public void ShowPromptClientRpc(int id)
    {
        combatPromptsScreen.SetActive(true);
        promptText.gameObject.SetActive(true);
        resultText.gameObject.SetActive(false);
        counter.gameObject.SetActive(false);

        combatPromptOnScreen = true;

        CombatPrompt prompt = getPromptFromId(id);

        promptText.text = prompt.prompt;

        foreach (string choiceText in prompt.choices)
        {
            CombatChoice c = Instantiate(combatChoiceTemplate, combatChoiceGroup.transform);

            if (choiceText.StartsWith("*"))
            {
                c.Init(choiceText.Substring(1), true);
            }
            else
            {
                c.Init(choiceText, false);
            }

            listOfCombatChoices.Add(c);
        }

        if (combatPromptOnScreen && IsServer)
        {
            StartCoroutine(WaitForAnswers());
        }
    }

    [ClientRpc]
    private void HidePromptClientRpc()
    {
        combatPromptsScreen.SetActive(false);
        combatPromptOnScreen = false;
    }

    [ClientRpc]
    private void ShowResultTextClientRpc(int numWrongPlayers)
    {
        if (numWrongPlayers == 0) //correct answer
        {
            resultText.text = "Congratulations!\nEveryone chose correctly!";
        }
        else
        {
            if(numPlayers == 1)
            {
                resultText.text = "Oh no!\nYou chose incorrectly!";
            }
            else if(numPlayers > 1 && numWrongPlayers == 1)
            {
                resultText.text = "Oh no!\n" + numWrongPlayers.ToString() + " person chose incorrectly!";
            }
            else
            {
                resultText.text = "Oh no!\n" + numWrongPlayers.ToString() + " people chose incorrectly!";
            }
        }

        StartCoroutine(WaitWhileReading());
    }

    public CombatPrompt getPromptFromId(int id)
    {
        foreach (CombatPrompt prompt in combatPrompts)
        {
            if (prompt.id == id)
            {
                return prompt;
            }
        }

        return null;
    }

    private IEnumerator WaitForAnswers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        numPlayers = players.Length;

        allPlayersAnswered = false;

        while (!allPlayersAnswered)
        {
            bool atLeastOnePlayerNotAnswered = false;

            foreach (GameObject p in players)
            {
                if(p.GetComponent<PlayerController>().pickedChoice.Value == -1)
                {
                    atLeastOnePlayerNotAnswered = true;
                }
            }

            if (atLeastOnePlayerNotAnswered)
            {
                yield return null;
            }
            else
            {
                allPlayersAnswered = true;
            }
        }

        bool atLeastOnePlayerAnsweredWrong = false;

        if (allPlayersAnswered)
        {
            int wrongPlayersCount = 0;

            foreach (GameObject p in players)
            {
                if (p.GetComponent<PlayerController>().pickedChoice.Value == 0)
                {
                    wrongPlayersCount++;
                    atLeastOnePlayerAnsweredWrong = true;
                }
            }

            if (atLeastOnePlayerAnsweredWrong)
            {
                BossController.instance.PlaySoundEffectClientRpc("Wrong");
                ShowResultTextClientRpc(wrongPlayersCount);

                BossController.instance.MaxBossHealthBar();
            }
            else
            {
                BossController.instance.PlaySoundEffectClientRpc("Correct");
                ShowResultTextClientRpc(wrongPlayersCount);

                foreach (GameObject p in players)
                {
                    PlayerController playerController = p.GetComponent<PlayerController>();

                    playerController.playerPowerIndex++;
                    playerController.GetPower(playerController.playerPowerIndex);
                }
            }
        }

        yield return new WaitForSeconds(3f);

        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().UpdatePickedChoiceServerRpc(-1);
        }

        HidePromptClientRpc();
    }

    private IEnumerator WaitWhileReading()
    {
        counter.gameObject.SetActive(true);
        promptText.gameObject.SetActive(false);

        int seconds = 3;

        while (seconds > 0)
        {
            counter.text = seconds.ToString();

            seconds--;

            yield return new WaitForSeconds(1f);
        }

        counter.gameObject.SetActive(false);

        resultText.text = "Waiting for other players...";
    }
}
