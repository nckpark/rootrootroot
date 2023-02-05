using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BroadcastManager : MonoBehaviour
{
    [SerializeField] private BroadcastType[] _broadcastTypes;

    private Watchable[] _broadcasters; 
    private ScoreManager _playerScoreManager;

    void Start()
    {
        _playerScoreManager = GameObject.FindObjectOfType<ScoreManager>();
        _broadcasters = GameObject.FindObjectsOfType<Watchable>().Where((w) => w.broadcaster == true).ToArray();
    }

    void Update()
    {
        foreach(Watchable caster in _broadcasters)
        {
            if(caster.currentBroadcast == null)
            {
                caster.SetBroadcast(new Broadcast(_playerScoreManager));
            }
        }
    }
}
