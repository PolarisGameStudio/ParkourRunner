using UnityEngine;

namespace BotWaypoints {
	public class Waypoint : MonoBehaviour {
		public Type WaypointType;
		public bool CanFall;

		public enum Type {
			Moving,
			Jump,
			SmallPlatformJump,
			MiddlePlatformJump,
			HighPlatformJump,
			Roll,
			BigJump,
		}
	}
}