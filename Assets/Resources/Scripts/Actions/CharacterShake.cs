using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterShake : MonoBehaviour
{
    public static CharacterShake instance;

    [SerializeField] Image character;

    private void Awake()
    {
        instance = this;
    }

    public void CharacterShakeMethod()
    {
        StartCoroutine(Shake.ShakeMethod(character));
    }
}
