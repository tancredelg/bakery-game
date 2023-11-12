using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    // Parameters
    [SerializeField][Range(0, 10)] private int MoveSpeed;
    [SerializeField][Range(0, 10)] private int TurnSpeed;
    
    
    private PlayerInputActions _playerInputActions;
    private IMovable _movableHovered, _movableSelected;
    private float _moveSpeed, _turnSpeed;
    
    // Component references
    private Rigidbody2D _rb;
    private Transform _grabTransform;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _grabTransform = transform.GetChild(0).GetChild(0);
        
        _moveSpeed = MoveSpeed;
        _turnSpeed = TurnSpeed;
        
        // Initialize input system
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
        if (_movableSelected == null && other.TryGetComponent(out IMovable movable))
        {
            _movableHovered?.UnHover();
            _movableHovered = movable;
            _movableHovered.Hover();
        }
        else if (_movableSelected is Prop && other.TryGetComponent(out Block baseItem))
        {
            baseItem.Hover();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_movableSelected == null && other.TryGetComponent(out IMovable movable))
        {
            if (_movableHovered != movable || !other.enabled) return;
            _movableHovered?.UnHover();
            _movableHovered = null;
        }
        else if (_movableSelected is Prop && other.TryGetComponent(out IMovable block))
        {
            block.UnHover();
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
        if (_movableHovered == null) return;
        
        if (_movableSelected != null)
        {
            if (_movableSelected.TryPlace())
            {
                _movableSelected = null;
                _moveSpeed = MoveSpeed;
                _turnSpeed = TurnSpeed;
            }
            return;
        }

        if (_movableHovered.TryGrab(_grabTransform))
        {
            _movableSelected = _movableHovered;
            float moveModifier = _movableSelected switch { Items_Inheritance_Approach.BaseItem => 0.6f, Prop => 0.8f, _ => 1 };
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