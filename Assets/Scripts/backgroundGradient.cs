using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class backgroundGradient : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(true);
    }
    private void Update()
    {
        transform.Rotate(0, 0, 2f);
    }
}
