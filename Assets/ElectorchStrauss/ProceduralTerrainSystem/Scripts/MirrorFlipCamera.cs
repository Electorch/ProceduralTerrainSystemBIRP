using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class MirrorFlipCamera : MonoBehaviour
    {
        /*
         * http://answers.unity.com/answers/1300369/view.html
         * This script flips the camera horizontally for BIRP only.
        */

        public Camera _camera;
        public bool flipHorizontal;

        void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        void OnPreCull()
        {
            _camera.ResetWorldToCameraMatrix();
            _camera.ResetProjectionMatrix();
            Vector3 scale = new Vector3(flipHorizontal ? -1 : 1, 1, 1);
            _camera.projectionMatrix *= Matrix4x4.Scale(scale);
        }

        void OnPreRender()
        {
            GL.invertCulling = flipHorizontal;
        }

        void OnPostRender()
        {
            GL.invertCulling = false;
        }
    }
}