using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerFocusController : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    [SerializeField] CinemachineVirtualCamera[] _virtualCameras;
    
    private int _activeCameraIdx = 0;
    private int _startCameraIdx = 0;
    
    public CanvasGroup promptsCanvas;
    public TMP_Text promptText; 

    // last watchable watched
    private Watchable _lastWatchableWatched;

    void Start()
    {
        _activeCameraIdx = System.Array.FindIndex(_virtualCameras, (cam) => cam.Priority > 10);
        _startCameraIdx = _activeCameraIdx;
    }

    void Update()
    {
        watchOnRaycast();
    }

    void StopWatchingLast()
    {
        // if last watchable is set, call stop watching and unset
        if (_lastWatchableWatched != null)
        {
            _lastWatchableWatched.StopWatching();
            _lastWatchableWatched = null;
            promptsCanvas.alpha = 0;
        }
    }

    void watchOnRaycast()
    {
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit))
        {
            if (hit.collider.gameObject.GetComponent<Watchable>())
            {
                watchHitWatchable(hit);
            }
            else
            {
                StopWatchingLast();
            }
        }
        else
        {
            StopWatchingLast();
        }
    }

    void watchHitWatchable(RaycastHit hit)
    {
        Watchable hitWatchable = hit.collider.gameObject.GetComponent<Watchable>();
        if (_lastWatchableWatched != hitWatchable)
        {
            StopWatchingLast();
        }

        if(
            (_lastWatchableWatched != hitWatchable || (promptsCanvas.alpha == 0)) && 
            hitWatchable.currentBroadcast != null &&
            hitWatchable.currentBroadcast.broadcastStatus == Broadcast.BroadcastStatus.Playing
        )
        {
            promptsCanvas.alpha = 1;
            int promptIdx = Random.Range(0, hitWatchable.currentBroadcast.shoutPrompts.Length);
            promptText.text = hitWatchable.currentBroadcast.shoutPrompts[promptIdx];
        }
        else if(
            hitWatchable.currentBroadcast != null &&
            hitWatchable.currentBroadcast.broadcastStatus != Broadcast.BroadcastStatus.Playing
        )
        {
            promptsCanvas.alpha = 0;
        }

        _lastWatchableWatched = hitWatchable;
        _lastWatchableWatched.StartWatching();
    }

    public void OnLookLeft()
    {
        if(_activeCameraIdx == 0)
            return;
        
        _virtualCameras[_activeCameraIdx].Priority = 10;
        _activeCameraIdx -= 1;
        _virtualCameras[_activeCameraIdx].Priority = 99;
    }

    public void OnLookRight()
    {
        if(_activeCameraIdx >= _virtualCameras.Length - 1)
            return;

        _virtualCameras[_activeCameraIdx].Priority = 10;
        _activeCameraIdx += 1;
        _virtualCameras[_activeCameraIdx].Priority = 99;
    }

    public void Reset()
    {
        _virtualCameras[_activeCameraIdx].Priority = 10;
        _activeCameraIdx = _startCameraIdx;
        _virtualCameras[_activeCameraIdx].Priority = 99;
    }

    public void OnCheer()
    {
        if(_lastWatchableWatched != null)
            _lastWatchableWatched.Encourage(0.1f);
    }
}
