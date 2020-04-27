using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SavedControl
{
    private Type controlType;
    private KeyCode key;
    private string name;

    public Type ControlType
    {
        get => controlType;
        set
        {
            controlType = value;
            if (Changed != null)
                Changed.Invoke();
        }
    }
    public KeyCode Key
    {
        get => key;
        set
        {
            key = value;
            if (Changed != null)
                Changed.Invoke();
        }
    }
    public string Name
    {
        get => name;
        set
        {
            name = value;
            if (Changed != null)
                Changed.Invoke();
        }
    }

    public enum Type
    {
        MoveLeft,
        MoveRight,
        MoveDown,
        Rotate,
        MoveToBottom,
        SaveTetromino
    }

    [NonSerialized]
    public UnityEvent Changed;

    public SavedControl(Type controlType, KeyCode key, string name)
    {
        ControlType = controlType;
        Key = key;
        Name = name;
        Changed = new UnityEvent();
    }

    public SavedControl(SavedControl control) : this(control.ControlType, control.Key, control.Name)
    {
    }
}
