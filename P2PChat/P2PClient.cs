using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

/* 
 * created by Maou Lim on 17-4-10
 */

namespace P2PChat {
    /* Main Object */
    public class P2PClient {
        private static UIComponent ui = null;

        private UdpReceiver _receiver = null;
        private UdpSender _sender = null;

        private UdpClient _client = null;
        private bool _running = false;

        public P2PClient(Int16 localPort, UIComponent ui) {
            try {
                _client = new UdpClient(localPort);
                _receiver = new UdpReceiver(_client, 200);
                _sender = new UdpSender(_client);

                P2PClient.ui = ui;
            }
            catch (Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
        }

        public void Start() {
            new Thread(RunController).Start();
            Application.Run((Form) ui);
        }

        private void RunController() {
            /* start the receiving component */
            _receiver.Start();

            _running = true;

            /* start main logic */
            MainLoop();

            /* when main logic terminated, cleanup resources */
            Cleanup();
        }

        /* main logic of client controller */
        private void MainLoop() {
            while (_running) {
                /* get a command from ui */
                string command = ui.Input();

                

                IPEndPoint where = new IPEndPoint(IPAddress.Any, 0);
                string what = string.Empty;

                /* send the command to the Parser */
                Parser.CommandType type = Parser.Parse(command, ref where, ref what);

                switch (type) {
                    case Parser.CommandType.QUIT: {
                        _running = false;
                        break;
                    }
                    case Parser.CommandType.SEND: {
                        _sender.Send(where, what);
                        break;
                    }
                    default: {
                        ui.Display("Unkown or invalid command: " + command + "\r\n", true);
                        break;
                    }
                }
                
                /* display the input message */
                ui.Display("self: " + what +  "\r\n", false);
            }
        }

        /* deallocate the resources */
        private void Cleanup() {
            try {
                _receiver.Stop();
                _client.Close();
            }
            catch (Exception) {
                /* catch all exception */
            }
        }

        /* udp sending service */
        private class UdpSender {
            private readonly UdpClient _client = null;

            public UdpSender(UdpClient client) {
                Debug.Assert(null != client);
                this._client = client;
            }

            public bool Send(IPEndPoint where, String what) {
                if (null == where || null == what) {
                    /* if the destination or message is empty, abort the invoking */
                    return false;
                }

                try {
                    /* encode by utf-8 */
                    byte[] buff = Encoding.UTF8.GetBytes(what);
                    _client.Send(buff, buff.Length, where);
                    return true;
                }
                catch (Exception) {
                    /* any exception return false */
                    return false;
                }
            }
        }

        /* udp receiving service */
        private class UdpReceiver {
            public static int RECEIVER_BUFFER_SIZE = 1024;
            public static int DEFAULT_TIMEOUT = 500;

            private readonly UdpClient _client = null;
            private bool _available = false;

            public UdpReceiver(UdpClient client, int timeout) {
                try {
                    _client = client;
                    _client.Client.SendTimeout = timeout < 0 ? DEFAULT_TIMEOUT : timeout;
                    _available = true;
                }
                catch (Exception) {
                    /* do nothing */
                }
            }

            public bool IsAvailable() {
                return _available;
            }

            public void Start() {
                if (!_available) {
                    return;
                }

                new Thread(this.Looping).Start();
            }

            public void Stop() {
                _available = false;
                Thread.Sleep(100);
            }

            private void Looping() {
                /* allow to receive from any address who send the datagram to the local port */
                var remoteEndPoint =
                    new IPEndPoint(IPAddress.Any, ((IPEndPoint) _client.Client.LocalEndPoint).Port);

                while (_available) {
                    try {
                        byte[] buff = _client.Receive(ref remoteEndPoint);

                        /* decode(utf-8) the bytes as a message string */
                        String message = Encoding.UTF8.GetString(buff, 0, buff.Length);

                        /* submit to the ui component */
                        ui.Display("[" + remoteEndPoint + "]: ", false);
                        ui.Display(message + "\r\n", false);
                    }
                    catch (TimeoutException) {
                        /* 
                         * receiving action timeout, and it's 
                         * time to check the _available flag 
                         */
                    }
                    catch (Exception) {
                        /* socket has been closed, try to stop receiver thread */
                        _available = false;
                    }
                }
            }
        }

        /* 
         * a static class, which help analyze 
         * the command and convert it to a action 
         */
        private class Parser {
            /* command types */
            public enum CommandType {
                SEND,
                QUIT,
                INVALID
            }

            public static CommandType Parse(string command, ref IPEndPoint where, ref string what) {
                /* set of delimeters */
                char[] delims = {' '};
                /* split the command into pieces */
                string[] splits = command.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                if (null == splits || 0 == splits.Length) {
                    return CommandType.INVALID;
                }

                if (1 == splits.Length && splits[0].Equals("quit")) {
                    return CommandType.QUIT;
                }

                if (3 <= splits.Length && splits[0].Equals("to")) {
                    /* 
                     * dangerous implementation!!! there is 
                     * still something to do with this case 
                     */

                    try {
                        // todo
                        delims[0] = ':';
                        string sockAddrStr = splits[1];

                        string[] tmp = sockAddrStr.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                        if (null == tmp || 2 != tmp.Length) {
                            return CommandType.INVALID;
                        }
                        /* 
                         * try to get the remote address and port,
                         * IPAddress.Parse and Int32.Parse might fail
                         */
                        where.Address = IPAddress.Parse(tmp[0]);
                        where.Port = Int32.Parse(tmp[1]);

                        /* get the message content */
                        int index = command.IndexOf(":", StringComparison.Ordinal);
                        index = command.IndexOf(" ", index + 1, StringComparison.Ordinal);
                        what = command.Substring(index);

                        return CommandType.SEND;
                    }
                    catch (Exception) {
                        /*  */
                    }
                }

                return CommandType.INVALID;
            }
        }
    }
}