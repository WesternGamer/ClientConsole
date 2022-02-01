using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using HarmonyLib;
using Microsoft.Win32.SafeHandles;
using Shared.Logging;
using VRage.Plugins;
using VRage.Utils;

namespace ClientPlugin
{
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin
    {
        public const string Name = "ClientConsole";
        public static readonly IPluginLogger Log = new KeenPluginLogger(Name);
        public static Plugin Instance;

        private static readonly Harmony Harmony = new Harmony(Name);

        private static readonly object InitializationMutex = new object();
        private static bool initialized;
        private static bool failed;

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            Instance = this;

            Log.Info("Loading");

            Log.Debug("Patching");
            try
            {
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Log.Critical(ex, "Patching failed");
                return;
            }

            Log.Debug("Successfully loaded");
        }

        public void Dispose()
        {
            try
            {
                FreeConsole();
            }
            catch (Exception ex)
            {
                Log.Critical(ex, "Dispose failed");
            }

            Instance = null;
        }

        public void Update()
        {
            EnsureInitialized();
            try
            {
                if (!failed)
                    CustomUpdate();
            }
            catch (Exception ex)
            {
                Log.Critical(ex, "Update failed");
                failed = true;
            }
        }

        private void EnsureInitialized()
        {
            lock (InitializationMutex)
            {
                if (initialized || failed)
                    return;

                Log.Info("Initializing");
                try
                {
                    Initialize();
                }
                catch (Exception ex)
                {
                    Log.Critical(ex, "Failed to initialize plugin");
                    failed = true;
                    return;
                }

                Log.Debug("Successfully initialized");
                initialized = true;
            }
        }

        private void Initialize()
        {
            AllocConsole();

            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput(), Encoding.GetEncoding(437));
            standardOutput.AutoFlush = false;
            Console.SetOut(standardOutput);

            //Console.Out.WriteLine($"[{Name}]: Space Engineers console loaded.");
            Log.Info("Space Engineers console loaded.");

        }

        private void CustomUpdate()
        {
            // TODO: Put your update code here. It is called on every simulation frame!
        }
    }
}
