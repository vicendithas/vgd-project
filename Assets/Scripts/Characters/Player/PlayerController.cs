using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float speed;
	public float notblockrotate;

	private float blockrotate;
	private float blockmove;

	private string L_XAxisname;
	private string L_YAxisname;
	private string R_XAxisname;
	private string R_YAxisname;
	private string Triggers_name;
	private string DPad_YAxis_name;

	private Animator myAnim;
	private float rotatespeed;
	private float speedmod;

	public float spellTimeStamp;

	// Use this for initialization
	void Start () {

		myAnim = GetComponent<Animator> ();
		myAnim.SetBool("blocking", false);
		myAnim.SetBool ("attacking", false);

		GetComponent<Rigidbody>().freezeRotation = true;
	
		spellTimeStamp = Time.time;
	
	}

	void Update(){

		blockrotate = GetComponent<PlayerStats> ().currentShield.GetComponent<ShieldStats> ().turnspeed;
		blockmove = GetComponent<PlayerStats> ().currentShield.GetComponent<ShieldStats> ().movespeed;

		float triggerval = Input.GetAxis (MyInput.Triggers_name);

		if(triggerval < -0.9 || Input.GetKey (KeyCode.Space)){
			myAnim.SetBool("attacking", true);
		} else {
			myAnim.SetBool("attacking", false);
		}

		if(triggerval > 0.9 || Input.GetKey (KeyCode.LeftShift)){
			myAnim.SetBool("blocking", true);
		} else {
			myAnim.SetBool("blocking", false);
		}

		// shoot spell
		if (transform.GetComponent<PlayerStats>().currentSpell != null && (Input.GetButtonDown(MyInput.RB_name) || Input.GetKeyDown(KeyCode.E))
		    && spellTimeStamp < Time.time)
		{
			GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Items/SpellProjectile"));
			obj.transform.position = transform.position + new Vector3(0f, 1f, 0f) + transform.forward;
            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule psmain = ps.main;
			psmain.startColor = transform.GetComponent<PlayerStats>().currentSpell.GetComponent<ParticleSystem>().GetComponent<Renderer>().material.color;
			obj.GetComponent<Rigidbody>().velocity = 3f * transform.forward;
			
			SpellStats spell = transform.GetComponent<PlayerStats>().currentSpell.GetComponent<SpellStats>();
			spellTimeStamp = Time.time + spell.cooldown;
		}



	}
	
	// Update is called once per frame
	void FixedUpdate () {

		turnPlayer ();

		movePlayer ();

	}

	void turnPlayer(){

		if(transform.position.y < 0){
			Vector3 temp = transform.position;
			temp.y = 0;
			transform.position = temp;
		}

		if(myAnim.GetCurrentAnimatorStateInfo(0).IsName("Blocking")){
			rotatespeed = blockrotate;
			speedmod = speed * blockmove;
		} else {
			rotatespeed = notblockrotate;
			speedmod = speed;
		}
			
		//Turn the player based on the right stick or mouse movement
		float xlook = Input.GetAxis (MyInput.R_XAxisname);
		float zlook = -1 * Input.GetAxis (MyInput.R_YAxisname);

		// alternate, non-Xbox controller controls
		// alternate, non-Xbox controller controls
		if (Input.GetKey (KeyCode.LeftArrow))
			xlook = -1.0f;
		else if (Input.GetKey (KeyCode.RightArrow))
			xlook = 1.0f;
		
		// alternate, non-Xbox controller controls
		if (Input.GetKey (KeyCode.UpArrow))
			zlook = 1.0f;
		else if (Input.GetKey(KeyCode.DownArrow))
			zlook = -1.0f;

		float newangle = -1 * (Mathf.Atan2 (zlook, xlook) * Mathf.Rad2Deg - 90);
		float currangle =  transform.eulerAngles.y - 45f;

		//First make sure both angles are positive by adding 360
		newangle += 360;
		currangle += 360;

		//Next take the modulus with 360 to get a result between 0 and 360
		newangle %= 360;
		currangle %= 360;

		//Lastly add 360 again to get a result between 360 and 720 so we can subtract
		newangle += 360;
		currangle += 360;


		float difference = newangle - currangle;
		float direction = Mathf.Sign (difference);

		if(Mathf.Abs (difference) > 180){
			direction *= -1;
		}

		//Only rotate if the stick is moved and there is a angle difference > 5
		if(((Mathf.Abs (xlook) > 0.5f) || (Mathf.Abs (zlook) > 0.5f)) && (Mathf.Abs (difference) > 5f)){
			transform.Rotate(0f, rotatespeed * direction * Time.deltaTime, 0f);
		}


	}

	void movePlayer(){		
		//Moving the player based on the left stick
		float xinput = Input.GetAxis (MyInput.L_XAxisname);
		float zinput = -1 * Input.GetAxis (MyInput.L_YAxisname);
		
		// alternate, non-Xbox controller controls
		if (Input.GetKey (KeyCode.W))
			zinput = 1.0f;
		else if (Input.GetKey (KeyCode.S))
			zinput = -1.0f;
		
		// alternate, non-Xbox controller controls
		if (Input.GetKey (KeyCode.A))
			xinput = -1.0f;
		else if (Input.GetKey(KeyCode.D))
			xinput = 1.0f;
		
		if (Mathf.Abs(zinput) == Mathf.Abs(xinput))	{
			zinput *= 0.75f;
			xinput *= 0.75f;
		}
		
		Vector3 newmotion = Quaternion.Euler(0, 45, 0) * new Vector3 (xinput, 0f, zinput);
		
		GetComponent<Rigidbody>().MovePosition (transform.position + (speedmod * newmotion * Time.deltaTime));

		if(Mathf.Abs (xinput) > 0.1f || Mathf.Abs (zinput) > 0.1f){
			myAnim.SetBool("walking", true);
		} else {
			myAnim.SetBool("walking", false);
		}

	}
}
