using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Step
{
    public string direction;
    public int amount;

    public Step(string direction, int amount)
    {
        this.direction = direction;
        this.amount = amount;
    }
}

public class Dial : MonoBehaviour
{
    public static Dial Instance;
    
    [SerializeField] private Transform dial;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float angleTolerance = 5f;
    
    private List<Step> _tempDirections;
    private int _currentIdx;
    private Step _currentStep;
    private int _currentDirection;
    private int _currentTurnAmount;
    private Quaternion _targetDirection;

    public Action ZeroHitEvent; 
    
    // TODO: REMOVE BELOW
    private bool _delayActive;
    private int _zeroCount;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        
        _tempDirections = LoadFile.LoadSteps("Assets/Data/steps.txt");
    }
    
    void Start()
    {
        dial.Rotate(0, 0, 180f);
        NextStep();
    }

    void Update()
    {
        if (_delayActive || _currentIdx >= _tempDirections.Count)
        {
            return;
        }
        dial.RotateAround(transform.position, transform.forward, _currentDirection * rotationSpeed * Time.deltaTime);

        var angle = Quaternion.Angle(dial.rotation, _targetDirection); 
        if (angle < angleTolerance)
        {
            dial.rotation = _targetDirection;
            if (Mathf.Abs(Mathf.DeltaAngle(dial.eulerAngles.z, 360f)) < 1f)
            {
                ZeroHitEvent?.Invoke();
            }
            _currentIdx++;
            StartCoroutine(WaitBetweenSteps());
        }
    }

    void NextStep()
    {
        if (_currentIdx >= _tempDirections.Count - 1)
        {
            return;
        }
        _currentStep = _tempDirections[_currentIdx];
        _currentDirection = _currentStep.direction == "L" ? -1 : 1;
        _currentTurnAmount = _currentStep.amount;
        _targetDirection = Quaternion.Euler(
            dial.eulerAngles.x, 
            dial.eulerAngles.y, 
            dial.eulerAngles.z + (_currentTurnAmount * _currentDirection * 3.6f)
        );
    }
    
    private IEnumerator WaitBetweenSteps()
    {
        _delayActive = true;
        yield return new WaitForSeconds(0.5f);
        NextStep();
        _delayActive = false;
    } 
}
