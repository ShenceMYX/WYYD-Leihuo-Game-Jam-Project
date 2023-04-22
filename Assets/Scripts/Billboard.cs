using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class Billboard : MonoBehaviour
	{
        private Transform mainCamTrans;

        private void Start()
        {
            mainCamTrans = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.LookRotation(mainCamTrans.forward);
        }
    }
}
