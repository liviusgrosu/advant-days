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
    
    [Tooltip("The dial that will be turned")]
    [SerializeField] private Transform dial;
    [Tooltip("How fast the dial will turn (degrees per second")]
    [SerializeField] private float rotationSpeed;
    [Tooltip("How close to the target angle the dial needs to be to be considered a hit")]
    [SerializeField] private float angleTolerance = 5f;
    [Tooltip("How long to wait between steps")]
    [SerializeField] private float stepDelay = 1f;
    
    private List<Step> _tempDirections;
    private int _currentIdx;
    private Step _currentStep;
    private int _currentDirection;
    private int _currentTurnAmount;
    private Quaternion _targetDirection;
    private bool _delayActive;
    
    public Action ZeroHitEvent; 
    
    
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

        // If angle is close enough to target, then just snap to it 
        var angle = Quaternion.Angle(dial.rotation, _targetDirection); 
        if (angle < angleTolerance)
        {
            dial.rotation = _targetDirection;
            // If it's really close to 0 then its a hit
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
        // Convert from 360 to 100 scale
        var targetTurnAmount = (_currentTurnAmount * _currentDirection * 3.6f);
        _targetDirection = Quaternion.Euler(
            dial.eulerAngles.x, 
            dial.eulerAngles.y, 
            dial.eulerAngles.z + targetTurnAmount
        );
    }
    
    private IEnumerator WaitBetweenSteps()
    {
        _delayActive = true;
        yield return new WaitForSeconds(stepDelay);
        NextStep();
        _delayActive = false;
    } 
}
