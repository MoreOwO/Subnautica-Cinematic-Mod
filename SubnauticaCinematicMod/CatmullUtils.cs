using System;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace SubnauticaCinematicMod
{
    public static class CatmullUtils
    {
        public static Segment GetSegment(CameraPoint p0, CameraPoint p1, CameraPoint p2, CameraPoint p3, float alpha, float tension)
        {
            float t01 = (float) Math.Pow(Vector3.Distance(p0.Position, p1.Position), alpha);
            float t12 = (float) Math.Pow(Vector3.Distance(p1.Position, p2.Position), alpha);
            float t23 = (float) Math.Pow(Vector3.Distance(p2.Position, p3.Position), alpha);

            Vector3 m1 = (1.0f - tension) *
                      (p2.Position - p1.Position + t12 * ((p1.Position - p0.Position) / t01 - (p2.Position - p0.Position) / (t01 + t12)));
            Vector3 m2 = (1.0f - tension) *
                      (p2.Position - p1.Position + t12 * ((p3.Position - p2.Position) / t23 - (p3.Position - p1.Position) / (t12 + t23)));

            return new Segment(p1.Position, p2.Position, m1, m2, p1.Rotation, p2.Rotation, p1.FOV, p2.FOV);
        }

        public static Vector3 GetPoint(Segment segment, float t)
        {
            return segment.A * (t * t * t) + segment.B * (t * t) + segment.C * t + segment.D;
        }
        
        public static float GetLengthOfSegment(Segment segment, float accuracy=0.1f)
        {
            float length = 0;
            Vector3 lastPoint = GetPoint(segment, 0);
            for (float i = 0; i <= 1; i += accuracy)
            {
                Vector3 currentPoint = GetPoint(segment, i);
                length += Vector3.Distance(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }

            return length;
        }
    }

    public struct Segment
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
        public Vector3 D;
        public Quaternion RotationStart;
        public Quaternion RotationEnd;
        public float FOVStart;
        public float FOVEnd;

        public Segment(Vector3 p1, Vector3 p2, Vector3 m1, Vector3 m2, Quaternion rotationStart, Quaternion rotationEnd, float fovStart, float fovEnd)
        {
            A = 2.0f * (p1 - p2) + m1 + m2;
            B = -3.0f * (p1 - p2) - m1 - m1 - m2;
            C = m1;
            D = p1;
            RotationStart = rotationStart;
            RotationEnd = rotationEnd;
            FOVStart = fovStart;
            FOVEnd = fovEnd;
        }
    }
}