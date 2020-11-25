using System.Collections.Generic;
using UnityEngine;

namespace BotWaypoints {
	public class WaypointsLine : MonoBehaviour {
		public Color GizmosColor = Color.green;
		public List<Waypoint> Waypoints = new List<Waypoint>();
		private void OnDrawGizmos() {
			Gizmos.color = GizmosColor;
			for (int i = 0; i < Waypoints.Count - 1; i++) {
				var wp = Waypoints[i];
				var wp2 = Waypoints[i + 1];

				Gizmos.DrawLine(wp.transform.position, wp2.transform.position);
			}
		}
	}
}