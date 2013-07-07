using UnityEngine;
using System.Collections;

public class LinesGL : MonoBehaviour {

	public Shader shader;

	private static Material m;
	private GameObject g;
	private float speed = 100.0f;
	private Vector3[] lp;
	private Vector3[] sp;
	private Vector3 s;

	private GUIStyle labelStyle;
	private GUIStyle linkStyle;
	
	void Start () {
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		
		linkStyle = new GUIStyle();
		linkStyle.normal.textColor = Color.blue;
		
		m = new Material(shader);
		g = new GameObject("g");
		lp = new Vector3[0];
		sp = new Vector3[0];
	}
	
	void processInput() {
		float s = speed * Time.deltaTime;
		if(Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) s = s * 0.1f;
		if(Input.GetKey(KeyCode.UpArrow)) g.transform.Rotate(-s, 0, 0);
		if(Input.GetKey(KeyCode.DownArrow)) g.transform.Rotate(s, 0, 0);
		if(Input.GetKey(KeyCode.LeftArrow)) g.transform.Rotate(0, -s, 0);
		if(Input.GetKey(KeyCode.RightArrow)) g.transform.Rotate(0, s, 0);
		
		if(Input.GetKeyDown(KeyCode.C)) {
			g.transform.rotation = Quaternion.identity;
			lp = new Vector3[0];
			sp = new Vector3[0];
		}
	}
	
	void Update() {
		processInput();
		
		if(Input.GetMouseButton(0)) {
			
			Vector3 e = GetNewPoint();
			
			if(s != Vector3.zero) {
				for(int i = 0; i < lp.Length; i += 2) {
					float d = Vector3.Distance(lp[i], e);
					if(d < 1 && Random.value > 0.9f) sp = AddLine(sp, lp[i], e, false);
				}
				
				lp = AddLine(lp, s, e, false);
			}
			
			s = e;
		} else {
			s = Vector3.zero;
		}
	}
	
	/** Replace the Update function with this one for a click&drag drawing option */
	void Update1() {
		processInput();
		
		Vector3 e;
		
		if(Input.GetMouseButtonDown(0)) {
			s = GetNewPoint();
		}
		
		if(Input.GetMouseButton(0)) {
			e = GetNewPoint();
			lp = AddLine(lp, s, e, true);
		}

		if(Input.GetMouseButtonUp(0)) {
			e = GetNewPoint();
			lp = AddLine(lp, s, e, false);
		}
	}
	
	Vector3[] AddLine(Vector3[] l, Vector3 s, Vector3 e, bool tmp) {
		int vl = l.Length;
		if(!tmp || vl == 0) l = resizeVertices(l, 2);
		else vl -= 2;
			
		l[vl] = s;
		l[vl+1] = e;
		return l;
	}
	
	Vector3[] resizeVertices(Vector3[] ovs, int ns) {
		Vector3[] nvs = new Vector3[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}
	
	Vector3 GetNewPoint() {
		return g.transform.InverseTransformPoint(
			Camera.main.ScreenToWorldPoint(
				new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1.0f)
			)
		);
	}

	void OnPostRender() {
		m.SetPass(0);
		GL.PushMatrix();
		GL.MultMatrix(g.transform.transform.localToWorldMatrix);
		GL.Begin( GL.LINES );
		GL.Color( new Color(0,0,0,0.4f) );
		
		for(int i = 0; i < lp.Length; i++) {
			GL.Vertex3(lp[i].x, lp[i].y, lp[i].z);
		}
		
		GL.Color( new Color(0,0,0,0.1f) );
		
		for(int i = 0; i < sp.Length; i++) {
			GL.Vertex3(sp[i].x, sp[i].y, sp[i].z);
		}
		
		GL.End();
		GL.PopMatrix();
	} 
	
	void OnGUI() {
		GUI.Label (new Rect (10, 10, 300, 24), "GL. Cursor keys to rotate (with Shift for slow)", labelStyle);
		int vc = lp.Length + sp.Length;
		GUI.Label (new Rect (10, 26, 300, 24), "Pushing " + vc + " vertices. 'C' to clear", labelStyle);
		
		GUI.Label (new Rect (10, Screen.height - 20, 250, 24), ".Inspired by a demo from ", labelStyle);
		if(GUI.Button (new Rect (150, Screen.height - 20, 300, 24), "mrdoob", linkStyle)) {
			Application.OpenURL("http://mrdoob.com/lab/javascript/harmony/");
		}
	}

}
