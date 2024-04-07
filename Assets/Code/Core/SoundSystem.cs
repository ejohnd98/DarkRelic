using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSystem : MonoBehaviour {
    
    [System.Serializable]
    public class VariantSound{
        public string name;
        public AudioClip[] sounds;
        private int soundIndex = 0;

        public AudioClip GetSound(){
            soundIndex = (soundIndex + Random.Range(0, sounds.Length-1)) % sounds.Length;
            return sounds[soundIndex];
        }
    }
    
    public static SoundSystem instance;

    public GameObject soundEffectPrefab;
    public AudioClip[] sfxList;
    public VariantSound[] variantSfxList;
    
    private Dictionary<string, AudioClip> sfx;
    private Dictionary<string, VariantSound> variantSfx;

    private void Awake() {
        if (instance != null && instance != this){
            Debug.LogAssertion("Duplicate SoundSystem created!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        if(sfx == null){
            sfx = new Dictionary<string, AudioClip>();
            variantSfx = new Dictionary<string, VariantSound>();

            foreach(AudioClip clip in sfxList){
                sfx.Add(clip.name, clip);
            }
            foreach(VariantSound variant in variantSfxList){
                variantSfx.Add(variant.name, variant);
            }
        }
    }
    
    public void PlaySound(string sndName){
        AudioClip clip;
        
        if (sfx.ContainsKey(sndName)){
            clip = sfx[sndName];
        }else if (variantSfx.ContainsKey(sndName)){
            clip = variantSfx[sndName].GetSound();
        }else {
            return;
        }

        GameObject sndObj = Instantiate(soundEffectPrefab, transform);
        AudioSource newSrc = sndObj.GetComponent<AudioSource>();
        newSrc.spatialize = false;
        newSrc.clip = clip;
    }
    
}
