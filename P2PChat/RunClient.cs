using System;

/* 
 * created by Maou Lim on 17-4-10
 */

namespace P2PChat {
    public class RunClient {

        public static Int16 DEFAULT_LOCAL_PORT = 23333;

        public static void Main(string[] args) {
            
            /* declare a p2pclient */
            P2PClient client = null;

            try {
                if (args.Length < 1) {
                    client = new P2PClient(DEFAULT_LOCAL_PORT, new MainForm(DEFAULT_LOCAL_PORT));
                }
                else {
                    Int16 port = Int16.Parse(args[0]);
                    client = new P2PClient(port, new MainForm(port));
                }
                
                client.Start();
            }
            catch (Exception ex) {
                System.Console.Error.WriteLine(ex.Message);
            }
        }
    }
}