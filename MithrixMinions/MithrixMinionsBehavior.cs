using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace MithrixMinions
{
    internal class MithrixMinionsBehavior : MonoBehaviour
    {
        private List<CharacterBody> minions = new List<CharacterBody>();

        private void OnDisable()
        {
            this.KillMinions();
        }

        public void AddMinion(CharacterBody minion)
        {
            minions.Add(minion);
        }

        public void KillMinions()
        {
            for (int i = 0; i < minions.Count; i++)
            {
                minions[i].healthComponent.health = 0.0f;
            }
            minions.Clear();
        }
    }
}
