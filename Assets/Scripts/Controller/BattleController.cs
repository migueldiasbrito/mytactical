using UnityEngine;

namespace MyTactial.Controller
{
    public class BattleController : MonoBehaviour
    {
        public static BattleController instance;

        public int[] TeamsSize;

        public Model.Battle Battle;

        private void Awake()
        {
            instance = this;

            Battle.teams = new Model.Team[TeamsSize.Length];

            for(int teamIndex = 0; teamIndex < Battle.teams.Length; teamIndex++)
            {
                Battle.teams[teamIndex] = new Model.Team(TeamsSize[teamIndex]);
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}