using UnityEngine;

[CreateAssetMenu(fileName = "NewQuiz", menuName = "THAWFALL/Quiz Data")]
public class QuizData : ScriptableObject
{
    public string quizTitle;
    public int passingScore = 3;         // need 3/4 to pass
    public QuizQuestion[] questions;
}

[System.Serializable]
public class QuizQuestion
{
    [TextArea(2, 4)]
    public string questionText;

    public string[] answers = new string[4];  // A, B, C, D

    public int correctAnswerIndex;        // 0=A  1=B  2=C  3=D

    [TextArea(2, 5)]
    public string wrongFeedback;          // shown when player picks wrong answer

    [TextArea(2, 5)]
    public string correctFeedback;        // shown when player picks right answer
}