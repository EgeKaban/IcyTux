using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    Animator animator;
    bool isLoading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        animator = GetComponentInChildren<Animator>();
    }

    public void ReloadScene()
    {
        StartCoroutine(SceneLoadAnimation());
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load!");
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && SceneManager.GetActiveScene().buildIndex >= 2 && !isLoading)
        {
            ReloadScene();
        }
    }

    IEnumerator SceneLoadAnimation()
    {
        animator.SetTrigger("Fade");
        isLoading = true;
        yield return new WaitForSecondsRealtime(1.5f);
        isLoading = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}

