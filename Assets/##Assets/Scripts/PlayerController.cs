using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.0f;
    public float sprintSpeed = 5.335f;
    public float rotationSmoothTime = 0.12f;
    public float speedChangeRate = 10.0f;

    [Header("Jump")]
    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;
    public float jumpTimeout = 0.50f;
    public float fallTimeout = 0.15f;

    [Header("Ground Check")]
    public bool grounded = true;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;

    [Header("Combat")]
    public float attackComboResetTime = 2.0f;

    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool forceDebugToScreen = false; // Inspector'da g�stermek i�in

    // Private variables
    private CharacterController _controller;
    private Animator _animator;
    private PlayerInput _playerInput;
    private Camera _mainCamera;

    // Movement
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _sprintInput;
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // Jump
    private bool _jumpInput;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // Combat
    private bool _attackInput;
    private bool _spinAttackInput;
    private bool _blockInput;
    private bool _deathInput;
    private int _attackComboCounter = 0;
    private float _lastAttackTime;
    private bool _isAttacking;
    private bool _isBlocking;
    private bool _isDead;

    // Debug status
    private string _debugStatus = "";
    private bool _inputSystemWorking = false;

    // Animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDAttack1;
    private int _animIDAttack2;
    private int _animIDSpinAttack;
    private int _animIDBlock;
    private int _animIDDeath;

    private const float _threshold = 0.01f;
    private bool _hasAnimator;


    PlayerAttack _playerAttack;


    void Awake()
    {
        // Get components
        _controller = GetComponent<CharacterController>();
        _hasAnimator = TryGetComponent(out _animator);
        _playerInput = GetComponent<PlayerInput>();
        _playerAttack = GetComponentInChildren<PlayerAttack>();

        // Find main camera
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        // Debug component check
        // LogDebug("=== COMPONENT CHECK ===");
        // LogDebug($"CharacterController: {_controller != null}");
        // LogDebug($"Animator: {_hasAnimator}");
        // LogDebug($"PlayerInput: {_playerInput != null}");
        // LogDebug($"Main Camera: {_mainCamera != null}");

        // PlayerInput detailed check
        if (_playerInput != null)
        {
            // LogDebug($"PlayerInput Behavior: {_playerInput.notificationBehavior}");
            // LogDebug($"PlayerInput Actions: {_playerInput.actions != null}");
            // LogDebug($"PlayerInput Current Action Map: {_playerInput.currentActionMap?.name}");
        }

        _debugStatus = "Components loaded";
    }

    void Start()
    {
        // LogDebug("=== START METHOD ===");

        AssignAnimationIDs();

        // Reset timeouts
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;

        // Cursor lock
        Cursor.lockState = CursorLockMode.Locked;

        // Test input system
        TestInputSystem();

        _debugStatus = "Initialization complete";
        // LogDebug("=== INITIALIZATION COMPLETE ===");
    }

    void Update()
    {
        if (_isDead) return;

        // Debug input every 60 frames to avoid spam
        if (Time.frameCount % 60 == 0 && showDebugInfo)
        {
            // LogDebug($"Frame {Time.frameCount}: Move={_moveInput}, Sprint={_sprintInput}, Jump={_jumpInput}, InputWorking={_inputSystemWorking}");
        }

        JumpAndGravity();
        GroundedCheck();
        Move();
        HandleCombat();
        HandleBlock();
        UpdateAnimations();
    }

    private void TestInputSystem()
    {
        // LogDebug("=== INPUT SYSTEM TEST ===");

        if (_playerInput == null)
        {
            // LogDebug("ERROR: PlayerInput is null!");
            return;
        }

        if (_playerInput.actions == null)
        {
            // LogDebug("ERROR: PlayerInput actions is null!");
            return;
        }

        // LogDebug($"Input Actions Asset: {_playerInput.actions.name}");
        // LogDebug($"Current Action Map: {_playerInput.currentActionMap?.name}");

        // List all actions
        foreach (var action in _playerInput.actions)
        {
            // LogDebug($"Action: {action.name}, Enabled: {action.enabled}, Bindings: {action.bindings.Count}");
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDAttack1 = Animator.StringToHash("Attack1");
        _animIDAttack2 = Animator.StringToHash("Attack2");
        _animIDSpinAttack = Animator.StringToHash("SpinAttack");
        _animIDBlock = Animator.StringToHash("Block");
        _animIDDeath = Animator.StringToHash("Death");

        // LogDebug($"Animation IDs assigned. Speed hash: {_animIDSpeed}");
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, grounded);
        }
    }

    private void Move()
    {
        //Move s�ras�nda sald�ramaz
        if (_isAttacking || _isBlocking) return;

        float targetSpeed = _moveInput.magnitude > 0.02f ?
                           (_sprintInput ? sprintSpeed : moveSpeed) : 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _moveInput.magnitude;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        float normalizedBlend = _animationBlend / sprintSpeed;

        Vector3 inputDirection = new Vector3(_moveInput.x, 0.0f, _moveInput.y).normalized;

        if (_moveInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, normalizedBlend);
        }
    }

    private void JumpAndGravity()
    {
        if (grounded)
        {
            _fallTimeoutDelta = fallTimeout;

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
            }

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_jumpInput && _jumpTimeoutDelta <= 0.0f && !_isAttacking && !_isBlocking)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (_hasAnimator)
                {
                    _animator.SetTrigger(_animIDJump);
                }
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = jumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            _jumpInput = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void HandleCombat()
    {
        // Sadece sald�r� ba�latma (ilk t�klama veya kombo s�ras�)
        if (_attackInput && grounded && !_isAttacking && !_isBlocking)
        {
            StartAttack();
        }

        if (_spinAttackInput && grounded && !_isAttacking && !_isBlocking)
        {
            PerformSpinAttack();
        }

        // _isAttacking sadece trigger verildikten sonra true olmal�
        // Animasyon event ile _isAttacking = false yap�lmal�
    }

    private void StartAttack()
    {
        if (_attackComboCounter == 0)
        {
            _attackComboCounter = 1;
            _isAttacking = true; // Sald�r� ba�lat�ld�
            if (_hasAnimator)
                _animator.SetTrigger(_animIDAttack1);
        }
        else if (_attackComboCounter == 1)
        {
            _attackComboCounter = 2;
            _isAttacking = true;
            if (_hasAnimator)
                _animator.SetTrigger(_animIDAttack2);
        }
        _attackInput = false;
    }

    //Attack1 ve Attack2 animasyonlar�n�n sonuna Animation Event
    public void OnAttackAnimationEnd()
    {
        if (_attackComboCounter == 1)
        {
            // Sadece kullan�c� ikinci kez t�klarsa Attack2 tetiklenmeli
            if (_attackInput)
            {
                _attackComboCounter = 2;
                _isAttacking = false;
                if (_hasAnimator)
                    _animator.SetTrigger(_animIDAttack2);
            }
            else
            {
                // Kombo bitmez, bekler
                _attackComboCounter = 0;
                _isAttacking = false;
            }
        }
        else if (_attackComboCounter == 2)
        {
            _attackComboCounter = 0;
            _isAttacking = false;
        }
        _attackInput = false; // inputu s�f�rla
    }

    private void PerformSpinAttack()
    {
        if (_hasAnimator)
        {
            _animator.SetTrigger(_animIDSpinAttack);
            _isAttacking = true; // Spin attack ba�lat�ld�
        }
        _spinAttackInput = false;
    }
    public void OnSpinAttackEnd()
    {
        _isAttacking = false;
    }

    private void HandleBlock()
    {
        if (_hasAnimator)
        {
            if (_blockInput && grounded && !_isAttacking)
            {
                _animator.SetTrigger(_animIDBlock);
                _isBlocking = true;
            }
            else if (!_blockInput)
            {
                _isBlocking = false;
            }
        }
    }

    private void UpdateAnimations()
    {
        if (!_hasAnimator) return;

        if (_deathInput && !_isDead)
        {
            _animator.SetTrigger(_animIDDeath);
            _isDead = true;
            _deathInput = false;
        }
    }

    // ... (di�er kodlar ayn�)

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        _inputSystemWorking = true;
        // LogDebug($"OnMove called: {_moveInput}, Phase: {context.phase}");
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
        // LogDebug($"OnLook called: {_lookInput}, Phase: {context.phase}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _jumpInput = context.ReadValueAsButton();
        // LogDebug($"OnJump called: {_jumpInput}, Phase: {context.phase}");
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        _sprintInput = context.ReadValueAsButton();
        // LogDebug($"OnSprint called: {_sprintInput}, Phase: {context.phase}");
    }

    // Unity Events Input Methods
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _attackInput = true;
            // LogDebug($"OnAttack called, Phase: {context.phase}");
        }
    }

    public void OnSpinAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _spinAttackInput = true;
            // LogDebug($"OnSpinAttack called, Phase: {context.phase}");
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        _blockInput = context.ReadValueAsButton();
        // LogDebug($"OnBlock called: {_blockInput}, Phase: {context.phase}");
    }

    public void OnDeath(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _deathInput = true;
            // LogDebug($"OnDeath called, Phase: {context.phase}");
        }
    }

    // Debug helper method
    private void LogDebug(string message)
    {
        if (showDebugInfo)
        {
            // Debug.Log($"[PlayerController] {message}");

            if (forceDebugToScreen)
            {
                _debugStatus = message;
            }
        }
    }

    // Animation Event fonksiyonlar� - PUBLIC olmal�
    public void EnableHitbox()
    {
        if (_playerAttack != null)
        {
            _playerAttack.EnableAttackCollider();
            Debug.Log("[PlayerController] EnableHitbox �a�r�ld� - Attack collider aktif edildi");
        }
        else
        {
            Debug.LogWarning("[PlayerController] EnableHitbox �a�r�ld� ama _playerAttack null!");
        }
    }
    public void DisableHitbox()
    {
        if (_playerAttack != null)
        {
            _playerAttack.DisableAttackCollider();
            Debug.Log("[PlayerController] DisableHitbox �a�r�ld� - Attack collider deaktif edildi");
        }
        else
        {
            Debug.LogWarning("[PlayerController] DisableHitbox �a�r�ld� ama _playerAttack null!");
        }
    }

    // Inspector'da debug bilgilerini g�stermek i�in
    void OnGUI()
    {
        if (!showDebugInfo || !forceDebugToScreen) return;

        GUILayout.BeginArea(new Rect(10, 10, 400, 300));
        GUILayout.Label("=== DEBUG INFO ===");
        GUILayout.Label($"Status: {_debugStatus}");
        GUILayout.Label($"Input System Working: {_inputSystemWorking}");
        GUILayout.Label($"Move Input: {_moveInput}");
        GUILayout.Label($"Sprint Input: {_sprintInput}");
        GUILayout.Label($"Jump Input: {_jumpInput}");
        GUILayout.Label($"Grounded: {grounded}");
        GUILayout.Label($"Speed: {_speed:F2}");
        GUILayout.Label($"Animation Blend: {_animationBlend:F2}");
        GUILayout.EndArea();
    }

    // Public Methods
    public void Die()
    {
        if (!_isDead)
        {
            _deathInput = true;
        }
    }

    public void Respawn()
    {
        _isDead = false;
        _attackComboCounter = 0;
        _isAttacking = false;
        _isBlocking = false;

        if (_hasAnimator)
        {
            _animator.SetTrigger("Respawn");
        }
    }

    // Gizmos
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded)
            Gizmos.color = transparentGreen;
        else
            Gizmos.color = transparentRed;

        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }
}
