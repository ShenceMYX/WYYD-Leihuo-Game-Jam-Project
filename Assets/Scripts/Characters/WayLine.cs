using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class WayLine : MonoBehaviour
	{
        public Transform[] WayPoints;

        private void Awake()
        {
            WayPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                WayPoints[i] = transform.GetChild(i);
            }
        }
    }
}
