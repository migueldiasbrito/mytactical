using System;

namespace MyTactial.Model
{
    [Serializable]
    public class Team
    {
        public Unit[] units;

		public Team(int totalUnits)
		{
			units = new Unit[totalUnits];
		}
	}
}
