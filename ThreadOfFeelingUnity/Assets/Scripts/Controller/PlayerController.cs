using Managers;
using UnityEngine;
using Components;

namespace Controller
{
    public class PlayerController : MonoBehaviour {

        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Rigidbody2D _rigidbody;
        private float speed = 3;
        private Vector3 move;
        private bool isStopped = false;

        GameObject scanObject;
        public float raycastDistance = 0.7f;
        private Vector2 lastMoveDir = Vector2.down;

        public LayerMask objectLayerMask; 


        void Start() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
            if (objectLayerMask == 0)
                objectLayerMask = LayerMask.GetMask("Object");
        }

        void Update() {
            if (isStopped) move = Vector3.zero;
            // 이동 입력 가져오기 8방향 정규화 벡터
            else move = InputManager.Instance.GetMoveInput(); 

            // 바라본 마지막 방향 저장
            if (move.magnitude > 0) lastMoveDir = move.normalized;

            // 마지막 바라보는 방향 기준으로 좌우 반전
            if (move.x < 0) _spriteRenderer.flipX = true;
            else if (move.x > 0) _spriteRenderer.flipX = false;

            // 플레이어 애니메이션
            if (move.magnitude > 0) _animator.SetTrigger("Move");
            else _animator.SetTrigger("Stop");

            // NPC 상호작용 Scan Object
            if (InputManager.Instance.GetSpaceKeyDown() && scanObject != null)
            {
                InteractableObject targetObject = scanObject.GetComponent<InteractableObject>(); 
                if (targetObject != null) {
                    targetObject.HandleInteraction();
                }
            }
        }

        private void FixedUpdate() {
            // 이동
            if (!isStopped) {
                Vector3 nextPosition = _rigidbody.position + (Vector2)(move * speed * Time.deltaTime);
                _rigidbody.MovePosition(nextPosition);
            }

            // Ray 시각화 - scanObject 용도
            Vector3 facingDir = lastMoveDir;
            Debug.DrawRay(transform.position, facingDir * raycastDistance, Color.green);
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, facingDir, raycastDistance, objectLayerMask);

            if (rayHit.collider != null) {
                scanObject = rayHit.collider.gameObject;
            }
            else scanObject = null;
        }

        public void StopMovement() {
            isStopped = true;
            _animator.SetTrigger("Stop"); // 멈출 때 애니메이션도 정지
        }

        public void ResumeMovement() { // 이동 재개 함수 추가
            isStopped = false;
        }
    }
}