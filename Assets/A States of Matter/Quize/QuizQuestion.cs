[System.Serializable]
public class QuizQuestion
{
    public string QuestionText; // The question
    public string[] Options; // Options for multiple-choice questions
    public int CorrectOptionIndex; // Correct index for multiple-choice questions
    public bool IsTrueFalse; // Is this a true/false question
    public bool CorrectAnswer; // Correct answer for true/false questions
    public string CorrectFeedback; // Feedback for correct answers
    public string IncorrectFeedback; // Feedback for incorrect answers
}
