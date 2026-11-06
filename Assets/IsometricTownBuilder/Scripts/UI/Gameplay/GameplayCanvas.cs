#nullable enable
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class GameplayCanvas : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private BuildingOptionPanel buildingOptionPanel;
    [SerializeField] private BuildCatalogPanel buildCatalogPanel;
    [SerializeField] private Image holdingImage;
    [SerializeField] private GenericButton exitDestroyModeButton;
    [SerializeField] private QuizPopup quizPopup;
    [SerializeField] private TextMeshProUGUI midScreenText;
    
    [field: SerializeField] public bool IsInitialized { get; private set; } = false;
    private GameplayUIManager gameplayUIManager => GamePlayManager.Instance.GamePlayUI;
    private Camera targetCamera => Camera.main;
    public BuildingOptionPanel BuildingOptionPanel => buildingOptionPanel;
    public  BuildCatalogPanel BuildCatalogPanel => buildCatalogPanel;
    public GenericButton ExitDestroyModeButton => exitDestroyModeButton;
    
    public QuizPopup QuizPopup => quizPopup;
    
    private void Start()
    {
        Initialize().Forget();
    }

    private async UniTask Initialize()
    {
        await UniTask.WaitUntil(() => gameplayUIManager != null);
        canvas = GetComponent<Canvas>();
        await buildCatalogPanel.Initialize();
        gameplayUIManager.SetGamePlayUI(this);
        buildingOptionPanel.Initialize(canvas);
        IsInitialized = true;
    }

    public void SetHoldingImageValue(float value)
    {
        holdingImage.fillAmount = value;
    }

    public void EnableHoldingImage(bool enable)
    {
        if (holdingImage.gameObject.activeInHierarchy == enable)
        {
            return;
        }
        
        holdingImage.gameObject.SetActive(enable);
    }

    public void SetHoldingPosition(Vector2 screenPos)
    {
        bool posIsInScreenRect = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)canvas.transform,
            screenPos, targetCamera,
            out Vector3 worldPos
        );
        if (posIsInScreenRect)
        {
            holdingImage.transform.position = worldPos;
        }
    }
    
    public void EnableExitDestroyMode(bool enable)
    {
        Debug.Log($"[GameplayCanvas] EnableExitDestroyMode {enable}".LogColor(Color.orangeRed),exitDestroyModeButton.gameObject);
        exitDestroyModeButton.gameObject.SetActive(enable);
    }

   
    
    public void SetMidScreenText(string text)
    {
        midScreenText.SetText(text);
    }

    public void EnableMidScreenText(bool enable)
    {
        midScreenText.gameObject.SetActive(enable);
    }
}
