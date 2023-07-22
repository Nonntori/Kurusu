using UnityEngine;

public class PlayerMovement
{
    private CharacterController _characterController;
    private Transform _transform;
    private Plane _groundPlane;

    private Vector3 _moveDirection;
    private Vector3 _mousePosition;
    
    private float _rayLength;
    private float _speedMovement;

    public PlayerMovement(CharacterController characterController, float speedMovement, Transform transform)
    {
        _characterController = characterController;
        _speedMovement = speedMovement;
        _transform = transform;

        _groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public void UpdateMovement()
    {
        _characterController.Move(_moveDirection * Time.deltaTime * _speedMovement);
        Rotation();
    }

    public void Move(Vector3 moveDirection)
    {
        _moveDirection = moveDirection;
        _moveDirection = new Vector3(_moveDirection.x, 0f, _moveDirection.y).normalized;
    }

    public void Aim(Vector2 currentMousePosition)
    {
        _mousePosition = currentMousePosition;
    }

    public void Rotation()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(_mousePosition);
        
        if (_groundPlane.Raycast(cameraRay, out _rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(_rayLength);
            Vector3 look = new Vector3(pointToLook.x, _transform.position.y, pointToLook.z);

            _transform.LookAt(look);
        }
    }
}
