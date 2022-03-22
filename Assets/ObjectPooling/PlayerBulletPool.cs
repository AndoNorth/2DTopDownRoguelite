using UnityEngine;
using UnityEngine.Pool;

public class PlayerBulletPool : MonoBehaviour
{
    // singleton pattern
    public static PlayerBulletPool instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private DamageProjectile _pfPoolObject;
    private ObjectPool<DamageProjectile> _poolObjectPool;
    private void Start()
    {
        SetupPool();
    }
    private void SetupPool()
    {
        _pfPoolObject = GameAssets.instance.templateBullet;
        _poolObjectPool = new ObjectPool<DamageProjectile>(() =>
        {
            return Instantiate(_pfPoolObject, transform);
        }, poolObject =>
        {
            poolObject.gameObject.SetActive(true); // on pool.Get()
        }, poolObject =>
        {
            poolObject.gameObject.SetActive(false); // on pool.Release()
        }, poolObject =>
        {
            Destroy(poolObject.gameObject); // if no.objects exceeds default capacity
        }, false, // collection check
           100,    // default capacity(allocates enough memory for this amount, similar to array declaration)
           250);   // max capacity of objects (limits the previous value)
    }
    public DamageProjectile Spawn()
    {
        DamageProjectile poolObject = _poolObjectPool.Get();
        poolObject.InitKillAction(KillPoolObject);
        return poolObject;
    }
    public void SetPoolObject(DamageProjectile pfPoolObject)
    {
        _pfPoolObject = pfPoolObject;
    }
    private void KillPoolObject(DamageProjectile poolObject)
    {
        _poolObjectPool.Release(poolObject);
    }
}
