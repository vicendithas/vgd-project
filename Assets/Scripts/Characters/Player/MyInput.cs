using UnityEngine;
using System.Collections;

public class MyInput : MonoBehaviour {

	public static string L_XAxisname;
	public static string L_YAxisname;
	public static string R_XAxisname;
	public static string R_YAxisname;
	public static string Triggers_name;
	public static string DPad_XAxis_name;
	public static string DPad_YAxis_name;
	public static string A_name;
	public static string B_name;
	public static string X_name;
	public static string Y_name;
	public static string LB_name;
	public static string RB_name;
	public static string Back_name;
	public static string Start_name;
	public static string LS_name;
	public static string RS_name;

	// Use this for initialization
	void Start () {
		setInputs ();
	}

	void setInputs(){
		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor){
			L_XAxisname = "L_XAxis_Win";
			L_YAxisname = "L_YAxis_Win";
			R_XAxisname = "R_XAxis_Win";
			R_YAxisname = "R_YAxis_Win";
			Triggers_name = "Triggers_Win";
			DPad_XAxis_name = "DPad_XAxis_Win";
			DPad_YAxis_name = "DPad_YAxis_Win";
			A_name = "A_Win";
			B_name = "B_Win";
			X_name = "X_Win";
			Y_name = "Y_Win";
			LB_name = "LB_Win";
			RB_name = "RB_Win";
			Back_name = "Back_Win";
			Start_name = "Start_Win";
			LS_name = "LS_Win";
			RS_name = "RS_Win";
		}
		
		if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor){
			L_XAxisname = "L_XAxis_OSX";
			L_YAxisname = "L_YAxis_OSX";
			R_XAxisname = "R_XAxis_OSX";
			R_YAxisname = "R_YAxis_OSX";
			Triggers_name = "Triggers_OSX";
			DPad_XAxis_name = "DPad_XAxis_OSX";
			DPad_YAxis_name = "DPad_YAxis_OSX";
			A_name = "A_OSX";
			B_name = "B_OSX";
			X_name = "X_OSX";
			Y_name = "Y_OSX";
			LB_name = "LB_OSX";
			RB_name = "RB_OSX";
			Back_name = "Back_OSX";
			Start_name = "Start_OSX";
			LS_name = "LS_OSX";
			RS_name = "RS_OSX";
		}
	}
}
