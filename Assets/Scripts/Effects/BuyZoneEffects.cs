using UnityEngine;

[RequireComponent(typeof(BuyZone))]
public class BuyZoneEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem _spawnEffect;
    [SerializeField] private Vector3 _offset;

    private BuyZone _area;

    private void Awake()
    {
        _area = GetComponent<BuyZone>();
    }

    private void OnEnable()
    {
        _area.Spawned += OnSpawned;
    }

    private void OnDisable()
    {
        _area.Spawned -= OnSpawned;
    }

    private void OnSpawned(GoodsHolder holder)
    {
        Instantiate(_spawnEffect.gameObject, transform.position + _offset, Quaternion.identity);
    }
}