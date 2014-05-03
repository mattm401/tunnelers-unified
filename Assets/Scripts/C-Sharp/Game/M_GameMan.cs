using UnityEngine;

public class M_GameMan : MonoBehaviour {

	public S_GameMan observedGameMan;
	public C_GameMan recieverGameMan;

	public void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
	
		int timeToEnd = (int)observedGameMan.timeToEnd;
		
		if (stream.isWriting) {
		
		//	Debug.Log("Server is writing");
			stream.Serialize (ref timeToEnd);
		
		} else {
		
			stream.Serialize (ref timeToEnd);

			recieverGameMan.serverTimeToEnd = timeToEnd;
			
		}
	}
	
}