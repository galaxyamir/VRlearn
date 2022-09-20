/*
 * Script inspired by: http://www.theappguruz.com/blog/bezier-curve-in-games
 */

using UnityEngine;
using System.Collections.Generic;

namespace VREasy
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class SplineController : MonoBehaviour
    {

        public float LineWidth
        {
            set
            {
                bool needsRedraw = _lineWidth != value;
                _lineWidth = value;
                if(needsRedraw) DrawCurve();
            }
            get
            {
                return _lineWidth;
            }
        }
        public float _lineWidth = 1.0f;

        public int VerticalAngle
        {
            set
            {
                bool needsRedraw = _verticalAngle != value;
                _verticalAngle = value;
                up = Quaternion.AngleAxis(-_verticalAngle, Vector3.forward) * Vector3.up;
                if (needsRedraw) DrawCurve();
            }
            get
            {
                return _verticalAngle;
            }
        }
        public int _verticalAngle = 0;

        public MeshFilter Mesh_Filter
        {
            get
            {
                if (meshFilter == null)
                {
                    meshFilter = GetComponent<MeshFilter>();
                }
                return meshFilter;
            }
        }
        private MeshFilter meshFilter;

        private MeshRenderer meshRenderer
        {
            get
            {
                if(_meshRenderer == null)
                {
                    _meshRenderer = GetComponent<MeshRenderer>();
                }
                return _meshRenderer;
            }
        }
        private MeshRenderer _meshRenderer;

        public Texture texture
        {
            get
            {
                return meshRenderer.sharedMaterial.mainTexture;
            }
            set
            {
                meshRenderer.sharedMaterial.mainTexture = value;
            }
        }
        

        public int ArrowCount
        {
            get
            {
                return _arrowCount;
            }
            set
            {
                bool needsRedraw = _arrowCount != value;
                _arrowCount = value;
                if (needsRedraw) DrawCurve();
            }
        }
        public int _arrowCount = 3;

        public List<Transform> ControlPoints = new List<Transform>();
        public float ScrollSpeed = 1.0f;
        public int BEZIER_MULTIPLIER = 3;
        public Vector3 up = Vector3.up;
        
        private int curveCount = 0;
        private Vector4 uvOffset = new Vector4(0,0,0,0);
        
        
        void Start()
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = new Material(Resources.Load<Material>("SplineArrow"));
            DrawCurve();
        }
        void Update()
        {
            uvOffset.x = -ScrollSpeed * Time.time;
            uvOffset.y = 0;
            meshRenderer.sharedMaterial.SetTextureOffset("_MainTex",uvOffset);
        }

        private void OnDestroy()
        {
            //DestroyImmediate(meshRenderer.sharedMaterial);
        }

        public void DrawCurve()
        {
            curveCount = ControlPoints.Count / (BEZIER_MULTIPLIER - 1);
            List<Vector3> points = new List<Vector3>();
            for (int j = 0; j < curveCount; j++) {
                for (int i = 1; i <= ArrowCount; i++) {
                    float t = i / (float)ArrowCount;
                    int nodeIndex = j * (BEZIER_MULTIPLIER-1);
                    if (!checkPoints(nodeIndex))
                        continue;
                    Vector3 pixel = Vector3.zero;
                    switch(BEZIER_MULTIPLIER)
                    {
                        case 3:
                            pixel = CalculateQuadraticBezierPoint(t, ControlPoints[nodeIndex].position, ControlPoints[nodeIndex + 1].position, ControlPoints[nodeIndex + 2].position);
                            break;
                        case 4:
                            pixel = CalculateCubicBezierPoint(t, ControlPoints[nodeIndex].position, ControlPoints[nodeIndex + 1].position, ControlPoints[nodeIndex + 2].position, ControlPoints[nodeIndex + 3].position);
                            break;
                    }
                    points.Add(pixel);
                }

            }

            // create mesh from point list
            //createMesh(points);
            VREasy_utils.MeshFromPoints(points, Mesh_Filter, up, LineWidth);
            
        }

        Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }
        Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            
            Vector3 p = uu * p0 + 2 * u * t * p1 + tt * p2;

            return p;
        }

        private bool checkPoints(int index)
        {
            for(int ii=0; ii < BEZIER_MULTIPLIER; ii++)
            {
                if (ControlPoints.Count <= index + ii)
                    return false;
                if(ControlPoints[index+ii] == null)
                {
                    ControlPoints.RemoveAt(index+ii);
                    return false;
                }
            }
            return true;
        }
    }
}