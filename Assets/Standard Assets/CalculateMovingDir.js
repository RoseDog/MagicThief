#pragma strict

static function  NearestPointFactor (lineStart : Vector3,  lineEnd : Vector3,  point : Vector3) : float
{
	    	var lineDirection = lineEnd-lineStart;
var magn = lineDirection.magnitude;
lineDirection /= magn;
	        
var closestPoint = Vector3.Dot((point-lineStart),lineDirection); //Vector3.Dot(lineDirection,lineDirection);
return closestPoint / magn;
}

static function Calculate ( p : Vector3,  a : Vector3,  b : Vector3, forwardLook : float) : Vector3
    {
		a.z = p.z;
b.z = p.z;
		
var magn = (a-b).magnitude;
if (magn == 0) return a;
		
var closest = Mathf.Clamp01 (NearestPointFactor (a, b, p));
var point = (b-a)*closest + a;
var distance = (point-p).magnitude;
		
var lookAhead = Mathf.Clamp (forwardLook - distance, 0, forwardLook);
		
var offset = lookAhead / magn;
offset = Mathf.Clamp (offset+closest,0.0,1.0);
return (b-a)*offset + a;
}