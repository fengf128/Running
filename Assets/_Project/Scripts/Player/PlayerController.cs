using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerShooter), typeof(PlayerInteractor))]
[RequireComponent(typeof(PlayerItemUser))]
public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;

    private CharacterController characterController;
    private PlayerInputActions inputActions;
    private PlayerShooter shooter;
    private PlayerInteractor interactor;
    private PlayerItemUser itemUser;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        shooter = GetComponent<PlayerShooter>();
        interactor = GetComponent<PlayerInteractor>();
        itemUser = GetComponent<PlayerItemUser>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void Update()
    {
        Vector2 moveInput = inputActions.Gameplay.Move.ReadValue<Vector2>();
        Vector3 horizontalVelocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);

        UpdateAim();
        UpdateFire();
        UpdateInteract();
        UpdateUseItem();
    }

    private void UpdateAim()
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector2 pointerPosition = inputActions.Gameplay.Aim.ReadValue<Vector2>();
        Ray pointerRay = mainCamera.ScreenPointToRay(pointerPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (!groundPlane.Raycast(pointerRay, out float distance))
        {
            return;
        }

        Vector3 aimPoint = pointerRay.GetPoint(distance);
        Vector3 aimDirection = aimPoint - transform.position;
        aimDirection.y = 0f;

        if (aimDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        transform.forward = aimDirection.normalized;
    }

    private void UpdateFire()
    {
        if (shooter == null)
        {
            return;
        }

        if (inputActions.Gameplay.Fire.WasPressedThisFrame())
        {
            shooter.TryFire();
        }
    }

    private void UpdateInteract()
    {
        if (interactor == null)
        {
            return;
        }

        if (inputActions.Gameplay.Interact.WasPressedThisFrame())
        {
            interactor.TryInteract();
        }
    }

    private void UpdateUseItem()
    {
        if (itemUser == null)
        {
            return;
        }

        if (inputActions.Gameplay.UseSlot1.WasPressedThisFrame())
        {
            itemUser.TryUseSlot(0);
        }
        else if (inputActions.Gameplay.UseSlot2.WasPressedThisFrame())
        {
            itemUser.TryUseSlot(1);
        }
        else if (inputActions.Gameplay.UseSlot3.WasPressedThisFrame())
        {
            itemUser.TryUseSlot(2);
        }
        else if (inputActions.Gameplay.UseSlot4.WasPressedThisFrame())
        {
            itemUser.TryUseSlot(3);
        }
        else if (inputActions.Gameplay.UseSlot5.WasPressedThisFrame())
        {
            itemUser.TryUseSlot(4);
        }
        else if (inputActions.Gameplay.UseSlot6.WasPressedThisFrame())
        {
            itemUser.TryUseSlot(5);
        }
    }
}
