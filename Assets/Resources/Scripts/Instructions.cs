using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour
{
    [SerializeField] Image instruction;
    [SerializeField] Button rightButton;
    [SerializeField] Button leftButton;

    private int currentPage;
    private int firstPage = 1;
    private int maxPages = 8;

    private void Start()
    {
        rightButton.onClick.AddListener(RightPage);
        leftButton.onClick.AddListener(LeftPage);
    }

    private void OnEnable()
    {
        instruction.sprite = Resources.Load<Sprite>("Art/Instructions 1");
        currentPage = firstPage;

        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);
    }

    private void RightPage()
    {
        int nextPage = currentPage + 1;
        instruction.sprite = Resources.Load<Sprite>("Art/Instructions " + nextPage);

        if (nextPage == maxPages)
        {
            rightButton.gameObject.SetActive(false);
        }
        else
        {
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(true);
        }

        currentPage = nextPage;
    }

    private void LeftPage()
    {
        int previousPage = currentPage - 1;
        instruction.sprite = Resources.Load<Sprite>("Art/Instructions " + previousPage);

        if (previousPage == firstPage)
        {
            leftButton.gameObject.SetActive(false);
        }
        else
        {
            rightButton.gameObject.SetActive(true);
            leftButton.gameObject.SetActive(true);
        }

        currentPage = previousPage;
    }
}
