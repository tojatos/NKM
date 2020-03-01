using System.Linq;
using Unity.Hex;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity
{
    public class MainCameraController : SingletonMonoBehaviour<MainCameraController>
    {
        public float MinZoom = 10f;
        public float MaxZoom;
        public float Sensitivity = 20f;
        public float PositionChange = 15f;

        private Camera _cam;
        private Vector3 _mainPosition;

        private Vector3 _origin;
        private Vector3 _diference;
        private bool _drag;

        public void Init()
        {
            _cam = GetComponent<Camera>();
            Bounds meshBounds = HexMapDrawer.Instance.HexMesh.Mesh.bounds;
            Vector3 boundsCenter = meshBounds.center;
            var startingPosition = new Vector3(boundsCenter.x, 300, boundsCenter.z + 10); // add 10 because of topbar
            _cam.transform.position = startingPosition;
            float startingZoom = meshBounds.size.magnitude / 2.8f;
            _cam.orthographicSize = startingZoom;
            MaxZoom = startingZoom;
            _mainPosition = startingPosition;
        }
        private void Update()
        {
            if(!new []{Scenes.MainGame, Scenes.MapEditor}.Contains(SceneManager.GetActiveScene().name)) return;
            if(!HexMapDrawer.Instance.Created) return;

            if (Input.touchCount == 2)
            {
                MultiTouch();
            }
            // Scroll forward
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                ZoomOrthoCamera(true);
            }

            // Scoll back
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                ZoomOrthoCamera(false);
            }
            //Drag camera
            // Math.Abs(_cam.orthographicSize - MaxZoom) > 0.01 - drag only on zoom
            if (Input.GetMouseButton(2) && !Utilities.IsPointerOverUiObject()) //drag only over a Game
            {
                _diference = _cam.ScreenToWorldPoint(Input.mousePosition) - _cam.transform.position;
                if (_drag == false)
                {
                    _drag = true;
                    _origin = _cam.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                _drag = false;
            }
            if (_drag)
            {
                _cam.transform.position = _origin - _diference;
            }
        }

        private void MultiTouch()
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            //MessageLogger.Instance.Log(deltaMagnitudeDiff.ToString());
            if (deltaMagnitudeDiff < -20)
            {
                ZoomOrthoCamera(true);
            }

            if (deltaMagnitudeDiff > 20)
            {
                ZoomOrthoCamera(false);
            }
        }

        private Vector3 GetScreenToWorldPoint() => _cam.ScreenToWorldPoint(Input.mousePosition);

        private void ZoomOrthoCamera(bool zoomFoward)
        {
            // Calculate how much we will have to move towards the zoomTowards position
            //float multiplier = (1.0f / _cam.orthographicSize * PositionChange);
            float multiplier = PositionChange / _cam.orthographicSize;
            Vector3 amountToMove;
            if (zoomFoward)
            {
                Vector3 zoomTowards = GetScreenToWorldPoint();
                amountToMove = (zoomTowards - transform.position) * multiplier;
            }
            else
            {
                amountToMove = MaxZoom -_cam.orthographicSize < 20 ? _mainPosition - transform.position : Vector3.zero; // * (PositionChange/_cam.orthographicSize);
            }

            // Move camera
            transform.position += amountToMove;
            // Zoom camera
            if (zoomFoward)
            {
                _cam.orthographicSize -= Sensitivity;
            }
            else
            {
                _cam.orthographicSize += Sensitivity;
            }

            // Limit zoom
            _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, MinZoom, MaxZoom);
        }
    }
}
