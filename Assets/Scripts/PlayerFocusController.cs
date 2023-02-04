using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFocusController : MonoBehaviour
{
    [SerializeField] private Transform _playerCameraTransform;

    void Update()
    {
        Debug.DrawRay(_playerCameraTransform.position, _playerCameraTransform.forward * 10f, Color.red, 0.1f);
        Physics.Raycast(_playerCameraTransform.position, _playerCameraTransform.forward, 10);  // TODO layer masks
    }
}
