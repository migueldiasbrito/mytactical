using System;
using UnityEngine;

namespace mdb.MyTactial.Model
{
	[Serializable]
	public class Team
	{
		public string Name { get { return _name; } }
		public bool IsAIControlled { get { return _isAIControlled; } }
		public Unit[] Units { get { return _units; } set { _units = value; } }

		[SerializeField]
		private string _name;
		[SerializeField]
		private bool _isAIControlled;
		[SerializeField]
		private Unit[] _units;

		public Team(string name, bool isAIControlled)
		{
			_name = name;
			_isAIControlled = isAIControlled;
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
