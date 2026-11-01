using UnityEngine;

public class RotateUI : MonoBehaviour
{
    [SerializeField] private float _speed = 90;
    private void Update()
    {
        transform.Rotate(0, 0, this._speed * Time.deltaTime);
    }
}