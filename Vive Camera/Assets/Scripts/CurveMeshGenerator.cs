using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CurveMeshGenerator : MonoBehaviour {

	private Mesh mesh = null;

	//biger resolution for smoother mesh and more vertices
	public int resolution = 20;
	public int width = 100;
	public int height = 100;
	public float curve_ratio = 3f;

	private List<Vector3> points = new List<Vector3> ();

	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvs = new List<Vector2> ();
//	private List<Vector3> normals = new List<Vector3>();

	// Use this for initialization
	void Start () {
		mesh = new Mesh ();
		createMesh ();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		;
//		mesh.normals = normals;
		GetComponent<MeshFilter> ().mesh = mesh;
		saveMesh (mesh, "Assets/CurveVideoSurface/Materials/curve.mat");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void createMesh(){
		//
		points.Clear();
		points.Add (new Vector3 (-width / 2.0f, 0f, 0f));
		points.Add (new Vector3 (-width / 6.0f, 0f, width/curve_ratio));
		points.Add (new Vector3 (width / 6.0f, 0f, width/curve_ratio));
		points.Add (new Vector3 (width / 2.0f, 0f, 0f));

		//
		for (int i = 0; i < resolution; i++) {
			float t = (float)i / (float)(resolution - 1);
			// Get the point on our curve using the 4 points generated above
			Vector3 p = getBezier(t, points[0], points[1], points[2], points[3]);
			AddTerrainPoint(p);
		}
	}

	void AddTerrainPoint(Vector3 point) {
		// Create a corresponding point along the bottom
		vertices.Add(new Vector3(point.x, -height/2, point.z));
		uvs.Add (new Vector3 (point.x/width+0.5f, 0, 0));
		// Then add our top point
		vertices.Add(new Vector3(point.x, height/2, point.z));
		uvs.Add (new Vector3 (point.x/width+0.5f, 1, 0));
		if (vertices.Count >= 4) {
			// We have completed a new quad, create 2 triangles
			int start = vertices.Count - 4;
			triangles.Add(start + 0);
			triangles.Add(start + 1);
			triangles.Add(start + 2);
			triangles.Add(start + 1);
			triangles.Add(start + 3);
			triangles.Add(start + 2);    
		}
	}

	public static Vector3 getBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3){
		float u, uu, uuu, tt, ttt;
		Vector3 p;

		u = 1 - t;
		uu = u * u;
		uuu = uu * u;

		tt = t * t;
		ttt = tt * t;

		p = uuu * p0;
		p += 3 * uu * t * p1;
		p += 3 * u * tt * p2;
		p += ttt * p3;

		return p;
	}

	private void saveMesh(Mesh mesh,String path){
		#if UNITY_EDITOR
		UnityEditor.AssetDatabase.CreateAsset (mesh, path);
		UnityEditor.AssetDatabase.SaveAssets ();
		#endif
	}
}
