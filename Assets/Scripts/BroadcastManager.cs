using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BroadcastManager : MonoBehaviour
{
    [SerializeField] private BroadcastType[] _broadcastTypes;
    [SerializeField] private CinemachineBrain _cameraBrain;
    [SerializeField] private CinemachineVirtualCamera _splashScreenCamera;
    [SerializeField] private CanvasGroup _splashScreenCanvas;

    public float focusTransitionTime = 0.5f;

    public float roundLengthMinutes = 3f;
    private float _roundTimer;
    private bool _roundActive;
    private bool _startTransitionComplete;

    private Watchable[] _broadcasters; 
    private ScoreManager _playerScoreManager;

    void Start()
    {
        _playerScoreManager = GameObject.FindObjectOfType<ScoreManager>();
        _broadcasters = GameObject.FindObjectsOfType<Watchable>().Where((w) => w.broadcaster == true).ToArray();

        _roundActive = false;
    }

    void Update()
    {
        if(_roundActive)
            _roundTimer -= Time.deltaTime;
        else if(Input.GetKeyDown(KeyCode.Space))
            StartNewRound();
        else
            return;

        if(_roundTimer <= 0f)
        {
            _roundActive = false;
        }

        if(_startTransitionComplete == false)
            return;

        foreach(Watchable caster in _broadcasters)
        {
            if(caster.currentBroadcast == null)
            {
                BroadcastType nextType = _broadcastTypes[Random.Range(0, _broadcastTypes.Length)];
                caster.SetBroadcast(new Broadcast(
                    _playerScoreManager,
                    nextType.videoSet,
                    nextType.pointValue,
                    nextType.broadcastDuration,
                    nextType.maxMomentum,
                    nextType.winThreshold,
                    nextType.momentumDecay,
                    nextType.decayDelay
                ));
            }
        }
    }

    public void StartNewRound()
    {
        _roundTimer = roundLengthMinutes;
        _roundActive = true;

        _splashScreenCamera.Priority = 0;
        _splashScreenCanvas.alpha = 0;

        Invoke("StartBroadcasters", _cameraBrain.m_DefaultBlend.m_Time);
    }

    public void StartBroadcasters()
    {
        _cameraBrain.m_DefaultBlend.m_Time = focusTransitionTime;
        _startTransitionComplete = true;
    }
}
