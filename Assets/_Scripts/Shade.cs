using System.Collections;
using UnityEngine;

public class Shade : MonoBehaviour
{
    public float speed = 1.0f;
    private Coroutine risingCoroutine;

    public void StartRising()
    {
        if (risingCoroutine == null)
        {
            risingCoroutine = StartCoroutine(Rise());
        }
    }

    public void StopRising()
    {
        if (risingCoroutine != null)
        {
            StopCoroutine(risingCoroutine);
            risingCoroutine = null;
        }
    }

    private IEnumerator Rise()
    {
        while (true)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            yield return null;
        }
    }
}
