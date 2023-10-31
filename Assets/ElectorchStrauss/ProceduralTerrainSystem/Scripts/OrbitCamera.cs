using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class OrbitCamera : MonoBehaviour
    {
        //This OrbitCamera script comes from this tutorial with some minor changes : https://catlikecoding.com/unity/tutorials/movement/orbit-camera/
        [TextArea]
        [Tooltip("What does this script do?")]
        public string Notes = "This script make the camera rotate around the player.";
        [SerializeField, Range(1f, 20f)] float distance = 7f;
        [SerializeField, Min(0f)] float focusRadius = 1f;
        public Vector3 focusPoint, previousFocusPoint;
        [SerializeField, Range(0f, 1f)] float focusCentering = 0.5f;
        public Vector2 orbitAngles = new(45f, 0f);
        [SerializeField, Range(1f, 360f)] float rotationSpeed = 1f;
        [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
        [SerializeField, Min(0f)] float alignDelay = 5f;
        float lastManualRotationTime;
        [SerializeField, Range(0f, 90f)] float alignSmoothRange = 45f;
        Camera regularCamera;
        [SerializeField] LayerMask obstructionMask = -1;
        Quaternion gravityAlignment = Quaternion.identity;
        Quaternion orbitRotation;
        [SerializeField] public Transform focus;
        void Awake()
        {
            regularCamera = GetComponent<Camera>();
            transform.localRotation = orbitRotation = Quaternion.Euler(orbitAngles);
        }
        Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                halfExtends.y =
                    regularCamera.nearClipPlane *
                    Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
                halfExtends.x = halfExtends.y * regularCamera.aspect;
                halfExtends.z = 0f;
                return halfExtends;
            }
        }
        void LateUpdate()
        {
            if (focus)
            {
                UpdateFocusPoint();

                ManualRotation();
                if (ManualRotation() || AutomaticRotation())
                {
                    ConstrainAngles();
                    orbitRotation = Quaternion.Euler(orbitAngles);
                }

                Quaternion lookRotation = gravityAlignment * orbitRotation;


                Vector3 lookDirection = lookRotation * Vector3.forward;
                Vector3 lookPosition = focusPoint - lookDirection * distance;
                Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
                Vector3 rectPosition = lookPosition + rectOffset;
                Vector3 castFrom = focus.position;
                Vector3 castLine = rectPosition - castFrom;
                float castDistance = castLine.magnitude;
                Vector3 castDirection = castLine / castDistance;

                if (Physics.BoxCast(
                    castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation, castDistance,
                    obstructionMask,
                    QueryTriggerInteraction.Ignore
                ))
                {
                    rectPosition = castFrom + castDirection * hit.distance;
                    lookPosition = rectPosition - rectOffset;
                }

                transform.SetPositionAndRotation(lookPosition, lookRotation);
            }
        }

        void OnValidate()
        {
            if (maxVerticalAngle < minVerticalAngle)
            {
                maxVerticalAngle = minVerticalAngle;
            }
        }

        void ConstrainAngles()
        {
            orbitAngles.x =
                Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

            if (orbitAngles.y < 0f)
            {
                orbitAngles.y += 360f;
            }
            else if (orbitAngles.y >= 360f)
            {
                orbitAngles.y -= 360f;
            }
        }

        public bool ManualRotation()
        {
            Vector2 input = new Vector2(
                -Input.GetAxis("Mouse Y"),
                Input.GetAxis("Mouse X")
            );
            const float e = 0.001f;
            if (input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
                lastManualRotationTime = Time.unscaledTime;
                return true;
            }

            return false;
        }

        bool AutomaticRotation()
        {
            if (Time.unscaledTime - lastManualRotationTime < alignDelay)
            {
                return false;
            }

            Vector3 alignedDelta =
                Quaternion.Inverse(gravityAlignment) *
                (focusPoint - previousFocusPoint);
            Vector2 movement = new Vector2(alignedDelta.x, alignedDelta.z);
            float movementDeltaSqr = movement.sqrMagnitude;
            if (movementDeltaSqr < 0.000001f)
            {
                return false;
            }

            float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
            float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
            float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
            if (deltaAbs < alignSmoothRange)
            {
                rotationChange *= deltaAbs / alignSmoothRange;
            }
            else if (180f - deltaAbs < alignSmoothRange)
            {
                rotationChange *= (180f - deltaAbs) / alignSmoothRange;
            }

            orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
            return true;
        }

        static float GetAngle(Vector2 direction)
        {
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            return direction.x < 0f ? 360f - angle : angle;
        }

        void UpdateFocusPoint()
        {
            previousFocusPoint = focusPoint;
            Vector3 targetPoint = focus.position;
            if (focusRadius > 0f)
            {
                float distance = Vector3.Distance(targetPoint, focusPoint);
                float t = 1f;
                if (distance > 0.01f && focusCentering > 0f)
                {
                    t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
                }

                if (distance > focusRadius)
                {
                    t = Mathf.Min(t, focusRadius / distance);
                }

                focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
            }
            else
            {
                focusPoint = targetPoint;
            }
        }
    }
