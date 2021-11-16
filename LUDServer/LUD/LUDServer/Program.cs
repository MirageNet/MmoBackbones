using System;
using LUD.Authenticators;
using LUD.Core;

Server _masterServer;

_masterServer = new(new RegionAuthenticator());

AppDomain.CurrentDomain.ProcessExit += OnAppClose;

Console.ReadLine();


void OnAppClose(object? sender, EventArgs eventArgs)
{
    _masterServer.Shutdown();
}
