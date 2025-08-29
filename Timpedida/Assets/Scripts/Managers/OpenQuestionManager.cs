using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenQuestionManager : MonoBehaviour
{
    const int MAX_ROUNDS = 4;

    public static event EventHandler OnTriggerWheel;

    public static event EventHandler OnCorrectAnswer;
    public static event EventHandler OnIncorrectAnswer;

    public event EventHandler OnNoMoreQuestionsLeft;

    [SerializeField] private GameObject _quizContainer;

    [Header("Buttons")]
    [SerializeField] private Button _visibilityButton;
    [SerializeField] private Button _correctButton;
    [SerializeField] private Button _incorrectButton;

    [Header("Photos")]
    [SerializeField] private Sprite[] _winPhotos;
    [SerializeField] private Sprite[] _losePhotos;

    [Header("Button Sprites")]
    [SerializeField] private Image _visibilityImage;
    [SerializeField] private Sprite _showSprite;
    [SerializeField] private Sprite _hideSprite;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TextMeshProUGUI _answerText;

    [Header("Animations")]
    [SerializeField] private GameObject _answerContainer;
    [SerializeField] private float _animaitonDuration;
    [SerializeField] private GameObject _photoAnimationPrefab;

    private bool _isRoundComplete;
    private int _roundsPlayed;

    private OpenQuestionData[] _openQuestionData;

    private CanvasGroup _answerCanvasGroup;

    //Photos handling
    RandomPicker<Sprite> _rndVictoryPhotos;
    RandomPicker<Sprite> _rndLosePhotos;

    public void Init(OpenQuestionData[] openQuestionData)
    {
        _openQuestionData = openQuestionData;

        WheelManager.OnWheelEnd += HandleWheelStopped;

        _answerCanvasGroup = _answerContainer.GetComponent<CanvasGroup>();
        _answerCanvasGroup.alpha = 0.0f;

        _rndVictoryPhotos = new RandomPicker<Sprite>(_winPhotos);
        _rndLosePhotos = new RandomPicker<Sprite>(_losePhotos);

        InitButtons();
    }

    public IEnumerator Play()
    {
        if (_openQuestionData.Any(data => data.IsUsed() == false))
        {
            _isRoundComplete = false;
            _roundsPlayed = 0;

            yield return StartCoroutine(GameFlow());
        }
        else
            OnNoMoreQuestionsLeft?.Invoke(this, EventArgs.Empty);
    }

    private void HandleWheelStopped(object sender, EventArgs e) => _isRoundComplete = true;

    private void InitButtons()
    {
        _visibilityButton.onClick.AddListener(() => ToggleAnswerVisibility());

        _correctButton.onClick.AddListener(() => StartCoroutine(CorrectAnswer()));
        _incorrectButton.onClick.AddListener(() => StartCoroutine(IncorrectAnswer()));

        _correctButton.gameObject.SetActive(false);
        _incorrectButton.gameObject.SetActive(false);
    }


    private void ToggleAnswerVisibility()
    {
        if (_answerCanvasGroup.alpha > 0.0f)
        {
            _answerCanvasGroup.alpha = 0.0f;
            _visibilityImage.sprite = _showSprite;
        }
        else
        {
            _visibilityImage.sprite = _hideSprite;
            StartCoroutine(FadeInAnswer());
        }
    }
    private IEnumerator FadeInAnswer()
    {
        float elapsedTime = 0.0f;
        float targetAlpha = 1.0f;
        float initAlpha = 0.0f;

        while (elapsedTime < _animaitonDuration)
        {
            _answerCanvasGroup.alpha = Mathf.Lerp(initAlpha, targetAlpha, elapsedTime / _animaitonDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _answerCanvasGroup.alpha = targetAlpha;

        _correctButton.gameObject.SetActive(true);
        _incorrectButton.gameObject.SetActive(true);
    }

    private IEnumerator CorrectAnswer()
    {
        ResetUI();

        OnCorrectAnswer?.Invoke(this, EventArgs.Empty);

        TriggerPhotoAnimation(_rndVictoryPhotos.GetNext());

        yield return PostProcessingManager.Instance.TriggerConfettiAnimation();
        OnTriggerWheel?.Invoke(this, EventArgs.Empty);


    }
    private IEnumerator IncorrectAnswer()
    {
        ResetUI();

        OnIncorrectAnswer?.Invoke(this, EventArgs.Empty);

        TriggerPhotoAnimation(_rndLosePhotos.GetNext());

        yield return PostProcessingManager.Instance.TriggerViggnetteAnimation();
        OnTriggerWheel?.Invoke(this, EventArgs.Empty);

    }

    private IEnumerator GameFlow()
    {
        _quizContainer.SetActive(true);

        while (_roundsPlayed < MAX_ROUNDS)
        {
            RoundStart();
            yield return RoundPlaying();
            RoundEnd();
        }

        _quizContainer.SetActive(false);
    }
    private void RoundStart()
    {
        _isRoundComplete = false;

        OpenQuestionData data = _openQuestionData.FirstOrDefault(data => data.IsUsed() == false);
        data.MarkUsed();

        _visibilityButton.interactable = true;

        _questionText.text = data.Question;
        _answerText.text = data.Answer;
    }
    private IEnumerator RoundPlaying()
    {
        while (!_isRoundComplete)
            yield return null;
    }
    private void RoundEnd()
    {
        _roundsPlayed++;
    }

    private void ResetUI()
    {
        _correctButton.gameObject.SetActive(false);
        _incorrectButton.gameObject.SetActive(false);

        _visibilityButton.interactable = false;

        ToggleAnswerVisibility();
    }

    private void TriggerPhotoAnimation(Sprite photo)
    {
        Animator animator = Instantiate(_photoAnimationPrefab).GetComponent<Animator>();
        float animationDuration = animator.runtimeAnimatorController.animationClips[0].length;

        SpriteRenderer spriteRenderer = animator.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = photo;

        Destroy(animator.gameObject, animationDuration);
    }
}
