using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpText : MonoBehaviour
{
    private float destroyTime = 1f;
    private Vector3 initialOffset = new Vector3(0,4.2f,0.1f);
    private Vector3 positionRandomize = new Vector3(0.5f, 0.2f, 0f);
    public Color shieldColor = new Color(0f,178f,200f);
    public Color noShieldColor = new Color(218f, 19f, 23f);
    public TMP_Text _text;
    public Material material;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        material = GetComponent<Renderer>().material;
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
        transform.localPosition += initialOffset;
        transform.localPosition += new Vector3(Random.Range(-positionRandomize.x, positionRandomize.x), Random.Range(-positionRandomize.y, positionRandomize.y), Random.Range(-positionRandomize.z, positionRandomize.z));

    }

}
