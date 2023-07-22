using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Weapon", order = 0)]
public class Weapon : ScriptableObject
{
    public WeaponType Type;
    public string Name;
    public int Damage;
    public int ClipSize;
    public int CurrentAmmo;
    public int CurrentClipAmmo = 100;
    public GameObject ModelPrefab;

    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public ShootConfig ShootConfig;
    public TrailConfig TrailConfig;

    private MonoBehaviour _activeMonoBehaviour;
    private GameObject _model;
    private float _lastShootTime;
    private ParticleSystem _shootSystem;
    private ObjectPool<TrailRenderer> _trailPool;


    public void Spawn(Transform parent, MonoBehaviour monoBehaviour)
    {
        _activeMonoBehaviour = monoBehaviour;
        _lastShootTime = 0f;
        _trailPool = new ObjectPool<TrailRenderer>(GetTrail);

        _model = Instantiate(ModelPrefab);
        _model.transform.SetParent(parent, false);
        _model.transform.SetLocalPositionAndRotation(SpawnPoint, Quaternion.Euler(SpawnRotation));

        _shootSystem = _model.GetComponentInChildren<ParticleSystem>();
    }

    public void Shoot()
    {
        if (Time.time > ShootConfig.FireRate + _lastShootTime)
        {
            _lastShootTime = Time.time;
            _shootSystem.Play();

            Vector3 shootDirection = ShootConfig.GetSpread(_shootSystem);

            CurrentAmmo--;

            if (Physics.Raycast(_shootSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, ShootConfig.HitMask))
            {
                _activeMonoBehaviour.StartCoroutine(PlayTrail(_shootSystem.transform.position, hit.point, hit));
            }
            else
            {
                _activeMonoBehaviour.StartCoroutine(PlayTrail(
                    _shootSystem.transform.position,
                    _shootSystem.transform.position + (shootDirection * TrailConfig.MissDistance),
                    new RaycastHit()));
            }
        }
    }

    public bool IsReaload()
    {
        return CurrentAmmo < ClipSize && CurrentAmmo > 0;
    }

    private TrailRenderer GetTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer instance = _trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                startPoint,
                endPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPoint;

        if (hit.collider != null && hit.collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(Damage);
        }

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;

        instance.emitting = false;
        instance.gameObject.SetActive(false);
        _trailPool.Release(instance);

    }

    public void Reload()
    {
        int maxRealoadAmount = Mathf.Min(ClipSize, CurrentAmmo);
        int availableBulletsInCirrentClip = ClipSize - CurrentClipAmmo;
        int reloadAmount = Mathf.Min(maxRealoadAmount, availableBulletsInCirrentClip);

        CurrentClipAmmo = CurrentClipAmmo + reloadAmount;
        CurrentAmmo -= reloadAmount;

        Debug.Log(CurrentAmmo);
    }
}
