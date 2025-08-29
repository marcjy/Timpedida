using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchTermManager : MonoBehaviour
{
    const int MAX_TERM_DISPLAY = 4;

    public static event EventHandler OnTriggerWheel;

    public static event EventHandler OnCorrectAnswer;
    public static event EventHandler OnIncorrectAnswer;

    public event EventHandler OnNoMoreQuesitonsLeft;

    [Header("Button prefabs")]
    public MatchTermButton MatchTermButtonPrefab;
    public MatchTermButton MatchDefinitionButtonPrefab;

    [Header("Containers")]
    [SerializeField] private GameObject _quizContainer;
    [SerializeField] private GameObject _nameContainer;
    [SerializeField] private GameObject _definitionContainer;

    [Header("CanvasGroups")]
    [SerializeField] private CanvasGroup _nameCanvas;
    [SerializeField] private CanvasGroup _descriptionCanvas;

    [Header("VictoryAnimation")]
    [SerializeField] private GameObject _victoryPrefab;

    private MatchData[] _matchData;
    private TermType _currentTermType = TermType.Fire;

    //Lists
    private List<MatchTermButton> _terms = new List<MatchTermButton>();
    private List<MatchTermButton> _definitions = new List<MatchTermButton>();

    //Id strings
    private string _termIdChoosen = String.Empty;
    private string _definitionIdChoosen = String.Empty;

    //Game flow handling
    private int _correctMatches = 0;
    private bool _matchIsCorrect = true;

    //Wheel handling
    private bool _wheelAnimationEnded = false;


    public void Init(MatchData[] matchData)
    {
        _matchData = matchData;

        WheelManager.OnWheelEnd += HandleWheelStopped;
    }

    private void HandleWheelStopped(object sender, EventArgs e) => _wheelAnimationEnded = true;


    public IEnumerator Play()
    {
        MatchData[] dataSet = GetDataSet();

        if (dataSet == null || dataSet.Length == 0)
        {
            OnNoMoreQuesitonsLeft?.Invoke(this, EventArgs.Empty);
            yield break;
        }

        yield return StartCoroutine(GameFlow(dataSet));
    }

    private void ResetCanvases()
    {
        _nameCanvas.interactable = true;
        _descriptionCanvas.interactable = true;
    }
    private void ResetContainers()
    {
        for (int i = 0; i < _nameContainer.transform.childCount; i++)
            Destroy(_nameContainer.transform.GetChild(i).gameObject);

        for (int i = 0; i < _definitionContainer.transform.childCount; i++)
            Destroy(_definitionContainer.transform.GetChild(i).gameObject);
    }
    private void ResetLists()
    {
        _terms.Clear();
        _definitions.Clear();
    }
    private void ResetButtons()
    {
        foreach (MatchTermButton termButton in _terms)
            termButton.ResetButton();

        foreach (MatchTermButton definitionButton in _definitions)
            definitionButton.ResetButton();
    }

    private void HandleTermButtonPressed(object sender, string id)
    {
        _nameCanvas.interactable = false;
        _termIdChoosen = id;

        CheckIfMatchIsCorrect();
    }
    private void HandleDefinitionTermPressed(object sender, string id)
    {
        _descriptionCanvas.interactable = false;
        _definitionIdChoosen = id;

        CheckIfMatchIsCorrect();
    }

    private void CreateButtons(MatchData[] dataSet)
    {
        foreach (MatchData matchData in dataSet)
        {
            MatchTermButton matchButtonTerm = Instantiate(MatchTermButtonPrefab);
            MatchTermButton matchButtonDefinition = Instantiate(MatchDefinitionButtonPrefab);

            string sharedId = matchData.GetId();

            matchButtonTerm.SetString(matchData.Question);
            matchButtonTerm.SetId(sharedId);
            matchButtonTerm.OnPressed += HandleTermButtonPressed;

            matchButtonDefinition.SetString(matchData.Answer);
            matchButtonDefinition.SetId(sharedId);
            matchButtonDefinition.OnPressed += HandleDefinitionTermPressed; ;

            _terms.Add(matchButtonTerm);
            _definitions.Add(matchButtonDefinition);
        }
    }
    private void AssignButtons()
    {
        foreach (MatchTermButton term in _terms)
            term.gameObject.transform.SetParent(_nameContainer.transform, false);

        foreach (MatchTermButton definition in _definitions)
            definition.gameObject.transform.SetParent(_definitionContainer.transform, false);
    }
    private void ShuffleButtons()
    {
        ShuffleChildren(_nameCanvas.transform);
        ShuffleChildren(_definitionContainer.transform);
    }
    private void ShuffleChildren(Transform parent)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parent)
            children.Add(child);

        for (int i = children.Count - 1; i > 0; i--)
        {
            int rand = UnityEngine.Random.Range(0, i + 1);
            Transform tmp = children[i];
            children[i] = children[rand];
            children[rand] = tmp;
        }

        // Apply new sibling order
        for (int i = 0; i < children.Count; i++)
            children[i].SetSiblingIndex(i);
    }

    private MatchData[] GetDataSet()
    {
        int nTermTypes = Enum.GetValues(typeof(TermType)).Length;
        int nTries = 0;
        MatchData[] data = Array.Empty<MatchData>();

        while (data.Length == 0 && nTries < nTermTypes)
        {
            data = _matchData
                .Where(data => data.TermType == _currentTermType && data.IsUsed() == false)
                .Take(MAX_TERM_DISPLAY)
                .ToArray();

            foreach (MatchData d in data)
                d.MarkUsed();

            NextTermTypeIteration();

            nTries++;
        }

        return data;
    }
    private void NextTermTypeIteration()
    {
        switch (_currentTermType)
        {
            case TermType.Fire:
                _currentTermType = TermType.Shit;
                break;
            case TermType.Shit:
                _currentTermType = TermType.Food;
                break;
            case TermType.Food:
                _currentTermType = TermType.Fire;
                break;
            default: break;
        }
    }

    private void CheckIfMatchIsCorrect()
    {
        Debug.Log("Called CheckIfMatchIsCorrect");
        Debug.Log("TermdId" + _termIdChoosen);
        Debug.Log("DescriptionId" + _definitionIdChoosen);

        if (!String.IsNullOrEmpty(_termIdChoosen) && !String.IsNullOrEmpty(_definitionIdChoosen))
        {
            _matchIsCorrect = _termIdChoosen == _definitionIdChoosen;

            if (_matchIsCorrect)
                _correctMatches++;
            Debug.Log("Match is" + _matchIsCorrect);

            Debug.Log("Disabling correct buttons");
            //Disable correct buttons
            _terms
                .FirstOrDefault(b => b.GetId() == _termIdChoosen)
                .DisableButton();
            _definitions
                .FirstOrDefault(b => b.GetId() == _definitionIdChoosen)
                .DisableButton();
            Debug.Log("Disabled");

            _termIdChoosen = string.Empty;
            _definitionIdChoosen = string.Empty;


            Debug.Log("Enabling canvases");
            _nameCanvas.interactable = true;
            _descriptionCanvas.interactable = true;
            Debug.Log("Enabled");

        }
    }

    private IEnumerator GameFlow(MatchData[] dataSet)
    {
        _quizContainer.SetActive(true);
        _correctMatches = 0;

        CreateButtons(dataSet);
        AssignButtons();

        while (_correctMatches < MAX_TERM_DISPLAY)
        {
            RoundStart();

            yield return RoundPlaying();

            RoundEnd();
        }

        ResetContainers();
        ResetLists();

        _quizContainer.SetActive(false);
    }

    private void RoundStart()
    {
        ShuffleButtons();

        _correctMatches = 0;
        _matchIsCorrect = true;

        _wheelAnimationEnded = false;
    }
    private IEnumerator RoundPlaying()
    {
        while (_matchIsCorrect && _correctMatches < MAX_TERM_DISPLAY)
            yield return null;

        if (_matchIsCorrect)
            yield return CorrectAnswer();
        else
            yield return IncorrectAnswer();

        while (!_wheelAnimationEnded)
            yield return null;
    }
    private void RoundEnd()
    {
        ResetCanvases();
        ResetButtons();
    }

    private IEnumerator CorrectAnswer()
    {
        _nameCanvas.interactable = false;
        _descriptionCanvas.interactable = false;

        OnCorrectAnswer?.Invoke(this, EventArgs.Empty);
        TriggerVictoryAnimation();
        yield return PostProcessingManager.Instance.TriggerConfettiAnimation();
        OnTriggerWheel?.Invoke(this, EventArgs.Empty);
    }
    private IEnumerator IncorrectAnswer()
    {
        _nameCanvas.interactable = false;
        _descriptionCanvas.interactable = false;

        OnIncorrectAnswer?.Invoke(this, EventArgs.Empty);
        yield return PostProcessingManager.Instance.TriggerViggnetteAnimation();
        OnTriggerWheel?.Invoke(this, EventArgs.Empty);
    }

    private void TriggerVictoryAnimation()
    {
        Animator animator = Instantiate(_victoryPrefab).GetComponent<Animator>();
        float animationDuration = animator.runtimeAnimatorController.animationClips[0].length;

        Destroy(animator.gameObject, animationDuration * 2);
    }
}
