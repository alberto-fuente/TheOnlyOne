using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    //visible
    private GameObject Canvas;
    private Outline itemOutline;
    //info
    public TMP_Text itemName;
    public TMP_Text itemType;
    public Image icon;
    public TMP_Text stat;
    public Image background;
    public Color color;

    public bool isPointed;
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
        Canvas.SetActive(isPointed);
        itemOutline.enabled = isPointed;

    }
}
