using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Computer : MonoBehaviour
{
    public static Computer Instance;
    public ScrollRect scrollRect;
    
    private List<(long, long)> _ranges;
 
    [SerializeField] private float scrollSpeed = 10f;
    [SerializeField] private Transform skuTextPrefab;
    [SerializeField] private Transform contentView;
    [SerializeField] private int windowRange = 10;
    [SerializeField] private TMP_Text totalSKUText;
    
    private Queue<Transform> _visibleText;
    
    private int currentRangeIdx;
    private int currentNumberIdx;
    private int _currentScrollIdx;
    private int _postScrollIdx;

    private int _currentViewedRangeIdx;
    private int _currentViewedNumberIdx;

    private long _totalSKU;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
     
        _visibleText = new Queue<Transform>();
        _ranges = LoadFile.LoadDay2Steps("Assets/Data/Day2.txt");
    }

    private void Start()
    {
        LoadInitialData();
        InvokeRepeating("ScrollToNextNumber", 0, scrollSpeed);
    }

    private void LoadInitialData()
    {
        for (var idx = 0; idx < windowRange; idx++)
        {
            AddNewNumber();
        }
    }

    private void ScrollToNextNumber()
    {
        scrollRect.content.anchoredPosition += new Vector2(0, 85);
        ValidateNumber();
        
        _currentScrollIdx++;

        if (_currentScrollIdx >= 20)
        {
            DestroyFirstNumber();
            AddNewNumber();
        }

    }

    private void DestroyFirstNumber()
    {
        var obj = _visibleText.Dequeue().gameObject;
        Destroy(obj);
    }

    private void AddNewNumber()
    {
        if (currentRangeIdx > _ranges.Count - 1)
        {
            return;
        }
        
        var value = _ranges[currentRangeIdx].Item1 + currentNumberIdx;
        var skuText = Instantiate(skuTextPrefab, contentView);
        skuText.GetComponent<TMP_Text>().text = value.ToString();
        skuText.SetParent(contentView);
        skuText.localPosition = Vector3.zero + new Vector3(0, -_postScrollIdx * 85, 0);
        skuText.name = $"{value.ToString()} - SKU";
        _visibleText.Enqueue(skuText);
       
        _postScrollIdx++;
        currentNumberIdx++;
        if (_ranges[currentRangeIdx].Item1 + currentNumberIdx > _ranges[currentRangeIdx].Item2)
        {
            currentRangeIdx++;
            currentNumberIdx = 0;
        }
    }

    private void ValidateNumber()
    {
        if (_currentViewedRangeIdx > _ranges.Count - 1)
        {
            return;
        }
            
        var value = _ranges[_currentViewedRangeIdx].Item1 + _currentViewedNumberIdx;
        
        var str = value.ToString();
        var len = str.Length;
        var cSplit = 2;
        var isFalseNumber = false;
        
        while (cSplit <= len)
        {
            if (len % cSplit != 0)
            {
                cSplit++;
                continue;
            }
            var parts = BreakIntoList(str, cSplit);
            var match = parts.Distinct().Count() == 1;
            if (match)
            {
                isFalseNumber = true;
                _totalSKU += value; 
                totalSKUText.text = $"Total: {_totalSKU.ToString()}";
                //count+=idx;
                break;
            }

            cSplit++;
        }
        
        var text = GameObject.Find($"{value.ToString()} - SKU");
        text.GetComponent<TMP_Text>().color = isFalseNumber ? Color.red : Color.green;
        
        _currentViewedNumberIdx++;
        if (_ranges[_currentViewedRangeIdx].Item1 + _currentViewedNumberIdx > _ranges[_currentViewedRangeIdx].Item2)
        {
            _currentViewedRangeIdx++;
            _currentViewedNumberIdx = 0;
        }
    }
    
    private static List<string> BreakIntoList(string input, int amount)
    {
        var size = input.Length / amount;
        return input
            .Select((ch, i) => new { ch, group = i / size })
            .GroupBy(x => x.group)
            .Select(g => new string(g.Select(x => x.ch).ToArray()))
            .ToList();
    }

}
