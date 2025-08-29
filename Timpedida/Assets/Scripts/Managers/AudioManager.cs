using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioClips")]
    [SerializeField] private AudioClip _correctAudio;
    [SerializeField] private AudioClip _incorrectAudio;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip[] _bgmClips;
    [SerializeField] private float _waitSecondsBetweenClips;

    private AudioSource _audioSourceOneShot;

    //BGM
    private AudioSource _audioSourceBMG;
    private bool _isBGMEnabled = false;

    private void Awake()
    {
        _audioSourceOneShot = new GameObject("AudioSourceOS").AddComponent<AudioSource>();
        _audioSourceOneShot.gameObject.transform.SetParent(gameObject.transform);
        _audioSourceOneShot.loop = false;
        _audioSourceOneShot.playOnAwake = false;

        _audioSourceBMG = new GameObject("AudioSourceBGM").AddComponent<AudioSource>();
        _audioSourceBMG.gameObject.transform.SetParent(gameObject.transform);
        _audioSourceBMG.loop = false;
        _audioSourceBMG.playOnAwake = true;
        _audioSourceBMG.volume = 0.25f;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        OpenQuestionManager.OnIncorrectAnswer += HandleIncorrectAnswer;
        OpenQuestionManager.OnCorrectAnswer += HandleCorrectAnswer;

        MatchTermManager.OnIncorrectAnswer += HandleIncorrectAnswer;
        MatchTermManager.OnCorrectAnswer += HandleCorrectAnswer;

        GameManager.OnStartGame += HandleStartGame;
        GameManager.OnEndGame += HandleEndGame;
    }

    private void OnDisable()
    {
        OpenQuestionManager.OnCorrectAnswer -= HandleCorrectAnswer;
        OpenQuestionManager.OnIncorrectAnswer -= HandleIncorrectAnswer;

        MatchTermManager.OnCorrectAnswer -= HandleCorrectAnswer;
        MatchTermManager.OnIncorrectAnswer -= HandleIncorrectAnswer;
    }


    private void HandleCorrectAnswer(object sender, System.EventArgs e) => _audioSourceOneShot.PlayOneShot(_correctAudio);
    private void HandleIncorrectAnswer(object sender, System.EventArgs e) => _audioSourceOneShot.PlayOneShot(_incorrectAudio);

    private void HandleStartGame(object sender, System.EventArgs e)
    {
        _isBGMEnabled = true;

        StartCoroutine(PlayBGM());
    }
    private void HandleEndGame(object sender, System.EventArgs e)
    {
        _isBGMEnabled = false;
    }

    private IEnumerator PlayBGM()
    {
        int clipsIndex = 0;
        int nClips = _bgmClips.Length;

        while (_isBGMEnabled)
        {
            _audioSourceBMG.clip = _bgmClips[clipsIndex];
            _audioSourceBMG.Play();

            yield return new WaitWhile(() => _audioSourceBMG.isPlaying);
            yield return new WaitForSeconds(_waitSecondsBetweenClips);

            clipsIndex = (clipsIndex + 1) % nClips;
        }

        _audioSourceBMG.Stop();
        _audioSourceBMG.clip = null;
    }


}
