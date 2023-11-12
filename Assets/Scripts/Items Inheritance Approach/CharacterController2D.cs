using UnityEngine;
using UnityEngine.InputSystem;

namespace Items_Inheritance_Approach
{
    public class CharacterController2D : MonoBehaviour
    {
        [SerializeField][Range(0, 10)] private int MoveSpeed;
        [SerializeField][Range(0, 10)] private int TurnSpeed;
    
        private PlayerInputActions _playerInputActions;
        private Rigidbody2D _rb;
        private Transform _grabTransform;
        private Item _itemSelected, _itemHovered;
        private float _moveSpeed, _turnSpeed;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _grabTransform = transform.GetChild(0).GetChild(0);
            _moveSpeed = MoveSpeed;
            _turnSpeed = TurnSpeed;
        
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.PauseGame.performed += OnPauseGamePerformed;
            _playerInputActions.Player.Select.performed += OnSelectPerformed;
            _playerInputActions.UI.ResumeGame.performed += OnResumeGamePerformed;
            _playerInputActions.UI.Confirm.performed += OnConfirmPerformed;
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_itemSelected == null && other.TryGetComponent(out Item item))
            {
                if (_itemHovered) _itemHovered.UnHover();
                _itemHovered = item;
                _itemHovered.Hover();
            }
            else if (_itemSelected is PropItem && other.TryGetComponent(out BaseItem baseItem))
            {
                baseItem.Hover();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_itemSelected == null && other.TryGetComponent(out Item item))
            {
                if (_itemHovered != item || !other.enabled) return;
                if (_itemHovered) _itemHovered.UnHover();
                _itemHovered = null;
            }
            else if (_itemSelected is PropItem && other.TryGetComponent(out BaseItem baseItem))
            {
                baseItem.UnHover();
            }
        }

        private void SwitchActionMap_PlayerToUI()
        {
            _playerInputActions.Player.Disable();
            _playerInputActions.UI.Enable();
        }
    
        private void SwitchActionMap_UIToPlayer()
        {
            _playerInputActions.UI.Disable();
            _playerInputActions.Player.Enable();
        }

        #region Player Actions

        private void Move()
        {
            var input = _playerInputActions.Player.Move.ReadValue<Vector2>();
            _rb.velocity = input * (30 * _moveSpeed * Time.deltaTime);
            if (input != Vector2.zero)
            {
                Turn(input);
            }
        }

        private void Turn(Vector3 direction)
        {
            float degreesDelta = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Vector3.forward, direction));
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(Vector3.forward, direction), 3f * degreesDelta * _turnSpeed * Time.deltaTime);
        }

        private void OnSelectPerformed(InputAction.CallbackContext context)
        {
            if (_itemHovered == null) return;
        
            if (_itemSelected != null)
            {
                if (_itemSelected.TryPlace())
                {
                    _itemSelected = null;
                    _moveSpeed = MoveSpeed;
                    _turnSpeed = TurnSpeed;
                }
                return;
            }

            if (_itemHovered.TryGrab(_grabTransform))
            {
                _itemSelected = _itemHovered;
                float moveModifier = _itemSelected switch { BaseItem => 0.6f, PropItem => 0.8f, _ => 1 };
                _moveSpeed = moveModifier * MoveSpeed;
                _turnSpeed = moveModifier * TurnSpeed;
            }
        }

        private void OnPauseGamePerformed(InputAction.CallbackContext context)
        {
            Debug.Log("pause game");
            Time.timeScale = 0;
            SwitchActionMap_PlayerToUI();
        }

        #endregion
    
        #region UI Actions
    
        private void OnConfirmPerformed(InputAction.CallbackContext obj)
        {
            Debug.Log("confirm");
        }

        private void OnResumeGamePerformed(InputAction.CallbackContext context)
        {
            Debug.Log("resume game");
            Time.timeScale = 1;
            SwitchActionMap_UIToPlayer();
        }
    
        #endregion
    }
}