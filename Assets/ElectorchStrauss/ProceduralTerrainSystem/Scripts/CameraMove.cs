using System;
using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class CameraMove : MonoBehaviour
    {
        private Camera mainCamera;
        private GameController _gameController;
        [SerializeField]private GameObject lookAtMesh,selfieStick,objectFocusing;
        private Vector3 rotations;
        private Vector3 previousPosition;
        [SerializeField][Range(0, 360)] private int maxRotationInOneSwipe = 360;
        [SerializeField] private float distanceToTarget = -900;
        [SerializeField] private float panSpeed = 500;
        [SerializeField] private float zoomSpeed = 5;

        public RectTransform heightCurvePanel;
        // Start is called before the first frame update
        void Start()
        {
            mainCamera = GetComponent<Camera>();
            _gameController = FindObjectOfType<GameController>();
            objectFocusing = lookAtMesh;
        }
        // Update is called once per frame
        void Update()
        {
            if (_gameController.rotateAroundAction.triggered || _gameController.moveInOut.y != 0)
            {
                previousPosition = mainCamera.ScreenToViewportPoint(_gameController.mousePos);
            }
            if ( _gameController.rotateAround > 0 || _gameController.moveInOut.y != 0)
            {
                Vector3 newPosition = mainCamera.ScreenToViewportPoint(_gameController.mousePos);
                Vector3 direction = previousPosition - newPosition;
                if (_gameController.moveShift == 0)
                {
                    mainCamera.transform.LookAt(objectFocusing.transform.position);
                    mainCamera.transform.position = objectFocusing.transform.position;
                    mainCamera.transform.Rotate(Vector3.right, direction.y * maxRotationInOneSwipe);
                    mainCamera.transform.Rotate(Vector3.up, -direction.x * maxRotationInOneSwipe, Space.World);
                    mainCamera.transform.Translate(0,0,distanceToTarget);
                }
                if(_gameController.moveShift > 0)
                {
                    objectFocusing = selfieStick;
                    mainCamera.transform.Translate(direction.y * panSpeed * Vector3.up);
                    mainCamera.transform.Translate(direction.x * panSpeed * Vector3.right);
                }
                previousPosition = newPosition;
            }
            selfieStick.transform.position = mainCamera.transform.position + mainCamera.transform.forward * -distanceToTarget;
            if (_gameController.focusSelectedAction.triggered)
            {
                objectFocusing = lookAtMesh;
                mainCamera.transform.LookAt(objectFocusing.transform.position);
            }
            if(!heightCurvePanel.gameObject.activeInHierarchy)
            {
                if (_gameController.moveInOut.y > 0)
                {
                    distanceToTarget += zoomSpeed;
                }

                if (_gameController.moveInOut.y < 0)
                {
                    distanceToTarget -= zoomSpeed;
                }
            }
            else
            {
                if (_gameController.moveInOut.y > 0)
                {
                    heightCurvePanel.localScale += Vector3.one*0.1f;
                }

                if (_gameController.moveInOut.y < 0)
                {
                    heightCurvePanel.localScale -= Vector3.one*0.1f;
                }
            }
            
            if (heightCurvePanel.localScale.x < 1)
            {
                heightCurvePanel.localScale = Vector3.one;
            }
        }
    }
}