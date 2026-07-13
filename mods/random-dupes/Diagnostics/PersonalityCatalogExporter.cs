using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Database;
using PeterHan.PLib.Options;
using RandomDupes.Configuration;
using UnityEngine;

namespace RandomDupes.Diagnostics
{
    internal static class PersonalityCatalogExporter
    {
        internal static void Export()
        {
            Export(Db.Get());
        }

        internal static void Export(Db database)
        {
            try
            {
                if (database?.Personalities == null)
                    return;

                string configFile = POptions.GetConfigFilePath(typeof(RandomDupesOptions));
                string directory = Path.GetDirectoryName(configFile);
                Directory.CreateDirectory(directory);
                string path = Path.Combine(directory, "available-duplicants.csv");

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Id,Name,Model,RequiredDlcId,Gender,StartingMinion,HeadShape,Eyes,Mouth,Hair,Body,ArmSkin,LegSkin");
                List<Personality> personalities = database.Personalities.GetAll(false, false);
                personalities.Sort((left, right) => string.Compare(left.Id, right.Id, StringComparison.OrdinalIgnoreCase));
                foreach (Personality personality in personalities)
                {
                    csv.Append(Escape(personality.Id)).Append(',')
                        .Append(Escape(personality.Name)).Append(',')
                        .Append(Escape(personality.model.ToString())).Append(',')
                        .Append(Escape(personality.requiredDlcId)).Append(',')
                        .Append(Escape(personality.genderStringKey)).Append(',')
                        .Append(personality.startingMinion).Append(',')
                        .Append(personality.headShape).Append(',')
                        .Append(personality.eyes).Append(',')
                        .Append(personality.mouth).Append(',')
                        .Append(personality.hair).Append(',')
                        .Append(personality.body).Append(',')
                        .Append(personality.arm_skin).Append(',')
                        .Append(personality.leg_skin).AppendLine();
                }

                File.WriteAllText(path, csv.ToString(), new UTF8Encoding(true));
                Debug.Log($"[RandomDupes] Exported {personalities.Count} available personalities to '{path}'.");
            }
            catch (Exception exception)
            {
                ErrorLogger.Write("Exporting available duplicants catalog", exception);
            }
        }

        private static string Escape(string value)
        {
            return "\"" + (value ?? string.Empty).Replace("\"", "\"\"") + "\"";
        }
    }
}
