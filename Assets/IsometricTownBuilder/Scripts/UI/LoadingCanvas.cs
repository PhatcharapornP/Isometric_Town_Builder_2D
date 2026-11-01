using System;
using UnityEngine;


public class LoadingCanvas : MonoBehaviour
{
    //TODO: wanna do DI + vital router later if have time, but this will do for the time being.
    public static LoadingCanvas Instance;
    [SerializeField] private SoftLoad softLoad;
    [SerializeField] private FullScreenLoading fullScreenLoading;
    
    public SoftLoad SoftLoad => softLoad;
    public FullScreenLoading FullScreenLoading => fullScreenLoading;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
