using System;
using UnityEngine;

public abstract class BaseQuizData : ScriptableObject
{

    [TextArea(1, 5)]
    public string Question;
    [TextArea(2, 5)]
    public string Answer;

    [SerializeField]
    private string _id;
    private bool _used;

    public string GetId() => _id;
    public bool IsUsed() => _used;
    public void MarkUsed() => _used = true;
    public void ResetIsUsed() => _used = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_id))
        {
            _id = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }

    }
#endif
}
