using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerController : MonoBehaviour {

    public Camera theCamera;
    public MazeGenerator mazeGenerator;
    private Pathfinding pathFinder;
    private Animator animator;
    private CameraController cameraController;
    [SerializeField] private float speed;
    private List<Node> path;
    private int pathIndex = 0;
    [SerializeField] private Vector2Int targetPos;
    public Vector2Int[] waypoints;
    [SerializeField] private int waypointIndex = 0;
    private bool detected;
    void Start() {
        cameraController = theCamera.GetComponent<CameraController>();
        pathFinder = mazeGenerator.GetComponent<Pathfinding>();
        animator = GetComponent<Animator>();
        waypoints = mazeGenerator.GetComponent<MazeGenerator>().waypoints;
    }

    void Update() {

        // Go to next waypoint if not going anywhere
        if ((path == null || path.Count == 0) && animator.GetBool("isRunning")) {
            if (!DetectPlayer()) path = null;
        }
        if (path == null || path.Count == 0) {
            Vector2Int currentPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
            waypointIndex = (waypointIndex == 8) ? 0 : waypointIndex + 1;
            targetPos = waypoints[waypointIndex];
            path = pathFinder.FindPath(currentPos, targetPos);
            pathIndex = 0;
            if (path != null && path.Count > 0) RotateTiger(path[pathIndex].GridPosition, currentPos);
            speed = 1f;
            animator.SetBool("isRunning", false);
        }

        // Going to target location
        if (path != null) {
            Vector3 targetPosition = new Vector3(path[pathIndex].GridPosition.x, 0.25f, path[pathIndex].GridPosition.y);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
                if (pathIndex < path.Count - 1) {
                    pathIndex++;
                    RotateTiger(path[pathIndex].GridPosition, path[pathIndex - 1].GridPosition);
                }
                else if (!DetectPlayer()) path = null;
            }
        }

        // Detect player with Raycast
        DetectPlayer();
    }

    void RotateTiger(Vector2Int newPos, Vector2Int oldPos) {
        Vector2Int distance = newPos - oldPos;
        if (distance == new Vector2Int(0, 1)) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (distance == new Vector2Int(0, -1)) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (distance == new Vector2Int(1, 0)) transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (distance == new Vector2Int(-1, 0)) transform.rotation = Quaternion.Euler(0f, -90f, 0f);
    }

    bool DetectPlayer() {
        int playerLayer = LayerMask.GetMask("Player");

        if (!cameraController.inPuzzle)
            for (int i = -90; i <= 90; i += 10) {
                Quaternion rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + i, 0f);
                Vector3 direction = rotation * Vector3.forward;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit)) {
                    if (hit.collider.CompareTag("Player")) {
                        float attackProb = 1f / (0.5f * Vector3.Distance(transform.position, hit.point));
                        attackProb *= Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
                        // Debug.Log(attackProb);
                        // Debug.Log(detected);
                        if ((Random.value < attackProb && !detected) || animator.GetBool("isRunning")) {
                            ChasePlayer(hit.point); }
                        detected = true;
                        return detected;
                    }
                }
                if (Physics.Raycast(transform.position, direction, out hit, 10f, playerLayer)) {
                    if (hit.collider.CompareTag("Player")) { 
                        float attackProb = 1f / (0.5f * Vector3.Distance(transform.position, hit.point));
                        attackProb *= Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
                        // Debug.Log(attackProb);
                        // Debug.Log(detected);
                        if ((Random.value < attackProb && !detected) || animator.GetBool("isRunning")) {
                            ChasePlayer(hit.point); }
                        detected = true;
                        return detected;
                    }
                }
            }
        detected = false;
        return detected;
    }

    void ChasePlayer(Vector3 pos) {
        Vector2Int currentPos = new Vector2Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));
        targetPos = new Vector2Int((int)Mathf.Round(pos.x), (int)Mathf.Round(pos.z));
        path = pathFinder.FindPath(currentPos, targetPos);
        pathIndex = 0;
        if (path != null && path.Count > 0) RotateTiger(path[pathIndex].GridPosition, currentPos);
        animator.SetBool("isRunning", true);
        speed = 3f;
    }
}