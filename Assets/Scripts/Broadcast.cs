using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broadcast
{
    // bool is winning
    public bool isWinning = false;

    public int pointValue = 0;

    // float momentum, any value
    public float momentum = 0.0f;

    // max momentum
    public float maxMomentum = 5000.0f;

    // float win threshold, how much momentum is needed to win
    public float winThreshold = 0.5f;

    // float momentum decay, how much momentum is lost per second
    public float momentumDecay = 0.1f;

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
        Lost
    }

    // broadcast status
    public BroadcastStatus broadcastStatus = BroadcastStatus.Playing;

    public ScoreManager playerScoreManager;

    public Broadcast(
        ScoreManager scoreManager,
        float duration=25.0f,
        float winThreshold=0.5f,
        float momentumDecay=0.1f, 
        float decayDelay=1.0f
    )
    {
        this.broadcastDuration = duration;
        this.winThreshold = winThreshold;
        this.momentumDecay = momentumDecay;
        this.decayDelay = decayDelay;
        this.playerScoreManager = scoreManager;
    }

    // Update is called by Watchable once per frame
    public void Update()
    {
        UpdateWinningStatus();
        if(broadcastStatus != BroadcastStatus.Playing)
            return;

        DecayIfNotOnDelay();
        if(isDecayDelayActive)
        {   
            decayDelayTimer -= Time.deltaTime;
            if(decayDelayTimer <= 0f)
                ResetDecayDelay();
        }
    }
    
    public void Encourage(float amount)
    {
        if(broadcastStatus != BroadcastStatus.Playing)
            return;

        // add to momentum
        momentum += amount;
        DelayDecay();
    }

    void OnStoppedWinning()
    {
        // Log "Warning Bark!"
        Debug.Log("Warning Bark!");

    }

    void OnBroadcastWon()
    {
        // log "Broadcast Won!"
        Debug.Log("Broadcast Won!");
        // award points
        playerScoreManager.AwardPoints(this);
    }

    void OnBroadcastLost()
    {
        // log "Broadcast Lost!"
        Debug.Log("Broadcast Lost!");
        playerScoreManager.AwardPoints(this);
    }

    public void EndBroadcast()
    {
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
        // if momentum is greater than win threshold
        if (momentum > winThreshold)
        {
            // set is winning to true
            isWinning = true;
            // make cube red
            watchable.GetComponent<Renderer>().material.color = Color.blue;
            // scale cube by momentum
            watchable.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * (momentum + 1.0f);

        }
        else
        {
            if (isWinning)
            {
                OnStoppedWinning();
            }    
            // set is winning to false
            isWinning = false;
            // make cube red
            watchable.GetComponent<Renderer>().material.color = Color.red;
            watchable.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * (momentum + 1.0f);
        }
    }

    void DecayIfNotOnDelay()
    {
        {
            // if decay delay is not active
            if (!isDecayDelayActive)
            {
                // decay momentum
                momentum -= momentumDecay * Time.deltaTime;
                // clamp momentum between 0 and 5000
                momentum = Mathf.Clamp(momentum, 0.0f, maxMomentum);
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
