using System;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;

namespace Multisig
{
    public delegate void PaymentReceivedHandler(object sender, OperationResponse e);

    class Integration
    {
        private readonly string HorizonUrl = "https://horizon-testnet.stellar.org";
        //private readonly string baseAccount = "SCAE4JAXOFRI3AGMS4U7KCYH3JZWCJ2SABSL427TT7ITWH7I3IEOQPSB";
        private readonly string baseAccountSecret = "SCQOMDEZYZUF6JHNRIPNGPO6ZHYWGXFXS3Q4OFVZQK6SODFSCXM5GX7O";
        private readonly string NetworkPassphrase = "Test SDF Network ; September 2015";
        public Integration()
        {

        }

        public void ListenForDeposits()
        {

            // Horizon Server settings
            Console.WriteLine("setting up horizon server...");
            //Network.UseTestNetwork();
            Network.Use(new Network(NetworkPassphrase));
            Server server = new Server(HorizonUrl);

            // Get the latest cursor position
            // [TODO - read lastToken from DB]

            // Listen for payments from where you last stopped
            // GET https://horizon-testnet.stellar.org/accounts/baseAccount}/payments?cursor={last_token}
            // var callBuilder = Server.Payments.forAccount(baseAccount);
            Console.WriteLine("Listen for payments...");
            KeyPair baseAccountKeys = KeyPair.FromSecretSeed(baseAccountSecret); ;
            var callBuilder = server.Payments.ForAccount(baseAccountKeys);

            Console.WriteLine("streaming...");
            //callBuilder.Stream();
            //callBuilder.stream({ onmessage: handlePaymentResponse});

            // TODO - Stream Callback
            callBuilder.Stream(HandlePaymentResponse);

        }

        private void HandlePaymentResponse(object sender, OperationResponse e)
        {
            // [TODO - credit the user’s account in the DB with the number of XLM they sent to deposit]
            Console.WriteLine("handlePaymentResponse OK");

        }

        public void SubmitTransaction(string exchangeAccount, string destinationAddress, int amountLumens)
        {
            // [TODO] queue up a transaction in the exchange’s StellarTransactions table
        }

        public void SubmitPendingTransactions(string exchangeAccount)
        {
            // [TODO] submitt all pending transactions, and calls the previous one
            // [TODO] This function should be run in the background continuously
        }

        public void HandleRequestWithdrawal(int userID, int amountLumens, string destinationAddress)
        {
            // [TODO] queue up a transaction in the exchange’s StellarTransactions table
        }

    }
}
