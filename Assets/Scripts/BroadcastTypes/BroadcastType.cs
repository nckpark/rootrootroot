using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu]
public class BroadcastType : ScriptableObject
{
    public VideoClip clip;
    public int pointValue = 0;
    public float broadcastDuration = 25.0f;
    public float maxMomentum = 5000.0f;

    // float win threshold, how much momentum is needed to win
    public float winThreshold = 0.5f;

    // float momentum decay, how much momentum is lost per second
    public float momentumDecay = 0.1f;

    // decay delay, how long until decay should resume
    public float decayDelay = 1.0f;
}
