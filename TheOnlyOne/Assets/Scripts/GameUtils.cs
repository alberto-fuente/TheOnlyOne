using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtils
{
    public const float maxDamage = 100f;
    public const float maxRange = 200f;
    public const float maxFireRate = 1f;
    public enum TypeOfItem { 
        MELEE, 
        GUN, 
        THROWEABLE, 
        CONSUMIBLE 
    }

    /*public enum Rarity
    {
        COMMON,
        RARE,
        EPIC,
        LEGENDARY
    }*/

}
