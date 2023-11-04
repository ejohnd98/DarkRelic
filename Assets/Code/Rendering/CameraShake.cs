using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    static CameraShake instance;

    Animator animator;
    float currentShake = 0.0f;
    float recoverySpeed = 12.0f;

    public float testInput = 1.0f;
    public bool testTrigger = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(currentShake > 0){
            currentShake = Mathf.Lerp(currentShake, 0, Time.deltaTime * recoverySpeed);
            animator.SetFloat("Shake", currentShake);
        }
        if(testTrigger){
            testTrigger = false;
            ShakeCamera(testInput);
        }
    }

    public static void ShakeCamera(float intensity){
        instance.currentShake = intensity;
    }
}
