using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] float floatDelay = 2f;
    [SerializeField] ParticleSystem crashEffect;
    [SerializeField] GameManager gameManager;

    bool hasCrashed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCrashed && ShouldCrashTrigger(collision))
        {
            Crash();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Va chạm với đá được xử lý thuần túy bởi vật lý (physics material bouncy).
        // Không cần xử lý gì ở đây.
    }

    private bool ShouldCrashTrigger(Collider2D collision)
    {
        return collision.CompareTag("Ground") ||
               collision.CompareTag("FallZone");
    }

    private void Crash()
    {
        hasCrashed = true;
        crashEffect.Play();
        GetComponent<AudioSource>().Play();
        FindFirstObjectByType<PlayerController>().DisableController();
        if (gameManager != null)
        {
            Invoke("ShowGameOver", floatDelay);
        }
        else
        {
            Invoke("ReloadLevel", floatDelay);
        }
    }

    void ShowGameOver()
    {
        gameManager.GameOver();
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}