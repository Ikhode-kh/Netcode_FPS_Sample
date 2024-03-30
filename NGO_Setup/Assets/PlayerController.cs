using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Game Components")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CharacterController characterController;

    [Header("Game Variables")]
    [SerializeField] private float walkSpeed = 15f;
    [SerializeField] private float runSpeed = 25f;
    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private float gravity = 10f;
    [SerializeField] private float lookSpeed = 3.5f;
    [SerializeField] private float lookXLimit = 45f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private float verticalVelocity;

    public override void OnNetworkSpawn()
    {
        // gameObject.transform.position = new Vector3(Random.Range(-10f, 10f), 5f, Random.Range(-10f, 10f));
    }

    private void Start()
    {
        Debug.Log("FPS controller => is owner = " + IsOwner + ": is local player + " + IsLocalPlayer + " : is server = " + IsServer);
    }

    private void Update()
    {
        if (!IsOwner || !IsLocalPlayer || !Application.isFocused)
        {
            playerCamera.enabled = false;
            return;
        }

        playerCamera.enabled = true;
        Player_Movement();
    }

    private void Player_Movement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);


        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal");

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpPower;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}
