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
                dropPos += GetRandomRadius(0.5f);
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
    private Vector3 GetRandomRadius(float magnitudeThreshold)
    {
        Vector2 randPosition = UnityEngine.Random.insideUnitCircle;
        if (randPosition.magnitude < magnitudeThreshold)
        {
            float angle = Vector2.Angle(Vector2.zero, randPosition);
            float rootThreshold = Mathf.Sqrt(magnitudeThreshold);
            randPosition += rootThreshold * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        return new Vector3(randPosition.x, randPosition.y);
    }
    [System.Serializable]
    private struct RandRange
    {
        public int min;
        public int max;
    }
}
