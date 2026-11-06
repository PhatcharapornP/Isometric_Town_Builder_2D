using UnityEngine;

public class SoftLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ShowSoftLoad()
    {
        EnableSoftLoadObj(true);
    }

    public void HideSoftLoad()
    {
        EnableSoftLoadObj(false);
    }

    private void EnableSoftLoadObj(bool enable)
    {
        if (gameObject.activeSelf == enable)
        {
            return;
        }
        
        gameObject.SetActive(enable);
    }
}
