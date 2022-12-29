 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float Gravity = -9.8f;

    private float _verticalVelocity;
    private CharacterController _cc;
    private Vector3 _movementVelocity;
    private PlayerInput _playerInput;
    private Animator _animator;

    // if Enemey, IsPlayer = false
    public bool IsPlayer = true;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;


    private void Awake() {
        _cc = GetComponent<CharacterController>(); 
        _animator = GetComponent<Animator>();

        // Enemy  
        if(!IsPlayer){
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;
        }else{
            _playerInput = GetComponent<PlayerInput>();
        }
    }   

    private void CalculateEnemyMovement(){
        // enemy가 mainchar항해서 쫓아오는 경우
        if (Vector3.Distance(TargetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance) {
            _navMeshAgent.SetDestination(TargetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        // enemy가 mainchar와 stoppingDistance보다 가까워져서 멈추는 경우
        }else{
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);
        }
        
        #pragma warning restore format
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
        // 이 코드를 mainchar와 enemy가 같이 사용함
        // 따라서 이 코드를 mainchar가 읽을 떄와 enemy가 읽을 때를 구분하여 작성

        if(IsPlayer)
            CalculatePlayerMovement();
        else
            CalculateEnemyMovement();
        
        if(IsPlayer){
            if(_cc.isGrounded == false){
                _verticalVelocity = Gravity;
            }else{
                _verticalVelocity = Gravity * 0.3f;
            }
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;


            _cc.Move(_movementVelocity);
        }
    }
}
