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
            Changed.Invoke();
        }
    }
    public KeyCode Key
    {
        get => key;
        set
        {
            key = value;
            Changed.Invoke();
        }
    }
    public string Name
    {
        get => name;
        set
        {
            name = value;
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
        Changed = new UnityEvent();
        ControlType = controlType;
        Key = key;
        Name = name;
    }

    public SavedControl(SavedControl control) : this(control.ControlType, control.Key, control.Name)
    {
    }
}
