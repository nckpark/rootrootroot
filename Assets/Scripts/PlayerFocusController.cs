using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFocusController : MonoBehaviour
{
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Physics.Raycast(transform.position, transform.forward, 10);  // TODO layer masks
    }
}
