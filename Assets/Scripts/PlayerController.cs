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
    private float jumpForce;

    private int count;
    private int total;
    private bool isGameActive = true;
    private bool isJumping = false;
    // starts in the air
    private bool inAir = true;

    private GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        speed = 5.0f;
        jumpForce = 200.0f;

        rb = GetComponent<Rigidbody>();
        total = GameObject.FindGameObjectsWithTag("PickUp").Length;
        SetCountText();

        winTextObject.SetActive(false);

        mainCamera = GameObject.Find("Main Camera");
    }

    private void FixedUpdate()
    {
        // adjust movement based on y rotation of main camera
        Quaternion mainCameraRotation = Quaternion.Euler(0, mainCamera.transform.localEulerAngles.y, 0);
        Vector3 moveForce = new Vector3(speed * m_Move.x, 0.0f, speed * m_Move.y);

        rb.AddForce(mainCameraRotation * moveForce);
    }

    private void Update()
    {
        // TODO: "Mouse Y" Input Axis?
        // use m_Look.y?
        // rotate around z axis relative to the camera

        if (!inAir && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

        isJumping = Input.GetKey(KeyCode.Space);
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
        inAir = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        inAir = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        inAir = true;
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
