using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
   
    public AudioSource skidSound;
    public ParticleSystem[] smoke;
    public TrailRenderer[] tyreMarks;
    private InputManager inputManager;
    private Controller controller;
    private bool smokeFlag = false;
    private bool tyreMarksFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<Controller>();
        inputManager = gameObject.GetComponent<InputManager>();
    }

    public void StartParticles() {
        if (smokeFlag)
            return;
        for (int i = 0; i < smoke.Length; i++) {
            var emission = smoke[i].emission;
            emission.rateOverTime = ((int)controller.KPH * 2 <= 250) ? (int)controller.KPH * 2 : 250;
            smoke[i].Play();
        }
        smokeFlag = true;
    }

    public void StopParticles()
    {
        for (int i = 0; i < smoke.Length; i++)
        {
            smoke[i].Stop();
        }
        smokeFlag = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (controller.toggleSmoke)
        {
            StartParticles();
        }
        else
            StopParticles();

        if (smokeFlag) {
            for (int i = 0; i < smoke.Length; i++) {
                var emission = smoke[i].emission;
                emission.rateOverTime = ((int)controller.KPH * 2 <= 1500) ? (int)controller.KPH * 2 : 1500;
            }
        }

        CheckDrift();
    }


    void CheckDrift()
    {
        if (inputManager.handbrake_)
        {
          
            StartTrail();
        }
        else
        {
            
            StopTrail();
        }
    }
    void StartTrail() {
        if (tyreMarksFlag)
            return;

        foreach (TrailRenderer trail in tyreMarks) {
            trail.emitting = true;
            skidSound.Play();
        }
        tyreMarksFlag = true;
    }

    void StopTrail()
    {
        if (!tyreMarksFlag)
            return;

        foreach (TrailRenderer trail in tyreMarks)
        {
            trail.emitting = false;
            skidSound.Stop();
        }
        tyreMarksFlag = false;
    }
}
