using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float floatDelay = 2f;
    [SerializeField] ParticleSystem finishEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            finishEffect.Play();
            GetComponent<AudioSource>().Play();
            FindFirstObjectByType<PlayerController>().DisableController();
            Invoke("ReloadLevel", floatDelay);
        }

    }
    void ReloadLevel()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ShowWin();
        }
        else
        {
            Debug.LogError("GameManager not found in scene!");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
