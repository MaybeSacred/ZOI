using UnityEngine;

public class WheelEnemy : BaseEnemy {
	public Material[] armMaterial;
	public float duration;
	public GameObject GraphicalObject;
	private WheelCollider wheel;
	public float lookAheadTime, rotationDelta, movementSpeed;
	
	public int side;
	public float lifetime;
	protected float lifetimeTimer;
	public ParticleSystem explosionPS;
	
	public float shieldDamage;
	public float healthDamage;
	public float timeOutCounter;
	public float explosionDuration;
	protected float endTime;
	
	// Use this for initialization
	void Start () {
		//		armMaterial[0].mainTextureOffset = new Vector2(0, .5f);
		//		armMaterial[1].mainTextureOffset = new Vector2(0, .25f);
		//		armMaterial[2].mainTextureOffset = Vector2.zero;
		wheel = gameObject.GetComponentInChildren<WheelCollider> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		//		float lerp = Mathf.PingPong(Time.time, duration) / duration;
		//		armMaterial[2].color = new Color(armMaterial[2].color.r, armMaterial[2].color.g, armMaterial[2].color.b, 1-lerp);
		//		for(int i = 0; i < armMaterial.Length; i++)
		//		{
		//			armMaterial[i].mainTextureOffset += new Vector2(0, Time.deltaTime);
		//		}
		
		//handles movement after the player
		Vector3 distance = Util.player.transform.position + Util.player.rigidbody.velocity*lookAheadTime;
		//lock x and z axis
		distance.z = 0;
		distance.y = 0;
		transform.position = Vector3.Lerp(transform.position,Util.player.transform.position, .5f-.5f*Mathf.Cos(movementSpeed));
		//this rotation works more consistently
		transform.LookAt (Util.player.transform.position);
		//		transform.rotation =  Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(distance), rotationDelta*Time.deltaTime);
		
		
		//change to if(isAwake) later
		//handles rotating the spinning object
		if (true) {
			GraphicalObject.transform.RotateAround(GraphicalObject.transform.position,transform.TransformDirection(Vector3.right),wheel.rpm);
			//GraphicalObject.transform.RotateAround(GraphicalObject.transform.up,Input.GetAxis("Horizontal"));
			//GraphicalObject.transform.rotation =  Quaternion.Lerp(GraphicalObject.transform.rotation,Quaternion.Euler(GraphicalObject.transform.rotation.x,0, 270),Time.deltaTime*5f);
			
		}
	}
	
	void OnCollisionEnter(Collision other)
	{
		print("collision1");
		if(other.collider.tag.Equals("Player")){
			ParticleSystem ps = Instantiate(explosionPS, transform.position - rigidbody.velocity*Time.fixedDeltaTime, transform.rotation) as ParticleSystem;
			
			timeOutCounter += Time.deltaTime;
			
			if(GetComponentInChildren<MeshRenderer>() != null)
			{
				GetComponentInChildren<MeshRenderer>().enabled = false;
			}
			Destroy(gameObject);
			ps.GetComponent<BasicExplosion>().explosionDuration = explosionDuration;
			ps.GetComponent<BasicExplosion>().shieldDamage = shieldDamage;
			ps.GetComponent<BasicExplosion>().healthDamage = healthDamage;
			ps.GetComponent<BasicExplosion>().side = side;
		}
	}
}
