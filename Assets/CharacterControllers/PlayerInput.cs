using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    private Vector2 _inputDirection = new Vector2(0f, 0f);
    private BasicCharacterController _characterController;
    private PlayerWeaponSystem _weaponSystem;
    private bool _rollThisFrame, _shootThisFrame, _swapWeaponThisFrame, _reloadThisFrame;
    private void Awake()
    {
        _characterController = GetComponent<BasicCharacterController>();
        _weaponSystem = GetComponent<PlayerWeaponSystem>();
    }
    void Update()
    {
        GatherInputs();
    }
    private void FixedUpdate()
    {
        HandleInputs();
    }
    private void GatherInputs()
    {
        _inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.Space))
        {
            _rollThisFrame = true;
        }
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) // checks whether the mouse is over a ui element,
            {
                return;
            }
            _shootThisFrame = true;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _swapWeaponThisFrame = true;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            _reloadThisFrame = true;
        }
    }
    private void HandleInputs()
    {
        _characterController.FaceTarget(GeneralUtility.GetMouseWorldPosition());
        _characterController.SetMoveVector(_inputDirection.normalized);
        if (_rollThisFrame)
        {
            _characterController.RollThisFrame();
            _rollThisFrame = false;
        }
        if (_swapWeaponThisFrame)
        {
            _weaponSystem.ToggleWeapon();
            _swapWeaponThisFrame = false;
        }
        if (_shootThisFrame)
        {
            _weaponSystem.Fire();
            _shootThisFrame = false;
        }
        if (_reloadThisFrame)
        {
            _weaponSystem.Reload();
            _reloadThisFrame = false;
        }
    }
}
