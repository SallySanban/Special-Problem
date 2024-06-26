using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    [SerializeField] Image mainMenuScreen;
    [SerializeField] GameObject buttonsGroup;
    [SerializeField] Button newGame;
    [SerializeField] GameObject disclaimerPopup;
    [SerializeField] Button disclaimerContinue;
    [SerializeField] Button instructions;
    [SerializeField] GameObject instructionsScreen;
    [SerializeField] Button exitInstructions;
    [SerializeField] Button credits;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] Button exitCredits;
    [SerializeField] Button exit;

    public bool gameStarted = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        newGame.onClick.AddListener(ShowDisclaimer);
        disclaimerContinue.onClick.AddListener(StartGame);
        instructions.onClick.AddListener(ShowInstructions);
        exitInstructions.onClick.AddListener(HideInstructions);
        credits.onClick.AddListener(ShowCredits);
        exitCredits.onClick.AddListener(HideCredits);
        exit.onClick.AddListener(ExitGame);

        if (EndScene.instance.endScene)
        {
            gameStarted = true;

            mainMenuScreen.gameObject.SetActive(false);
            buttonsGroup.gameObject.SetActive(false);

            ActionManager.instance.doAction(ActionManager.instance.mainActionId);
        }
        else
        {
            gameStarted = false;
        }
    }

    private void ShowDisclaimer()
    {
        disclaimerPopup.SetActive(true);
    }

    private void StartGame()
    {
        mainMenuScreen.gameObject.SetActive(false);
        buttonsGroup.gameObject.SetActive(false);
        disclaimerPopup.SetActive(false);

        ActionManager.instance.doAction(ActionManager.instance.mainActionId); //start with the 1st id

        gameStarted = true;
    }

    private void ShowInstructions()
    {
        instructionsScreen.gameObject.SetActive(true);
    }

    public void HideInstructions()
    {
        instructionsScreen.SetActive(false);
    }

    private void ShowCredits()
    {
        creditsScreen.SetActive(true);
    }

    public void HideCredits()
    {
        creditsScreen.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
