using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
