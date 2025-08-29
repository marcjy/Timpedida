using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager Instance;

    [SerializeField] private Volume _postProcessingVolume;
    [SerializeField] private float _flashScreenInterval;
    [SerializeField] private float _vignetteAnimationDuration = 4.0f;

    [SerializeField] private ParticleSystem _confetti;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public IEnumerator TriggerViggnetteAnimation()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < _vignetteAnimationDuration)
        {
            yield return FadePostProcesing(0.75f, 1.0f, _flashScreenInterval);
            elapsedTime += _flashScreenInterval;

            yield return FadePostProcesing(1.0f, 0.75f, _flashScreenInterval);
            elapsedTime += _flashScreenInterval;
        }

        _postProcessingVolume.weight = 0.0f;
    }
    private IEnumerator FadePostProcesing(float from, float target, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            _postProcessingVolume.weight = Mathf.Lerp(from, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _postProcessingVolume.weight = target;
    }

    public IEnumerator TriggerConfettiAnimation()
    {
        _confetti.Play();
        yield return new WaitForSeconds(_confetti.main.duration);
    }
}