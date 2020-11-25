using System;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance;

    public ParticleSystem MotionSpeedEffect;
    public bool IsRunningFast;
    public bool IsHighJumping;

    private void Awake()
    {
        print("CameraEffects awake");
        Instance = this;
    }


    private void OnDestroy() {
        Debug.Log("Destroy CameraEffects");
    }


    private void Update()
    {
        if (IsRunningFast || IsHighJumping)
        {
            if (!MotionSpeedEffect.isPlaying)
                MotionSpeedEffect.Play();
        }
        else
        {
            if (MotionSpeedEffect.isPlaying)
            {
                MotionSpeedEffect.Stop();
            }
        }
    }
}
