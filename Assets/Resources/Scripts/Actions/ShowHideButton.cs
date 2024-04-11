using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowHideButton : MonoBehaviour
{
    public static ShowHideButton instance;

    [SerializeField] Button playAgain;

    public bool buttonOnScreen = false;

    private void Awake()
    {
        instance = this;

        ShowHideButtonMethod(false);
    }

    private void Start()
    {
        playAgain.onClick.AddListener(PlayAgain);
    }

    public void ShowHideButtonMethod(bool show)
    {
        if (show)
        {
            if (!buttonOnScreen)
            {
                MainMenu.instance.gameStarted = false;

                playAgain.gameObject.SetActive(true);
                playAgain.image.color = new Color(1, 1, 1, 0);

                StartCoroutine(Fade.FadeMethod(playAgain.image, true));
                buttonOnScreen = true;
            }
        }
        else
        {
            playAgain.gameObject.SetActive(false);

            buttonOnScreen = false;
        }
    }

    private void PlayAgain()
    {
        EndScene.instance.endScene = false;

        AudioManager.instance.stopSoundLoop(1f);

        LobbyManager.SignOut();
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);

        SceneManager.LoadScene("VisualNovel");
    }
}
