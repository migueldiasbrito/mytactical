using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
	[Serializable]
	public class Team
	{
		public Unit[] Units { get { return _units; } set { _units = value; } }

		[SerializeField]
		private Unit[] _units;
	}
}
