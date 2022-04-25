using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    private Vector2 _inputDirection = new Vector2(0f, 0f);
    private BasicCharacterController _characterController;
    private PlayerWeaponSystem _weaponSystem;
    private bool _rollThisFrame, _shootThisFrame, _swapWeaponThisFrame, _reloadThisFrame, _interactThisFrame;
    private bool _dropThisFrame, _swapToLastWeaponThisFrame;
    private int _weaponIdx;

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
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            _weaponIdx = 0;
            _swapWeaponThisFrame = true;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            _weaponIdx = 1;
            _swapWeaponThisFrame = true;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            _weaponIdx = 2;
            _swapWeaponThisFrame = true;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _swapWeaponThisFrame = true;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            _reloadThisFrame = true;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            _interactThisFrame = true;
        }
        if (Input.GetKeyUp(KeyCode.F))
        {

        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            _dropThisFrame = true;
        }
    }
    private void HandleInputs()
    {
        if (_interactThisFrame)
        {
            SelectableObject selectedObj = SelectionManager.instance.CurrentlySelectedItem;
            if (selectedObj != null && selectedObj.IsInteractable)
            {
                SelectionManager.instance.CurrentlySelectedItem.GetComponent<IInteractable>().Interact();
                _interactThisFrame = false;
            }
        }
        _characterController.FaceTarget(GeneralUtility.GetMouseWorldPosition());
        _characterController.SetMoveVector(_inputDirection.normalized);
        if (_rollThisFrame)
        {
            _characterController.RollThisFrame();
            ResetInputs();
        }
        if (_dropThisFrame)
        {
            _weaponSystem.DropCurrentWeapon();
            ResetInputs();
            return;
        }
        if (_swapWeaponThisFrame)
        {
            _weaponSystem.ChangeWeapon(_weaponIdx);
            ResetInputs();
            return;
        }
        if (_swapToLastWeaponThisFrame)
        {
            _weaponSystem.ToggleWeaponSlot();
            ResetInputs();
            return;
        }
        if (_shootThisFrame)
        {
            _weaponSystem.Fire();
            ResetInputs();
        }
        if (_reloadThisFrame)
        {
            _weaponSystem.Reload();
            ResetInputs();
        }
    }
    private void ResetInputs()
    {
        _rollThisFrame = false;
        _shootThisFrame = false;
        _swapWeaponThisFrame = false;
        _reloadThisFrame = false;
        _interactThisFrame = false;
        _dropThisFrame = false;
    }
}
