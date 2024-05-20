using System.Collections.Generic;
using UnityEngine;

namespace TwoPersonProject
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class CharacterMovement : MonoBehaviour
    {
        public float acceleration = 0;
        public float airAcceleration = 0;
        public float deceleration = 0;
        public float maxSpeed = 0;
        public float jumpHeight = 0;
        public float jumpCooldown = 0;
        public float additionalJumpHeight = 0;
        public float additionalJumpHeightTime = 0;
        public bool canWallHang = false;
        public float wallHangTime = 2.5f;

        private static readonly ContactFilter2D groundFilter = new()
        {
            useNormalAngle = true,
            minNormalAngle = 89,
            maxNormalAngle = 91
        };

        private static readonly ContactFilter2D rightSideFilter = new()
        {
            useNormalAngle = true,
            minNormalAngle = 175,
            maxNormalAngle = 185
        };
        private static readonly ContactFilter2D leftSideFilter = new()
        {
            useNormalAngle = true,
            minNormalAngle = -5,
            maxNormalAngle = 5
        };

        private static readonly List<Collider2D> _tempList = new();

        private Rigidbody2D _rb;

        private bool _hasWallHangEnded = false;
        private bool _canStartWallHang = false;
        private float _lastWallHangTime = 0;
        private float _timeWallHanging = 0;
        private float _lastJumpTime = 0;
        private bool _spaceDown = false;
        private bool _spaceLetGo = false;

        public bool IsFacingLeft { get; private set; }
        public Vector2 Velocity => _rb.velocity;
        public bool IsGrounded => _rb.IsTouching(groundFilter);
        public List<Collider2D> LeftSideContacts
        {
            get
            {
                _rb.GetContacts(leftSideFilter, _tempList);
                return _tempList;
            }
        }

        public List<Collider2D> RightSideContacts
        {
            get
            {
                _rb.GetContacts(rightSideFilter, _tempList);
                return _tempList;
            }
        }

        private int WallDirection
        {
            get
            {
                if (_rb.IsTouching(leftSideFilter))
                {
                    return -1;
                }
                else if (_rb.IsTouching(rightSideFilter))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        private bool IsOnWall => WallDirection != 0;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (Controls.Instance.Movement.Jump.WasPressedThisFrame())
            {
                _spaceDown = true;
            }
            else if (Controls.Instance.Movement.Jump.WasReleasedThisFrame())
            {
                _spaceLetGo = true;
            }
        }

        // Fixed update should be used to handle physics things, but input keydown/up
        // won't function properly in it, which is why I also have update
        private void FixedUpdate()
        {
            // Horizontal input will be whatever is supplied by the Move method
            // if there is none, then use the axis value
            float horzInput = Controls.Instance.Movement.LeftRight.ReadValue<float>();
            bool isInputPressed = Mathf.Abs(horzInput) > 0.01;

            // Do accel
            float currentAcceleration = horzInput * (IsGrounded ? acceleration : airAcceleration);
            _rb.velocityX += currentAcceleration * Time.fixedDeltaTime;

            // If input pressed, change facing direction and limit velocity
            if (isInputPressed)
            {
                IsFacingLeft = horzInput < 0;
                _rb.velocityX = Mathf.Clamp(_rb.velocityX, -maxSpeed, maxSpeed);
            }
            else if (IsGrounded)
            {
                // If not pressing buttons and grounded, decelerate
                float adjSpeed = _rb.velocityX * deceleration;
                _rb.velocityX = Mathf.Abs(adjSpeed) < 0.01 ? 0 : adjSpeed;
            }

            // Check if horz input is in the same direction as the wall
            if (isInputPressed && !IsGrounded && IsOnWall && Mathf.Sign(horzInput) == WallDirection && _canStartWallHang)
            {
                // Start wall hang
                _lastWallHangTime = Time.realtimeSinceStartup;
                _hasWallHangEnded = false;
                _canStartWallHang = false;

                _rb.gravityScale = 0;
                _rb.velocityY = 0;
            }
            else
            {
                if (Time.realtimeSinceStartup - _lastWallHangTime + _timeWallHanging > wallHangTime)
                {
                    // If not on a wall, or we've been hanging for longer than
                    // wall hang time, then end wall hang
                    _hasWallHangEnded = true;
                    _rb.gravityScale = 1;
                }
                else if ((!IsOnWall || !isInputPressed) && !_canStartWallHang)
                {
                    // If player falls off the wall when previously wall
                    // hanging, let them hang again, while remembering how long
                    // they were on the wall
                    _timeWallHanging += Time.realtimeSinceStartup - _lastWallHangTime;
                    _canStartWallHang = true;
                    _rb.gravityScale = 1;
                }
            }

            // Allow wall hangs once grounded
            if (IsGrounded)
            {
                _lastWallHangTime = 0;
                _timeWallHanging = 0;
                _canStartWallHang = true;
                _hasWallHangEnded = true;
            }

            float timeSinceLastJump = Time.realtimeSinceStartup - _lastJumpTime;
            if (_spaceDown)
            {
                // If grounded or current wall hanging and jump cooldown has worn off
                if ((IsGrounded || (IsOnWall && !_hasWallHangEnded)) && timeSinceLastJump > jumpCooldown)
                {
                    // Jump. Add horz force if on a wall and not grounded
                    _rb.AddForce(new Vector2(IsGrounded ? 0 : -WallDirection * jumpHeight * 0.7f, jumpHeight) * _rb.mass);
                    _lastJumpTime = Time.realtimeSinceStartup;

                    // Stop wall hang
                    _rb.gravityScale = 1;
                    _hasWallHangEnded = true;
                }
                _spaceDown = false;
                _spaceLetGo = false;
            }
            else if (!_spaceLetGo && timeSinceLastJump <= additionalJumpHeightTime && !IsOnWall)
            {
                // Stop jump extension if let go of space
                _rb.AddForce(new Vector2(0, additionalJumpHeight * Time.fixedDeltaTime) * _rb.mass);
            }
        }
    }
}
