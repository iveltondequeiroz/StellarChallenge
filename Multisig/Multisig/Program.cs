using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Multisig
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {

            /// Generates a multi-signed transaction and transfer amount to a 3rd account

            AppConfig();
            string amountToTransfer = "35";

            Wallet wallet = new Wallet( Configuration["Stellar:MasterAccount"],
                Configuration["Stellar:MasterSecret"], 
                Configuration["Stellar:SignerAccount"],
                Configuration["Stellar:SignerSecret"],
                Configuration["Stellar:DestinationAccount"],
                Configuration["Stellar:HorizonURL"],
                amountToTransfer
            );

            try
            {
                Task.Run(async () =>
                {
                    Console.WriteLine("Multisig ..");
                    await wallet.MultiSigTransfer();
                    Console.WriteLine("Press any key  to exit...");
                    Console.ReadLine();
                }).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                Console.WriteLine("error : " + exception.Message);
                Console.ReadLine();
            }
        }

        static void AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }
    }
}
