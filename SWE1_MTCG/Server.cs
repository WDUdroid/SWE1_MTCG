﻿using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SWE1_MTCG.DBHandler;
using SWE1_MTCG.REST;


namespace SWE1_MTCG
{
    public class Server
    {
        // Setting up multiple Clients, for demonstration purposes allow only one thread at a time,
        // which alleviates the need for mutex
        static readonly SemaphoreSlim ConcurrentConnections = new SemaphoreSlim(2);

        // messageData stores all messages, everything is in-memory, meaning there is no file-handling
        private static List<string> messagesData = new List<string>();

        public static Battle BattleCenter = new Battle();

        static Task Main(string[] args)
        {
            BattleCenter = new Battle();

            ConsoleOutputs.WelcomeMessage();
            Console.WriteLine("__________STARTING REST-SERVER__________");

            TcpHandler tcpHandler = null;

            var tasks = new List<Task>();

            try
            {
                tcpHandler = new TcpHandler();

                Console.WriteLine(">>REST-Server startup successful");
                Console.WriteLine("...");
                Console.WriteLine("________________________________________");
                Console.WriteLine("");
                Console.WriteLine("_________STARTING BATTLE-CENTER_________");
                Console.WriteLine("...");

                tasks.Add(Task.Run(() => BattleCenter.BattleFunc()));

                Console.WriteLine("________________________________________");
                Console.WriteLine("");
                Console.WriteLine("____SWITCHING TO CLIENT SERVICE AREA____");

                while (true)
                {
                    ConcurrentConnections.Wait();
                    tasks.Add(Task.Run(() => ClientReception(tcpHandler)));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            finally
            {
                tcpHandler?.Stop();
                Task.WaitAll(tasks.ToArray());
            }
        }

        // ClientReception is like a lobby in a hotel. Here every client checks in
        // and every client checks out
        private static void ClientReception(TcpHandler tcpHandler)
        {
            WebHandler webHandler = new WebHandler(tcpHandler, BattleCenter);
            var content = webHandler.GetHttpContent();

            Console.WriteLine("\n\n----------RECEIVED HTTP-REQUEST----------");
            Console.WriteLine(content);
            Console.WriteLine("--------RECEIVED HTTP-REQUEST END--------\n");

            webHandler.WorkHttpRequest(content, messagesData);

            Console.WriteLine("\n\n--------------SENT RESPONSE--------------");
            webHandler.SendHttpContent();
            Console.WriteLine("------------SENT RESPONSE END------------\n");

            tcpHandler.CloseClient(webHandler.Client);

            ConcurrentConnections.Release();
            Console.WriteLine(">>Client finished\n\n\n\n\n");
        }
    }
}