using System.Collections.Generic;
using UnityEngine;

namespace VoxelStudy
{
    public class PlayerController : BaseController
    {
        public float dragThreshold = 15.0f;
        private float dragDistance;
        private Vector2 touchOrigin = -Vector2.one;

        protected override void Start()
        {
            dragDistance = Screen.height * (dragThreshold / 100f);
            base.Start();
        }

        void Update()
        {
            horizontal = 0;
            vertical = 0;

            if (Time.timeScale > 0 && activate)
            {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.touches[0];
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchOrigin = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
                    {
                        Vector2 touchEnd = touch.position;
                        float x = touchEnd.x - touchOrigin.x;
                        float y = touchEnd.y - touchOrigin.y;

                        if (Mathf.Abs(x) > dragDistance || Mathf.Abs(y) > dragDistance)
                        {
                            if (Mathf.Abs(x) > Mathf.Abs(y))
                            {
                                horizontal = x > 0 ? 1 : -1;
                            }
                            else
                            {
                                vertical = y > 0 ? 1 : -1;
                            }
                        }
                        else
                        {
                            vertical = 1;
                        }
                    }
                }
#else
                horizontal = (int)Input.GetAxisRaw("Horizontal");
                vertical = (horizontal == 0) ? (int)Input.GetAxisRaw("Vertical") : 0;
#endif
                if (idle)
                {
                    if (horizontal != 0 || vertical != 0)
                    {
                        AttemptRotate();
                        AttemptMove();
                    }
                }
            }
        }
    }
}