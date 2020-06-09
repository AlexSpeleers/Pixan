using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

delegate void GameEndedHandler();
public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler 
{
    private GameEndedHandler gameEndedHandler;
    private GameState gameState;
    [SerializeField] private Image bgImage;
    [SerializeField] private Image joystickImage;
    public Player player;
    private bool isTouch = false;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 inputVector;

    private void Start()
    {
        gameEndedHandler = delegate { this.gameObject.SetActive(false); };
        gameState.OnGameLost.AddListener(gameEndedHandler.Invoke);
        gameState.OnPlayerWinLvl.AddListener(gameEndedHandler.Invoke);
    }
    [Inject]
    private void Init(GameState gameState)
    {
        this.gameState = gameState;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        startPoint = new Vector3(eventData.position.x, eventData.position.y, Camera.main.transform.position.z);
        bgImage.rectTransform.position = startPoint;
        bgImage.gameObject.SetActive(true);
    }
    public void OnDrag(PointerEventData eventData)
    {
        isTouch = true;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform, eventData.position, eventData.pressEventCamera, out endPoint))
        {
            endPoint.x = (endPoint.x / bgImage.rectTransform.sizeDelta.x);
            endPoint.y = (endPoint.y / bgImage.rectTransform.sizeDelta.y);
        }
        inputVector = new Vector2(endPoint.x * 2 + 1, endPoint.y * 2 - 1);
        inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
        joystickImage.rectTransform.anchoredPosition =
            new Vector2(inputVector.x * (bgImage.rectTransform.sizeDelta.x / 3), inputVector.y * (bgImage.rectTransform.sizeDelta.y / 3));
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
        joystickImage.transform.position = Vector3.zero;
        bgImage.gameObject.SetActive(false);
        player.DisableMove();
        player.FindClosestEnemy();
    }

    private void FixedUpdate()
    {
        if (isTouch) 
        {
            player.Move(inputVector);            
        }
    }
    private void OnDestroy()
    {
        gameState.OnGameLost.RemoveListener(gameEndedHandler.Invoke);
        gameState.OnPlayerWinLvl.RemoveListener(gameEndedHandler.Invoke);
    }
}