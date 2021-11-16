using System;
using LUD.Authenticators;
using LUD.Core;
using LUD.Misc;

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine(Title.Logo);
Console.ForegroundColor = ConsoleColor.White;

Server _masterServer;

_masterServer = new Server(new RegionAuthenticator());

AppDomain.CurrentDomain.ProcessExit += OnAppClose;

Console.ReadLine();


void OnAppClose(object? sender, EventArgs eventArgs)
{
    _masterServer.Shutdown();
}
