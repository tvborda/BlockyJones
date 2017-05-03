using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelStudy
{
    public class Spikes : Tile
    {
        public float delay = 0.0f;
        public float spikeUpTime = 2.0f;
        public float spikeDownTime = 2.0f;
        public float spikeUpY = 0.75f;
        public float spikeDownY = 0.0f;
        public float spikeMoveUpSpeed = 15.0f;
        public float spikeMoveDownSpeed = 10.0f;

        private Transform spikes;
        
        protected override void Start()
        {
            base.Start();
            AssignParts();
            if (spikes != null)
            {
                InvokeRepeating("SetSpikeUp", delay, spikeUpTime + spikeDownTime);
                InvokeRepeating("SetSpikeDown", delay + spikeUpTime, spikeUpTime + spikeDownTime);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            AssignParts();
        }

        private void AssignParts()
        {
            Transform prefab = transform.GetChild(0);
            for (int i = 0; i < prefab.childCount; i++)
            {
                if (prefab.GetChild(i).name == "Spikes")
                {
                    spikes = prefab.GetChild(i).transform;
                }
            }
            InspectorUpdate();
        }

        public override void Reset()
        {
            if (spikes)
                spikes.localPosition = Vector3.zero;
            base.Reset();
        }

        public override void InspectorUpdate()
        {
            if (spikes)
            {
                if (delay == 0.0f)
                {
                    spikes.localPosition = new Vector3(0, spikeUpY, 0);
                }
                else
                {
                    spikes.localPosition = new Vector3(0, spikeDownY, 0);
                }
            }
            base.InspectorUpdate();
        }

        private void SetSpikeUp()
        {
            Vector3 target = new Vector3(0, spikeUpY, 0);
            StartCoroutine(ChangeSpikeState(target, spikeMoveDownSpeed));
        }

        private void SetSpikeDown()
        {
            Vector3 target = new Vector3(0, spikeDownY, 0);
            StartCoroutine(ChangeSpikeState(target, spikeMoveUpSpeed));
        }

        private IEnumerator ChangeSpikeState(Vector3 target, float spikeMoveSpeed)
        {
            float sqrRemainingDistance = (spikes.localPosition - target).sqrMagnitude;
            while (sqrRemainingDistance > float.Epsilon)
            {
                spikes.localPosition = Vector3.MoveTowards(spikes.localPosition, target, spikeMoveSpeed * Time.deltaTime);
                sqrRemainingDistance = (spikes.localPosition - target).sqrMagnitude;
                yield return null;
            }
        }
    }
}
