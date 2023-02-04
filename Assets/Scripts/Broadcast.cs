using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broadcast : MonoBehaviour
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

    // bool is decay delay active
    public bool isDecayDelayActive = false;

    // broadcast duration
    public float broadcastDuration = 25.0f;

    // enum broadcast status
    public enum BroadcastStatus
    {
        Playing,
        Won,
        Lost
    }

    // broadcast status
    public BroadcastStatus broadcastStatus = BroadcastStatus.Playing;

    // ScoreManager of player
    public ScoreManager playerScoreManager;

    // Start is called before the first frame update
    void Start()
    {
        playerScoreManager = GameObject.Find("PlayerCapsule").GetComponent<ScoreManager>();   
        EndBroadcastAfterDuration();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWinningStatus();
        DecayIfNotOnDelay();
        // call encourage when space bar pressed and watchable is being watched
        if (Input.GetKeyDown(KeyCode.Space) && GetComponent<Watchable>().isWatched)
        {
            Encourage(0.1f);
        }
    }
    
    public void Encourage(float amount)
    {
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

    public void EndBroadcastAfterDuration()
    {
        // call EndBroadcast() after broadcastDuration
        Invoke("EndBroadcast", broadcastDuration);
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
            GetComponent<Renderer>().material.color = Color.blue;
            // scale cube by momentum
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * (momentum + 1.0f);

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
            GetComponent<Renderer>().material.color = Color.red;
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * (momentum + 1.0f);

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

        // reset decay delay active to false after delay
        Invoke("ResetDecayDelay", decayDelay);
    }

    // Reset decay delay
    void ResetDecayDelay()
    {
        // set decay delay active to false
        isDecayDelayActive = false;
    }
}
