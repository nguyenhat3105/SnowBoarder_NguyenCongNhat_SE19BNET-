using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SurfaceEffector2D se;
    [SerializeField] float TorqueAmount;
    [SerializeField] float boostSpeed;
    [SerializeField] float baseSpeed;
    [SerializeField] float slowSpeed;
    [SerializeField] float jumpForce = 15f;
    [SerializeField] LayerMask groundLayer;

    bool canMove = true;
    bool isJumping = false;

    float totalRotation = 0f;
    float lastRotation = 0f;
    float savedSpeed = 0f;
    int comboMultiplier = 1;
    public int score = 0;

    public TMP_Text scoreText;
    public TMP_Text speedText;
    public TMP_Text comboText;

    Vector2 startPosition;
    float distanceTraveled = 0f;
    public TMP_Text distanceText;

    [SerializeField] GameObject spinTextPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        se = FindFirstObjectByType<SurfaceEffector2D>();
        startPosition = transform.position;
        StyleHudText();
    }

    private void StyleHudText()
    {
        Color primaryColor = new Color(0.95f, 1f, 0.05f, 1f);
        Color outlineColor = new Color(0.01f, 0.02f, 0.06f, 1f);

        ApplyHudTextStyle(scoreText, primaryColor, outlineColor);
        ApplyHudTextStyle(speedText, primaryColor, outlineColor);
        ApplyHudTextStyle(distanceText, primaryColor, outlineColor);
        ApplyHudTextStyle(comboText, new Color(1f, 0.45f, 0.04f, 1f), outlineColor);
    }

    private static void ApplyHudTextStyle(TMP_Text text, Color faceColor, Color outlineColor)
    {
        if (text == null)
        {
            return;
        }

        text.color = faceColor;
        text.faceColor = faceColor;
        text.outlineColor = outlineColor;
        text.outlineWidth = 0.22f;
        text.UpdateMeshPadding();
    }

    private void Update()
    {
        if (canMove)
        {
            RotatePlayer();
            Boost();
            CalculateSpeedScore();
            CalculateDistance();
            UpdateSpeedUI();
            UpdateDistanceUI();
            PerformSpin();
            HandleJump();
        }
    }
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            Debug.Log("Jump");
            float currentHorizontalSpeed = rb.linearVelocity.x;
            rb.linearVelocity = new Vector2(currentHorizontalSpeed, jumpForce);
            isJumping = true;
        }
    }

    void CalculateDistance()
    {
        distanceTraveled = Vector2.Distance(startPosition, transform.position);
    }
    void UpdateDistanceUI()
    {
        if (distanceText != null)
        {
            distanceText.text = "Distance: " + distanceTraveled.ToString("F2") + " m";
        }
    }
    void Boost()
    {
        if (!isJumping)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                se.speed = boostSpeed;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                se.speed = slowSpeed;
            }
            else
            {
                se.speed = baseSpeed;
            }
        }
        //else
        //{
        //    rb.linearVelocity = new Vector2(savedSpeed, rb.linearVelocity.y);
        //}
    }

    void RotatePlayer()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddTorque(TorqueAmount);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddTorque(-TorqueAmount);
        }
    }

    public void DisableController()
    {
        canMove = false;
    }

    void CalculateSpeedScore()
    {
        //float speed = rb.linearVelocity.magnitude;
        //if (speed > 3f)
        //{
        //    score += (int)(speed * Time.deltaTime * 10 * comboMultiplier);
        //}
        //UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (comboText != null)
        {
            comboText.text = "Combo x" + comboMultiplier;
        }
    }

    void UpdateSpeedUI()
    {
        if (speedText != null)
        {
            float speed = rb.linearVelocity.magnitude;
            speedText.text = "Speed: " + speed.ToString("F2") + " m/s";
        }
    }

    void PerformSpin()
    {
        float currentRotation = transform.rotation.eulerAngles.z;
        float rotationDifference = Mathf.DeltaAngle(lastRotation, currentRotation);
        totalRotation += Mathf.Abs(rotationDifference);
        lastRotation = currentRotation;

        if (totalRotation >= 290f)
        {
            comboMultiplier++;
            score += 100 * comboMultiplier;
            UpdateScoreUI();
            totalRotation = 0f;

            ShowSpinText();
            UpdateComboUI();
        }
    }

    void UpdateComboUI()
    {
        if (comboText != null)
        {
            comboText.text = "Combo x" + comboMultiplier;
        }
    }


    // Nhận dạng bề mặt có thể đứng / nhảy: đất hoặc đá
    bool IsGroundSurface(GameObject obj)
    {
        return obj.CompareTag("Ground") || obj.name.Contains("Rock") || obj.CompareTag("Obstacle");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundSurface(collision.gameObject))
        {
            isJumping = false;
            totalRotation = 0f;
            comboMultiplier = 1;
            UpdateComboUI();
            // Chỉ cập nhật SurfaceEffector2D khi là mặt đất thật (có component đó)
            SurfaceEffector2D newSurface = collision.gameObject.GetComponent<SurfaceEffector2D>();
            if (newSurface != null)
            {
                se = newSurface;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundSurface(collision.gameObject))
        {
            isJumping = true;
            lastRotation = transform.rotation.eulerAngles.z;
            savedSpeed = rb.linearVelocity.x;
        }
    }

    void ShowSpinText()
    {
        if (spinTextPrefab != null)
        {
            GameObject spinText = Instantiate(spinTextPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            TextMeshPro tmp = spinText.GetComponent<TextMeshPro>();

            if (tmp != null)
            {
                tmp.text = "+100 x" + comboMultiplier;
                Renderer renderer = tmp.GetComponent<Renderer>();
                renderer.sortingLayerName = "UI";
                renderer.sortingOrder = 100;

                StartCoroutine(AnimateSpinText(spinText));
            }
        }
    }

    IEnumerator AnimateSpinText(GameObject textObject)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPos = textObject.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 5, 0);

        TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component not found!");
            yield break;
        }
        Color startColor = textMesh.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            textObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, 1 - t);

            yield return null;
        }

        Destroy(textObject);
    }
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

}
