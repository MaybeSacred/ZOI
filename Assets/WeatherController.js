#pragma strict
public var weather : ParticleSystem;
public var minEmission : float;
public var maxEmission : float;
public var minStart : float;
public var maxStart : float;
private var timeTilNextChange : float;
private var currentTime : float;
private var waxing : boolean;
function Start () {
	timeTilNextChange = Random.Range(minStart, maxStart);
}

function Update () {
	if(!Physics.Raycast(this.transform.position, Vector3.up, float.PositiveInfinity)){
		if(currentTime > timeTilNextChange)
		{
			waxing = !waxing;
			currentTime -= timeTilNextChange;
			timeTilNextChange = Random.Range(minStart, maxStart);
		}
		if(waxing)
		{
			weather.emissionRate = Mathf.Lerp(minEmission, maxEmission, currentTime/timeTilNextChange);
		}
		else
		{
			weather.emissionRate = Mathf.Lerp(maxEmission, minEmission, currentTime/timeTilNextChange);
		}
	}
	currentTime += Time.deltaTime;
}