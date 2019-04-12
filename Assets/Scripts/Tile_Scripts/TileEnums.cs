using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace TileEnums {

	public enum TYPE
	{
		floor,
		wall,
		front_wall,
		breakable_wall,
		stair_up,
		stair_down
	}

	public enum CONTAINED
	{
		item,
		door,
		furniture,
		survivor,
		none,
        safepoint,
    }

	public static class EnumExtensions
	{
		public static Object GetPrefab(this CONTAINED contained)
		{
			return Resources.Load("Prefabs/Containeds/" + contained.ToString());
		}

		public static bool IsWall(this TYPE type) { return type == TYPE.wall || type == TYPE.breakable_wall; }

		public static bool IsFloor(this TYPE type) { return type == TYPE.floor; }

		public static bool IsStair(this TYPE type) { return type == TYPE.stair_up || type == TYPE.stair_down; }

		public static bool IsStairUp(this TYPE type) { return type == TYPE.stair_up; }

		public static bool IsStairDown(this TYPE type) { return type == TYPE.stair_down; }
		
        public static bool IsFlammable(this TYPE type) { return type == TYPE.floor; }

        public static bool IsFlammable(this CONTAINED contained) { return contained != CONTAINED.door && contained != CONTAINED.safepoint && contained != CONTAINED.none; }

		public static bool ContainsNone(this CONTAINED contained) { return contained == CONTAINED.none; }

        public static bool IsSurvivor(this CONTAINED contained) { return contained == CONTAINED.survivor;  }
	}

	public static class TileUtils
	{
		public static GameObject GetTile()
		{
			return Resources.Load("Prefabs/Tile") as GameObject;;
		}
	}
}
