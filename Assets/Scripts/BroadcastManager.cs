using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastManager : MonoBehaviour
{
    [SerializeField] private BroadcastType[] _broadcastTypes;

    private Broadcast[] _broadcasters; 

    void Start()
    {
        _broadcasters = GameObject.FindObjectsOfType<Broadcast>();
    }

    void Update()
    {
        foreach(Broadcast broadcast in _broadcasters)
        {
            
        }
    }
}
