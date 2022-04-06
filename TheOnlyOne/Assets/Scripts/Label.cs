using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    private Transform cameraTransform;
    public TMP_Text itemName;
   [SerializeField] private Image background;
    [SerializeField] private Color backColor = new Color(7f, 0f, 126f, 10f);
    void Start()
    {
        background = GetComponent<Image>();
        background.color = backColor;

    }

}
