using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;


public class PlaceOnMesh : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField, Tooltip("The hand to visualize.")]
    private MLHandType _handType = MLHandType.Left;

    private GameObject obj;

    /// <summary>
    /// Returns the hand based on the hand type.
    /// </summary>
    private MLHand Hand
    {
        get
        {
            if (_handType == MLHandType.Left)
            {
                return MLHands.Left;
            }
            else
            {
                return MLHands.Right;
            }
        }
    }

    private Vector3 lPosition;
    private Vector3 lDirection;
    private float speed = 0.1f;
    private bool placed = false;
    private float placeTimer = 0.0f;
    
    // Start is called before the first frame update

    void Start()
    {
        MLWorldRays.Start();
        obj = Instantiate(prefab);
        obj.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        //obj.SetActive(false);
        /*
        MLResult result = MLHands.Start();
        if (!result.IsOk)
        {
            Debug.LogErrorFormat("Error: HandTrackingVisualizer failed starting MLHands, disabling script. Reason: {0}", result);
            enabled = false;
            return;
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        var hPosition = Hand.Index.Tip.Position;
        var rayOrigin = Hand.Wrist.Center.Position;
        var rayDirection = Hand.Index.PIP.Position;
        if (MLHands.IsStarted)
        {
            MLHandKeyPose pose = KeyPose(Hand);
            if (pose == MLHandKeyPose.Pinch)
            {
                //obj.SetActive(true);
                obj.transform.position = hPosition;
                obj.GetComponent<ActionStates>().deactivateSpider();
            }

            if (Hand.GetKeyPoseDown(MLHandKeyPose.OpenHandBack))
            {
                obj.GetComponent<ActionStates>().backUp();
            }

            if (pose == MLHandKeyPose.Finger)
            {
                if (Hand.GetKeyPoseDown(MLHandKeyPose.Finger))
                {
                    lPosition = rayDirection;
                    lDirection = rayDirection - rayOrigin;
                }
                else
                {
                    lPosition = Vector3.MoveTowards(lPosition, rayDirection, speed * Time.deltaTime);
                    lDirection = Vector3.MoveTowards(lDirection, rayDirection - rayOrigin, speed * Time.deltaTime);
                }
                CastRay(lPosition, lDirection);

            }
        }

    }

    private void CastRay(Vector3 position, Vector3 direction)
    {
        MLWorldRays.QueryParams _raycastParams = new MLWorldRays.QueryParams
        {
            // Update the parameters with our Hand's transform
            Position = position,
            Direction = direction,
            UpVector = new Vector3(0, 1, 0),
            // Provide a size of our raycasting array (1x1)
            Width = 1,
            Height = 1
        };
        // Feed our modified raycast parameters and handler to our raycast request
        MLWorldRays.GetWorldRays(_raycastParams, HandleOnReceiveRaycast);
    }
    private void OnDestroy()
    {
        MLWorldRays.Stop();
        Destroy(obj);
    }
    private IEnumerator NormalMarker(Vector3 point, Vector3 normal)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        var camera = Camera.main;
        //obj.transform.SetPositionAndRotation(point, rotation);
        obj.transform.position = point;
        var lookPosition = new Vector3(camera.transform.position.x, point.y, camera.transform.position.z);
        obj.transform.LookAt(lookPosition);
        obj.GetComponent<ActionStates>().activateSpider();
        //GameObject go = Instantiate(prefab, point, rotation);
        yield return new WaitForSeconds(2);
        //Destroy(go);
    }

    void HandleOnReceiveRaycast(MLWorldRays.MLWorldRaycastResultState state, UnityEngine.Vector3 point, UnityEngine.Vector3 normal, float confidence)
    {
        if (state == MLWorldRays.MLWorldRaycastResultState.HitObserved)
        {
            StartCoroutine(NormalMarker(point, normal));
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


