using System.Collections.Generic;
using Database;
using UnityEngine;

namespace RandomDupes.Randomization
{
    internal static class AppearancePools
    {
        private static readonly Dictionary<Tag, List<Personality>> AllPools =
            new Dictionary<Tag, List<Personality>>();
        private static readonly Dictionary<Tag, List<Personality>> StarterPools =
            new Dictionary<Tag, List<Personality>>();

        internal static void Rebuild(Db database)
        {
            AllPools.Clear();
            StarterPools.Clear();

            if (database?.Personalities == null)
                return;

            List<Personality> personalities = database.Personalities.GetAll(true, false);
            foreach (Personality personality in personalities)
            {
                if (personality == null || !personality.model.IsValid)
                    continue;

                if (!AllPools.TryGetValue(personality.model, out List<Personality> pool))
                {
                    pool = new List<Personality>();
                    AllPools.Add(personality.model, pool);
                }

                pool.Add(personality);
                if (personality.startingMinion)
                {
                    if (!StarterPools.TryGetValue(personality.model, out List<Personality> starterPool))
                    {
                        starterPool = new List<Personality>();
                        StarterPools.Add(personality.model, starterPool);
                    }
                    starterPool.Add(personality);
                }
            }

            Debug.Log($"[RandomDupes] Built {AllPools.Count} model-specific physical appearance pools from {personalities.Count} enabled personalities.");
        }

        internal static bool TryGet(Tag model, bool starter, out List<Personality> personalities)
        {
            if (AllPools.Count == 0)
            {
                Db database = Db.Get();
                if (database?.Personalities != null && database.AccessorySlots != null)
                    Rebuild(database);
            }

            Dictionary<Tag, List<Personality>> source = starter ? StarterPools : AllPools;
            return source.TryGetValue(model, out personalities) && personalities.Count > 0;
        }
    }
}
