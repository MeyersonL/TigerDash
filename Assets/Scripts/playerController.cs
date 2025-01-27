using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour {
    public Camera theCamera;
    public Slider healthBar;
    public Slider sliderBar;
    public TextMeshProUGUI jumpsLeft;
    public TextMeshProUGUI shieldsLeft;
    public AudioClip walkSound;
    private AudioSource audioSource;
    private Animator animator;
    private CameraController cameraController;
    public float speed;
    public float normalSpeed = 6.0f;
    public float sprintSpeed = 18.0f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    [SerializeField]
    public float health = 0.75f;
    private float speedTimeLeft;
    [SerializeField]
    private int jumpCount;
    private bool isJumping;
    private bool isJumpInAir;
    private bool isJumpStarted;
    private bool collided;
    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothVelocity;
    private Vector3 moveDir;
    [SerializeField] private int shieldsCollected;

    void Start() {
        cameraController = theCamera.GetComponent<CameraController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        // Movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && animator.GetBool("isWalking")) { 
            animator.SetBool("isRunning", true);
            speed = sprintSpeed; }
        else {
            animator.SetBool("isRunning", false);
            speed = normalSpeed; }

        bool canMove = true;

        if (direction.magnitude >= 0.1f) { 
            // Calculate target angle
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, 0.5f)) {
                if (hit.collider.CompareTag("Wall")) {
                    canMove = false;
                }
            }

            // Move the player
            if (canMove && !cameraController.inPuzzle && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))) {
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                if (!(isJumpStarted && !isJumping)) controller.Move(moveDir.normalized * speed * ((speedTimeLeft > 0) ? 2f : 1f) * Time.deltaTime);
                animator.SetBool("isWalking", true);

                if (!audioSource.isPlaying) {
                    audioSource.PlayOneShot(walkSound);
                }
            }
            else {
                animator.SetBool("isWalking", false);
                audioSource.Stop();
            }
        }
        else {
            animator.SetBool("isWalking", false);
            audioSource.Stop();
        }

        speedTimeLeft -= Time.deltaTime;

        // Apply gravity
        if (!controller.isGrounded) {
            velocity.y += gravity * Time.deltaTime;
            if (isJumping) { isJumpInAir = true; }
        }

        controller.Move(velocity * Time.deltaTime);

        // Jumping
        if (controller.isGrounded && isJumpInAir) {
            isJumping = false;
            isJumpInAir = false;
            isJumpStarted = false;
            animator.SetTrigger("Idle");
        }
        else if (controller.isGrounded && Input.GetKey(KeyCode.Space) && !isJumpStarted && jumpCount > 0) {
            isJumping = false;
            isJumpInAir = false;
            isJumpStarted = true;
            jumpCount--;
            animator.SetTrigger("Jump");
        }

        if (cameraController.inPuzzle) {
            health -= Time.deltaTime / 90f;
        }

        healthBar.value = health;
        sliderBar.value = speedTimeLeft;
        jumpsLeft.text = $"Jumps Boosts: {jumpCount}";
        shieldsLeft.text = $"Shields Left: {8 - shieldsCollected}";

        if (health <= 0f) { SceneManager.LoadScene("Lose"); }
        else if (transform.position.x >= 34f && shieldsCollected == 8) { SceneManager.LoadScene("Win"); }
        else {
            Vector3 currentPosition = transform.position;
            float clampedX = Mathf.Clamp(currentPosition.x, 0f, 34f);
            float clampedY = Mathf.Clamp(currentPosition.z, 0f, 34f);
            Vector3 targetPosition = new Vector3(clampedX, transform.position.y, clampedY);
            Vector3 movement = targetPosition - transform.position;
            controller.Move(movement);
        }
    }


    private void OnJumpStart() {
        isJumping = true;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Power-Up")) {
            powerUpController powerUp = other.gameObject.GetComponent<powerUpController>();
            if (powerUp.type == "Speed") {
                Debug.Log("Speed power-up acquired!");
                speedTimeLeft = 5f;
            }
            else if (powerUp.type == "Jump") {
                Debug.Log("Jump power-up acquired!");
                jumpCount++;
            }
            else if (powerUp.type == "Health") {
                Debug.Log("Health power-up acquired!");
                health = Mathf.Min(1f, health + 0.25f);
            }
        }
        else if (other.CompareTag("Collectable")) {
            shieldsCollected++;
        }
        else if (other.CompareTag("Tiger") && !cameraController.inPuzzle) {
            int puzzleType = Random.Range(0, 2);
            if (puzzleType == 0) {
                NumberLinkGame numberLinkGame = GameObject.Find("Numberlink Game Manager").GetComponent<NumberLinkGame>();
                cameraController.timeElapsed = 0;
                cameraController.inPuzzle = true;
                numberLinkGame.Initialize();
            }
            else if (puzzleType == 1) {
                TwentyFourGame twentyFourGame = GameObject.Find("Twenty-Four Game Manager").GetComponent<TwentyFourGame>();
                cameraController.timeElapsed = 0;
                cameraController.inPuzzle = true;
                twentyFourGame.Initialize();
            }
        }
    }
}