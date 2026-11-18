using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DEBUG_FloatingTestFeedback : MonoBehaviour
{
    public TMP_Text TMPText;
    private Coroutine _disableCoroutine;

    private void Start()
    {
        TMPText.enabled = false;
    }

    public void DisplayText(String text)
    {
        TMPText.enabled = true;
        TMPText.text = text;
        if (_disableCoroutine != null)
        {
            StopCoroutine(_disableCoroutine);
        }

        _disableCoroutine = StartCoroutine(DisableTextCoroutine());
    }

    private IEnumerator DisableTextCoroutine(float duration = 0.2f)
    {
        yield return new WaitForSeconds(duration);
        TMPText.enabled = false;
    }
}
