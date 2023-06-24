using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerLiz : MonoBehaviour
{
    private Rigidbody playerRb;
    private Animator playerAnim;
    private AudioSource playerAudio;
    public AudioClip additionalLaugh;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public GameObject powerupEffect;
    public AudioClip jumpSound;
    public AudioClip crashSound;
    public AudioClip powerupSound;
    private float speed = 15.0f;
    public float jumpForce = 10;
    private float xBound = 16;
    private float zBound = 0;
    public float gravityModifier;
    public float doubleJumpForce;
    public bool doubleJumpUsed = false;
    public bool isOnGround = true;
    private bool isOnBroom = false;
    public float maxPlayerXPosition = 17f;
    public GameObject resetPosition;
    public GameObject bubblePrefab; // Reference to the bubble prefab
    public bool IsOnBroom { get; private set; } // Property to check if the player is on the broom ENCAPSULATION

    public bool gameOver;
    public GameObject gameOverPanel;
    private Transform broomObject; // Reference to the broom object

    private bool isInvulnerable = false; // Flag to track invulnerability status
    public float invulnerabilityDuration = 5f; // Duration of invulnerability in seconds
    private float invulnerabilityEndTime; // Variable to store the end time of invulnerability
    private float remainingPowerUpDuration = 0f;
    public float additionalPowerUpDuration = 5f;
    private bool isPowerupSoundPlaying = false;
    private bool isPowerupActive = false;

    [SerializeField] private GameManagerLiz gameManager;

    public void SetCanMove(bool canMove)
    {
        // Set the player's movement based on the value of canMove
        // For example, if you have a player movement script, you can enable/disable it here
        // You can also disable other player controls or animations based on the value of canMove
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        Physics.gravity *= gravityModifier;

        // Find the GameManagerLiz component and assign it to gameManager
        gameManager = FindObjectOfType<GameManagerLiz>();
        if (gameManager == null)
        {
            Debug.LogWarning("GameManagerLiz not found. Make sure the GameManagerLiz object is present in the scene.");
        }

        powerupEffect.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isOnBroom && broomObject != null)
        {
            // Set the player's position to follow the broom's position
            transform.position = broomObject.position;

            // Check if the player wants to jump off the broom
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JumpOffBroom();
            }
        }
        else
        {
            MovePlayer();
            ConstrainPlayerPosition();
        }
    }

    // Moves player based on left and right arrow keys and spacebar for jump
    void MovePlayer()
    {
        // If the game is over, don't allow player movement
        if (gameOver)
        {
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");

        // Update the player's position based on the input
        float movement = horizontalInput * speed * Time.deltaTime;
        transform.position += new Vector3(movement, 0, 0);

        // Clamp the player's position to the screen bounds
        float clampedXPosition = Mathf.Clamp(transform.position.x, -xBound, maxPlayerXPosition);
        transform.position = new Vector3(clampedXPosition, transform.position.y, transform.position.z);

        // Perform jump logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameOver)
            {
                return; // Don't allow jumping if the game is over
            }

            if (isOnGround && !isOnBroom)
            {
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isOnGround = false;
                playerAnim.Play("Fly");
                dirtParticle.Stop();
                playerAudio.PlayOneShot(jumpSound, 0.6f);

                doubleJumpUsed = false;
            }
            else if (!isOnGround && !isOnBroom && !doubleJumpUsed)
            {
                doubleJumpUsed = true;
                playerRb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
                playerAnim.Play("Fly");
                playerAudio.PlayOneShot(jumpSound, 0.6f);
            }
            else if (isOnBroom)
            {
                JumpOffBroom();
            }
        }
    }

    // Prevent player from leaving left or right side of screen and while on broom
    void ConstrainPlayerPosition()
    {
        if (IsOnBroom)
        {
            // Clamp player's X position when on the broom
            float clampedXPosition = Mathf.Clamp(transform.position.x, -xBound, maxPlayerXPosition);
            float clampedZPosition = Mathf.Clamp(transform.position.z, -zBound, zBound);
            transform.position = new Vector3(clampedXPosition, transform.position.y, clampedZPosition);

            // Check if the player has reached the X position boundary on the broom
            if (clampedXPosition >= maxPlayerXPosition)
            {
                JumpOffBroom(); // Detach the player from the broom
            }
        }
        else
        {
            // Clamp player's X position when not on the broom
            float clampedXPosition = Mathf.Clamp(transform.position.x, 0, xBound);
            float clampedZPosition = Mathf.Clamp(transform.position.z, -zBound, zBound);
            transform.position = new Vector3(clampedXPosition, transform.position.y, clampedZPosition);
        }
    }


    // Method to set the player on or off the broom
    public void SetOnBroom(bool onBroom)
    {
        IsOnBroom = onBroom;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!gameOver)
            {
                isOnGround = true;
                playerAnim.Play("Run");
                dirtParticle.Play();
            }
        }
        else if (collision.gameObject.CompareTag("Broom"))
        {
            if (!gameOver)
            {
                if (!isOnBroom)
                {
                    isOnBroom = true;
                    playerAnim.Play("Idle_A");
                    dirtParticle.Stop();

                    broomObject = collision.transform;

                    transform.SetParent(broomObject);
                    transform.localPosition = Vector3.zero;
                }
            }
        }
        else if (collision.gameObject.CompareTag("Cauldron"))
        {
            if (collision.gameObject.CompareTag("Broom"))
            {
                Instantiate(bubblePrefab, collision.transform.position, Quaternion.identity);
                Destroy(collision.gameObject);
                isOnBroom = false;
                playerAnim.Play("Run");
                dirtParticle.Play();
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Cauldron"))
        {
            if (!gameOver && !isInvulnerable)
            {
                explosionParticle.Play();

                if (isOnBroom)
                {
                    JumpOffBroom();
                }

                Debug.Log("Game Over");
                gameOver = true;
                playerAnim.Play("Death");
                dirtParticle.Stop();
                playerAudio.PlayOneShot(crashSound, 0.5f);

                if (playerAudio != null && additionalLaugh != null)
                {
                    StartCoroutine(PlayLaughSound());
                }

                if (gameOverPanel != null)
                {
                    gameOverPanel.SetActive(true);
                }

                SetCanMove(false);
                playerRb.isKinematic = true;
                playerRb.velocity = Vector3.zero;

                explosionParticle.Play();

                if (gameManager != null)
                {
                    gameManager.GameOver();
                }
            }
            else if (isInvulnerable && collision.gameObject.CompareTag("Obstacle"))
            {
                gameManager.IncreaseScore(5);

                Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;
                enemyRigidbody.AddForce(awayFromPlayer * 5, ForceMode.Impulse);
            }
        }
        else if (collision.gameObject.CompareTag("Boundary"))
        {
            if (isOnBroom)
            {
                // Detach the player from the broom
                JumpOffBroom();
            }
        }
        else if (collision.gameObject.CompareTag("Powerup"))
        {
            Debug.Log("Powerup collected");

            playerAudio.PlayOneShot(powerupSound, 0.3f);
            GameObject powerupObject = collision.gameObject;
            Destroy(powerupObject);

            if (isInvulnerable)
            {
                remainingPowerUpDuration += additionalPowerUpDuration;
            }
            else
            {
                isInvulnerable = true;
                remainingPowerUpDuration = invulnerabilityDuration;
                StartInvulnerability(powerupObject);
            }
        }
    }

    private IEnumerator PlayLaughSound()
    {
        yield return new WaitForSeconds(4.0f); // Initial delay of 4 seconds

        while (true)
        {
            playerAudio.PlayOneShot(additionalLaugh);

            float clipLength = additionalLaugh.length;
            yield return new WaitForSeconds(clipLength); // Wait for the duration of the audio clip
        }
    }

    // Jump off the broom
    private void JumpOffBroom()
    {
        if (isOnBroom && !gameOver)
        {
            isOnBroom = false;
            playerAnim.Play("Fly");

            transform.SetParent(null);

            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 0.6f);

            doubleJumpUsed = false;
        }
    }

    // Coroutine to handle invulnerability duration

    private IEnumerator InvulnerabilityCoroutine(GameObject powerupObject)
    {
        // Calculate the end time based on the remaining duration
        invulnerabilityEndTime = Time.time + remainingPowerUpDuration;

        while (Time.time < invulnerabilityEndTime)
        {
            if (remainingPowerUpDuration > invulnerabilityDuration)
            {
                // Additional power-up collected, extend the duration
                invulnerabilityEndTime += additionalPowerUpDuration;
                remainingPowerUpDuration -= additionalPowerUpDuration;
            }

            yield return null;
        }

        // Revert the effects of the power-up here
        // For example, make the player vulnerable again or restore normal collision behavior

        isInvulnerable = false;
        remainingPowerUpDuration = 0f;

        // Wait for a short delay to allow the power-up effect to continue playing
        yield return new WaitForSeconds(0.5f);

        // Stop the power-up effect
        StopPowerupEffect();
    }

    // Method to stop the powerup sound and visual effect
    private void StopPowerupEffect()
    {
        if (isPowerupActive)
        {
            powerupEffect.SetActive(false);
            isPowerupActive = false;
        }
    }

    private void SetGameOver()
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;
        playerAnim.Play("Death");
        dirtParticle.Stop();
        playerAudio.PlayOneShot(crashSound, 0.5f);

        if (playerAudio != null && additionalLaugh != null)
        {
            StartCoroutine(PlayLaughSound());
        }

        // Activate the Game Over Panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Disable player movement and physics
        SetCanMove(false);
        playerRb.isKinematic = true;
        playerRb.velocity = Vector3.zero;

        // Show the explosion particle effect
        explosionParticle.Play();

        // Call GameOver() method in GameManager
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }

    // Method to start the powerup sound and visual effect
    private void StartPowerupEffect()
    {
        playerAudio.PlayOneShot(powerupSound, 0.3f);
        powerupEffect.SetActive(true);
        isPowerupActive = true;
    }
    private void StartInvulnerability(GameObject powerupObject)
    {
        StartPowerupEffect();
        StartCoroutine(InvulnerabilityCoroutine(powerupObject));
    }
}