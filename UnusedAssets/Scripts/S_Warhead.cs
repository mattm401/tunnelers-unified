using UnityEngine;

[RequireComponent (typeof (NetworkView))]

public class S_Warhead : MonoBehaviour {

	public bool left;
	public bool onLeftDown;
	public bool onLeftUp;
	
	bool previousLeft;
	
	public bool middle;
	public bool onMiddleDown;
	public bool onMiddleUp;
	
	bool previousMiddle;
	
	public bool right;	
	public bool onRightDown;
	public bool onRightUp;
	
	bool previousRight;
	
	/// <summary>
	/// Updates the client mouse motion.
	/// </summary>
	/// <param name="l">The left button.</param>
	/// <param name="m">The middle button.</param>
	/// <param name="r">The right button.</param>
	[RPC]
	public void UpdateClientMouse (bool l, bool m, bool r) {
	
		left = l;
		middle = m;
		right = r;
	
	}
	
	void Awake () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
	}
	
	/// <summary>
	/// Updates onButtonDown & onButtonUp events.
	/// </summary>
	void Update () {
	
		if (!Network.isClient && (!Network.isServer && !Network.isClient)) {
			enabled = false;
			return;
		}
		
		if (left && left != previousLeft) {
			onLeftDown = true;
			onLeftUp = false;
//			Debug.Log ("MouseDown");
		} else if (!left && left != previousLeft) {
			onLeftDown = false;
			onLeftUp = true;
//			Debug.Log ("Mouse Up");
		} else {
			onLeftDown = false;
			onLeftUp = false;
//			Debug.Log ("Neither.");
		}
		
		if (middle && middle != previousMiddle) {
			onMiddleDown = true;
			onMiddleUp = false;
		} else if (!middle && middle != previousMiddle) {
			onMiddleDown = false;
			onMiddleUp = true;
		} else {
			onMiddleDown = false;
			onMiddleUp = false;
		}
		
		if (right && right != previousRight) {
			onRightDown = true;
			onRightUp = false;
		} else if (!right && right != previousRight) {
			onRightDown = false;
			onRightUp = true;
		} else {
			onRightDown = false;
			onRightUp = false;
		}
		
		previousLeft = left;
		previousMiddle = middle;
		previousRight = right;
		
	}

}

