using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RB_PlayerMovement : MonoBehaviour
{
    //Player movement properties
    [Header("Player movement properties")]
    [HideInInspector] public Vector3 LastDirection;
    [SerializeField] private float _movementAcceleration;
    [SerializeField] private float _movementMaxSpeed;
    [SerializeField] private float _movementFrictionForce;
    [SerializeField] private Vector3 _directionToMove;
    private Vector3 _currentVelocity;
    private bool _isMoving = false;
    private bool _canMove = true;

    //Dash properties
    [Header("Dash properties")]
    [SerializeField] private float _dashCooldown; public float DashCooldown { get { return _dashCooldown; } }
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _fadeOutInterval;
    [SerializeField] private float _fadeForce;
    [SerializeField] private float _zFadeOffset;
    [SerializeField] private GameObject _spritePrefab;
    private Vector3 _dashEndPos;
    private Vector3 _dashDirection;
    private bool _canDash = true;
    private bool _isDashing = false;
    private float _lastUsedDashTime = 0;
    public UnityEvent EventDash;

    //Components
    [Header("Components")]
    private Rigidbody _rb;
    private Transform _transform;

    //Debug components
    [Header("Debug Components")]
    [SerializeField] private TextMeshProUGUI _debugSpeedText;

    //Debug properties
    private Vector3 _previousPosition;
    private Vector3 _currentPosition;

    public static RB_PlayerMovement Instance;
    private void Awake()
    {   
        Instance = this;
        _rb = GetComponentInChildren<Rigidbody>();
        _transform = transform;
    }
    private void Start()
    {
        Invoke("DebugSpeed", 0);
    }

    //Starting movement
    public void StartMove()
    {
        _isMoving = true;
    }

    //Stopping movement
    public void StopMove()
    {
        _isMoving = false;
    }

    private void GetDirection(Vector3 direction)
    {
        _directionToMove = new Vector3(direction.x, 0, direction.y);
        //Setting the direction to the player
        _transform.forward = _directionToMove;
    }

    private void ClampingSpeed()
    {
        //Clamping to max speed in the x and z axes
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _movementMaxSpeed);
        _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
    }

    private void FrictionForce()
    {
        Vector3 frictionForce = (-_rb.velocity) * Time.fixedDeltaTime * _movementFrictionForce;
        _rb.AddForce(new Vector3(frictionForce.x, 0, frictionForce.z)); //Friction force (stop the movement)
    }

    //Moving the player
    public void Move()
    {
        if(_currentVelocity.magnitude < _movementMaxSpeed)
        {
            //Adding velocity to player
            _rb.AddForce(_directionToMove * _movementMaxSpeed * Time.fixedDeltaTime * _movementAcceleration);
        }
    }

    public bool CanMove()
    {
        //if is moving and not dashing
        return _isMoving && !_isDashing;
    }

    private void SetSpeed()
    {
        //Calculating the real time speed
        _currentPosition = transform.position;
        _currentVelocity = (_currentPosition - _previousPosition) / Time.deltaTime;
        _previousPosition = _currentPosition;
    }

    public Vector3 GetVelocity()
    {
        return _currentVelocity;
    }

    public void DashAnim()
    {
        //Create animation for dashing
        if (_isDashing)
        {
            //Spawn a "white shadow" behind the player
            GameObject spawnedSprite =  Instantiate(_spritePrefab, new Vector3(_transform.position.x, 1, _previousPosition.z + _zFadeOffset), Quaternion.identity);
            spawnedSprite.GetComponent<RB_SpriteFadeOut>().FadeForce = _fadeForce;
            Invoke("DashAnim", _fadeOutInterval);
        }
    }
    
    public void StartDash()
    {
        //Starting dash
        _dashEndPos = _transform.position + _transform.forward * _dashDistance;
        _dashDirection = _transform.forward;
        _lastUsedDashTime = Time.time;
        _isDashing = true;
        //Starting dash animation
        DashAnim();
    }

    
    public void StopDash()
    {
        //Stopping dash
        _isDashing = false;
    }

    public void Dash()
    {
        if(_isDashing)
        {
            //Dashing
            _rb.velocity = _dashSpeed * _dashDirection;
            if (Vector3.Distance(_rb.position, _dashEndPos) < 0.5f)
            {
                StopDash();
                EventDash.Invoke();
            }
        }
    }

    public bool CanDash()
    {
        //Cooldown dash
        return _canDash && Time.time > (_lastUsedDashTime + _dashCooldown);
    }

    private void DebugSpeed()
    {
        //Printing debug speed to screen
        if(_debugSpeedText != null)
        {
            _debugSpeedText.text = ((int)_currentVelocity.magnitude).ToString();
            Invoke("DebugSpeed", 0.1f);
        }
    }

    private void FixedUpdate()
    {
        //Clamping the speed to the max speed
        ClampingSpeed();

        //Adding friction force
        FrictionForce();

        //Calling the speed calcul in real time
        SetSpeed();

        //If the player can move
        if(CanMove())
        {
            //Get the direction to move
            GetDirection(RB_InputManager.Instance.MoveValue);

            //Call the movement function
            Move();
        }

        //Call the dash function
        Dash();

        
    }


}
