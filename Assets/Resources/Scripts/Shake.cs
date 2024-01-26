using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shake : MonoBehaviour
{
    public static IEnumerator ShakeMethod(Image image)
    {
        for (int i = 0; i < 25; i++)
        {
            image.transform.localPosition += new Vector3(20f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            image.transform.localPosition -= new Vector3(20f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
