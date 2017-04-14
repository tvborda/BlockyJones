using UnityEngine;

namespace VoxelStudy
{
    public class PlayerController : BaseController
    {
        protected override void Start()
        {
            base.Start();
        }

        void Update()
        {
            if (Time.timeScale > 0 && activate)
            {
                if (idle)
                {
                    horizontal = (int)Input.GetAxisRaw("Horizontal");
                    vertical = (horizontal == 0) ? (int)Input.GetAxisRaw("Vertical") : 0;
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