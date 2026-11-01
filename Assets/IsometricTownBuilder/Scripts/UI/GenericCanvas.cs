using System;
using UnityEngine;

public class GenericCanvas : MonoBehaviour
{
    [SerializeField] private SoftLoad softLoad;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
