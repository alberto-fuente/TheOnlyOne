using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
     public enum TypeOfItem { MELEE,GUN,THROWEABLE,CONSUMIBLE}
     public TypeOfItem typeOfItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableItem()
    {
        gameObject.SetActive(true);

        //animacion de sacar objeto
    }
    public void disableItem()
    {
        gameObject.SetActive(false);

        //animacion de guardar objeto
    }
}
