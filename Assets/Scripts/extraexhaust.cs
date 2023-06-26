using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extraexhaust : MonoBehaviour
{
    public GameObject exhaustone, exhausttwo;
    private ParticleSystem exhaustoneParticle, exhausttwoParticle;
    public GameObject rocket;
    private RocketController rockets;
    

    // Start is called before the first frame update
    void Start()
    {
        exhaustoneParticle = exhaustone.GetComponent<ParticleSystem>();
        exhausttwoParticle = exhausttwo.GetComponent<ParticleSystem>();
        rockets = rocket.GetComponent<RocketController>();
    }

    // Update is called once per frame
    void Update()
    {
        //exhaustone.transform.position = new Vector3(rocket.transform.position.x - 0.43f, rocket.transform.position.y - 0.089f);
        //exhausttwo.transform.position = new Vector3(rocket.transform.position.x + 0.48f, rocket.transform.position.y - 0.089f, rocket.pos);
        
    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        
        if(rockets.movement.magnitude > 0.1f && rockets.currentFuel >= 0f)
        {
            exhaustoneParticle.Play();
            exhaustoneParticle.startLifetime = rockets.movement.magnitude * 0.182f;
            exhausttwoParticle.Play();
            exhausttwoParticle.startLifetime = rockets.movement.magnitude * 0.182f;
        }
        else
        {
            exhaustoneParticle.Stop();
            exhausttwoParticle.Stop();
        }
    }
}
