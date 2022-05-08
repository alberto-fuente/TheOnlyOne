using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]

public class EnemyBlueprint : ScriptableObject
{
    public int enemyId;
    public GameObject enemyPrefab;
    public int minDamage;
    public int maxDamage;
    public float attackDelay;
    [Range(0,1)]
    public float hitProbability;
    public AudioClip shootSound;
    //agent
    public float sightRange;
    public float attackRange;
    public float stopChasingRange;

}
