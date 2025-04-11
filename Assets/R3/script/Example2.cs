using System;
using System.Collections.Generic;
using UnityEngine;
using R3;

public class Example2 : MonoBehaviour
{
    private readonly KeyCode[] _comboKeys = { KeyCode.A, KeyCode.S, KeyCode.D };
    private readonly float _comboTimeLimit = 2.0f;
    private R3.Subject<Unit> _comboDetectedSubject = new R3.Subject<Unit>();
    
    private void Start()
    {
        // Observable cho combo được phát hiện
        _comboDetectedSubject
            .Subscribe(_ => OnComboDetected())
            .AddTo(this);
            
        // Khởi tạo combo detection
        StartComboDetection();
    }
    
    private void StartComboDetection()
    {
        // Tạo observable cho phím được nhấn
        var keyObservable = Observable.EveryUpdate()
            .Select(_ => GetCurrentKeyDown())
            .Where(key => key != KeyCode.None);
        
        // Theo dõi trạng thái combo
        int comboIndex = 0;
        IDisposable timeoutDisposable = null;
        
        // Đăng ký xử lý phím
        keyObservable.Subscribe(key => 
        {
            // Hủy timer hiện tại (nếu có)
            if (timeoutDisposable != null)
            {
                timeoutDisposable.Dispose();
                timeoutDisposable = null;
            }
            
            // Kiểm tra nếu phím không phải là phím tiếp theo trong combo
            if (key != _comboKeys[comboIndex])
            {
                // Nếu là phím đầu tiên trong combo, bắt đầu combo mới
                if (key == _comboKeys[0])
                {
                    comboIndex = 1;
                    
                    // Đặt timer mới
                    timeoutDisposable = Observable.Timer(TimeSpan.FromSeconds(_comboTimeLimit))
                        .Subscribe(_ => 
                        {
                            Debug.Log("Combo timeout - reset");
                            comboIndex = 0;
                        });
                }
                else
                {
                    // Reset combo
                    comboIndex = 0;
                }
                
                return;
            }
            
            // Tăng chỉ số combo
            comboIndex++;
            
            // Đặt timer mới
            timeoutDisposable = Observable.Timer(TimeSpan.FromSeconds(_comboTimeLimit))
                .Subscribe(_ => 
                {
                    Debug.Log("Combo timeout - reset");
                    comboIndex = 0;
                });
            
            // Phát hiện combo hoàn chỉnh
            if (comboIndex == _comboKeys.Length)
            {
                _comboDetectedSubject.OnNext(Unit.Default);
                comboIndex = 0;
                
                // Hủy timer vì combo đã hoàn thành
                if (timeoutDisposable != null)
                {
                    timeoutDisposable.Dispose();
                    timeoutDisposable = null;
                }
            }
        }).AddTo(this);
    }

    private KeyCode GetCurrentKeyDown()
    {
        foreach (KeyCode key in _comboKeys)
        {
            if (Input.GetKeyDown(key))
                return key;
        }
        return KeyCode.None;
    }
    
    private void OnComboDetected()
    {
        Debug.Log("Combo A-S-D hoàn thành!");
        
        // Hiển thị hiệu ứng trực quan
        Observable.Timer(TimeSpan.FromSeconds(0.1))
            .Subscribe(_ => 
            {
                Debug.Log("Hiệu ứng combo bắt đầu");
                
                Observable.Timer(TimeSpan.FromSeconds(0.5))
                    .Subscribe(_ => Debug.Log("Hiệu ứng combo kết thúc"))
                    .AddTo(this);
            })
            .AddTo(this);
    }
}