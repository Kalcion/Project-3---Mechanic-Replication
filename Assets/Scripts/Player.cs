using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 6f;
    [SerializeField]
    private float _gravity = 9.81f;
    [SerializeField]
    private float _jumpForce = 3.5f;
    [SerializeField]
    private float _doubleJumpMultiplier = 0.75f;
    [SerializeField]
    public float rotationSpeed = 240.0f;
    private Vector3 _moveDir = Vector3.zero;

    private CharacterController _controller;
    public AudioSource _sound;

    private float _directionY;
    private bool _canDoubleJump = false;
    private bool _canBulletJump = false;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        Sprint();

        //Get Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //test//
        transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);

        //calculate movement direction
        Vector3 direction = new Vector3(horizontal, 0, vertical);

        //If player on ground, then jump is allowed
        if (_controller.isGrounded)
        {
            //possibility to double jump is allowed 
            _canDoubleJump = true;
            _canBulletJump = true;

            //Bullet Jump
            if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
            {
                _directionY = _jumpForce * 1.5f;
                direction.z = _moveSpeed * 10;
                _canBulletJump = false;
                _sound.Play();
                Debug.Log("Bullet Jump");
            }
            //Jump
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                _directionY = _jumpForce;
                Debug.Log("Jump");
            }
        }
        else
        {
            //mid-air bullet jump
            if ((Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl)) && _canBulletJump)
            {
                _directionY = _jumpForce * 1.5f;
                direction.z = _moveSpeed * 10;
                _canDoubleJump = false;
                _canBulletJump = false;
                _sound.Play();
                Debug.Log("Bullet Jump");
            }
            //mid air regular jump
            else if(Input.GetKeyDown(KeyCode.Space) && _canDoubleJump)
            {
                //second jump is weaker and deactivates allowing a 3rd jump
                _directionY = _jumpForce * _doubleJumpMultiplier;
                _canDoubleJump = false;
                _canBulletJump = false;
            }
        }

        //Enacting gravity
        _directionY -= _gravity * Time.deltaTime;

        //Storing height from jump in temp variable so its not reset every update
        direction.y = _directionY;

        //Moving player 
        _controller.Move(direction * _moveSpeed * Time.deltaTime);
    }

    void Sprint()
    {
        //check if running or not
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _moveSpeed = 10f;

        }
        else
        {
            _moveSpeed = 6f;
        }
    }
}
