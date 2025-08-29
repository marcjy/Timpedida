using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static event EventHandler OnStartGame;
    public static event EventHandler OnEndGame;

    [Header("Data")]
    [SerializeField] private BaseQuizData[] _quizData;

    [Header("Managers")]
    [SerializeField] private OpenQuestionManager _openQuestionManager;
    [SerializeField] private MatchTermManager _matchTermManager;

    private bool _hasOpenQuestions = true;
    private bool _hasMatchQuestions = true;

    private void Start()
    {
        ResetQuizDataUsed();

        Debug.Log("----First Id----" + _quizData.OfType<MatchData>().FirstOrDefault().GetId());

        _openQuestionManager.Init(_quizData.OfType<OpenQuestionData>().ToArray());
        _matchTermManager.Init(_quizData.OfType<MatchData>().ToArray());

        _openQuestionManager.OnNoMoreQuestionsLeft += HandleNoMoreOpenQuestions;
        _matchTermManager.OnNoMoreQuesitonsLeft += HandleNoMoreTermsToMatch;

        StartCoroutine(GameFlow());
    }

    private void HandleNoMoreOpenQuestions(object sender, System.EventArgs e) => _hasOpenQuestions = false;
    private void HandleNoMoreTermsToMatch(object sender, System.EventArgs e) => _hasMatchQuestions = false;

    private void ResetQuizDataUsed()
    {
        foreach (BaseQuizData data in _quizData)
            data.ResetIsUsed();
    }

    private IEnumerator GameFlow()
    {
        OnStartGame?.Invoke(this, EventArgs.Empty);

        while (true)
        {
            if (_hasOpenQuestions)
                yield return _openQuestionManager.Play();
            if (_hasMatchQuestions)
                yield return _matchTermManager.Play();

            if (!_hasOpenQuestions && !_hasMatchQuestions)
                break;
        }

        OnEndGame?.Invoke(this, EventArgs.Empty);

        SceneManager.LoadScene("End");
    }
}