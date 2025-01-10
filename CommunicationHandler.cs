﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ThServer
{
    internal class CommunicationHandler
    {
        GamesPool gamesPool;
        private UdpClient _udpServer;
        public CommunicationHandler(UdpClient udpServer)
        {
            _udpServer = udpServer;
            gamesPool = new GamesPool();
        }

        public void SendResponse(string response, IPEndPoint clientEndpoint)
        {
            var bytes = Encoding.ASCII.GetBytes(response);
            _udpServer.Send(bytes, bytes.Length, clientEndpoint);
        }

        public async Task SendResponseAsync(string response, IPEndPoint clientEndpoint)
        {
            var bytes = Encoding.ASCII.GetBytes(response);
            await _udpServer.SendAsync(bytes, bytes.Length, clientEndpoint);
        }

        public async Task SendResponseBytesAsync(byte[] bytes, IPEndPoint clientEndpoint)
        {
            await _udpServer.SendAsync(bytes, bytes.Length, clientEndpoint);
        }

        public string ReciveRequest(IPEndPoint clientEndpoint)
        {
            var request = _udpServer.Receive(ref clientEndpoint);
            return Encoding.ASCII.GetString(request);
        }

        public async Task<Tuple<string, IPEndPoint>> ReciveRequestAsync()
        {
            var request = await _udpServer.ReceiveAsync();
            return Tuple.Create(Encoding.ASCII.GetString(request.Buffer), request.RemoteEndPoint);
        }

        public async Task HandleRequest(string request, IPEndPoint clientEndpoint)
        {
            var requestSplit = request.Split(' ');
            if (requestSplit[0] != "TH") return;
            var gid = int.Parse(requestSplit[1]);
            var pid = int.Parse(requestSplit[2]);
            string prop = "";
            var command = requestSplit[3];
            if (requestSplit.Length > 4)
            {
                prop = requestSplit[4];
            }

            switch (command)
            {
                case "hello":
                    await SendResponseAsync("Threasure hunters sever is ready!", clientEndpoint);
                    break;
                case "start":
                    int newGid = gamesPool.AddSoloGame();
                    gamesPool.AddClient(clientEndpoint, newGid);
                    byte[] response = gamesPool.SoloGames[newGid].GetInitResponse();
                    await SendResponseBytesAsync(response, clientEndpoint);
                    Task.Run(async () => gamesPool.SoloGames[newGid].StartGameLoop(this, gamesPool));
                    break;
                case "go":
                    switch (prop)
                    {
                        case "u":
                            gamesPool.SoloGames[gid].Player.SetDirection(Directions.Up);
                            break;
                        case "d":
                            gamesPool.SoloGames[gid].Player.SetDirection(Directions.Down);
                            break;
                        case "l":
                            gamesPool.SoloGames[gid].Player.SetDirection(Directions.Left);
                            break;
                        case "r":
                            gamesPool.SoloGames[gid].Player.SetDirection(Directions.Right);
                            break;
                    }
                    break;
            }
        }
    }
}