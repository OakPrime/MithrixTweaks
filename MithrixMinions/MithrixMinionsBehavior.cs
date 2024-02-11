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
            for (int i = 0; i < minions.Count; i++)
            {
                minions[i].healthComponent.health = 0.0f;
            }
            minions.Clear();
        }

        public int MinionCount()
        {
            minions.RemoveAll(minion => minion == null);
            return minions.Count;
        }

        public void AddMinion(CharacterBody minion)
        {
            minions.Add(minion);
        }
    }
}
