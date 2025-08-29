using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchTermButton : MonoBehaviour
{
    public event EventHandler<string> OnPressed;

    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _pressedSprite;

    [SerializeField] private TextMeshProUGUI _text;

    private string _termId;

    private Button _button;
    private Image _buttonImage;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _buttonImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(() =>
        {
            Debug.Log("Button clicked, id:" + _termId);
            OnPressed?.Invoke(this, _termId);
            PressButtonSprite();
        });
    }
    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
    private void PressButtonSprite() => _buttonImage.sprite = _pressedSprite;



    public void SetString(string name) => _text.text = name;

    public void SetId(string id) => _termId = id;
    public string GetId() => _termId;

    public void DisableButton() => _button.interactable = false;

    public void ResetButton()
    {
        _buttonImage.sprite = _defaultSprite;
        _button.interactable = true;

    }
}
