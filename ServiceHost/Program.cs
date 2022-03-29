using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Service;

namespace ServiceHostApp
{
    class Program
    {
        static ServiceHost mainServiceHost = null;
        static ServiceHost chatServiceHost = null;
        ChatService cs = new ChatService();

        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.StartMainService();
            prog.StartChatService();
            Console.ReadLine();
        }

        private void StartMainService()
        {
            var adrs = new Uri[1];
            adrs[0] = new Uri("http://localhost:3980/MainService");
            mainServiceHost = new ServiceHost(typeof(ManagerService), adrs);
            try
            {
                // Open the ServiceHost to start listening for messages.
                mainServiceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The main service is ready.");
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine(timeProblem.Message);
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine(commProblem.Message);
            }
        }

        private void StartChatService()
        {
            chatServiceHost = new ServiceHost(typeof(ChatService));
            try
            {
                // Open the ServiceHost to start listening for messages.
                chatServiceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The chat service is ready.");
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine(timeProblem.Message);
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine(commProblem.Message);
            }
        }
    }
}
