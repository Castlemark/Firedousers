﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace TileEnums {

	public enum TYPE
	{
		floor,
		wall,
		breakable_wall,
		stair_up,
		stair_down,
		safepoint
	}

	public enum CONTAINED
	{
		item,
		door,
		furniture,
		survivor,
		none
		
	}

	public static class EnumExtensions
	{
		public static Object GetPrefab(this CONTAINED contained)
		{
			return Resources.Load("Prefabs/Containeds/" + contained.ToString());
		}

		public static bool IsWall(this TYPE type) { return type == TYPE.wall || type == TYPE.breakable_wall; }

		public static bool IsStair(this TYPE type) { return type == TYPE.stair_up || type == TYPE.stair_down; }
		
		public static bool IsSafePoint(this TYPE type) { return type == TYPE.safepoint; }

        public static bool IsFlammable(this TYPE type) { return type == TYPE.floor; }

		public static bool ContainsNone(this CONTAINED contained)
		{
			return contained == CONTAINED.none;
		}
	}

	public static class TileUtils
	{
		public static GameObject GetTile()
		{
			return Resources.Load("Prefabs/Tile") as GameObject;;
		}
	}
}
