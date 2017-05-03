using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelStudy
{
    public class Door : Tile
    {
        public Transform doorLeft = null;
        public Transform doorRight = null;

        public float delay = 0.0f;
        public float doorOpenedTime = 2.0f;
        public float doorClosedTime = 2.0f;
        public float doorOpenSpeed = 2.0f;
        public float doorCloseSpeed = 2.0f;

        private Vector3 closedScale = new Vector3(1.5f, 1.0f, 1.0f);
        private Vector3 openedScale = Vector3.one;

        protected override void Start()
        {
            base.Start();
            AssignParts();
            if ((doorLeft != null) && (doorRight != null))
            {
                InvokeRepeating("SetDoorOpen", delay, doorOpenedTime + doorClosedTime);
                InvokeRepeating("SetDoorClose", delay + doorOpenedTime, doorOpenedTime + doorClosedTime);
            }
        }

        private void AssignParts()
        {
            Transform[] prefabs = transform.GetComponentsInChildren<Transform>();
            foreach (Transform t in prefabs)
            {
                if (t.gameObject.name == "DoorLeft")
                {
                    doorLeft = t;
                }
                else if (t.gameObject.name == "DoorRight")
                {
                    doorRight = t;
                }
            }
            InspectorUpdate();
        }

        private void SetDoorOpen()
        {
            StartCoroutine(ChangeDoorState(closedScale, openedScale, doorOpenSpeed));
        }

        private void SetDoorClose()
        {
            StartCoroutine(ChangeDoorState(openedScale, closedScale, doorCloseSpeed));
        }

        private IEnumerator ChangeDoorState(Vector3 from, Vector3 to, float doorMoveSpeed)
        {
            float progress = 0;
            while (progress <= 1)
            {
                doorLeft.localScale = Vector3.Lerp(from, to, progress);
                doorRight.localScale = Vector3.Lerp(from, to, progress);
                progress += doorMoveSpeed * Time.deltaTime;
                yield return null;
            }
            doorLeft.localScale = to;
            doorRight.localScale = to;
        }
    }
}