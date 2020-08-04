using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
	[Serializable]
	public class Team
	{
		public string Name { get { return _name; } }
		public Unit[] Units { get { return _units; } set { _units = value; } }

		[SerializeField]
		private string _name;
		[SerializeField]
		private Unit[] _units;

		public Team(string name)
		{
			_name = name;
		}

		public bool HasLiveUnits()
		{
			foreach (Unit unit in Units)
			{
				if (unit.GetState() != Unit.State.Dead)
				{
					return true;
				}
			}

			return false;
		}
	}
}
