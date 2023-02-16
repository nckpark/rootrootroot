using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumMeter : MonoBehaviour
{

    // game object for the current value bar
    public GameObject currentValueBar;

    // current value of meter, between 0 and 1
    public float currentValue;

    // max scale value of meter
    public float maxValue = 2.5f;

    public Color winColor;
    public Color loseColor;

    // Start is called before the first frame update
    void Start()
    {
        // set reference to child game object
        currentValueBar = transform.Find("CurrentValue").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCurrentValue(float newValue, float maxValue, bool isWinning)
    {
        // Normalize new value as percent of max value
        //currentValue = newValue / maxValue;

        // multiply current value by max scale to get new scale value
        float newScale = newValue / maxValue;
        // Clamp between 0 and 1
        newScale = Mathf.Clamp(newScale, 0.0f, 1.0f);
        
        // transform local scale to new scale (scale self, not current value bar)

        transform.localScale = new Vector3(newScale, transform.localScale.y, transform.localScale.z);

        // Try to set current value bar material to green if winning or red if not
        // if current value bar is null, do nothing
        if (currentValueBar != null)
        {
            if (isWinning)
            {
                currentValueBar.GetComponent<Renderer>().material.color = winColor;
            }
            else
            {
                currentValueBar.GetComponent<Renderer>().material.color = loseColor;
            }
        }

    }

}
