using System.Collections;
using TMPro;
using UnityEngine;

public sealed class GameMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField, Min(0.1f)] private float showSeconds = 2f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    public void Show(string message)
    {
        if (messageText == null)
        {
            Debug.LogError("Message Text is not assigned.", this);
            return;
        }

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        messageText.text = message;
        messageText.gameObject.SetActive(true);
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(showSeconds);
        messageText.gameObject.SetActive(false);
        hideRoutine = null;
    }
}
