using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyTweens
{
    public class BezierSpline : MonoBehaviour
    {
        [SerializeField] private Vector3[] points;
        [SerializeField] List<float> legths = new List<float>();
        [SerializeField] float totalLength;

        [SerializeField] private BezierControlPointMode[] modes;

        [SerializeField] private bool loop;

        public bool lockXAxis;
        public bool lockYAxis;
        public bool lockZAxis;

        public Vector3[] Points => points;

        public int ControlPointCount => points.Length;

        public int CurveCount => (points.Length - 1) / 3;

        public bool Loop
        {
            get { return loop; }
            set
            {
                if (loop != value)
                {
                    loop = value;
                    if (value)
                    {
                        var diff = points[0] - points[points.Length - 1];
                        AddCurve(points[points.Length - 1] + diff * 0.4f, points[points.Length - 1] + diff * 0.6f, points[0]);
                        modes[modes.Length - 1] = modes[0];
                        SetControlPoint(0, points[0]);
                    }
                    else
                    {
                        RemoveCurve((points.Length - 1) / 3);
                    }
                }
            }
        }

        public Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        public Vector3 GetWorldPoint(float t)
        {
            if (Mathf.Approximately(t, 1f))
                t = 1f;
            else if (t > 1f)
                t -= (int) t;
            
            float currDist = t * totalLength;

            int i = 0;
            while (i < legths.Count && legths[i] < currDist)
            {
                currDist -= legths[i];
                i++;
            }

            float t2 = currDist / legths[i];

            return transform.TransformPoint(Bezier.GetPoint(points[i * 3], points[i * 3 + 1], points[i * 3 + 2],
                points[i * 3 + 3], t2));
        }

        public Vector3 GetWorldDirection(float t)
        {
            if (Mathf.Approximately(t, 1f))
                t = 1f;
            else if (t > 1f)
                t -= (int) t;
            
            float currDist = t * totalLength;

            int i = 0;
            while (i < legths.Count && legths[i] < currDist)
            {
                currDist -= legths[i];
                i++;
            }

            float t2 = currDist / legths[i];

            return transform.TransformDirection(Bezier.GetFirstDerivative(points[i * 3], points[i * 3 + 1],
                points[i * 3 + 2], points[i * 3 + 3], t2)).normalized;
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - points[index];
                if (loop)
                {
                    if (index == 0)
                    {
                        points[1] += delta;
                        points[points.Length - 2] += delta;
                        points[points.Length - 1] = point;
                    }
                    else if (index == points.Length - 1)
                    {
                        points[0] = point;
                        points[1] += delta;
                        points[index - 1] += delta;
                    }
                    else
                    {
                        points[index - 1] += delta;
                        points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        points[index - 1] += delta;
                    }

                    if (index + 1 < points.Length)
                    {
                        points[index + 1] += delta;
                    }
                }
            }

            points[index] = point;
            EnforceMode(index);
        }

        public BezierControlPointMode GetControlPointMode(int index)
        {
            return modes[(index + 1) / 3];
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            int modeIndex = (index + 1) / 3;
            modes[modeIndex] = mode;
            if (loop)
            {
                if (modeIndex == 0)
                {
                    modes[modes.Length - 1] = mode;
                }
                else if (modeIndex == modes.Length - 1)
                {
                    modes[0] = mode;
                }
            }

            EnforceMode(index);
        }

        public void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;

            if (modeIndex >= modes.Length) return;

            BezierControlPointMode mode = modes[modeIndex];
            if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
            {
                return;
            }

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                {
                    fixedIndex = points.Length - 2;
                }

                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Length)
                {
                    enforcedIndex = 1;
                }
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= points.Length)
                {
                    fixedIndex = 1;
                }

                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                {
                    enforcedIndex = points.Length - 2;
                }
            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
            }

            points[enforcedIndex] = middle + enforcedTangent;
            RecalculateCurveLengths();
        }

        public void AddCurve(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Array.Resize(ref points, points.Length + 3);
            points[points.Length - 3] = p0;
            points[points.Length - 2] = p1;
            points[points.Length - 1] = p2;

            Array.Resize(ref modes, modes.Length + 1);
            modes[modes.Length - 1] = modes[modes.Length - 2];

            if (loop)
            {
                points[0] = points[points.Length - 1];
                modes[modes.Length - 1] = modes[0];
                EnforceMode(0);
            }

            RecalculateCurveLengths();
        }

        public void RemoveCurvePoint(int pointIndex)
        {
            if (points.Length <= 4) return;

            if (points.Length - 3 > pointIndex)
            {
                int startRemoveIndex = Math.Max(pointIndex - 1, 0);
                for (int i = startRemoveIndex; i < points.Length - 3; i++)
                {
                    points[i] = points[i + 3];
                }
            }

            int modeIndex = (pointIndex + 1) / 3;
            for (int i = modeIndex; i < modes.Length - 1; i++)
            {
                modes[i] = modes[i + 1];
            }

            Array.Resize(ref points, points.Length - 3);
            Array.Resize(ref modes, modes.Length - 1);

            RecalculateCurveLengths();
        }

        public void RemoveCurve(int curveIndex)
        {
            if (CurveCount <= 1) return;

            int pointIndex = curveIndex * 3 + 1;

            for (int i = pointIndex; i < points.Length - 3; i++)
            {
                points[i] = points[i + 3];
            }

            Array.Resize(ref points, points.Length - 3);
            Array.Resize(ref modes, modes.Length - 1);

            EnforceMode(points.Length - 4);

            if (loop)
            {
                points[points.Length - 1] = points[0];
                modes[modes.Length - 1] = modes[0];
                EnforceMode(0);
            }

            RecalculateCurveLengths();
        }

        public void RecalculateCurveLengths(float precision = 0.1f)
        {
            legths.Clear();
            for (int i = 1; i < points.Length; i += 3)
            {
                legths.Add(Bezier.GetBezierLength(points[i - 1], points[i], points[i + 1], points[i + 2], precision));
            }

            totalLength = legths.Sum();
        }

        public void Reset()
        {
            points = new Vector3[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(4f / transform.lossyScale.x, 0f, 0f),
                new Vector3(6f/ transform.lossyScale.x, 0f, 0f),
                new Vector3(10f/ transform.lossyScale.x, 0f, 0f)
            };
            modes = new BezierControlPointMode[]
            {
                BezierControlPointMode.Free,
                BezierControlPointMode.Free
            };
        }

#if UNITY_EDITOR
        public Color GizmoColor = Color.white;
        public float GizmoWidth = 6f;
        public bool DrawGizmosUnselected = true;

        private void OnDrawGizmos()
        {
            if (!DrawGizmosUnselected) return;

            DrawPathGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (!DrawGizmosUnselected)
            {
                DrawPathGizmos();
            }
        }

        private void DrawPathGizmos()
        {
            for (int i = 1; i < ControlPointCount; i += 3)
            {
                Vector3 p0 = transform.TransformPoint(points[i - 1]);
                Vector3 p1 = transform.TransformPoint(points[i]);
                Vector3 p2 = transform.TransformPoint(points[i + 1]);
                Vector3 p3 = transform.TransformPoint(points[i + 2]);

                Handles.DrawBezier(p0, p3, p1, p2, GizmoColor, null, GizmoWidth);
            }
        }
#endif
    }
}