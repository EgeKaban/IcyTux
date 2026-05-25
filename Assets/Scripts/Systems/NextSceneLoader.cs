using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class NextSceneLoader : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }
}