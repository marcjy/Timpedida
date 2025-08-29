using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{
    public static event EventHandler OnWheelEnd;

    [SerializeField] private GameObject _wheelContainer;
    [SerializeField] private RectTransform _wheel;

    [Header("Spins")]
    [SerializeField] private int _minSpins = 3;
    [SerializeField] private int _maxSpins = 6;
    [SerializeField] private float _spinDuration = 3.0f;

    [Header("Button")]
    [SerializeField] private Button _nextButton;

    [Header("Audio")]
    [SerializeField] private AudioClip _wheelSpinning;
    private AudioSource _wheelAudioSource;

    private void Start()
    {
        OpenQuestionManager.OnTriggerWheel += HandleTriggerWheel;
        MatchTermManager.OnTriggerWheel += HandleTriggerWheel;

        _wheelAudioSource = new GameObject("WheelAudioSource").AddComponent<AudioSource>();
        _wheelAudioSource.clip = _wheelSpinning;
        _wheelAudioSource.loop = true;
        _wheelAudioSource.playOnAwake = false;

        InitNextButton();
    }

    private void InitNextButton()
    {
        _nextButton.onClick.AddListener(() =>
        {
            OnWheelEnd?.Invoke(this, EventArgs.Empty);
            ToggleWheelVisibility();
        });
    }

    private void HandleTriggerWheel(object sender, System.EventArgs e)
    {
        StartCoroutine(SpinWheel());
        _nextButton.gameObject.SetActive(false);
    }

    private IEnumerator SpinWheel()
    {
        ToggleWheelVisibility();

        _wheelAudioSource.Play();


        int randomSection = UnityEngine.Random.Range(0, 8);
        float targetAngle = 45.0f * randomSection; //45º

        int extraSpins = UnityEngine.Random.Range(_minSpins, _maxSpins + 1);
        float totalRotation = 360 * extraSpins + targetAngle; //360º

        float startRotation = _wheel.rotation.eulerAngles.z;
        float elapsedTime = 0.0f;

        float previousRotation = startRotation;

        while (elapsedTime < _spinDuration)
        {
            float t = elapsedTime / _spinDuration;

            float eased = 1.0f - Mathf.Pow(1.0f - t, 3.0f);

            float newRotationZ = Mathf.Lerp(startRotation, startRotation + totalRotation, eased);
            _wheel.rotation = Quaternion.Euler(0, 0, newRotationZ);

            float angularSpeed = Mathf.Abs(newRotationZ - previousRotation) / Time.deltaTime;
            float maxAngularsSpeed = 720.0f;
            _wheelAudioSource.pitch = Math.Clamp(angularSpeed / maxAngularsSpeed, 0.1f, 1.5f);

            previousRotation = newRotationZ;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _wheel.rotation = Quaternion.Euler(0, 0, startRotation + totalRotation);
        _nextButton.gameObject.SetActive(true);

        _wheelAudioSource.Stop();
        _wheelAudioSource.pitch = 1.0f;

    }

    private void ToggleWheelVisibility() => _wheelContainer.SetActive(!_wheelContainer.activeSelf);

}
