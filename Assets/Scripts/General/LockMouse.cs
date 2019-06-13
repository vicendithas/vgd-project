using UnityEngine;
using System.Collections;

public class LockMouse : MonoBehaviour {

	void Update()
	{
		if( Application.platform == RuntimePlatform.OSXWebPlayer ||
			Application.platform == RuntimePlatform.WindowsWebPlayer )
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				Screen.lockCursor = false;
			else if(Input.GetKeyDown (KeyCode.Mouse0))
				Screen.lockCursor = true;
		}
	}
}
