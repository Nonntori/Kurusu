using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Weapon/Shoot Config", order = 2)]
public class ShootConfig : ScriptableObject
{
    public LayerMask HitMask;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);

    public float FireRate = 0.25f;

    public Vector3 GetSpread(ParticleSystem shootSystem)
    {
        Vector3 shootDirection = shootSystem.transform.forward
                + new Vector3(
                    Random.Range(-Spread.x, Spread.x),
                    Random.Range(-Spread.y, Spread.y),
                    Random.Range(-Spread.z, Spread.z)
                    );
        shootDirection.Normalize();

        return shootDirection;
    }
}
