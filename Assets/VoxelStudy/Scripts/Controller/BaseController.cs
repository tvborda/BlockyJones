using System.Collections;
using UnityEngine;

namespace VoxelStudy
{
    public class BaseController : MonoBehaviour
    {
        // Collision Settings
        public LayerMask barrierLayerMask;
        public Vector3 barrierOffset = new Vector3(0.0f, 0.25f, 0.0f);
        public LayerMask groundLayerMask;
        public Vector3 groundOffset = new Vector3(0.0f, 0.125f, 0.0f);

        // Animation Settings
        public string idleAnimationName = "Idle";
        public string jumpAnimationName = "Jump";
        public Transform characterBody = null;

        // Character Settings
        public float moveTime = 0.4f;
        public float jumpHeight = 2.0f;
        public float fallSpeed = 40f;

        protected bool idle = true;
        protected int horizontal = 0;
        protected int vertical = 0;

        private Animator anim = null;
        //private Rigidbody rb = null;
        private float originalMoveTime = 0.0f;
        private GroundCollider sourceTile = null;
        private GroundCollider destinationTile = null;

        protected virtual void Start()
        {
            //rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            originalMoveTime = moveTime;
            CheckGround(true);
        }

        protected void AttemptRotate()
        {
            if (horizontal > 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
            }
            else if (horizontal < 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, -90f, 0));
            }

            if (vertical > 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0f, 0));
            }
            else if (vertical < 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, -180f, 0));
            }
        }

        protected void AttemptMove()
        {
            Vector3 start = transform.position;
            Vector3 end = start + new Vector3(horizontal * PatternSettings.tiledSize, 0f, vertical * PatternSettings.tiledSize);

            RaycastHit hitInfo;
            // Check for any barriers on destination tile
            if (Physics.Linecast(start + barrierOffset, end + barrierOffset, out hitInfo, barrierLayerMask.value) == false)
            {
                // Check for an available ground on destination tile, also if it is not occupied
                if (IsDestinationTileAvailable(end))
                {
                    idle = false;
                    anim.SetTrigger("Jump");
                    StartCoroutine(SmoothMovement(start, end));
                }
                //else
                //{
                //    anim.SetTrigger("Idle");
                //}
            }
        }

        private bool IsDestinationTileAvailable(Vector3 end)
        {
            RaycastHit hitInfo;
            // Check for an available ground on destination tile, also if it is not occupied
            if (Physics.Linecast(end + groundOffset, end + groundOffset + Vector3.down, out hitInfo, groundLayerMask.value))
            {
                destinationTile = hitInfo.transform.GetComponent<GroundCollider>();
                if (destinationTile.occupied == false)
                {
                    destinationTile.occupied = true;
                    return true;
                }
                else
                {
                    destinationTile = null;
                    return false;
                }
            }
            destinationTile = null;
            return true;
        }

        private IEnumerator SmoothMovement(Vector3 start, Vector3 end)
        {
            Vector3 moveDirection = (end - start);
            float t = 0;
            for (t = Time.deltaTime / moveTime; t < 1; t += Time.deltaTime / moveTime)
            {
                characterBody.localPosition = new Vector3(0, jumpHeight * Mathf.Sin(Mathf.PI * t), 0);
                transform.position = start + (moveDirection * t);
                if (t > 0.5f)
                {
                    if (sourceTile)
                    {
                        sourceTile.occupied = false;
                        sourceTile = null;
                    }
                }
                yield return null;
            }
            characterBody.localPosition = Vector3.zero;
            moveTime = originalMoveTime;
            CheckGround();
        }

        private void CheckGround(bool firstCheck = false)
        {
            Vector3 start = transform.position;
            Vector3 end = start + Vector3.down;

            RaycastHit hitInfo;
            if (Physics.Linecast(start + groundOffset, end + groundOffset, out hitInfo, groundLayerMask.value))
            {
                if (firstCheck)
                {
                    sourceTile = hitInfo.transform.GetComponent<GroundCollider>();
                    sourceTile.occupied = true;
                }
                else
                {
                    if (destinationTile)
                    {
                        sourceTile = destinationTile;
                    }
                }
                idle = true;
            }
            else
            {
                Falling();
            }
        }

        private void Falling()
        {
            Vector3 end = transform.position + new Vector3(0, -50f, 0);
            anim.SetTrigger("Idle");
            StartCoroutine(Fall(end));
        }

        private IEnumerator Fall(Vector3 end)
        {
            float sqrRemainingDistance = (transform.localPosition - end).sqrMagnitude;
            while (sqrRemainingDistance > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.localPosition, end, fallSpeed * Time.deltaTime);
                sqrRemainingDistance = (transform.localPosition - end).sqrMagnitude;
                yield return null;
            }
            CharacterDead();
        }

        protected virtual void CharacterDead()
        {
            if (sourceTile)
            {
                sourceTile.occupied = false;
                sourceTile = null;
            }
            if (destinationTile)
            {
                destinationTile.occupied = false;
                destinationTile = null;
            }
            Destroy(gameObject);
        }
    }
}
