using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpText : MonoBehaviour
{
    private float destroyTime = 2f;
    private Vector3 initialOffset = new Vector3(0,0f,0f);
    private Vector3 positionRandomize = new Vector3(0.5f, 0.2f, 0.1f);
    public Color shieldColor = new Color(0f,178f,200f);
    public Color noShieldColor = new Color(218f, 19f, 23f);
    public TMP_Text _text;
    public Material material;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        material = GetComponent<Renderer>().material;
        Destroy(gameObject, destroyTime);
    }

}
