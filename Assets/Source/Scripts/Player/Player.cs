using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Transform))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _speedMovement;
    [SerializeField] private WeaponType _weapon;
    [SerializeField] private Transform _gunParent;
    [SerializeField] private List<Weapon> _weapons;

    private Weapon _activeGun;
    private PlayerMovement _movement;
    private CharacterController _characterController;
    private Transform _transform;

    public Weapon ActiveWeapon => _activeGun;
    public PlayerMovement Movement => _movement;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _transform = GetComponent<Transform>();
        _movement = new PlayerMovement(_characterController, _speedMovement,_transform);
        SelectorWeapon();
    }

    private void Update()
    {
        _movement.UpdateMovement();
    }

    private void SelectorWeapon()
    {
        Weapon weapon = _weapons.Find(weapon => weapon.Type == _weapon);

        if (weapon == null)
        {
            Debug.LogError($"No GunScriptableObject found for GunType: {weapon}");
            return;
        }

        _activeGun = weapon;
        weapon.Spawn(_gunParent, this);
    }
}
