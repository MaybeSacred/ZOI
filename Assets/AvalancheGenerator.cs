using UnityEngine;
using System.Collections;

public class AvalancheGenerator : MonoBehaviour {
	public Transform rock;
	public float startSize;
	public float sizeRange;
	public float spawnRate;
	private float spawnTimer;
	private int spawnCount;
	public int totalToSpawn;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(spawnCount < totalToSpawn)
		{
			if(spawnTimer > spawnRate)
			{
				Transform temp = Instantiate(rock, transform.position, Quaternion.identity) as Transform;
				temp.localScale = new Vector3(Random.Range(startSize-sizeRange, startSize+sizeRange), Random.Range(startSize-sizeRange, startSize+sizeRange), Random.Range(startSize-sizeRange, startSize+sizeRange));
				temp.rigidbody.mass = temp.localScale.magnitude*rock.rigidbody.mass;
				spawnTimer -= spawnRate;
				spawnCount++;
			}
			spawnTimer += Time.deltaTime;
		}
	}
}
