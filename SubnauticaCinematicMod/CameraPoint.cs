using UnityEngine;

namespace SubnauticaCinematicMod
{
    public struct CameraPoint
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float FOV;

        public CameraPoint(Vector3 position, Quaternion rotation, float fov)
        {
            Position = position;
            Rotation = rotation;
            FOV = fov;
        }

        public override string ToString()
        {
            return $"CameraPoint(Position={Position}, Rotation={Rotation}, FOV={FOV})";
        }
    }
}