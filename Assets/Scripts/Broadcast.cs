using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broadcast
{
    // bool is winning
    public bool isWinning = false;

    public int pointValue = 10;

    // float momentum, any value
    public float momentum = 0.0f;

    // max momentum
    public float maxMomentum = 1.0f;

    // float win threshold, how much momentum is needed to win
    public float winThreshold = 0.5f;

    // float momentum decay, how much momentum is lost per second
    public float momentumDecay = 0.1f;

    private float feedbackDelay = 0.1f;

    // decay delay, how long until decay should resume
    public float decayDelay = 1.0f;
    private float decayDelayTimer = 0.0f;

    // bool is decay delay active
    public bool isDecayDelayActive = false;

    // broadcast duration
    public float broadcastDuration = 25.0f;

    public Watchable watchable;


    // enum broadcast status
    public enum BroadcastStatus
    {
        Playing,
        Won,
        Lost,
        Stopped
    }

    // broadcast status
    public BroadcastStatus broadcastStatus = BroadcastStatus.Playing;

    public ScoreManager playerScoreManager;

    public VideoSet videoSet;
    public string[] shoutPrompts;

    // Start is called before the first frame update
    public Broadcast(
       ScoreManager scoreManager,
       VideoSet videoSet,
       string[] shoutPrompts,
       int pointValue = 0,
       float duration = 25.0f,
       float maxMomentum = 5000f,
       float winThreshold = 0.5f,
       float momentumDecay = 0.1f,
       float decayDelay = 1.0f
    )
    {
        this.shoutPrompts = shoutPrompts;
        this.pointValue = pointValue;
        this.broadcastDuration = duration;
        this.maxMomentum = maxMomentum;
        this.winThreshold = winThreshold;
        this.momentumDecay = momentumDecay;
        this.decayDelay = decayDelay;
        this.playerScoreManager = scoreManager;
        this.videoSet = videoSet;
    }

    public void Begin()
    {
        this.NotifyVideoSwitcher();
    }
    
    // Update is called by Watchable once per frame
    public void Update()
    {
        UpdateWinningStatus();
        if(broadcastStatus != BroadcastStatus.Playing)
            return;

        DecayIfNotOnDelay();
        if (isDecayDelayActive)            
        {   
            decayDelayTimer -= Time.deltaTime;

            if (decayDelayTimer > 0f) {
                float feedbackProgress = feedbackDelay - (decayDelayTimer / feedbackDelay);
                if (feedbackProgress < 1f)
                {
 
                    float newScale = (2f + (Mathf.Sin(feedbackProgress * Mathf.PI / 8)) * 0.1f);
                  
                    watchable.transform.localScale = new Vector3(newScale, newScale, 1);
                } else
                {
                    watchable.transform.localScale = new Vector3(2, 2, 1);
                }
            }else
            {
                watchable.transform.localScale = new Vector3(2, 2, 1);
                ResetDecayDelay();
            }
                
        }
    }

    // update momentum meter
    void updateMomentumMeter()
    {
        if (watchable.momentumMeter != null)
            watchable.momentumMeter.UpdateCurrentValue(momentum, maxMomentum, isWinning);
    }

    public void Encourage(float amount)
    {
        if(broadcastStatus != BroadcastStatus.Playing)
            return;

        // add to momentum
        momentum += amount;
        DelayDecay();
        updateMomentumMeter();
    }

    void OnStoppedWinning()
    {
        // Log "Warning Bark!"
        Debug.Log("Warning Bark!");

    }

    // Notify video switcher
    void NotifyVideoSwitcher()
    {
        // log notifying
        Debug.Log("Notifying switcher!");
        // call switch video clip
        if (watchable != null)
            watchable.SwitchVideoClip(broadcastStatus);
    }

    void OnBroadcastWon()
    {
        // log "Broadcast Won!"
        Debug.Log("Broadcast Won!");
        playerScoreManager.AwardPoints(this);
        NotifyVideoSwitcher();
    }

    void OnBroadcastLost()
    {
        Debug.Log("Broadcast Lost!");
        playerScoreManager.AwardPoints(this);
        NotifyVideoSwitcher();
    }

    public void EndBroadcast()
    {
        if(broadcastStatus == BroadcastStatus.Stopped)
            return;

        if (isWinning)
        {
            broadcastStatus = BroadcastStatus.Won;
            OnBroadcastWon();
        }
        else
        {
            broadcastStatus = BroadcastStatus.Lost;
            OnBroadcastLost();
        }
    }

    void UpdateWinningStatus()
    {
        if (momentum > winThreshold)
        {
            isWinning = true;
            watchable.GetComponent<Renderer>().material.color = Color.blue;

        }
        else
        {
            if (isWinning)
            {
                OnStoppedWinning();
            }    
            isWinning = false;
            watchable.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void DecayIfNotOnDelay()
    {
        {
            if (!isDecayDelayActive)
            {
                // decay momentum
                momentum -= momentumDecay * Time.deltaTime;
                // clamp momentum between 0 and 5000
                momentum = Mathf.Clamp(momentum, 0.0f, maxMomentum);
                updateMomentumMeter();
            }
        }
    }

    // set decay delay active to true and reset to false after delay
    public void DelayDecay()
    {
        // set decay delay active to true
        isDecayDelayActive = true;
        decayDelayTimer = decayDelay;
    }

    // Reset decay delay
    void ResetDecayDelay()
    {
        // set decay delay active to false
        isDecayDelayActive = false;
    }
}
