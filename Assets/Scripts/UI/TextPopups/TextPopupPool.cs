using UnityEngine;
using UnityEngine.Pool;

public class TextPopupPool : MonoBehaviour
{
    // singleton pattern
    public static TextPopupPool instance;
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
    private ObjectPool<TemplateProject.TextPopup> _poolObjectPool;
    private void Start()
    {
        SetupPool();
    }
    private void SetupPool()
    {
        _poolObjectPool = new ObjectPool<TemplateProject.TextPopup>(() =>
        {
            return Instantiate(GameAssets.instance.pfTextPopup, transform);
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
    public TemplateProject.TextPopup Spawn()
    {
        TemplateProject.TextPopup poolObject = _poolObjectPool.Get();
        poolObject.Init(KillPoolObject);
        return poolObject;
    }
    private void KillPoolObject(TemplateProject.TextPopup poolObject)
    {
        _poolObjectPool.Release(poolObject);
    }
}
