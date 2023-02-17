using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using UnityEngine.Video;

public class Watchable : MonoBehaviour
{

    public bool isWatched = false;

    public bool broadcaster = true;
    public Broadcast currentBroadcast;
    public VideoPlayer videoPlayer;
    public MomentumMeter momentumMeter;

    private ParticleSystem _winParticleSystem;
    private ParticleSystem _loseParticleSystem;

    public AudioSource _winAudioSystem;
    public AudioSource _loseAudioSystem;

    public float nextBroadcastDelay = 3f;

    private float _nextBroadcastDelayTimer;

    // Start is called before the first frame update
    void Start()
    {
        // Assign Child Video Player
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        // Assign Child Momentum Meter
        momentumMeter = GetComponentInChildren<MomentumMeter>();
        // Assign Child Particle Systems
        _winParticleSystem = transform.Find("WinParticleSystem").GetComponent<ParticleSystem>();
        _loseParticleSystem = transform.Find("LoseParticleSystem").GetComponent<ParticleSystem>();
        // Assign Child Audio Sources
        _winAudioSystem = transform.Find("WinAudio").GetComponent<AudioSource>();
        _loseAudioSystem = transform.Find("LoseAudio").GetComponent<AudioSource>();
    }


    public void Update()
    {
        if(currentBroadcast != null)
        {
            currentBroadcast.Update();

            if(currentBroadcast.broadcastStatus != Broadcast.BroadcastStatus.Playing)
            {
                if(_nextBroadcastDelayTimer <= 0f)
                {
                    currentBroadcast = null;
                }
                else
                {
                    _nextBroadcastDelayTimer -= Time.deltaTime;
                }
            }
        }
    }

    public void SetBroadcast(Broadcast broadcast)
    {
        broadcast.watchable = this;
        currentBroadcast = broadcast;
        currentBroadcast.Begin();
        Invoke("EndBroadcast", currentBroadcast.broadcastDuration);
    }

    public void StartWatching()
    {
        isWatched = true;
    }
    public void StopWatching()
    {
        isWatched = false;
    }

    public void Encourage(float amount)
    {
        if(isWatched && currentBroadcast != null)
            currentBroadcast.Encourage(amount);
    }

    public void EndBroadcast()
    {
        if(currentBroadcast == null)
            return;
            
        currentBroadcast.EndBroadcast();
        StartNextBroadcastDelay();
    }

    public void StartNextBroadcastDelay()
    {
        _nextBroadcastDelayTimer = nextBroadcastDelay;
    }

    public void PlayWinParticles()
    {
        _winParticleSystem.Play();
    }
    
    public void PlayWinAudio()
    {
        _winAudioSystem.Play();
    }

    public void PlayLoseAudio()
    {
        _loseAudioSystem.Play();
    }

    public void PlayLoseParticles()
    {
        _loseParticleSystem.Play();
    }

    public void SwitchVideoClip(Broadcast.BroadcastStatus broadcastStatus)
    {
        if (currentBroadcast == null) return;
        if (videoPlayer == null) return;

        // Log message that we're switching
        // Debug.Log("Switching video clip to " + broadcastStatus.ToString());

        // switch video clip based on broadcast status
        switch (broadcastStatus)
        {
            case Broadcast.BroadcastStatus.Playing:
                videoPlayer.clip = currentBroadcast.videoSet.idleClip;
                break;
            case Broadcast.BroadcastStatus.Won:
                videoPlayer.clip = currentBroadcast.videoSet.winningClip;
                break;
            case Broadcast.BroadcastStatus.Lost:
                videoPlayer.clip = currentBroadcast.videoSet.losingClip;
                break;
        }

        //videoPlayer.Play();
    }
}
