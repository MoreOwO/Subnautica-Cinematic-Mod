using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using QModManager.API;
using UnityEngine;
using UnityEngine.PostProcessing;
using Logger = QModManager.Utility.Logger;


namespace SubnauticaCinematicMod
{
    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeCameraMain")]
    public class PathManager: MonoBehaviour
    {
        private static PathManager _instance;
        private static GameObject _dummyObject;
        private readonly List<CameraPoint> _points = new();
        private int _pathDurationSeconds = 30;
        private const float Alpha = 0.5f;
        private const float Tension = 0f;
        private bool _isDoingPath;
        private float _timeElapsed;
        private float _timeElapsedInSegment;
        private readonly List<Segment> _segments = new();
        private LineRenderer _linearLine;
        private LineRenderer _interpolatedLine;
        private const float InterpolatedLineSmoothness = 0.01f;
        private bool _firstTimeRunningPath = true;
        private readonly List<float> _segmentsDurations = new();
        private int _currentIndex;
        public bool showLines = false;

        public static PathManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Logger.Log(Logger.Level.Debug, "Creating PathManager");
                _dummyObject = new GameObject("PathManager");
                _dummyObject.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(_dummyObject);
                _dummyObject.AddComponent<SceneCleanerPreserve>();
                _instance = _dummyObject.AddComponent<PathManager>();
                return _instance;
            }
        }

        // ReSharper disable all UseObjectOrCollectionInitializer
        public void Start()
        {
            GameObject linearLineGO = new GameObject("LinearLine");
            linearLineGO.hideFlags = HideFlags.HideInHierarchy;
            DontDestroyOnLoad(linearLineGO);
            linearLineGO.AddComponent<SceneCleanerPreserve>();
            _linearLine = linearLineGO.AddComponent<LineRenderer>();
            _linearLine.startWidth = _linearLine.endWidth = 0.1f;
            _linearLine.startColor = _linearLine.endColor = new Color(1f, 0, 0, 1f);
            _linearLine.material = UnityEngine.UI.Graphic.defaultGraphicMaterial;

            
            GameObject interpolatedLineGO = new GameObject("InterpolatedLine");
            interpolatedLineGO.hideFlags = HideFlags.HideInHierarchy;
            DontDestroyOnLoad(interpolatedLineGO);
            interpolatedLineGO.AddComponent<SceneCleanerPreserve>();
            _interpolatedLine = interpolatedLineGO.AddComponent<LineRenderer>();
            _interpolatedLine.startWidth = _interpolatedLine.endWidth = 0.1f;
            _interpolatedLine.startColor = _interpolatedLine.endColor = new Color(1f, 1f, 0, 1f);
            _interpolatedLine.material = UnityEngine.UI.Graphic.defaultGraphicMaterial;
        }

        public float? RunPath(int? duration)
        {
            _segments.Clear();
            _segmentsDurations.Clear();
            if (duration != null) _pathDurationSeconds = duration.Value;
            if (_points.Count < 2)
            {
                Logger.Log(Logger.Level.Error, "Not enough points");
                QModServices.Main.AddCriticalMessage($"Not enough points, only {_points.Count} instead of 2");
                return null;
            }

            if (_firstTimeRunningPath)
            {
                _points.Insert(0, new CameraPoint(_points[0].Position - (_points[1].Position - _points[0].Position).normalized, Quaternion.identity, 0));
                _points.Add(new CameraPoint(_points[_points.Count - 1].Position - (_points[_points.Count - 1].Position - _points[_points.Count - 2].Position).normalized, Quaternion.identity, 0));
            }
            else
            {
                return _pathDurationSeconds;
            }
            for (int i = 1; i < _points.Count - 2; i++)
            {
                Segment segment = CatmullUtils.GetSegment(_points[i - 1], _points[i],
                    _points[i + 1], _points[i + 2], Alpha, Tension);
                _segments.Add(segment); 
            }
            // _timePerSegment = _pathDurationSeconds / (float) _segments.Count;
            float[] lengthOfEverySegment = new float[_segments.Count];
            for (int i=0; i < _segments.Count; i++)
            {
                lengthOfEverySegment[i] = CatmullUtils.GetLengthOfSegment(_segments[i], InterpolatedLineSmoothness);
            }
            
            float totalDistance = lengthOfEverySegment.Sum();

            foreach (float length in lengthOfEverySegment)
            {
                _segmentsDurations.Add((length / totalDistance) * _pathDurationSeconds);
            }

            for (int i = 0; i < lengthOfEverySegment.Length; i++)
            {
                Logger.Log(Logger.Level.Debug, $"index: {i}, dTotale: {totalDistance}, tTotal: {_pathDurationSeconds}, " +
                                               $"dSegement: {lengthOfEverySegment[i]}, tSegment: {_segmentsDurations[i]}, vSegment: {lengthOfEverySegment[i] / _segmentsDurations[i]}");
            }
            
            // _beforePathTransform = new CameraPoint(Camera.main.transform.position, Camera.main.transform.rotation, 70f);
            StartPathPlayerInit();
            return _pathDurationSeconds;
        }

        private int CurrentSegmentIndex(float currentTime, float timePerSegment)
        { 
            return (int) Math.Floor(currentTime / timePerSegment);
        }

        // ReSharper disable all PossibleNullReferenceException
        public void Update()
        {
            Transform cameraTransform = MainCamera.camera.transform;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Vector3 rotation = cameraTransform.eulerAngles;
                rotation.z += 1;
                cameraTransform.eulerAngles = rotation;
            } 
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Vector3 rotation = cameraTransform.eulerAngles;
                rotation.z -= 1;
                cameraTransform.eulerAngles = rotation;
            } else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 rotation = cameraTransform.eulerAngles;
                rotation.z = 0;
                cameraTransform.eulerAngles = rotation;
            } else if (Input.GetKey(KeyCode.KeypadPlus))
            {
                Camera.main.fieldOfView += 1;
            } else if (Input.GetKey(KeyCode.KeypadMinus))
            {
                Camera.main.fieldOfView -= 1;
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Camera.main.fieldOfView = 70;
            } else if (Input.GetKeyDown(KeyCode.Escape) && _isDoingPath)
            {
                StopPathPlayerFree();
                return;
            }
            if (!_isDoingPath) return;
            if (_timeElapsed > _pathDurationSeconds)
            {
                StopPathPlayerFree();
                return;
            }

            if (_timeElapsedInSegment > _segmentsDurations[_currentIndex])
            {
                _currentIndex++;
                _timeElapsedInSegment = 0;
            }
            float timeInSegment = _timeElapsedInSegment / _segmentsDurations[_currentIndex];
            Vector3 pos = CatmullUtils.GetPoint(_segments[_currentIndex], timeInSegment);
            SNCameraRoot.main.transform.position = pos;
            SNCameraRoot.main.transform.rotation = Quaternion.Slerp(_segments[_currentIndex].RotationStart, _segments[_currentIndex].RotationEnd, (float) -(Math.Cos(Math.PI * timeInSegment) - 1) / 2);
            SNCameraRoot.main.mainCam.fieldOfView = Mathf.Lerp(_segments[_currentIndex].FOVStart,
                _segments[_currentIndex].FOVEnd, (float) -(Math.Cos(Math.PI * timeInSegment) - 1) / 2);
            _timeElapsed += Time.deltaTime;
            _timeElapsedInSegment += Time.deltaTime;
        }

        public void AddPoint(Camera camera)
        {
            if (!_firstTimeRunningPath)
            {
                _points.RemoveAt(0);
                _points.RemoveAt(_points.Count - 1);
                _firstTimeRunningPath = true;
            }
            var cameraTransform = camera.transform;
            _points.Add(new CameraPoint(cameraTransform.position, cameraTransform.rotation, camera.fieldOfView));
            if (_points.Count > 1 && showLines)
            {
                RefreshLine();
            }
        }

        public void ClearPoints()
        {
            _points.Clear();
            _segments.Clear();
            _segmentsDurations.Clear();
            _timeElapsed = 0;
            _linearLine.positionCount = 0;
            _interpolatedLine.positionCount = 0;
            _firstTimeRunningPath = true;
        }
        

        private void RefreshLine()
        {
            CameraPoint[] linePoints = new CameraPoint[_points.Count + 2];
            List<Segment> lineSegments = new List<Segment>();
            List<Vector3> interpolatedPoints = new List<Vector3>();
            
            linePoints[0] = new CameraPoint(_points[0].Position - (_points[1].Position - _points[0].Position).normalized, Quaternion.identity, 0);
            linePoints[linePoints.Length - 1] = new CameraPoint(_points[_points.Count - 1].Position - (_points[_points.Count - 1].Position - _points[_points.Count - 2].Position).normalized, Quaternion.identity, 0);
            
            for (int i = 0; i < _points.Count; i++)
            {
                linePoints[i + 1] = _points[i];
            }

            for (int i = 1; i < linePoints.Length - 2; i++)
            {
                Segment segment = CatmullUtils.GetSegment(linePoints[i - 1], linePoints[i],
                    linePoints[i + 1], linePoints[i + 2], Alpha, Tension);
                lineSegments.Add(segment); 
            }

            Vector3[] lineVectors = new Vector3[linePoints.Length - 2];
            for (int i = 1; i < linePoints.Length - 1; i++)
            {
                lineVectors[i - 1] = linePoints[i].Position;
            }
            if (_linearLine == null) Logger.Log(Logger.Level.Debug, "Linear Line, null");
            _linearLine.positionCount = lineVectors.Length;
            _linearLine.SetPositions(lineVectors);

            for (float i = 0; i < lineSegments.Count; i += InterpolatedLineSmoothness)
            {
                int currentSegmentIndex = CurrentSegmentIndex(i, 1);
                float timeInSegment = (i - currentSegmentIndex * 1) / ((currentSegmentIndex + 1) * 1 - currentSegmentIndex * 1);
                interpolatedPoints.Add(CatmullUtils.GetPoint(lineSegments[currentSegmentIndex], timeInSegment));
            }
            if (_interpolatedLine == null) Logger.Log(Logger.Level.Debug, "Interpolated line, null");
            _interpolatedLine.positionCount = interpolatedPoints.Count;
            _interpolatedLine.SetPositions(interpolatedPoints.ToArray());
        }

        public void ShowPath()
        {
            if (_points.Count > 1 && showLines)
            {
                RefreshLine();
            }
        }

        public void HidePath()
        {
            _linearLine.positionCount = 0;
            _interpolatedLine.positionCount = 0;
        }

        public void StopPath()
        {
            StopPathPlayerFree();
        }

        private void StartPathPlayerInit()
        {
            if (Player.main is null)
            {
                Logger.Log(Logger.Level.Error, "Player.main is null at enabling cinematic mode", showOnScreen:true);
                _isDoingPath = false;
                _firstTimeRunningPath = false;
                return;
            }
            HidePath();
            _isDoingPath = true;
            Player player = Player.main;
            player.EnterLockedMode(null, false);
            player.SetHeadVisible(true);
            MainCameraControl.main.enabled = false;
        }

        private void StopPathPlayerFree()
        {
            if (Player.main is null)
            {
                Logger.Log(Logger.Level.Error, "Player.main is null at disabled cinematic mode, restart your game", showOnScreen:true);
                return;
            }
            ShowPath();
            _isDoingPath = false;
            _timeElapsed = 0;
            _currentIndex = 0;
            Player player = Player.main;
            player.ExitLockedMode(false, false);
            player.SetHeadVisible(false);
            MainCameraControl.main.enabled = true;
            SNCameraRoot.main.transform.localPosition = Vector3.zero;
            SNCameraRoot.main.transform.localRotation = Quaternion.identity;
        }
        

        public void OnDestroy()
        {
            Destroy(_linearLine.gameObject);
            Destroy(_interpolatedLine.gameObject);
            Destroy(_dummyObject);
        }

        public void DestroyPathManager()
        {
            Logger.Log(Logger.Level.Debug, "Destroying Path Manager");
            OnDestroy();
        }
    }
}