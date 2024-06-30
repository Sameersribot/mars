using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class mainmenuBgMovement : MonoBehaviour
{
    private float randomX, randomY;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("generateMovement",1f,10f);
    }

    // Update is called once per frame

    void generateMovement()
    {
        randomX = Random.Range(-24.1f, 30.5f);
        randomY = Random.Range(-15.1f, 18.8f);
        Vector3 movingPos = new Vector3(randomX, randomY, transform.position.z);
        transform.DOMove(movingPos, 10f, false);
        Vector3 direction = (movingPos - transform.position).normalized;

        // Calculate the angle between the object's forward vector and the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float newAngle = Mathf.LerpAngle(transform.rotation.z, angle, 20f * Time.deltaTime);

        // Apply the rotation to the object
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90f));
    }
}
