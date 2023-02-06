using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public Image warnBarkImage;
    public Image wonBarkImage;
    public Image lostBarkImage;
    public CanvasGroup leftBarkCanvas;
    public CanvasGroup rightBarkCanvas;

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

    public void OnQuit()
    {
        Application.Quit();
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

    public (CanvasGroup, int) GetBarkTargetCanvas(Watchable watchable)
    {
        int  watchableCamIndex = System.Array.FindIndex(_virtualCameras, (cam) => cam.LookAt == watchable.transform);
        if(watchableCamIndex < _activeCameraIdx)
        {
            return (leftBarkCanvas, 1);
        }
        else if(watchableCamIndex > _activeCameraIdx)
        {
            return (rightBarkCanvas, -1);
        }
       
        return (null, 0);
    }

    public void AddWarningBark(Watchable watchable)
    {
        (CanvasGroup targetCanvas, int xMult) = GetBarkTargetCanvas(watchable);
        if(targetCanvas != null)
            AddBarkToCanvas(warnBarkImage, targetCanvas, xMult);
    }

    public void AddWonBark(Watchable watchable)
    {
        (CanvasGroup targetCanvas, int xMult) = GetBarkTargetCanvas(watchable);
        if(targetCanvas != null)
            AddBarkToCanvas(wonBarkImage, targetCanvas, xMult);
    }

    public void AddLostBark(Watchable watchable)
    {
        (CanvasGroup targetCanvas, int xMult) = GetBarkTargetCanvas(watchable);
        if(targetCanvas != null)
            AddBarkToCanvas(lostBarkImage, targetCanvas, xMult);
    }

    public void AddBarkToCanvas(Image barkImagePrefab, CanvasGroup canvas, int xMult=1)
    {
        Image imageObject = Instantiate(barkImagePrefab);
        imageObject.transform.SetParent(canvas.transform, false);

        imageObject.rectTransform.anchoredPosition = new Vector2(
            xMult * -50f, 
            Random.Range(-110, 110)
        );
        imageObject.rectTransform.rotation = Quaternion.Euler(
            0, 0, xMult * Random.Range(10f, 45f)
        );
        StartCoroutine("FadeImage", imageObject);
    }

    public IEnumerator FadeImage(Image img)
    {
        while(img.color.a > 0f)
        {
            Color tempColor = img.color;
            tempColor.a -= (0.75f * Time.deltaTime);
            img.color = tempColor;
            yield return null;
        }
        Destroy(img.gameObject);
    }
}
