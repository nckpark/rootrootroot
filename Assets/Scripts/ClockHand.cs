using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockHand : MonoBehaviour
{
    [SerializeField] private BroadcastManager _broadcastManager;

    private float startRotation;

    void Start()
    {
        startRotation = transform.rotation.eulerAngles.z - 360;
    }

    void Update()
    {
        if(_broadcastManager.roundActive)
        {
            float roundCompletePercent = (_broadcastManager.roundLength - _broadcastManager.roundTimer) / _broadcastManager.roundLength;
            float newZRotation = startRotation - (roundCompletePercent * (360 - System.Math.Abs(startRotation)));

            Quaternion target = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, newZRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, startRotation);
        }
    }
}
