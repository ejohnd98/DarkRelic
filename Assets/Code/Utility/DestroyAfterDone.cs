using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DestroyAfterDone : MonoBehaviour
{
    public bool afterSound = false, afterParticles = false, afterTime = false;
    public bool persistThroughLoad = false;
    public float timeLength;

    private AudioSource audioSource;
    private ParticleSystem particles;
    // Start is called before the first frame update
    void Awake()
    {
        if(persistThroughLoad){
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start(){
        if(afterSound){
            audioSource = GetComponent<AudioSource>();
            audioSource.Play();
        }
        if(afterParticles){
            particles = GetComponent<ParticleSystem>();
            particles.Play();
        }
        if (afterTime){
            StartCoroutine(DestroyTimer());

        }
        
    }

    IEnumerator DestroyTimer(){
        yield return new WaitForSeconds(timeLength);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(afterSound && !audioSource.isPlaying){
            Destroy(this.gameObject);
        }
        if(afterParticles && !particles.isPlaying){
            Destroy(this.gameObject);
        }
    }

    public void SetPersist(bool state){
        if(state){
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
