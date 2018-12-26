using System;
using System.Threading.Tasks;

namespace Multisig
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Generates a multi-signed transaction 
            // and transfer amount to a 3rd account,  if signed correctly
            Wallet wallet = new Wallet();
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
    }
}
