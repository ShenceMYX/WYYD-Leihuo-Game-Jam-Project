using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class GroundChecker : MonoBehaviour
	{
        public bool OnGround { get; private set; }

        private Vector2 _normal;

        private void OnCollisionExit(Collision collision)
        {
            OnGround = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        private void EvaluateCollision(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                _normal = collision.GetContact(i).normal;
                OnGround |= _normal.y >= 0.9f;
            }
        }
        
    }
}
