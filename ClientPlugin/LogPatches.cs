using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using VRage.Utils;

namespace ClientPlugin
{
    [HarmonyPatch(typeof(MyLog))]
    internal class LogPatches
    {
		private static FieldInfo field = AccessTools.Field(typeof(MyLog), "m_stream");

        [HarmonyPostfix]
        [HarmonyPatch("Init")]
        private static void WriteString(MyLog __instance)
        {
			using (StreamReader sr = new StreamReader((Stream)field.GetValue(__instance)))
			{
				string line;
				// Read and display lines from the file until the end of
				// the file is reached.
				while ((line = sr.ReadLine()) != null)
				{
					Console.WriteLine(line);
				};
			}
		}
    }
}
