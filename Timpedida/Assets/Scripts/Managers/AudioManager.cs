using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioClips")]
    [SerializeField] private AudioClip _correctAudio;
    [SerializeField] private AudioClip _incorrectAudio;


    private AudioSource _audioSourceOneShot;
    private AudioSource _audioSourceLooping;


    private void Awake()
    {
        _audioSourceOneShot = new GameObject("AudioSourceOS").AddComponent<AudioSource>();
        _audioSourceOneShot.loop = false;
        _audioSourceOneShot.playOnAwake = false;
    }

    private void Start()
    {
        OpenQuestionManager.OnIncorrectAnswer += HandleIncorrectAnswer;
        OpenQuestionManager.OnCorrectAnswer += HandleCorrectAnswer;

        MatchTermManager.OnIncorrectAnswer += HandleIncorrectAnswer;
        MatchTermManager.OnCorrectAnswer += HandleCorrectAnswer;
    }

    private void HandleIncorrectAnswer(object sender, System.EventArgs e) => _audioSourceOneShot.PlayOneShot(_incorrectAudio);
    private void HandleCorrectAnswer(object sender, System.EventArgs e) => _audioSourceOneShot.PlayOneShot(_correctAudio);

}
