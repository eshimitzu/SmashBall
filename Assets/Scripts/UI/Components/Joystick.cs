using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace SmashBall.UI.Components
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Joystick Components")]
        [SerializeField] private RectTransform joystickBackground;
        [SerializeField] private RectTransform joystickHandle;
    
        [Header("Joystick Settings")]
        [SerializeField] private float handleRange = 100f;
    
        private Vector2 inputVector = Vector2.zero;

        public Vector2 Direction => inputVector;
        public float Horizontal => inputVector.x;
        public float Vertical => inputVector.y;
        public float Magnitude => inputVector.magnitude;
    
        [Inject] private PlayerInput playerInput;


        private void Start()
        {
            playerInput.IsPointerUp = false;
            ResetJoystick();
        }

    
        public void OnPointerDown(PointerEventData eventData)
        {
            joystickBackground.gameObject.SetActive(true);
            joystickBackground.anchoredPosition = eventData.position;
        
            joystickHandle.gameObject.SetActive(true);
            joystickHandle.anchoredPosition = eventData.position;
        }

    
        public void OnDrag(PointerEventData eventData)
        {
            if (joystickBackground == null || joystickHandle == null) return;

            inputVector = (eventData.position - joystickBackground.anchoredPosition) / handleRange;
            if (inputVector.magnitude > 1f)
                inputVector = inputVector.normalized;
        
            joystickHandle.anchoredPosition = joystickBackground.anchoredPosition + inputVector * handleRange;
        
            playerInput.SetInputVector(inputVector);
        }

    
        public void OnPointerUp(PointerEventData eventData)
        {
            ResetJoystick();
            playerInput.IsPointerUp = true;
        }


        private void ResetJoystick()
        {
            inputVector = Vector2.zero;
            playerInput.SetInputVector(inputVector);

            joystickHandle.anchoredPosition = Vector2.zero;
            joystickBackground.gameObject.SetActive(false);
            joystickHandle.gameObject.SetActive(false);
        }
    }
}