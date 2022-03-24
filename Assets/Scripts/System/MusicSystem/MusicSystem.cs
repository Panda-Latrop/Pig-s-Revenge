using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSystem : MonoBehaviour
{
    public enum MusicChangeState
    {
        fade,
        rise,
        play,
    }

    [SerializeField]
    protected MusicHolder musicHolder;
    protected bool hasHolder;
    [SerializeField]
    protected AudioSource audioSource;
    protected bool canChange = true, softChange;
    protected MusicChangeState changeState;
    protected int musicId;
    protected string path = string.Empty;
    protected AudioClip forChange;
    protected float riseSpeed = 2.0f, fadeSpeed = 1.25f;
    protected float time;


    protected int currentMusic;
    public void SetMusicHolder(MusicHolder musicHolder)
    {
        bool msh = musicHolder != null;
        bool tmshnemsh = false;
        if(msh && hasHolder)
            tmshnemsh = !this.musicHolder.name.Equals(musicHolder.name);


        if ((!hasHolder && msh) || tmshnemsh || (msh && !audioSource.isPlaying))
        {
            this.musicHolder = musicHolder;
            audioSource.outputAudioMixerGroup = musicHolder.GetMixerGroup();
            hasHolder = true;
            Change();
        }
        else if(!msh){
            hasHolder = false;
            Stop();
        }
    }
    public void SetAudioSource(AudioSource audioSource)
    {
        this.audioSource = audioSource;
        this.audioSource.spatialBlend = 0.0f;
    }
    public void Stop()
    {
        audioSource.Stop();
        changeState = 0;
        canChange = true;
    }
    public void Pause()
    {
        audioSource.Pause();
    }
    public void Resume()
    {
        audioSource.UnPause();
    }
    public void Change(string path)
    {
        this.path = path;
        Change(Resources.Load(path) as AudioClip,false);
    }
    public void Change(AudioClip music, bool canChange = true)
    {
        changeState = MusicChangeState.fade;
        forChange = music;
        this.canChange = canChange;
    }
    [ContextMenu("Change")]
    public void Change()
    {
        path = string.Empty;
        var rand = Random.Range(0, musicHolder.GetCount());
        if (rand == musicId)
        {
            musicId++;
            if (musicId >= musicHolder.GetCount())
                musicId = 0;
        }
        else
        {
            musicId = rand;
        }
        Change(musicHolder.Get(musicId));
    }
    protected void LateUpdate()
    {
        switch (changeState)
        {
            case MusicChangeState.fade:
                audioSource.volume -= fadeSpeed * Time.deltaTime;
                if (audioSource.volume <= 0 || !audioSource.isPlaying)
                {
                    audioSource.volume = 0;
                    audioSource.Stop();
                    audioSource.time = 0;
                    audioSource.clip = forChange;
                    audioSource.Play();
                    changeState = MusicChangeState.rise;
                } 
                break;
            case MusicChangeState.rise:
                audioSource.volume += riseSpeed * Time.deltaTime;
                if (audioSource.volume >= 1)
                {
                    audioSource.volume = 1;
                    changeState = MusicChangeState.play;
                }
                break;
            default:
                if (!audioSource.isPlaying && audioSource.time == 0 && hasHolder)
                {
                    if (canChange)
                        Change();
                    else
                        audioSource.Play();
                }
                if (audioSource.volume < 1)
                {
                    audioSource.volume += riseSpeed * Time.deltaTime;
                }
                break; 
        }
        time = audioSource.time;
    }
}
