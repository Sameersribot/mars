using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class objectsMover : MonoBehaviour
{
    public GameObject[] world;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("movement", 20f,10f);
    }
    void movement()
    {
        foreach (GameObject g in world)
        {
            if (g.CompareTag("world1"))
            {
                float x = Random.Range(-66f, 25.1f);
                float y = Random.Range(326.5f, 515f);
                g.transform.DOMove(new Vector2(x, y), 20f, false);
            }
            else if (g.CompareTag("world2"))
            {
                float x = Random.Range(-225.6f, -116.3f);
                float y = Random.Range(296.3f, 403.2f);
                g.transform.DOMove(new Vector2(x, y), 20f, false);
            }
            else if (g.CompareTag("world3"))
            {
                float x = Random.Range(57.2f, 166.6f);
                float y = Random.Range(615.9f,700.9f);
                g.transform.DOMove(new Vector2(x, y), 20f, false);
            }
            else if (g.CompareTag("world4"))
            {
                float x = Random.Range(274f, 384f);
                float y = Random.Range(296.3f, 368.2f);
                g.transform.DOMove(new Vector2(x, y), 20f, false);
            }
            else if (g.CompareTag("world5"))
            {
                float x = Random.Range(135.9f, 251.3f);
                float y = Random.Range(362.8f, 453.1f);
                g.transform.DOMove(new Vector2(x, y), 20f, false);
            }
        }
    }
}
