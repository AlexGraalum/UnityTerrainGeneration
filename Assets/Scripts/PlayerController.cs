using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
     public CharacterController characterController;
     public Rigidbody characterRigidBody;
     public float moveSpeed = 6;

     [Header("Jump Force")]
     public float jumpForce = 1.0f;

     [Header("Gravity")]
     private float gravity = 9.87f;
     private float vSpeed = 0.0f;

     [Header("Camera")]
     public Transform cameraTransform;
     public float sensitivity = 2.0f;
     public float upLimit = -50.0f;
     public float downLimit = 50.0f;

     private void Start() {
          Cursor.lockState = CursorLockMode.Locked;
     }

     private void Update() {
          if (Input.GetKeyDown(KeyCode.Escape)) {
#if UNITY_EDITOR
               UnityEditor.EditorApplication.isPlaying = false;
#else
               Application.Quit();
#endif
          }

          Move();
          Rotate();
     }

     private void Move() {
          float horizontalMove = Input.GetAxis("Horizontal");
          float verticalMove = Input.GetAxis("Vertical");

          vSpeed += characterController.isGrounded ? Input.GetKeyDown(KeyCode.Space) ? jumpForce : 0.0f : -gravity * Time.deltaTime;

          Vector3 gravMove = new Vector3(0.0f, vSpeed, 0.0f);
          Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;
          characterController.Move(moveSpeed * Time.deltaTime * move + gravMove * Time.deltaTime);
     }

     private void Rotate() {
          float hRot = Input.GetAxis("Mouse X");
          float vRot = Input.GetAxis("Mouse Y");

          transform.Rotate(0, hRot * sensitivity, 0);
          cameraTransform.Rotate(-vRot * sensitivity, 0, 0);

          Vector3 currRot = cameraTransform.localEulerAngles;
          if (currRot.x > 180.0f) currRot.x -= 360.0f;

          currRot.x = Mathf.Clamp(currRot.x, upLimit, downLimit);

          cameraTransform.localRotation = Quaternion.Euler(currRot);
     }
}
