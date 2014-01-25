#pragma strict

public var theLight : Transform;
private var lights : Transform[];
private var halos : Light[];
public var numberOfLights : int;
public var offset : float;
public var startRadius : float;
public var radiusOffset : float;
public var oscillationSpeed : float;
function Start () {
	lights = new Transform[numberOfLights];
	halos = new Light[numberOfLights];
	for(var i = 0; i < numberOfLights; i++)
	{
		lights[i] = Instantiate(theLight, new Vector3(0, 0, 0), Quaternion.identity) as Transform;
		lights[i].parent = this.transform;
		lights[i].localPosition = new Vector3(0, 0, -(numberOfLights-1)*offset/2 + i*offset);
	}
	for(i = 0; i < lights.Length; i++)
	{
		halos[i] = lights[i].GetComponent(Light) as Light;
	}
}

function Update () {
	for(var i = 0; i < lights.Length; i++)
	{
		halos[i].range = radiusOffset*Mathf.Sin(oscillationSpeed*Time.timeSinceLevelLoad)+startRadius;
	}
}