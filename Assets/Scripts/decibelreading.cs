using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decibelreading : MonoBehaviour
{

    // float decibel level
    public float decibelLevel = 0.0f;
    public float activationLevel = 0.5f;

    // cheer cooldown
    public float cheerCooldown = 0.0f;
    // current cheer cooldown
    
    // reference to player focus controller
    public PlayerFocusController playerFocusController;

    // Start is called before the first frame update
    void Start()
    {
        // get player focus controller on same object
        playerFocusController = GetComponent<PlayerFocusController>();
    }

    // Update is called once per frame
    void Update()
    {
        decibelLevel = MicInput.MicLoudness;
        decibelLevel = 20 * Mathf.Log10(decibelLevel);
        // convert decibels to po
        decibelLevel = (decibelLevel + 60) / 60;
        
        float scale = decibelLevel + 1;
        // scale cannot be below 1
        

        
        if (decibelLevel > activationLevel)
        {
            // call OnCheer of Player Focus Controller if it's not null
            if (playerFocusController != null)
            {
                // if not on cheer cooldown
                if (cheerCooldown <= 0f)
                {
                    // call OnCheer
                    playerFocusController.OnCheer();
                    // reset cheer cooldown
                    cheerCooldown = 0.2f;
                }
                else
                {
                    // decrement cheer cooldown
                    cheerCooldown -= Time.deltaTime;
                }
            }
            
        } else
        {
            if (cheerCooldown > 0.0f)
            {
                cheerCooldown -= Time.deltaTime;
            } else
            {
                cheerCooldown = 0.0f;
            }
            
        }
    }
}
