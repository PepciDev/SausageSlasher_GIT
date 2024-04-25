using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    public ParticleSystem blinkParticle;
    public ParticleSystem slashParticle;
    public GameObject ambushParticle;
    public GameObject smokeParticle;
    public GameObject landingParticle;

    public ParticleSystem xParticles;

    public Transform instantiatePos;

    private void Start()
    {
        //stop particles at start just in case
        slashParticle.Stop();
        slashParticle.gameObject.SetActive(false);
    }

    void SmokeParticles()
    {
        //instantiate smoke
        Instantiate(smokeParticle, instantiatePos.position, smokeParticle.transform.rotation);
    }

    void AmbushParticles()
    {
        //instantiate ambush appear particles
        Instantiate(ambushParticle, instantiatePos.position, ambushParticle.transform.rotation);
    }

    void BlinkParticle()
    {
        //sword blink particle
        blinkParticle.transform.localScale = new Vector3(1, 1, 1);
        blinkParticle.Play();
    }

    public void BigBlinkParticle()
    {
        //big sword blink particle
        blinkParticle.transform.localScale = new Vector3(1.7f, 1.7f, 1.06f);
        blinkParticle.Play();
    }

    void LandingParticles()
    {
        //instantiate landing particles
        Instantiate(landingParticle, instantiatePos.position, landingParticle.transform.rotation);
    }

    public void StartSlash()
    {
        //start slash + particles
        slashParticle.gameObject.SetActive(true);
        slashParticle.Play();
    }

    public void EndSlash()
    {
        //end slash + particles
        slashParticle.Stop();
        slashParticle.gameObject.SetActive(false);
    }

    public void StartXParticle()
    {
        xParticles.Play();
    }

    public void StopXParticle()
    {
        xParticles.Stop();
    }
}
