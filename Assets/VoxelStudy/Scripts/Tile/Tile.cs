using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VoxelStudy
{
    public class Tile : MonoBehaviour
    {
        public Transform tilePrefab;
        [HideInInspector]
        public Transform instancePrefab = null;

        protected virtual void Start()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            if (instancePrefab == null)
            {
                if (transform.childCount == 0)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                    {
                        instancePrefab = PrefabUtility.InstantiatePrefab(tilePrefab) as Transform;
                    }
                    else
                    {
                        instancePrefab = TileObjectPool.instance.SpawnObject(tilePrefab);
                    }
#else
					instancePrefab = TileObjectPool.instance.SpawnObject(tilePrefab);
#endif
                    instancePrefab.parent = transform;
                }
                else
                {
                    instancePrefab = transform.GetChild(0);
                }

                instancePrefab.localPosition = Vector3.zero;
                instancePrefab.localRotation = Quaternion.identity;
                instancePrefab.localScale = new Vector3(Mathf.Abs(instancePrefab.localScale.x), Mathf.Abs(instancePrefab.localScale.y), Mathf.Abs(instancePrefab.localScale.z));
                instancePrefab.gameObject.SetActive(true);
            }
        }

        public virtual void Reset()
        {

        }

        public virtual void InspectorUpdate()
        {
            
        }

    }
}
