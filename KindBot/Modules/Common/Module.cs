namespace KindBot.Modules.Common
{
    public abstract class Module
    {
        public string ModuleName { get; protected set; }
        public bool Enabled { get; protected set; } = true;

        public abstract void Run();
    }
}