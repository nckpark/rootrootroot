using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Watchable : MonoBehaviour
{

    public bool isWatched = false;

    public bool broadcaster = true;
    public Broadcast currentBroadcast;
    public float nextBroadcastDelay = 3f;

    private float _nextBroadcastDelayTimer;

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
        currentBroadcast.EndBroadcast();
        StartNextBroadcastDelay();
    }

    public void StartNextBroadcastDelay()
    {
        _nextBroadcastDelayTimer = nextBroadcastDelay;
    }
}
