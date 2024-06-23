using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class objectsMover : MonoBehaviour
{
    public GameObject[] movingObjects;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("movement", 20f,10f);
    }

    // Update is called once per frame
    void Update()
    {
    }
    void movement()
    {
        for (int i = 0; i < 4; i++)
        {
            float x = Random.Range(-66f, 25.1f);
            float y = Random.Range(326.5f, 515f);
            movingObjects[i].transform.DOMove(new Vector2(x,y), 20f, false);
        }
    }
}
