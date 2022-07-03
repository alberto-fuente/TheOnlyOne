using UnityEngine;

public class KillUI : MonoBehaviour
{
    public CanvasGroup background;

    public void OnEnable()
    {
        background.alpha = 0;
        background.LeanAlpha(1, .5f).setOnComplete(FadeOut);
        transform.LeanScale(new Vector2(.2f, .2f), 0.5f);
        //transform.LeanMoveLocalY(transform.position.y + 30, 1f);
    }
    private void Start()
    {
        transform.localScale = Vector3.zero;

    }
    void DestryMe()
    {
        Destroy(gameObject);
    }
    void FadeOut()
    {
        background.LeanAlpha(0, .5f).setOnComplete(DestryMe);
    }

}
