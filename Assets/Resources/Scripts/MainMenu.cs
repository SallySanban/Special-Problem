using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    [SerializeField] Image mainMenuScreen;
    [SerializeField] GameObject buttonsGroup;
    [SerializeField] Button newGame;
    [SerializeField] Button instructions;
    [SerializeField] GameObject instructionsScreen;
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
        newGame.onClick.AddListener(StartGame);
        instructions.onClick.AddListener(ShowInstructions);
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

    private void StartGame()
    {
        mainMenuScreen.gameObject.SetActive(false);
        buttonsGroup.gameObject.SetActive(false);

        ActionManager.instance.doAction(ActionManager.instance.mainActionId); //start with the 1st id

        gameStarted = true;
    }

    private void ShowInstructions()
    {

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
