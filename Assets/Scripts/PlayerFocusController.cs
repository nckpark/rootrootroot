using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFocusController : MonoBehaviour
{
    [SerializeField] private Transform _playerCameraTransform;

    // last watchable watched
    private Watchable _lastWatchableWatched;

    void Update()
    {
        Debug.DrawRay(_playerCameraTransform.position, _playerCameraTransform.forward * 10f, Color.red, 0.1f);
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
        if (Physics.Raycast(_playerCameraTransform.position, _playerCameraTransform.forward, out RaycastHit hit, 20f))
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

}
