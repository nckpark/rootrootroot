using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BroadcastManager : MonoBehaviour
{
    [SerializeField] private BroadcastType[] _broadcastTypes;
    [SerializeField] private float _difficultyWaveDuration = 20f;
    private float _difficultyWaveTimer;
    private Dictionary<int, List<BroadcastType>> _broadcastTypesByValue;
    private int _currentPointLimit;
    private int _minPointValue;
    private int _maxPointValue;

    public AudioClip splashClip;
    public AudioClip gameplayClip;

    [SerializeField] private CinemachineBrain _cameraBrain;
    [SerializeField] private PlayerFocusController _playerController;

    [SerializeField] private CinemachineVirtualCamera _splashScreenCamera;
    [SerializeField] private CanvasGroup _splashScreenCanvas;
    
    [SerializeField] private CanvasGroup _gameOverCanvas;
    [SerializeField] private TMP_Text _wonText;
    [SerializeField] private TMP_Text _lostText;
    [SerializeField] private TMP_Text _totalText;

    public float focusTransitionTime = 0.5f;
    public float startTransitionTime = 2f;

    public float roundLength = 90f;
    public float roundTimer;
    public bool roundActive;
    private bool _startTransitionComplete;
    private bool _gameOver;

    private Watchable[] _broadcasters; 
    private ScoreManager _playerScoreManager;

    void Start()
    {
        _playerScoreManager = GameObject.FindObjectOfType<ScoreManager>();
        _broadcasters = GameObject.FindObjectsOfType<Watchable>().Where((w) => w.broadcaster == true).ToArray();

        _broadcastTypesByValue = new Dictionary<int, List<BroadcastType>>();
        foreach(BroadcastType castType in _broadcastTypes)
        {
            if(!_broadcastTypesByValue.ContainsKey(castType.pointValue))
                _broadcastTypesByValue[castType.pointValue] = new List<BroadcastType>();
            _broadcastTypesByValue[castType.pointValue].Add(castType);
        }
        _minPointValue = _broadcastTypesByValue.Aggregate((l, r) => l.Key < r.Key ? l : r).Key;
        _maxPointValue = _broadcastTypesByValue.Aggregate((l, r) => l.Key > r.Key ? l : r).Key;
        
        _currentPointLimit = _minPointValue;
        _difficultyWaveTimer = _difficultyWaveDuration;

        _cameraBrain.m_DefaultBlend.m_Time = startTransitionTime;

        roundActive = false;
    }

    void Update()
    {
        if(!roundActive)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(_gameOver == true)
                {
                    ReturnToSplashScreen();
                }
                else
                {
                    StartNewRound();
                }
            }
            // on arrow up, increase activation level by 0.1
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // find player object
                GameObject player = GameObject.Find("Player");
                decibelreading decibelReading = player.GetComponent<decibelreading>();
                decibelReading.activationLevel += 0.1f;
                // clamp decibel reading to 1
                if (decibelReading.activationLevel > 1)
                {
                    decibelReading.activationLevel = 1;
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                // find player object
                GameObject player = GameObject.Find("Player");
                decibelreading decibelReading = player.GetComponent<decibelreading>();
                decibelReading.activationLevel -= 0.1f;
                // clamp decibel reading to 0
                if (decibelReading.activationLevel < 0)
                {
                    decibelReading.activationLevel = 0;
                }
            }
            return;
        }
        else if(!_startTransitionComplete)
        {
            return;
        }
        else
        {
            roundTimer -= Time.deltaTime;
        }

        if(roundTimer <= 0f)
        {
            EndRound();
            return;
        }

        if(_startTransitionComplete == false)
            return;

        _difficultyWaveTimer -= Time.deltaTime;
        if(_difficultyWaveTimer <= 0f)
        {
            _difficultyWaveTimer = _difficultyWaveDuration;
            if(_currentPointLimit < _maxPointValue)
                _currentPointLimit++;
        }

        foreach(Watchable caster in _broadcasters)
        {
            BroadcastType nextType = PickNextBroadcastType();
            if(caster.currentBroadcast == null)
            {
                caster.SetBroadcast(new Broadcast(
                    _playerController,
                    _playerScoreManager,
                    nextType.videoSet,
                    nextType.shoutPrompts,
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

    public BroadcastType PickNextBroadcastType()
    {
        int pointValue = Random.Range(1, _currentPointLimit + 1);
        int optionsCount = _broadcastTypesByValue[pointValue].Count;
        BroadcastType nextType = _broadcastTypesByValue[pointValue][Random.Range(0, optionsCount)];
        return nextType;
    }

    public void ClearAllBroadcasts()
    {
        foreach(Watchable caster in _broadcasters)
        {
            if(caster.currentBroadcast != null)
            {
                caster.currentBroadcast.broadcastStatus = Broadcast.BroadcastStatus.Stopped;
                caster.currentBroadcast = null;
            }
        }
    }

    public void StartNewRound()
    {
        _startTransitionComplete = false;
        _playerScoreManager.Reset();
        _currentPointLimit = _minPointValue;
        _difficultyWaveTimer = _difficultyWaveDuration;
        roundTimer = roundLength;
        roundActive = true;

        _cameraBrain.m_DefaultBlend.m_Time = startTransitionTime;
        _splashScreenCamera.Priority = 0;
        _splashScreenCanvas.alpha = 0;

        _playerController.GetComponent<AudioSource>().Play();

        Invoke("StartBroadcasters", _cameraBrain.m_DefaultBlend.m_Time);
    }

    public void EndRound()
    {
        _gameOver = true;
        roundActive = false;
        ClearAllBroadcasts();

        _playerController.promptsCanvas.alpha = 0;

        _wonText.text = _playerScoreManager.broadcastsWon.Count.ToString();
        _lostText.text = _playerScoreManager.broadcastsLost.Count.ToString();
        _totalText.text = (_playerScoreManager.broadcastsWon.Count - _playerScoreManager.broadcastsLost.Count).ToString();

        _gameOverCanvas.alpha = 1;
        
        _splashScreenCamera.Priority = 999;
        _playerController.Reset();
    }

    public void ReturnToSplashScreen()
    {
        _gameOver = false;
        _splashScreenCanvas.alpha = 1;
        _gameOverCanvas.alpha = 0;
    }

    public void StartBroadcasters()
    {
        _cameraBrain.m_DefaultBlend.m_Time = focusTransitionTime;
        _startTransitionComplete = true;
    }
}
