using UnityEngine;

public class DropRandomAmountOfObjects : MonoBehaviour
{
    [SerializeField] GameObject _dropItem;
    [SerializeField] RandRange _dropAmountRange = new RandRange { min = 3, max = 5};
    private int _dropAmount;
    [SerializeField] private bool _dropInRandomRadius;

    private void Start()
    {
        _dropAmount = UnityEngine.Random.Range(_dropAmountRange.min, _dropAmountRange.max);
    }
    private void OnDisable()
    {
        DropObjects(transform.position);
    }
    public void DropObjects(Vector3 dropPosition)
    {
        for (int i = 0; i < _dropAmount; i++)
        {
            Vector3 dropPos = dropPosition;
            if (_dropInRandomRadius)
            {
                Vector2 randPosition = UnityEngine.Random.insideUnitCircle;
                if(randPosition.magnitude < 0.5f)
                {
                    float angle = Vector2.Angle(Vector2.zero, randPosition);
                    float rootTwo = 0.707107f;
                    randPosition += rootTwo * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }
                dropPos += new Vector3(randPosition.x, randPosition.y);
            }
            DropObject(dropPos);
        }
    }
    private void DropObject(Vector3 dropPosition)
    {
        GameObject.Instantiate(
            _dropItem,
            dropPosition,
            Quaternion.identity);
    }
    [System.Serializable]
    private struct RandRange
    {
        public int min;
        public int max;
    }
}
