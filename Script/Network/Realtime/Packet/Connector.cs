﻿using System;
using System.Net;
using System.Net.Sockets;

using FrameWork.Network;

    public class Connector
    {
        Func<Session> _sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _sessionFactory = sessionFactory;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;

                RegisterConnect(args);
            }
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
            {
				RealtimeExceptionHandler.AddSocketError(args.SocketError);
                return;
            }

            bool pending = socket.ConnectAsync(args);
            if (pending == false)
                OnConnectCompleted(null, args);
        }

        // 서버 연결 등록이 완료되면 실행
        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == System.Net.Sockets.SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
				RealtimeExceptionHandler.AddSocketError(args.SocketError);
            }
        }
    }
