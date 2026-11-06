using Cysharp.Threading.Tasks;
using UnityEngine;

//In case needed animation or something else to be done with the building
public class BuildingInteractionBlocker : MonoBehaviour
{
    public float blockTime = .2f;

    public async UniTask StartTimer()
    {
        await UniTask.WaitForSeconds(blockTime);
        Destroy(this);
    }
}