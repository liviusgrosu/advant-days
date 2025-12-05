using UnityEngine;

public class Counter : MonoBehaviour
{
    private int _count;

    [SerializeField] private Transform firstDigit;
    [SerializeField] private Transform secondDigit;
    [SerializeField] private Transform thirdDigit;
    
    private void Start()
    {
        Dial.Instance.ZeroHitEvent += IncreaseCount;
    }

    private void IncreaseCount()
    {
        Debug.Log("Hit");
        _count++;

        if (_count % 100 == 0)
        {
            thirdDigit.Rotate(0, 0, -36f);
        }
        if (_count % 10 == 0)
        {
            secondDigit.Rotate(0, 0, -36f);
        }
        
        firstDigit.Rotate(0, 0, -36f);
    }
}
