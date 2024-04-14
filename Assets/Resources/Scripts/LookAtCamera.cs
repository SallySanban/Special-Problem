using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
