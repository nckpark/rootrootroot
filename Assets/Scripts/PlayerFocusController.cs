using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFocusController : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    private List<CinemachineVirtualCamera> _virtualCameras;
    private int _activeCameraIdx = 0;
    
    // last watchable watched
    private Watchable _lastWatchableWatched;

    void Start()
    {
        _virtualCameras = new List<CinemachineVirtualCamera>(
            GameObject.FindObjectsOfType<CinemachineVirtualCamera>()
        );
        _activeCameraIdx = _virtualCameras.FindIndex((cam) => cam.Priority == 99);
    }


    void Update()
    {

        Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * 10f, Color.red, 0.1f);
        watchOnRaycast();
        
    }

    void StopWatchingLast()
    {
        // if last watchable is set, call stop watching and unset
        if (_lastWatchableWatched != null)
        {
            _lastWatchableWatched.StopWatching();
            _lastWatchableWatched = null;
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
        _lastWatchableWatched = hitWatchable;
        _lastWatchableWatched.StartWatching();
    }

    public void OnLookLeft()
    {
        _virtualCameras[_activeCameraIdx].Priority = 10;
        _activeCameraIdx = ((_activeCameraIdx - 1) + _virtualCameras.Count) % _virtualCameras.Count;
        _virtualCameras[_activeCameraIdx].Priority = 99;
    }

    public void OnLookRight()
    {
        _virtualCameras[_activeCameraIdx].Priority = 10;
        _activeCameraIdx = (_activeCameraIdx + 1) % _virtualCameras.Count;
        _virtualCameras[_activeCameraIdx].Priority = 99;
    }
}
