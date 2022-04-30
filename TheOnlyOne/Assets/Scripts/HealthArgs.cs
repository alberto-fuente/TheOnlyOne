using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthArgs : EventArgs
{
    public int Amount;
    public HealthArgs(int amount)
    {
        Amount = amount;
    }
}
