using UnityEngine;
using System.Collections;
using TMPro;

public class UIPopUp : MonoBehaviour
{
    public bool FollowPlayer = false;
    GameObject Player;
    TMP_Text text;
    [SerializeField] float FollowSpeed = 0.2f;
    [SerializeField] float FadeOutTime = 2f;
    [SerializeField] float FadeInTime = 0.5f;
    Vector2 velocity = Vector2.zero;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        text = GetComponentInChildren<TMP_Text>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        StartCoroutine(UnFade());
    }

    void Update()
    {
        if (!FollowPlayer)
            return;
        transform.position = Vector2.SmoothDamp((Vector2)transform.position, new Vector2(Player.transform.position.x, Player.transform.position.y -1.5f), ref velocity, FollowSpeed);
    }

    public void StartFade()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < FadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(text.color.a, 0f, elapsedTime / FadeOutTime);
            text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator UnFade()
    {
        float elapsedTime = 0f;

        while (elapsedTime < FadeInTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(text.color.a, 1f, elapsedTime / FadeInTime);
            text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
            yield return null;
        }
    }
}
