using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] int pointValue = 10;
    [SerializeField] AudioClip collectSound;
    AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddScore(pointValue);
            }
            PlaySoundAndDestroy();
        }
    }
    void PlaySoundAndDestroy()
    {
        if (collectSound != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        Destroy(gameObject);
    }
}
