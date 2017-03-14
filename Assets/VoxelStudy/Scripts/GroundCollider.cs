using UnityEngine;
using System.Collections;

namespace VoxelStudy
{
	[RequireComponent(typeof(BoxCollider))]
	public class GroundCollider: MonoBehaviour {
		public bool occupied = false;
	}
}
