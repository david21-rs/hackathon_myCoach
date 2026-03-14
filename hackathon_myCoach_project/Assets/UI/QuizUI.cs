using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuizUI : MonoBehaviour
{
    public static QuizUI Instance;

    [Header("Wire these in the Inspector")]
    public GameObject panel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;         // 4 buttons: A, B, C, D
    public TextMeshProUGUI feedbackText;
    public GameObject feedbackPanel;
    public Button continueButton;

    private QuizData currentQuiz;
    private int currentQuestionIndex = 0;
    private int currentScore = 0;
    private Action onPassCallback;
    private Action onFailCallback;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void StartQuiz(QuizData quiz, Action onPass, Action onFail)
    {
        currentQuiz = quiz;
        currentQuestionIndex = 0;
        currentScore = 0;
        onPassCallback = onPass;
        onFailCallback = onFail;

        panel.SetActive(true);
        Time.timeScale = 0f;
        ShowQuestion();
    }

    void ShowQuestion()
    {
        feedbackPanel.SetActive(false);

        QuizQuestion q = currentQuiz.questions[currentQuestionIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;  // capture for lambda
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text
                = q.answers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int selectedIndex)
    {
        QuizQuestion q = currentQuiz.questions[currentQuestionIndex];
        bool correct = selectedIndex == q.correctAnswerIndex;

        if (correct) currentScore++;

        // Show feedback
        feedbackPanel.SetActive(true);
        feedbackText.text = correct
            ? "✓ Correct!\n" + q.correctFeedback
            : "✗ Not quite.\n" + q.wrongFeedback;

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinue);
    }

    void OnContinue()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex < currentQuiz.questions.Length)
        {
            ShowQuestion();
        }
        else
        {
            FinishQuiz();
        }
    }

    void FinishQuiz()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;

        if (currentScore >= currentQuiz.passingScore)
            onPassCallback?.Invoke();
        else
            onFailCallback?.Invoke();
    }
}