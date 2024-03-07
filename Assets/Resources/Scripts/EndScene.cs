using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : MonoBehaviour
{
    public static EndScene instance;

    public bool endScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
