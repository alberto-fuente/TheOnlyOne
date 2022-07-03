using System;
using UnityEngine;

public class HealthEventArgs : EventArgs
{
    public int Amount;
    public bool ByPlayer;
    public Transform SourceTransform;
    public HealthEventArgs(int _amount)
    {
        Amount = _amount;
    }
    public HealthEventArgs(bool _byPlayer)
    {
        ByPlayer = _byPlayer;
    }
    public HealthEventArgs(int _amount, bool _byPlayer)
    {
        Amount = _amount;
        ByPlayer = _byPlayer;
    }
    public HealthEventArgs(int _amount, bool _byPlayer, Transform _sourceTransform)
    {
        Amount = _amount;
        ByPlayer = _byPlayer;
        SourceTransform = _sourceTransform;
    }
}

