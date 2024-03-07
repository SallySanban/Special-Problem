using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCamera;

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
    }

    private void Update()
    {
        if(mainCamera != null)
        {
            transform.LookAt(2 * transform.position - mainCamera.position);
        }
    }
}
