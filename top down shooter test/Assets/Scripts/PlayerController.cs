using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 cameraOffset = new Vector3(0, 0, -10);
    public float moveSpeed = 5f;
    private Rigidbody2D player;
    private Animator playerAnimator;
    private GameObject legPivot;
    private GameObject head;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        legPivot = GameObject.Find("Player/LegPivot");
        head = GameObject.Find("Player/Head");

    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        LookAtMouse();
        SetWalkingAnimation();
        // Move the Camera to the player
        Camera.main.transform.position = player.transform.position + cameraOffset;
    }

    private void MovePlayer()
    {
        // Initialize movement vector
        Vector2 playerMovement = Vector2.zero;

        // Check for WASD inputs and apply the direction to the velocity direction
        if (Input.GetKey(KeyCode.W))
        {
            playerMovement.y = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerMovement.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerMovement.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerMovement.x = 1;
        }

        // Normalize the movement vector to avoid diagonal movement being faster
        playerMovement = playerMovement.normalized * moveSpeed;

        // Apply the movement to the Rigidbody2D velocity
        player.linearVelocity = new Vector2(playerMovement.x, playerMovement.y);
    }
        void LookAtMouse()
    {
        // Get the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the sprite to the mouse
        Vector2 direction = (mousePosition - head.transform.position).normalized;

        // Calculate the angle between the sprite's forward vector and the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        // Apply the rotation to the head
        head.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void SetWalkingAnimation()
    {
        // Toggle the Animation Parameters based on whether the player is moving
        if (player.linearVelocity != Vector2.zero)
        {
            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetBool("isIdle", false);
            //set the walk angle based on the velocity direction
            float walkAngle = Mathf.Atan2(player.linearVelocity.y, player.linearVelocity.x) * Mathf.Rad2Deg + 90;
            legPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, walkAngle));
        }
        else
        {
            playerAnimator.SetBool("isIdle", true);
            playerAnimator.SetBool("isWalking", false);
        }
    }
}
