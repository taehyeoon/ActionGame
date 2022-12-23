 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float Gravity = -9.8f;
    public float _verticalVelocity;

    private CharacterController _cc;
    private Vector3 _movementVelocity;
    private PlayerInput _playerInput;
    private Animator _animator;

    private void Awake() {
        _cc = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
    }

    private void CalculatePlayerMovement(){
        _movementVelocity.Set(_playerInput.HorizontalInput, 0f,_playerInput.VerticalInput);
        _movementVelocity.Normalize();

        _animator.SetFloat("Speed", _movementVelocity.magnitude);
 
        // 카메라가 캐릭터를 -45도 방향에서 보고 있음
        // 따라서 캐릭터도 -45도 회전하고 나서 움직여야 한다
        _movementVelocity = Quaternion.Euler(0,-45f,0) * _movementVelocity;
        _movementVelocity *= MoveSpeed * Time.deltaTime;

        if(_movementVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_movementVelocity);

        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }

    private void FixedUpdate() {
        CalculatePlayerMovement();

        if(_cc.isGrounded == false){
            _verticalVelocity = Gravity;
        }else{
            _verticalVelocity = Gravity * 0.3f;
        }
        _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;


        _cc.Move(_movementVelocity);
         
    }
}
