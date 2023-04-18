using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject playerCamera;

    private Rigidbody rb;
    private Vector2 m_Look;
    private Vector2 m_Move;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float jumpVelocity;

    [SerializeField]
    private float torque;

    [SerializeField]
    private float torqueAir;

    private int count;
    private int total;
    private bool isGameActive = true;
    private bool isJumpPressed = false;
    private bool isColliding = false;

    private GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        speed = 3.0f;
        jumpVelocity = 4.0f;
        torque = 0.8f;
        torqueAir = 0.4f;

        rb = GetComponent<Rigidbody>();

        rb.maxAngularVelocity = float.PositiveInfinity;

        // TODO: change this?
        //rb.maxDepenetrationVelocity

        total = GameObject.FindGameObjectsWithTag("PickUp").Length;
        SetCountText();

        winTextObject.SetActive(false);

        mainCamera = GameObject.Find("Main Camera");
    }

    private void FixedUpdate()
    {
        //UpdateAngularVelocity();
        UpdateTorque();

        //if (!isColliding)
        //{
            UpdateMoveForce();
        //}

        if (isJumpPressed && isColliding)
        {
            // BUG?:
            // with high torque (and high height?), the marble doesn't catch any friction from the ground
            // this might just be expected but it feels weird in some situations (eg after a wall hit)

            // TODO: velocity after jumping should be perpendicular to collision surface
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
    }

    /*
     * Updates the force of the marble based on camera rotation and move input
     */
    void UpdateMoveForce()
    {
        // adjust movement based on y rotation of main camera
        Quaternion mainCameraRotation = Quaternion.Euler(0, mainCamera.transform.localEulerAngles.y, 0);
        Vector3 moveForce = new Vector3(speed * m_Move.x, 0.0f, speed * m_Move.y);
        Vector3 rotatedMoveForce = mainCameraRotation * moveForce;

        rb.AddForce(rotatedMoveForce);
    }

    /*
     * Updates angular velocity instead of adding force
     * This results in 0 air movement but very fast spinning in the air
     * (maybe you can give the air friction to make this more playable?)
     */
    void UpdateAngularVelocity()
    {
        // adjust movement based on y rotation of main camera
        // NB: need to rotate by an additional 90 because we are adjusting angular velocity
        Quaternion mainCameraRotation = Quaternion.Euler(0, mainCamera.transform.localEulerAngles.y + 90, 0);
        Vector3 moveForce = new Vector3(speed * m_Move.x, 0.0f, speed * m_Move.y);

        Vector3 rotatedMoveForce = mainCameraRotation * moveForce;

        Vector3 flattenedAngularVelocity = new Vector3(rb.angularVelocity.x, 0.0f, rb.angularVelocity.z);

        Vector3 adjustedAngularVelocity = new Vector3(flattenedAngularVelocity.x + rotatedMoveForce.x, 0.0f, flattenedAngularVelocity.z + rotatedMoveForce.z);

        rb.angularVelocity = adjustedAngularVelocity;
    }
    void UpdateTorque()
    {
        // adjust movement based on y rotation of main camera
        // NB: need to rotate by an additional 90 because we are adjusting torque
        Quaternion mainCameraRotation = Quaternion.Euler(0, mainCamera.transform.localEulerAngles.y + 90, 0);
        Vector3 moveForce = new Vector3(m_Move.x, 0.0f, m_Move.y) * (isColliding ? torque : torqueAir);

        Vector3 rotatedMoveForce = mainCameraRotation * moveForce;

        rb.AddTorque(rotatedMoveForce);
    }

    private void Update()
    {
        // TODO: "Mouse Y" Input Axis?
        // use m_Look.y?
        // rotate around z axis relative to the camera

        isJumpPressed = Input.GetKey(KeyCode.Space);
    }

    void OnMove(InputValue movementValue)
    {
        if (isGameActive)
        {
            m_Move = movementValue.Get<Vector2>();
        }
    }
    public void OnLook(InputValue value)
    {
        m_Look = value.Get<Vector2>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    void SetCountText()
    {
        countText.text = count + " / " + total;

        if (count == total)
        {
            winTextObject.SetActive(true);
            isGameActive = false;
            m_Move = Vector2.zero;
        }
    }
}
