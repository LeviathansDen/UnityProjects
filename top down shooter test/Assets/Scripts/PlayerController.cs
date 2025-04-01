using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    private Vector3 cameraOffset = new Vector3(0, 0, -10);
    public float moveSpeed = 5f;
    private Rigidbody2D player;
    private Animator playerAnimator;
    private GameObject legPivot;
    private GameObject armPivot;
    private GameObject head;
    private bool unarmedEquipped = true;
    private GameObject pistolObject;
    private SpriteRenderer pistolSprite;
    private GameObject pistolFlashObject;
    private SpriteRenderer pistolFlashSprite;
    private bool pistolEquipped = false;
    private bool isAiming = false;
    Vector3 mousePosition;
    [SerializeField]
    private float flashDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        legPivot = GameObject.Find("Player/LegPivot");
        armPivot = GameObject.Find("Player/ArmPivot");
        head = GameObject.Find("Player/Head");
        pistolObject = GameObject.Find("ArmPivot/Pistol");
        pistolSprite = pistolObject.GetComponent<SpriteRenderer>();
        pistolFlashObject = GameObject.Find("ArmPivot/Pistol/Flash");
        pistolFlashSprite = pistolFlashObject.GetComponent<SpriteRenderer>();
        HidePistol();
        HidePistolFlash();
        //Time.timeScale = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        WeaponHandle();
        SetAnimation();
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
    private void WeaponHandle()
    {
        if (!isAiming)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // Switch to unarmed
            {
                HidePistol();
                unarmedEquipped = true;
                pistolEquipped = false;

            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) // Switch to pistol
            {
                ShowPistol();
                unarmedEquipped = false;
                pistolEquipped = true;
            }
        }
    }

    private void SetAnimation()
    {
        float walkAngle = Mathf.Atan2(player.linearVelocity.y, player.linearVelocity.x) * Mathf.Rad2Deg + -90;
        bool aimHandled = false;
        // Toggle the Animation Parameters based on whether the player is moving
        if (player.linearVelocity != Vector2.zero)
        {
            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetBool("isIdle", false);
            //set the walk angle based on the velocity direction
            legPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, walkAngle));
            HandleAim(walkAngle);
            aimHandled = true;
        }
        else
        {
            playerAnimator.SetBool("isIdle", true);
            playerAnimator.SetBool("isWalking", false);
        }

        if (!aimHandled)
        {
            HandleAim(walkAngle);
        }
        LookAtMouse(head);
        Fire();
    }
    private void HandleAim(float walkAngle)
    {
        if (Input.GetButtonDown("Aim") || Input.GetButton("Aim"))
        {
            if (pistolSprite.enabled)
            {
                playerAnimator.SetBool("isAiming", true);
                isAiming = true;
            }
            LookAtMouse(armPivot);
        }
        else if (Input.GetButtonUp("Aim"))
        {
            playerAnimator.SetBool("isAiming", false);
            isAiming = false;
        }
        else if (player.linearVelocity != Vector2.zero)
        {            
            armPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, walkAngle));          
        }
    }
    private void LookAtMouse(GameObject Object)
    {
        // Get the mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the sprite to the mouse
        Vector2 direction = (mousePosition - Object.transform.position).normalized;

        // Calculate the angle between the sprite's forward vector and the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        // Apply the rotation to the head
        Object.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Fire()
    {
        if(isAiming && pistolEquipped)
        {
            RaycastHit2D ray = Physics2D.Raycast(player.transform.position,  Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position, 10);
            Debug.DrawRay(player.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position, Color.red, 0.25f);
            if(Input.GetButtonDown("Fire"))
            {
                StartCoroutine(PistolFlashCoroutine());
            }
        }
    }

    public void PistolLayerAbove()
    {
        pistolSprite.sortingOrder = 3;
    }

    public void PistolLayerBelow()
    {
        pistolSprite.sortingOrder = 1;
    }

    void HidePistol()
    {
        pistolSprite.enabled = false;
    }

    void ShowPistol()
    {
        pistolSprite.enabled = true;
    }

    void HidePistolFlash()
    {
        pistolFlashSprite.enabled = false;
    }

    void ShowPistolFlash()
    {
        pistolFlashSprite.enabled = true;
    }

    private IEnumerator PistolFlashCoroutine()
    {
        Debug.Log("coroutine hit");
        flashDuration = 0.05f;
        ShowPistolFlash();
        yield return new WaitForSeconds(flashDuration);  // Wait for the duration
        HidePistolFlash();  // Hide the sprite
    }
}
