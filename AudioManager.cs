using System;
using UnityEngine;

public class AudioManager : MonoBehaviour{
    static AudioManager instance = null;
    public Sound[] sounds;

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this){
            Destroy(this.gameObject);
            return;
        }
        foreach(Sound sound in sounds){
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }
    public void Play(string name){
        Sound sound = Array.Find(sounds, item => item.name == name);
        if (sound != null && !sound.source.isPlaying)
            sound.source.Play();
        else if (sound == null)
            Debug.LogWarning("Sound '" + name + "' not found!");
    }
}
