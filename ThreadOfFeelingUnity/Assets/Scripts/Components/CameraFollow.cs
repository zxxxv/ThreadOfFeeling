using UnityEngine;

namespace Components
{
    public class CameraFollow : MonoBehaviour {

        public Transform target;
        public Vector3 offset;

        void LateUpdate() {
            if (target == null) {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) {
                    target = player.transform;
                }   
            }

            if (target != null) {
                transform.position = target.position + offset;
            }
        }
    }
}