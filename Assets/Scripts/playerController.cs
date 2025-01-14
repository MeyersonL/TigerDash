using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private Animator animator;
    public float speed;
    public float normalSpeed = 6.0f;
    public float sprintSpeed = 18.0f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    public float health = 0.75f;
    private float speedTimeLeft;
    private int jumpCount;
    private bool isJumping;
    private bool isJumpInAir;
    private bool isJumpStarted;
    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothVelocity;

    void Start() {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        // Movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift)) { 
            animator.SetBool("isRunning", true);
            speed = sprintSpeed; }
        else {
            animator.SetBool("isRunning", false);
            speed = normalSpeed; }

        if (direction.magnitude >= 0.1f) {
            // Calculate target angle
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move the player
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (!(isJumpStarted && !isJumping)) controller.Move(moveDir.normalized * speed * ((speedTimeLeft > 0) ? 2f : 1f) * Time.deltaTime);
            animator.SetBool("isWalking", true);
        }
        else { animator.SetBool("isWalking", false); }

        speedTimeLeft -= Time.deltaTime;

        // Apply gravity
        if (!controller.isGrounded) {
            velocity.y += gravity * Time.deltaTime;
            if (isJumping) { isJumpInAir = true; }
        }
        
        // Debug.Log(speedTimeLeft);
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
    }
}