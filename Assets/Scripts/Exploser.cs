using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class Exploser : MonoBehaviour
	{
        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Ground"))
            {
                Destroy(gameObject.GetComponent<Rigidbody>());
                GetComponent<BoxCollider>().isTrigger = true;
                Destroy(this);
            }
        }
    }
}
