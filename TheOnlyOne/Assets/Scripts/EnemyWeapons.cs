using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapons : MonoBehaviour
{
    [SerializeField] private WeaponBlueprint[] weaponsAvailable;
    private WeaponBlueprint weaponData;
    [SerializeField] private Transform anchorPoint;
    private void Awake()
    {
        weaponData = weaponsAvailable[Random.Range(0, weaponsAvailable.Length)];
        Instantiate(weaponData.enemyPrefab,anchorPoint.position,anchorPoint.rotation,anchorPoint);
    }

}
