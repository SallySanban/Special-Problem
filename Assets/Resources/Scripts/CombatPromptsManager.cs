using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class CombatPromptsManager : NetworkBehaviour
{
    public static CombatPromptsManager instance;

    [SerializeField] GameObject combatPromptsScreen;
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] CombatChoice combatChoiceTemplate;
    [SerializeField] public GameObject combatChoiceGroup;
    [SerializeField] public TextMeshProUGUI waitText;

    public List<CombatChoice> listOfCombatChoices = new List<CombatChoice>();

    private CombatPrompt[] combatPrompts;

    public bool combatPromptOnScreen = false;

    private int[] playerDamage = { 5, 10, 15, 20, 25 };
    private int playerDamageIndex = 1;

    private void Awake()
    {
        instance = this;

        combatPrompts = JSONReader.getCombatPrompts(Resources.Load<TextAsset>("JSON/CombatPrompts"));
    }

    [ClientRpc]
    public void ShowPromptClientRpc(int id)
    {
        combatPromptsScreen.SetActive(true);
        waitText.gameObject.SetActive(false);

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

        bool allPlayersAnswered = false;

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
            foreach (GameObject p in players)
            {
                if (p.GetComponent<PlayerController>().pickedChoice.Value == 0)
                {
                    atLeastOnePlayerAnsweredWrong = true;
                }
            }

            if (atLeastOnePlayerAnsweredWrong)
            {
                BossController.instance.MaxBossHealthBar();
            }
            else
            {
                BulletController.damage = playerDamage[playerDamageIndex];
                playerDamageIndex++;
            }
        }

        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().UpdatePickedChoiceServerRpc(-1);
        }

        HidePromptClientRpc();
    }
}
