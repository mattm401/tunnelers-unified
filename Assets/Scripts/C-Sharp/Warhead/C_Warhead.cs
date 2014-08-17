﻿//
//  C_Warhead.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/Tunnelers-Unified/>.
//
//  Copyright (c) 2014 Juraj Fiala<doctorjellyface@riseup.net>
//
//  Tunnelers: Unified is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Tunnelers: Unified is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Tunnelers: Unified.  If not, see <http://www.gnu.org/licenses/>.
//

using UnityEngine;

[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (S_Warhead))]

[AddComponentMenu ("Network/Warhead")]

public class C_Warhead : MonoBehaviour {

	public PlayerMan parent;

	bool lastLeft;
	
	void Awake () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
	}
	
	void OnConnectedToServer () {
	
		enabled = true;
		
	}
	
	void Update () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		if (parent.GetOwner () == null || parent.GetOwner () != Network.player) {
			return;
		}
	
		lastLeft = Input.GetMouseButton (0);

		networkView.RPC ("UpdateClientMouse", RPCMode.Server, lastLeft);
	
	}
}
