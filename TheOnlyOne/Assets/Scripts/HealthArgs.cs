using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthArgs : EventArgs
{
    public int Amount;
    public bool ByPlayer;
    public Transform SourceTransform;
    public HealthArgs(int amount)
    {
        Amount = amount;
    }
    public HealthArgs(bool byPlayer)
    {
        ByPlayer = byPlayer;
    }
    public HealthArgs(int amount,bool byPlayer)
    {
        Amount = amount;
        ByPlayer = byPlayer;
    }
    public HealthArgs(int amount, bool byPlayer, Transform sourceTransform)
    {
        Amount = amount;
        ByPlayer = byPlayer;
        SourceTransform = sourceTransform;
    }
}

