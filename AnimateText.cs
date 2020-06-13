using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimateText : MonoBehaviour{
    string[] text;
    float timeElapsed = 0f, interval = 5f;

    // Start is called before the first frame update
    void Start(){
        text = new string[] { 
            "The AI will show you how to play",
            "Tap anywhere on the screen to flap\n upwards",
            "Let go to have the parrot fall\n by himself",
            "Try to flap through as many pipes\n as you can",
            "Avoid hitting pipes or falling\n to the ground",
            "Be careful to approach each pipe\n at the right angle",
            "Up for the challenge?",
            "Tap 'Back to Main Menu' then 'Play' to\n take your chances",
            "Don't forget to upload your high score!",
            "Good luck :-)",
            "Or you can hang around and see how far\n the bot makes it ;-)",
            ""
        };
    }

    void FixedUpdate(){
        int index = Convert.ToInt32(Mathf.Floor(timeElapsed / interval));
        if (index > text.Length - 1)
            index = text.Length - 1;
        gameObject.GetComponent<Text>().text = text[index];
        timeElapsed += Time.fixedDeltaTime;
    }
}
