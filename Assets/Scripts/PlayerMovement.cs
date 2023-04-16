using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    private CharacterController controller;
    private PlayerInput input;
    private Vector3 velocity;
    private bool grounded;
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float jumpHeight = 1f;
    [SerializeField]
    private float gravity = -9.81f;
    private Transform playerCamera;
    [SerializeField]
    private Transform eyes;

    [SerializeField]
    private float inspectDistance;

    private void Start() {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main.transform;
        if (IsOwner) FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = eyes;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        transform.position = new Vector3(0, 2, 0);
    }

    private void Update() {
        if (IsOwner) {
            Move();
            if (input.actions["Pause"].WasPressedThisFrame()) {
                if (UIManager.Instance.isInspecting) {
                    UIManager.Instance.CloseInspect();
                } else {
                    UIManager.Instance.TogglePause();
                }
            }

            if (input.actions["Inspect"].WasPressedThisFrame() && !UIManager.Instance.paused) {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit[] hits = Physics.RaycastAll(ray, inspectDistance);

                foreach (RaycastHit hit in hits) {
                    if (hit.collider.TryGetComponent<MoneyManager>(out MoneyManager moneyManager)) {
                        UIManager.Instance.OpenInspect(moneyManager.money.Value);
                        break;
                    }
                }
            }
        }
    }

    private void Move() {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -0.25f;

        Vector2 moveInput = input.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized /* <- I did this to stop the player from trying to move up or down. The line below fixes it, but it would leave the speed less than it should be */ * move.z + playerCamera.right * move.x;
        move.y = 0;
        controller.Move(move * Time.deltaTime * speed);

        if (input.actions["Jump"].triggered && grounded) velocity.y += Mathf.Sqrt(jumpHeight * -3f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
