using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextMover : MonoBehaviour
{
    public float Speed = 5f;
    public float ExitCreditScreen = 75f;
    private void Start() {
        StartCoroutine(GoMenu());
    }
    void Awake()
    {
        Time.timeScale = 1f;
    }
    void Update()
    {
        transform.position += new Vector3(0, Speed, 0) * Time.deltaTime;
    }

    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(ExitCreditScreen);
        SceneManager.LoadScene("Menu");
    }
}
