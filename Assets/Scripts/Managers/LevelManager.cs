using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    Animator animator;
    [HideInInspector] public bool isLoading = false;
    [HideInInspector] public TMP_Text DashText;
    [HideInInspector] public Slider StaminaSlider;
    public Transform DisableOnMenu;

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

        // Passing 'true' ensures it finds the components even if the UI is hidden/disabled at start
        DashText = GetComponentInChildren<TMP_Text>(true);
        StaminaSlider = GetComponentInChildren<Slider>(true);
        animator = GetComponentInChildren<Animator>(true);
    }

    // --- SAHNE YÜKLENME OLAYLARI (SCENE EVENTS) ---
    private void OnEnable()
    {
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Automatically toggles the object based on the build index.
        // True if index >= 2, False if index < 2.
        if (DisableOnMenu != null)
        {
            DisableOnMenu.gameObject.SetActive(scene.buildIndex >= 2);
        }
    }
    // ----------------------------------------------

    public void ReloadScene()
    {
        StartCoroutine(SceneLoadAnimation());
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(NextSceneLoadAnimation(nextSceneIndex));
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

            // Note: Make sure CharacterMovement.Instance exists and is accessible!
            if (CharacterMovement.Instance != null)
                CharacterMovement.Instance.Die();

            ReloadScene();
        }
    }
    // ------------------------------------------------

    IEnumerator NextSceneLoadAnimation(int nextSceneIndex)
    {
        animator.SetTrigger("Fade");
        isLoading = true;
        yield return new WaitForSecondsRealtime(1.5f);
        isLoading = false;
        SceneManager.LoadScene(nextSceneIndex);
    }

    IEnumerator LoadCustomScene(int index)
    {
        animator.SetTrigger("Fade");
        isLoading = true;
        yield return new WaitForSecondsRealtime(1.5f);
        isLoading = false;
        SceneManager.LoadScene(index);
    }

    IEnumerator SceneLoadAnimation()
    {
        if (isLoading)
            yield break;

        animator.SetTrigger("Fade");
        isLoading = true;
        yield return new WaitForSecondsRealtime(1.5f);
        isLoading = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenuScene()
    {
        StartCoroutine(LoadCustomScene(1));
    }

    public void QuitGamte()
    {
        Application.Quit();
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
}