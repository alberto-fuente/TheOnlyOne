using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 originalPosition;
    private void Start()
    {
        originalPosition = transform.localPosition;
    }
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    private IEnumerator Shake(float duration, float magnitude)
    {

        float elapsed = 0;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

}
