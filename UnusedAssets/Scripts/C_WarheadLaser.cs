using UnityEngine;
using System.Collections;

public class C_WarheadLaser : MonoBehaviour {

	public S_WarheadLaser serverScript;
	
	void Awake () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
	}

	[RPC]
	IEnumerator ShootLaser () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return false;
		}
		
		for (float i = 0.001f; i < serverScript.range; i += serverScript.laserSpeed * Time.deltaTime) {		
			
			RaycastHit hit;
			if (Physics.Raycast (serverScript.transform.position, transform.forward, out hit, i)) {
				
				if (hit.collider.tag == "Tank") {
									
					serverScript.line.SetPosition (1, serverScript.transform.localPosition + new Vector3 (0, 0, hit.distance));					
				
				} else {
					
					serverScript.line.SetPosition (1, serverScript.transform.localPosition + new Vector3 (0, 0, hit.distance));
					yield return new WaitForSeconds (serverScript.waitForSec);
					serverScript.line.SetPosition (1, serverScript.transform.localPosition);
					yield break;
					
				}
				
				
			} else {

				serverScript.line.SetPosition (1, serverScript.transform.localPosition + new Vector3 (0, 0, i));
				
			}

			yield return 0;
			
		}

		
		RaycastHit hit2;
		if (Physics.Raycast (serverScript.transform.position, transform.forward, out hit2, serverScript.range)) {
			
			if (hit2.collider.tag == "Tank") {

				serverScript.line.SetPosition (1, serverScript.transform.localPosition + new Vector3 (0, 0, hit2.distance));
				yield return new WaitForSeconds (serverScript.waitForSec);
				serverScript.line.SetPosition (1, serverScript.transform.localPosition);
				yield break;
				
			} else {
						
				serverScript.line.SetPosition (1, serverScript.transform.localPosition + new Vector3 (0, 0, hit2.distance));		
				yield return new WaitForSeconds (serverScript.waitForSec);
				serverScript.line.SetPosition (1, serverScript.transform.localPosition);
				yield break;
				
			}
			
		} else {
			
			serverScript.line.SetPosition (1, serverScript.transform.localPosition + new Vector3 (0, 0, serverScript.range));
			
			yield return new WaitForSeconds (serverScript.waitForSec);
			
			serverScript.line.SetPosition (1, serverScript.transform.localPosition);
			
			yield break;
			
		}
		
	}

}

