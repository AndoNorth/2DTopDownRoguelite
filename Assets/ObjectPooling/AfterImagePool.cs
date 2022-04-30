using UnityEngine;
using UnityEngine.Pool;

public class AfterImagePool : MonoBehaviour
{
    // singleton pattern
    public static AfterImagePool instance;
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

    [SerializeField] private AfterImage _pfPoolObject;
    private ObjectPool<AfterImage> _poolObjectPool;
    private void Start()
    {
        SetupPool();
    }
    private void SetupPool()
    {
        _poolObjectPool = new ObjectPool<AfterImage>(() =>
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
    public AfterImage Spawn()
    {
        AfterImage poolObject = _poolObjectPool.Get();
        poolObject.InitKillAction(KillPoolObject);
        return poolObject;
    }
    public void SetPoolObject(AfterImage pfPoolObject)
    {
        _pfPoolObject = pfPoolObject;
    }
    private void KillPoolObject(AfterImage poolObject)
    {
        _poolObjectPool.Release(poolObject);
    }
}
