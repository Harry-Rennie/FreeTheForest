using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour
{
    [SerializeField] public TMP_Text messageText;
    private string dialogue;
    [SerializeField] float timePerCharacter;
    private float timer;
    private int characterIndex;

    private void Awake()
    {
        float health = PlayerInfoController.instance.MaxHealth * 0.25f;
        int healthInt = (int)health;
       messageText.text = "A mysterious entity whispers in your ear... You gain " + healthInt.ToString() + " health." ;
       dialogue = messageText.text;
    }

    public void AddWriter(string dialogue, float timePerCharacter)
    {
        this.timePerCharacter = timePerCharacter;
        timer = timePerCharacter; // Start immediately on the first character
        characterIndex = 0; // Reset the characterIndex when adding new dialogue

        messageText.text = dialogue; // Update the text content with the new dialogue
    }

    private void Update()
    {
        if (messageText.text != null && characterIndex < dialogue.Length) // Check if characterIndex is within the dialogue string's length
        {
            timer -= Time.deltaTime;
            if (timer <= 0f) // Check if the timer has reached or passed 0
            {
                timer = timePerCharacter; // Reset the timer for the next character
                characterIndex++;
                messageText.text = dialogue.Substring(0, characterIndex); // Update the text to show characters up to characterIndex
            }
        }
    }
}