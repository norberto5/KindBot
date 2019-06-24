using System;
using System.Collections.Generic;
using System.IO;
using KindBot.Common;
using KindBot.Objects;
using KindBot.Tools;
using MoonSharp.Interpreter;

namespace KindBot.Lua
{
    public class LuaController
    {
        public LuaEvents Events = new LuaEvents();
        private List<Script> scripts = new List<Script>();

        private Script ConsoleScript
        {
            get
            {
                if(consoleScript == null)
                {
                    consoleScript = InitializeScript();
                }
                return consoleScript;
            }
        }
        private Script consoleScript;

        public void InitializeLua()
        {
            scripts = new List<Script>();
            ConsoleEx.WriteLine("Loading lua scripts...");
            UserData.RegisterAssembly();
            LoadLuaScripts();
            RunMainFunctions();
        }

        public void RunConsoleScript(string scriptString)
        {
            try
            {
                ConsoleScript.DoString(scriptString);
            }
            catch
            {
                try
                {
                    string printScriptResult = $"print({scriptString})";
                    ConsoleScript.DoString(printScriptResult);
                }
                catch(Exception ex)
                {
                    CatchLuaException(ConsoleScript, ex);
                }
                return;
            }
        }

        private Script LoadLuaScript(string filename, bool lineScript = false)
        {
            if(!lineScript && !File.Exists($"{filename}")) return null;

            Script script = InitializeScript();
            try
            {
                script.Globals["FILENAME"] = filename;
                script.DoFile($"{filename}");
                return script;
            }
            catch(Exception ex)
            {
                CatchLuaException(script, ex);
                return null;
            }
        }

        private Script InitializeScript()
        {
            var script = new Script(CoreModules.Preset_SoftSandbox | CoreModules.LoadMethods);
            try
            {
                Script.DefaultOptions.DebugPrint = s => ConsoleEx.WriteLine(s);

                Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<DateTime>((s, v) => DynValue.NewString(v.ToString())); // Convert DateTime to Lua string
                Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<TimeSpan>((s, v) => DynValue.NewNumber(v.TotalSeconds)); // Convert TimeSpan to Lua number (in seconds)

                script.Globals["console"] = typeof(ConsoleEx);
                script.Globals["log"] = typeof(Logs);
                script.Globals["commands"] = typeof(Commands);

                script.Globals["user"] = typeof(User);
                script.Globals["channel"] = typeof(Channel);
                script.Globals["channelFactory"] = typeof(ChannelFactory);
                script.Globals["serverInfo"] = ServerInfo.Instance;

                script.Globals["event"] = Events;

                return script;
            }
            catch(Exception ex)
            {
                CatchLuaException(script, ex);
                return null;
            }
        }

        private void LoadLuaScripts()
        {
            try
            {
                Directory.CreateDirectory("scripts");
                string[] files = Directory.GetFiles("scripts", "*.lua");
                int n = 0;
                foreach(string s in files)
                {
                    ConsoleEx.WriteLine($"Loading lua script - {s}");
                    Script script = LoadLuaScript(s);
                    if(script != null)
                    {
                        scripts.Add(script);
                        n++;
                    }
                }
                ConsoleEx.WriteLine($"Sucessfully loaded {n} lua scripts.");
            }
            catch(Exception)
            {
                ConsoleEx.Error($"There was an error while loading lua scripts.");
            }
        }

        private void RunMainFunctions()
        {
            foreach(Script s in scripts)
            {
                try
                {
                    DynValue luaMainFunction = s.Globals.Get("main");
                    if(luaMainFunction != DynValue.Nil) s.Call(luaMainFunction);
                }
                catch(Exception ex)
                {
                    CatchLuaException(s, ex);
                    return;
                }
            }
        }

        private void CatchLuaException(Script script, Exception exception)
        {
            string filename = script.Globals.Get("FILENAME").ToString();
            if(exception is ScriptRuntimeException screx)
            {
                ConsoleEx.Error($"Oh, there is a script runtime error in {filename} script. Error: {screx.DecoratedMessage}");
            }

            else if(exception is SyntaxErrorException synex)
            {
                ConsoleEx.Error($"Oh, there is a syntax error in {filename} script. Error: {synex.DecoratedMessage}");
            }
            else if(exception is InternalErrorException intex)
            {
                ConsoleEx.Error($"Oh, there is an internal error in {filename} script. Error: {intex.DecoratedMessage}");
            }
            else
                ConsoleEx.Error($"Oh, there is an error in {filename} script. Error: {exception.Message}");
        }
    }
}
