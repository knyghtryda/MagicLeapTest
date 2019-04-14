using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class HandTrackingMotion : MonoBehaviour
    {
        #region Private Variables

        [Space, SerializeField, Tooltip("Flag to specify if left hand should be tracked.")]
        private bool _trackLeftHand = true;

        [SerializeField, Tooltip("Flag to specify id right hand should be tracked.")]
        private bool _trackRightHand = true;
        #endregion
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var camera = Camera.main;
            var speed = 0.1f;
            
            float step = speed * Time.deltaTime; // calculate distance to move

            var moveVector = transform.position - camera.transform.position;
            var forward2d = Vector3.forward;
            forward2d.y = 0.0f;
            var camera2d = camera.transform.position;
            camera2d.y = 0.0f;

            if (MLHands.IsStarted)
            {
                MLHandKeyPose pose = KeyPose(MLHands.Left);
                if (pose == MLHandKeyPose.OpenHandBack)
                {
                    transform.Translate(forward2d.x * Time.deltaTime, 0, forward2d.z * Time.deltaTime, Camera.main.transform);
                }
            }
        }

        private MLHandKeyPose KeyPose(MLHand hand)
        {
            if (hand != null)
            {
                if (hand.KeyPoseConfidence > 0.75)
                {
                    return hand.KeyPose;
                }
            }
            return MLHandKeyPose.NoPose;
        }
    }
}
