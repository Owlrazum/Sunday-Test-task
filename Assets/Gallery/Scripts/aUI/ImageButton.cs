using System;
using UnityEngine.UI;

public class ImageButton : Button
{
    public int Id;
    
    Action<int> _action;
    public void Initialize(Action<int> action, int id)
    {
        _action = action;
        Id = id;
        onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        _action(Id);
    }
}