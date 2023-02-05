using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // list of broadcasts won
    public List<Broadcast> broadcastsWon = new List<Broadcast>();

    // list of broadcasts lost
    public List<Broadcast> broadcastsLost = new List<Broadcast>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Award points for broadcast
    public void AwardPoints(Broadcast broadcast)
    {
        if (broadcast.isWinning)
        {
            // add broadcast to broadcasts won
            broadcastsWon.Add(broadcast);
        }
        else
        {
            // add broadcast to broadcasts lost
            broadcastsLost.Add(broadcast);
        }

    }

    public void Reset()
    {
        broadcastsWon.Clear();
        broadcastsLost.Clear();
    }
}
