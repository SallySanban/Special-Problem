using System.Collections;
using UnityEngine;
using TMPro;
public class TextTypewriter
{
    private TextMeshProUGUI tmpro_ui;
    public TMP_Text tmpro => tmpro_ui;

    public string targetText { get; private set; } = "";
    public string preText { get; private set; } = "";

    public string fullTargetText => preText + targetText;

    public enum TextType
    {
        instant,
        typewriter
    }
    public TextType textType = TextType.typewriter;

    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 1;
    private float speedMultiplier = 1;

    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    public TextTypewriter(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    public void Stop()
    {
        if (!isBuilding)
        {
            return;
        }

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building()
    {
        while (Fade.currentlyFading)
        {
            yield return null;
        }

        Prepare();

        switch (textType)
        {
            case TextType.typewriter:
                yield return buildTypewriter();
                break;

        }

        OnComplete();
    }

    private void OnComplete()
    {
        buildProcess = null;
    }

    public void ForceComplete()
    {
        switch (textType)
        {
            case TextType.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
        }

        Stop();
        OnComplete();
    }

    private void Prepare()
    {
        switch (textType)
        {
            case TextType.typewriter:
                prepareTypewriter();
                break;
            case TextType.instant:
                prepareInstant();
                break;

        }
    }

    private void prepareInstant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void prepareTypewriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        if(preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

    private IEnumerator buildTypewriter()
    {
        while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += charactersPerCycle;

            yield return new WaitForSeconds(0.015f / speed);
        }
    }
}
