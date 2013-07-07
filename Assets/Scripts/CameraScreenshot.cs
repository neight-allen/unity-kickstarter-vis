// ******  Notice : It doesn't works in Wep Player environment.  ******
// ******    It works in PC environment.                         ******
// Default method have some problem, when you take a Screen shot for your game. 
// So add this script.
// CF Page : http://technology.blurst.com/unity-jpg-encoding-javascript/
// made by Jerry ( sdragoon@nate.com )
 
using UnityEngine;
using System.Collections;
using System.IO;
 
public class CameraScreenshot : MonoBehaviour
{
    private int count = 0;
 
    void Update()
    {
        if (Input.GetKeyDown("k"))
            StartCoroutine(ScreenshotEncode());
    }
 
    IEnumerator ScreenshotEncode()
    {
        // wait for graphics to render
        yield return new WaitForEndOfFrame();
 
        // create a texture to pass to encoding
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
 
        // put buffer into texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
 
        // split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
        yield return 0;
 
        byte[] bytes = texture.EncodeToPNG();
 
        // save our test image (could also upload to WWW)
		string filePath = Application.dataPath + "/screenshots/" + KSMetric.List[GraphAxis.me.xLabel].Name + "_" 
				+ KSMetric.List[GraphAxis.me.yLabel].Name + "/"
				+ GraphAxis.me.legendTitles[GraphAxis.me.selectedLegend] + ".png";
		FileInfo file = new System.IO.FileInfo(filePath);
		file.Directory.Create(); // If the directory already exists, this method does nothing.
		File.WriteAllBytes(file.FullName, bytes);
        //File.WriteAllBytes(filePath, bytes);
		//File.WriteAllBytes(Application.dataPath + "/../testscreen-" + count + ".png", bytes);
        count++;
 
        // Added by Karl. - Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
        DestroyObject( texture );
 
        //Debug.Log( Application.dataPath + "/../testscreen-" + count + ".png" );
    }
}