using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    Animator animator;
    bool isLoading = false;

    // --- EKLENEN DEĞİŞKEN ---
    private int killCountInWindow = 0;

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
            StartCoroutine(NextSceneLoadAnimation());
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

    // --- EKLENEN METOTLAR (Dash ve Ölüm Kontrolü) ---

    // Düşman öldüğünde LevelManager'a haber vermek için
    public void RegisterEnemyDeath()
    {
        killCountInWindow++;
    }

    // Oyuncu son dash'ini attığında bu metodu çağıracağız
    public void CheckLastDash()
    {
        StartCoroutine(LastDashTimerCoroutine());
    }

    private IEnumerator LastDashTimerCoroutine()
    {
        // Zamanlayıcı başlarken skoru sıfırla
        killCountInWindow = 0;

        // 1 saniye bekle (Oyun duraklatılmadığı sürece çalışır)
        yield return new WaitForSecondsRealtime(1f);

        // Eğer 1 saniye geçti, hiç düşman ölmedi VE sahne zaten yüklenmiyorsa
        if (killCountInWindow == 0 && !isLoading)
        {
            Debug.Log("Son dash kullanıldı ve 1 saniye içinde hiçbir şey ölmedi. Restarting...");
            ReloadScene();
        }
    }
    // ------------------------------------------------

    IEnumerator NextSceneLoadAnimation()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        animator.SetTrigger("Fade");
        isLoading = true;
        yield return new WaitForSecondsRealtime(1.5f);
        isLoading = false;
        SceneManager.LoadScene(nextSceneIndex);
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