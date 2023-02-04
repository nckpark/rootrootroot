using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Watchable : MonoBehaviour
{

    public bool isWatched = false;
   
    public void StartWatching()
    {
        isWatched = true;
    }
    public void StopWatching()
    {
        isWatched = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
