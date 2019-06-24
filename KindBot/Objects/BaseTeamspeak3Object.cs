using KindBot.Communication;
using KindBot.Tools;

namespace KindBot.Objects
{
    public abstract class BaseTeamspeak3Object
    {
        protected abstract string GetParameterCommand { get; }
        protected abstract string SetParameterCommand { get; }

        protected T GetParameter<T>(string parameter)
        {
            string output = TelnetConnector.Instance.Execute(GetParameterCommand);
            return TeamspeakTools.GetParameter<T>(output, parameter);
        }

        protected void SetParameter<T>(string parameter, T value)
        {
            if(value is bool b)
            {
                TelnetConnector.Instance.Execute($"{SetParameterCommand} {parameter}={(b ? "1" : "0")}");
            }
            else if(value is string s)
            {
                TelnetConnector.Instance.Execute($"{SetParameterCommand} {parameter}={(s ?? "").ConvertToTeamspeakString()}");
            }
            else
            {
                TelnetConnector.Instance.Execute($"{SetParameterCommand} {parameter}={value}");
            }
        }
    }
}