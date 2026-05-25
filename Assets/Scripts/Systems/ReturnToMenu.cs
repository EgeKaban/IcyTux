using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ReturnToMenu : MonoBehaviour
{
    public CanvasGroup messageCanvas;
    public float fadeDuration = 1f;
    public float displayTime = 2f;

    private bool hasShownMessage = false;
    private bool canReturn = false;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!hasShownMessage)
            {
                StartCoroutine(ShowMessage());
            }
            else if (canReturn)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    IEnumerator ShowMessage()
    {
        hasShownMessage = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            messageCanvas.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        messageCanvas.alpha = 1f;
        canReturn = true;

        yield return new WaitForSeconds(displayTime);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            messageCanvas.alpha = Mathf.Clamp01(1 - (elapsed / fadeDuration));
            yield return null;
        }

        messageCanvas.alpha = 0f;
        hasShownMessage = false;
        canReturn = false;
    }
}
