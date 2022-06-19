using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
    [Header("Properties")]
    private float destroyTime = 2f;
    private Color armorColor = Color.blue;
    private Color noArmorColor = Color.red;
    private TMP_Text text;

    public Color ArmorColor { get => armorColor; set => armorColor = value; }
    public Color NoArmorColor { get => noArmorColor; set => noArmorColor = value; }
    public TMP_Text Text { get => text; set => text = value; }

    private void Awake()
    {
        Text = GetComponent<TMP_Text>();
        Destroy(gameObject, destroyTime);
    }
}
