using UnityEngine;

public class Input : MonoBehaviour
{
    [SerializeField] private Player _player;

    private PlayerInput _input;
    private Vector2 _currentMousePosition;
    private bool _isReload = true;

    void Awake()
    {
        _input = new PlayerInput();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.Movement.performed += OnMovement;
        _input.Player.Movement.canceled += OnStopMovement;

        _input.Player.Shoot.performed += OnShoot;
    }


    private void OnDisable()
    {
        _input.Disable();

        _input.Player.Movement.performed -= OnMovement;
        _input.Player.Movement.canceled -= OnStopMovement;

        _input.Player.Shoot.performed -= OnShoot;
    }

    private void Update()
    {
        _currentMousePosition = _input.Player.Aim.ReadValue<Vector2>();
        _player.Movement.Aim(_currentMousePosition);
    }

    private void OnShoot(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_isReload && _player.ActiveWeapon.CurrentClipAmmo == 0 
            && _player.ActiveWeapon.IsReaload())
        {
            _player.ActiveWeapon.Reload();
        }

        _player.ActiveWeapon.Shoot();
    }

    private void OnMovement(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _player.Movement.Move(_input.Player.Movement.ReadValue<Vector2>());
    }

    private void OnStopMovement(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _player.Movement.Move(Vector2.zero);
    }
}
