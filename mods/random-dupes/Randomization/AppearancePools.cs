using System.Collections.Generic;
using Database;
using UnityEngine;

namespace RandomDupes.Randomization
{
    internal static class AppearancePools
    {
        private static readonly Dictionary<Tag, List<Personality>> Pools =
            new Dictionary<Tag, List<Personality>>();

        internal static void Rebuild()
        {
            Pools.Clear();

            Db db = Db.Get();
            if (db?.Personalities == null)
                return;

            List<Personality> personalities = db.Personalities.GetAll(true, false);
            foreach (Personality personality in personalities)
            {
                if (personality == null || !personality.model.IsValid)
                    continue;

                if (!Pools.TryGetValue(personality.model, out List<Personality> pool))
                {
                    pool = new List<Personality>();
                    Pools.Add(personality.model, pool);
                }

                pool.Add(personality);
            }

            Debug.Log($"[RandomDupes] Built {Pools.Count} model-specific appearance pools from {personalities.Count} enabled personalities.");
        }

        internal static bool TryGet(Tag model, out List<Personality> personalities)
        {
            if (Pools.Count == 0)
                Rebuild();

            return Pools.TryGetValue(model, out personalities) && personalities.Count > 0;
        }
    }
}
