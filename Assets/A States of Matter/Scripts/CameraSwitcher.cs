using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    // Reference to the GameObject to toggle
    [SerializeField] private GameObject cameraGameObject;

    // Reference to the Image component to change the sprite
    [SerializeField] private Image buttonImage;

    // Sprites for toggling
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    // Tracks the current state
    private bool isCameraActive;

    // Initialize the state in Start
    void Start()
    {
        if (cameraGameObject != null)
        {
            // Sync the initial state
            isCameraActive = cameraGameObject.activeSelf;
            UpdateButtonSprite(); // Update sprite to match the initial state
        }
        else
        {
            Debug.LogWarning("Target Object is not assigned!");
        }
    }

    // Update checks for input to toggle the GameObject and the sprite
    void Update()
    {
        // Check if the player presses the "T" key
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleActiveState();
        }
    }

    // Toggles the GameObject's active state and updates the sprite
    public void ToggleActiveState()
    {
        if (cameraGameObject != null)
        {
            // Toggle the isCameraActive variable
            isCameraActive = !isCameraActive;

            // Set the active state of the GameObject based on isCameraActive
            cameraGameObject.SetActive(isCameraActive);

            // Update the button sprite
            UpdateButtonSprite();
        }
        else
        {
            Debug.LogWarning("Target Object is not assigned!");
        }
    }

    // Updates the sprite of the button based on the current state
    private void UpdateButtonSprite()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = isCameraActive ? spriteOn : spriteOff;
        }
        else
        {
            Debug.LogWarning("Button Image is not assigned!");
        }
    }
}
