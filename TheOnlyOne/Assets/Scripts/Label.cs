using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    [Header("UI components")]
    private GameObject Canvas;
    private Outline itemOutline;

    [Header("Properties")]
    public TMP_Text itemName;
    public TMP_Text itemType;
    public Image icon;
    public TMP_Text stat;
    public Image background;
    public Color color;

    private bool isPointed;
    public bool IsPointed { get => isPointed; set => isPointed = value; }

    private void Start()
    {
        Canvas = transform.GetChild(0).gameObject;
        background.color = color;

        itemOutline = GetComponentInParent<Outline>();
        itemOutline.OutlineColor = color;
    }
    private void Update()
    {
        CheckVisible();
    }
    private void CheckVisible()
    {
        Canvas.SetActive(IsPointed);
        itemOutline.enabled = IsPointed;

    }
}
