using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class ExplosionController : MonoBehaviour
	{
		public float randomRadius = 2;
		public float minExplodeRange = 10;
		public float maxExplodeRange = 15;

		public float minExplosionForce = 800;
		public float maxExplosionForce = 1500;
		public float minExplosionY = 1;
		public float maxExplosionY = 2;

		public GameObject Explode(GameObject prefab)
        {
			Vector2 randomCircle = Random.insideUnitCircle * randomRadius;
			GameObject go = Instantiate(prefab, transform.position + new Vector3(randomCircle.x, Random.Range(minExplosionY, maxExplosionY), randomCircle.y), Quaternion.identity);
			go.AddComponent<Exploser>();
			Rigidbody rb = go.AddComponent<Rigidbody>();
			rb.AddExplosionForce(Random.Range(minExplosionForce, maxExplosionForce), transform.position, Random.Range(minExplodeRange, maxExplodeRange));

			return go;
		}

		
    }
}
