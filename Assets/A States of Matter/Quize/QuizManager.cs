using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public GameObject QuizeCanvas;
    public TMP_Text QuestionText;
    public GameObject MultipleChoicePanel;
    public Button[] OptionButtons;
    public GameObject TrueFalsePanel;
    public Button TrueButton;
    public Button FalseButton;

    public GameObject PopupPanel; // The popup panel
    public TMP_Text FeedbackText; // Text element in the popup
    public Button NextButton; // Button to proceed to the next question
    public Button RetryButton; // Button to retry the current question
    public Button OKButton; // Button to close the quiz

    public QuizData[] AllQuizData; // Array to hold all quiz data assets
    private QuizData currentQuizData; // The currently selected quiz data
    private int currentQuestionIndex = 0;
    public Sprite CorrectSprite; // Sprite to show when the answer is correct
    public Sprite IncorrectSprite;
    public Image FeedbackImage;
    public Color customColor;

    void Start()
    {
        // Assign listeners to the buttons
        NextButton.onClick.AddListener(NextQuestion);
        RetryButton.onClick.AddListener(RetryQuestion);
        OKButton.onClick.AddListener(CloseQuiz);

        PopupPanel.SetActive(false); // Ensure popup starts hidden
    }

    public void SelectQuiz(int index)
    {
        if (index >= 0 && index < AllQuizData.Length)
        {
            currentQuizData = AllQuizData[index];
            currentQuestionIndex = 0; // Reset the question index for the new quiz
            ShowQuestion();
        }
        else
        {
            Debug.LogError("Invalid quiz index selected!");
        }
    }

    void ShowQuestion()
    {
        PopupPanel.SetActive(false); // Hide the popup

        if (currentQuizData == null)
        {
            Debug.LogError("No quiz data selected!");
            return;
        }

        // Reset button colors to default for all option buttons
        foreach (var button in OptionButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }

        // Reset True/False button colors
        TrueButton.GetComponent<Image>().color = Color.white;
        FalseButton.GetComponent<Image>().color = Color.white;

        QuizQuestion currentQuestion = currentQuizData.Questions[currentQuestionIndex];
        QuestionText.text = currentQuestion.QuestionText;

        if (currentQuestion.IsTrueFalse)
        {
            // Show True/False UI
            TrueFalsePanel.SetActive(true);
            MultipleChoicePanel.SetActive(false);

            TrueButton.onClick.RemoveAllListeners();
            FalseButton.onClick.RemoveAllListeners();

            TrueButton.onClick.AddListener(() => CheckAnswer(true));
            FalseButton.onClick.AddListener(() => CheckAnswer(false));
        }
        else
        {
            // Show Multiple Choice UI
            MultipleChoicePanel.SetActive(true);
            TrueFalsePanel.SetActive(false);

            for (int i = 0; i < OptionButtons.Length; i++)
            {
                if (i < currentQuestion.Options.Length)
                {
                    OptionButtons[i].gameObject.SetActive(true);
                    OptionButtons[i].GetComponentInChildren<TMP_Text>().text = currentQuestion.Options[i];
                    int index = i; // Capture loop variable
                    OptionButtons[i].onClick.RemoveAllListeners();
                    OptionButtons[i].onClick.AddListener(() => CheckAnswer(index));
                }
                else
                {
                    OptionButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void CheckAnswer(bool answer)
    {
        QuizQuestion currentQuestion = currentQuizData.Questions[currentQuestionIndex];

        if (currentQuestion.IsTrueFalse)
        {
            if (currentQuestion.CorrectAnswer == answer)
            {
                ShowPopup(currentQuestion.CorrectFeedback, true);
            }
            else
            {
                ShowPopup(currentQuestion.IncorrectFeedback, false);
            }
        }
    }

    void CheckAnswer(int selectedIndex)
    {
        QuizQuestion currentQuestion = currentQuizData.Questions[currentQuestionIndex];

        // Reset all button colors to default before checking
        foreach (var button in OptionButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }

        if (!currentQuestion.IsTrueFalse)
        {
            if (currentQuestion.CorrectOptionIndex == selectedIndex)
            {
                OptionButtons[selectedIndex].GetComponent<Image>().color = Color.green; // Correct answer
                ShowPopup(currentQuestion.CorrectFeedback, true);
            }
            else
            {
                
                OptionButtons[selectedIndex].GetComponent<Image>().color =customColor; // Incorrect answer
                OptionButtons[currentQuestion.CorrectOptionIndex].GetComponent<Image>().color = Color.green; // Highlight the correct answer
                ShowPopup(currentQuestion.IncorrectFeedback, false);
            }
        }
    }
   
    void ShowPopup(string feedback, bool isCorrect)
    {
        FeedbackText.text = feedback; // Set feedback text

        // Update the feedback image based on whether the answer is correct
        if (isCorrect)
        {
            FeedbackImage.sprite = CorrectSprite; // Show correct image
            TMP_Text feedbackChildText = FeedbackImage.gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
            feedbackChildText.text = "Correct";
            feedbackChildText.color = Color.green;
        }
        else
        {
            FeedbackImage.sprite = IncorrectSprite; // Show incorrect image
            TMP_Text feedbackChildText = FeedbackImage.gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
            feedbackChildText.text = "Incorrect";
            feedbackChildText.color = customColor;

        }

        // Make sure the FeedbackImage is visible
        FeedbackImage.gameObject.SetActive(true);

        PopupPanel.SetActive(true); // Show the popup

        bool isLastQuestion = currentQuestionIndex == currentQuizData.Questions.Length - 1;

        if (isLastQuestion)
        {
            if (isCorrect)
            {
                OKButton.gameObject.SetActive(true);
                NextButton.gameObject.SetActive(false);
                RetryButton.gameObject.SetActive(false);
            }
            else
            {
                OKButton.gameObject.SetActive(true);
                NextButton.gameObject.SetActive(false);
                RetryButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (isCorrect)
            {
                OKButton.gameObject.SetActive(false);
                NextButton.gameObject.SetActive(true);
                RetryButton.gameObject.SetActive(false);
            }
            else
            {
                OKButton.gameObject.SetActive(false);
                NextButton.gameObject.SetActive(true);
                RetryButton.gameObject.SetActive(true);
            }
        }
    }


    void RetryQuestion()
    {
        PopupPanel.SetActive(false); // Hide the popup
        ShowQuestion(); // Re-display the current question
    }

    void NextQuestion()
    {
        PopupPanel.SetActive(false); // Hide the popup
        currentQuestionIndex++;

        if (currentQuestionIndex < currentQuizData.Questions.Length)
        {
            ShowQuestion();
        }
        else
        {
            Debug.Log("Quiz Completed!");
            // Add logic for end-of-quiz behavior
        }
    }

    void CloseQuiz()
    {
        // Close or end the quiz, such as by loading a new scene or quitting
        Debug.Log("Quiz Closed!");
        // Optionally, use SceneManager to load a different scene
        // SceneManager.LoadScene("MainMenu"); 
    }
}
