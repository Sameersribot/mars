using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class powerRotater : MonoBehaviour
{
    private void Update()
    {
        rotateObjects();
    }
    private void rotateObjects()
    {
        transform.Rotate(new Vector3(0f, 0f, 2f));
    }

}
