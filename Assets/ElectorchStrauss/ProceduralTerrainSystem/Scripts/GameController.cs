using UnityEngine;
using UnityEngine.InputSystem;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class GameController : MonoBehaviour
    {
        PlayerInput playerInput;

        [SerializeField, HideInInspector] public InputAction rotateAroundAction,
            focusSelectedAction,
            moveShiftAction,
            moveInOutAction,
            mousePosAction,
            leftClickAction,
            moveAction,
            jumpAction,
            lookAction,
            altAction,
            escpAction;

        [Header("Rotate around : press middle mouse button")]
        public float rotateAround;

        [Header("Focus selected object : numpad . key")]
        public float focusSelected;

        [Header("Move left right up and down : shift key")]
        public float moveShift;

        [Header("Move forward backward : mouse scroll wheel up and down")]
        public Vector2 moveInOut;

        public Vector2 mousePos;
        public float leftClick;
        
        public Vector2 look;
        [Tooltip("X is Horizontal Y is Vertical")] [Header("Movement")]
        public Vector2 move;
        [Header("SpaceBar")] public float jump;
        [Header("Camera Unlock")] public float alt;
        [Header("Escp")] public float escp;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            rotateAroundAction = playerInput.actions["RotateAround"];
            focusSelectedAction = playerInput.actions["FocusSelected"];
            moveShiftAction = playerInput.actions["MoveShift"];
            moveInOutAction = playerInput.actions["MoveInOut"];
            mousePosAction = playerInput.actions["MousePos"];
            leftClickAction = playerInput.actions["LeftClick"];
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            lookAction = playerInput.actions["Look"];
            altAction = playerInput.actions["Alt"];
            escpAction = playerInput.actions["Escp"];
        }

        void Update()
        {
            InputActionReadValue();
        }

        private void InputActionReadValue()
        {
            rotateAround = rotateAroundAction.ReadValue<float>();
            focusSelected = focusSelectedAction.ReadValue<float>();
            moveShift = moveShiftAction.ReadValue<float>();
            moveInOut = moveInOutAction.ReadValue<Vector2>();
            mousePos = mousePosAction.ReadValue<Vector2>();
            leftClick = leftClickAction.ReadValue<float>();
            move = moveAction.ReadValue<Vector2>();
            jump = jumpAction.ReadValue<float>();
            look = lookAction.ReadValue<Vector2>();
            alt = altAction.ReadValue<float>();
            escp = escpAction.ReadValue<float>();
        }
    }
}