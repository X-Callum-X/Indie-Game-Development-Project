using UnityEngine.UI;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Slider speedMeter;
    public Camera playerCamera;
    public GameObject respawnPoint;
    public TMP_Text machText;

    [Header("Variables")]
    public float walkSpeed;
    public float diveSpeed;

    public float minSpeed;
    public float maxSpeed;

    public float jumpPower;
    public float gravity;

    public float lookSpeed;
    public float lookXLimit;

    private int machLevel = 1;

    [SerializeField] private float currentOrbs;
    [SerializeField] private float maxOrbs;

    [Header("Movement")]
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        speedMeter.value = currentOrbs;
        speedMeter.maxValue = maxOrbs;

        minSpeed = walkSpeed;

        machText.text = "Mach " + machLevel.ToString();
    }

    void Update()
    {
        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? walkSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? walkSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (curSpeedX > 0 || curSpeedX < 0)
        {
            walkSpeed += Time.deltaTime * 5;

            if (walkSpeed >= maxSpeed)
            {
                walkSpeed = maxSpeed;
            }
        }

        else
        {
            walkSpeed = minSpeed;
        }

        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = 0;

                moveDirection.y -= diveSpeed;
            }
        }
        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        #endregion

        #region Handles Respawning
        if (transform.position.y < -20)
        {
            transform.position = respawnPoint.transform.position;
        }

        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Orb"))
        {
            if (currentOrbs >= maxOrbs)
            {
                currentOrbs = 0;

                speedMeter.value = currentOrbs;

                machLevel += 1;

                walkSpeed += machLevel * 5;

                maxSpeed += 10;

                machText.text = "Mach " + machLevel.ToString();
            }

            else
            {
                currentOrbs += 1;

                speedMeter.value = currentOrbs;
            }

            Destroy(other.gameObject);
        }
    }
}
