using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class MicThresholdListener : MonoBehaviour
{

    // reference to player
    public GameObject player;
    
    // reference to decibel reading component
    public decibelreading decibelReading;

    // reference to slider component
    public UnityEngine.UI.Slider slider;

    // float decibel level
    public float decibelLevel = 0.0f;

    // reference to child cube
    public GameObject cube;

    // reference to child text
    public TextMeshProUGUI text;

    // cooldown for updating levels
    public float cooldown = 0.5f;

    // sprite for not shouting
    public Sprite notShoutingSprite;

    // sprite for shouting 
    public Sprite shoutingSprite;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        decibelReading = player.GetComponent<decibelreading>();
        slider = GetComponentInChildren<UnityEngine.UI.Slider>();
        // find child ojbect called "IsShoutingText" and get its text component
        text = GameObject.Find("IsShoutingText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // update level if not on cooldown
        if (cooldown <= 0)
        {
            UpdateLevel();
            cooldown = 0.1f;
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
    }

    private void UpdateLevel()
    {
        slider.value = decibelReading.activationLevel;
        decibelLevel = MicInput.MicLoudness;
        decibelLevel = 20 * Mathf.Log10(decibelLevel);
        // convert decibels to po
        decibelLevel = (decibelLevel + 60) / 60;

        float scale = decibelLevel + 1;
        // scale cannot be below 1

        if (decibelLevel > decibelReading.activationLevel)
        {
            //text.GetComponent<TextMeshPro>().text = "Shouting";
            text.SetText("SHOUTING");
            // Set to shouting sprite
            cube.GetComponent<UnityEngine.UI.Image>().sprite = shoutingSprite;
        }
        else
        {


            //cube.GetComponent<Renderer>().material.color = Color.red;
            // change cube image color to red
            text.SetText("");
            // Set to shouting sprite
            cube.GetComponent<UnityEngine.UI.Image>().sprite = notShoutingSprite;
        }
        // scale cube
        // clamp scale between 1 and 2
        scale = Mathf.Clamp(scale, 1, 2);
        // scale up rect transform on cube
        cube.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
    }

}
