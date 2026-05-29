using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AudioTextSync : MonoBehaviour
{
    public AudioSource audioSource;  // Audio source for playing the audio
    public TMP_Text displayText;     // Text UI element to display the lines
    public string fullText;          // Complete text to split into lines
    public float timePerLetter = 0.05f; // Time delay between each letter
    public float delayBetweenLines = 1f; // Time delay between lines
    public AudioClip audioClip;      // Audio clip to be played with the text

    private string[] lines;          // Array of lines from the text
    private int currentLine = 0;     // Index of the current line being displayed
    public Button actionButton;
    void Start()
    {
        actionButton.gameObject.SetActive(false);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        lines = fullText.Split(new[] { '\n' }, System.StringSplitOptions.None);
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
        if (lines.Length > 0)
        {
            StartCoroutine(DisplayLine(lines[currentLine]));
        }
    }

    IEnumerator DisplayLine(string line)
    {
        actionButton.gameObject.SetActive(false);
        displayText.text = "";
        foreach (char letter in line)
        {
            displayText.text += letter;
            yield return new WaitForSeconds(timePerLetter);
        }
        yield return new WaitForSeconds(delayBetweenLines);

        currentLine++;
        if (currentLine < lines.Length)
        {
            StartCoroutine(DisplayLine(lines[currentLine]));
        }
        else
        {
            actionButton.gameObject.SetActive(true);
            // Enable the button when all lines are complete

        }
    }

    // Method to dynamically assign an audio clip and text
    public void Setup(AudioClip newClip, string newText)
    {
        audioClip = newClip;
        fullText = newText;

        // Reset current line index
        currentLine = 0;

        // Restart the process
        Start();
    }

    
}
