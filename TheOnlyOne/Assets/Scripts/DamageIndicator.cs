using System;
using System.Collections;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private const float timeLimit = 7.0f;
    private float timer = timeLimit;

    private CanvasGroup canvasGroup = null;
    protected CanvasGroup CanvasGroup
    {
        get
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();

                }
            }
            return canvasGroup;
        }
    }
    private RectTransform rect = null;
    protected RectTransform Rect
    {
        get
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
                if (rect == null)
                {
                    rect = gameObject.AddComponent<RectTransform>();

                }
            }
            return rect;
        }
    }

    public Transform Target { get; protected set; } = null;
    private Transform player = null;

    private IEnumerator IE_Countdown = null;
    private Action unRegister = null;

    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 targetPos = Vector3.zero;

    public void Appear(Transform _target, Transform _player, Action _unRegister)
    {
        this.Target = _target;
        this.player = _player;
        this.unRegister = _unRegister;
        StartCoroutine(RotateTowardsTarget());
        StartTimer();
    }
    public void Restart()
    {
        timer = timeLimit;
        StartTimer();
    }
    private void StartTimer()
    {
        if (IE_Countdown != null) { StopCoroutine(IE_Countdown); }
        IE_Countdown = Countdown();
        StartCoroutine(IE_Countdown);
    }

    private IEnumerator Countdown()
    {
        while (CanvasGroup.alpha < 1.0f)
        {
            CanvasGroup.alpha += 7 * Time.deltaTime;
            yield return null;
        }
        while (timer > 0)
        {
            timer--;
            yield return new WaitForSeconds(0.1f);
        }
        while (CanvasGroup.alpha > 0.0f)
        {
            CanvasGroup.alpha -= 8 * Time.deltaTime;
            yield return null;
        }
        unRegister();
        Destroy(gameObject);
    }

    IEnumerator RotateTowardsTarget()
    {
        while (enabled)
        {
            if (Target)
            {
                targetPos = Target.position;
                targetRotation = Target.rotation;
            }
            Vector3 direction = player.position - targetPos;

            targetRotation = Quaternion.LookRotation(direction);
            targetRotation.z = -targetRotation.y;
            targetRotation.x = 0;
            targetRotation.y = 0;

            Vector3 northDirection = new Vector3(0, 0, player.eulerAngles.y);
            Rect.localRotation = targetRotation * Quaternion.Euler(northDirection);

            yield return null;
        }
    }
}
