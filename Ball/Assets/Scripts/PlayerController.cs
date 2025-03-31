using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;
    private float horizontalInput;
    private float verticalInput;
    private Vector2  moveVelocity;
    private float rotationSpeed;
    private Rigidbody2D player;
    private CircleCollider2D playerCollider;
    [SerializeField]
    private Vector3 offset = new Vector3(0, 0, -10);
    private RaycastHit2D ray;
    [SerializeField]
    private LayerMask layerToIgnore;//allows the ray to ignore the player and only look for terrain. set in the inspector
    private float rayLength;
    private PhysicsMaterial2D originalMaterial;
    private PhysicsMaterial2D noFrictionMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CircleCollider2D>();
        rayLength = playerCollider.radius * 1.1f;    
        originalMaterial = playerCollider.sharedMaterial;
        /*noFrictionMaterial = playerCollider.   */ 
    }

    // Update is called once per frame
    void Update()
    {
        //draw a ray down from the player which is 1.1 * player radius 
        RaycastHit2D ray = Physics2D.Raycast(player.transform.position, Vector3.down, rayLength, ~layerToIgnore);
        //draw the debug ray
        Debug.DrawRay(player.transform.position, Vector3.down * rayLength, Color.red, 1.0f);

        //if we are colliding with the ground we read the verticalInput

        verticalInput = Input.GetAxis("Vertical"); 
            if(ray)
            {                 
                moveVelocity = new Vector2(0,verticalInput);
                
                if(verticalInput < 0)
                {
                    moveVelocity = Vector2.zero;
                    Debug.Log("moveVelocity zero");
                    /*playerCollider.sharedMaterial == noFriction*/
                }
            }

        horizontalInput = Input.GetAxis("Horizontal");
        rotationSpeed = horizontalInput * -1;
        
        if(moveVelocity != Vector2.zero)
        {
            player.position += (moveVelocity * movementSpeed) * Time.deltaTime;
        }
        player.angularVelocity += rotationSpeed;
        Camera.main.transform.position = player.transform.position + offset;
    }
}
