using UnityEngine;

public class FullScreenLoading : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ShowLoading()
    {
        EnableLoading(true);
    }
    
    public void HideLoading()
    {
        EnableLoading(false);
    }
    
    private void EnableLoading(bool enable)
    {
        if (gameObject.activeSelf == enable)
        {
            return;
        }
        
        gameObject.SetActive(enable);
    }
    
}
