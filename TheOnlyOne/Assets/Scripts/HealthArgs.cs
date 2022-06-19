using System;
using UnityEngine;

public class HealthArgs : EventArgs
{
    public int Amount;
    public bool ByPlayer;
    public Transform SourceTransform;
    public HealthArgs(int _amount)
    {
        Amount = _amount;
    }
    public HealthArgs(bool _byPlayer)
    {
        ByPlayer = _byPlayer;
    }
    public HealthArgs(int _amount,bool _byPlayer)
    {
        Amount = _amount;
        ByPlayer = _byPlayer;
    }
    public HealthArgs(int _amount, bool _byPlayer, Transform _sourceTransform)
    {
        Amount = _amount;
        ByPlayer = _byPlayer;
        SourceTransform = _sourceTransform;
    }
}

