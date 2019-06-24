using System.Collections.Generic;

namespace KindBot.Modules.Common
{
    public class ModulesController
    {
        public int Count => modules.Count;
        private readonly List<Module> modules = new List<Module>();
        private int nextModule = 0;

        public void LoadModules(List<Module> list)
        {
            foreach(Module module in list)
            {
                modules.Add(module);
            }
        }

        public void RunNextModule()
        {
            while(true)
            {
                if(nextModule >= modules.Count) nextModule = 0;
                Module mod = modules[nextModule];
                nextModule++;

                if(!mod.Enabled)
                {
                    //ConsoleEx.WriteDebug(mod.ModuleName + " is turned off");
                    continue;
                }
                mod.Run();
                //ConsoleEx.WriteDebug($"Currently running module: {mod.ModuleName}");
                return;
            }
        }
    }
}