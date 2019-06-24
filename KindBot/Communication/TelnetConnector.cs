// minimalistic telnet implementation
// conceived by Tom Janssens on 2007/06/06  for codeproject
// http://www.corebvba.be
// modified by norberto5 ( https://norberto5.pl ) 2017-2019
using System;
using System.Net.Sockets;
using System.Text;
using KindBot.Tools;

namespace KindBot.Communication
{
    public class TelnetConnector : IConnector
    {
        public event EventHandler<string> EventHandled;

        public static TelnetConnector Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new TelnetConnector();
                }
                return instance;
            }
        }

        public bool Connected => tcpSocket != null ? tcpSocket.Client != null && tcpSocket.Connected && !timeouted : false;

        private TcpClient tcpSocket;
        private bool timeouted;

        private readonly TimeSpan timeoutTime = TimeSpan.FromSeconds(10);
        private static TelnetConnector instance;

        private TelnetConnector() { }

        public bool Connect(string hostname, int port)
        {
            if(Connected) return false;

            timeouted = false;
            try
            {
                tcpSocket = new TcpClient()
                {
                    SendTimeout = 1000,
                    ReceiveTimeout = 1000
                };
                return tcpSocket.ConnectAsync(hostname, port).Wait(1000);
            }
            catch(Exception ex)
            {
                ConsoleEx.Error($"Couldn't connect to the telnet server. Reason: {ex.Message}");
                return false;
            }
        }

        public bool Login(LoginDetails loginDetails)
        {
            if(!Connected) return false;

            string s;
            do
            {
                s = Read();
                if(s == null)
                {
                    ConsoleEx.Error("Failed to connect : server is not responding!");
                    return false;
                }
                else if(s.EndsWith("."))
                    break;
            }
            while(true);

            s = Execute($"login {loginDetails.Username} {loginDetails.Password}", true);
            if(!s.EndsWith("msg=ok"))
            {
                ConsoleEx.Error("Failed to connect : login or password is wrong");
                return false;
            }

            s = Execute($"use {loginDetails.VirtualServerId}", true);
            if(!s.EndsWith("msg=ok"))
            {
                ConsoleEx.Error("Failed to connect : virtualserver id error");
                return false;
            }
            return true;
        }

        public void Stop()
        {
            tcpSocket?.Close();
            tcpSocket?.Dispose();
        }

        public string Execute(string cmd, bool attachError = false)
        {
            if(!Connected) return string.Empty;
            DateTime startedExecutingTime = DateTime.Now;
            if(cmd.Trim().Length < 1) return string.Empty;
            WriteLine(cmd);
            string output = string.Empty;
            string temp = string.Empty;
            while(!temp.StartsWith("error"))
            {
                if(startedExecutingTime.Add(timeoutTime) < DateTime.Now)
                {
                    timeouted = true;
                    Stop();
                    return string.Empty;
                }
                temp = Read() ?? string.Empty;
                if(!attachError && temp.StartsWith("error")) break;
                output += "\n" + temp;
            }
            return output;
        }

        private void WriteLine(string cmd) => Write(cmd + "\n");

        private void Write(string cmd)
        {
            if(!Connected) return;
            byte[] buf = Encoding.UTF8.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        private string Read()
        {
            if(!Connected) return null;
            DateTime startedReadingTime = DateTime.Now;
            var sb = new StringBuilder();
            do
            {
                if(ParseTelnet(sb)) break;
                if(startedReadingTime.Add(timeoutTime) < DateTime.Now)
                {
                    timeouted = true;
                    Stop();
                    return string.Empty;
                }
            } while(Connected);

            string str = sb.ToString();

            if(str.IndexOf("notify") == 0)
            {
                EventHandled?.Invoke(this, str);
            }

            return sb.ToString().TrimEnd();
        }

        private bool ParseTelnet(StringBuilder sb)
        {
            while(Connected && tcpSocket.Available > 0)
            {
                int input = tcpSocket.GetStream().ReadByte();
                switch(input)
                {
                    case 0xA: // CR end of line
                    case 0xD: // LF
                        sb.Append((char)input);
                        return true;
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = tcpSocket.GetStream().ReadByte();
                        if(inputverb == -1) break;
                        switch(inputverb)
                        {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = tcpSocket.GetStream().ReadByte();
                                if(inputoption == -1) break;
                                tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                                if(inputoption == (int)Options.SGA)
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                tcpSocket.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
            return false;
        }

        private enum Verbs
        {
            WILL = 251,
            WONT = 252,
            DO = 253,
            DONT = 254,
            IAC = 255
        }

        private enum Options
        {
            SGA = 3
        }
    }
}