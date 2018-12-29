using System;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using System.Threading.Tasks;

        
namespace Multisig
{
    class Wallet
    {
        private string HorizonUrl { get; set; }
        private string MasterAccount { get; set; }
        private string MasterSecret { get; set; }
        private string SignerAccount { get; set; }
        private string SignerSecret { get; set; }
        private string DestinationAccount { get; set; }
        private string AmountToTransfer { get; set; }


        public Wallet(string master, string masterSecret, string signer, string signerSecret, string destination, string url, string amount)
        {
            MasterAccount = master;
            MasterSecret = masterSecret;
            SignerAccount = signer;
            SignerSecret = signerSecret;
            DestinationAccount = destination;
            HorizonUrl = url;
            AmountToTransfer = amount;
        }

        /// Generates a multi-signed transaction       
        /// and transfer amount to a 3rd account, if signed correctly
        public async Task MultiSigTransfer()
        {
            // Horizon settings
            Network.UseTestNetwork();
            Server server = new Server(HorizonUrl);

            // master account
            Console.WriteLine("Generating key pairs...");
            KeyPair masterKeyPair = KeyPair.FromSecretSeed(MasterSecret);
            AccountResponse masterAccountResponse = await server.Accounts.Account(masterKeyPair);
            Account masterAccount = new Account(masterAccountResponse.KeyPair, masterAccountResponse.SequenceNumber);

            // generating keypairs
            KeyPair signerKeyPair = KeyPair.FromAccountId(SignerAccount);
            KeyPair signerSecretKeyPair = KeyPair.FromSecretSeed(SignerSecret); ;
            KeyPair destinationKeyPair = KeyPair.FromAccountId(DestinationAccount);
            var signerKey = stellar_dotnet_sdk.Signer.Ed25519PublicKey(signerKeyPair);
            
            // set signer operation
            SetOptionsOperation signerOperation = new SetOptionsOperation.Builder().SetSigner(signerKey, 1).Build();

            // set flag
            // for clearing flags -> SetOptionsOperation flagOperation = new SetOptionsOperation.Builder().SetClearFlags(1).Build();
            SetOptionsOperation flagOperation = new SetOptionsOperation.Builder().SetSetFlags(1).Build();

            // set medium threshold
            SetOptionsOperation thresholdOperation = new SetOptionsOperation.Builder().SetMediumThreshold(2).Build();

            // payment operation
            string amountToTransfer = "35";
            Asset asset = new AssetTypeNative();
            PaymentOperation paymentOperation = new PaymentOperation.Builder(destinationKeyPair, asset, amountToTransfer).SetSourceAccount(masterKeyPair).Build();

            // create transaction
            Transaction transaction = new Transaction.Builder(masterAccount)
                .AddOperation(flagOperation)
                .AddOperation(thresholdOperation)
                .AddOperation(signerOperation)
                .AddOperation(paymentOperation)
                .Build();

            // sign Transaction
            transaction.Sign(masterKeyPair);
            transaction.Sign(signerSecretKeyPair);

            // try to send transaction
            try
            {
                Console.WriteLine("Sending Transaction...");
                await server.SubmitTransaction(transaction);
                Console.WriteLine("Success!");

                await this.GetBalance(MasterAccount);
                await this.GetBalance(SignerAccount);
                await this.GetBalance(DestinationAccount);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Send Transaction Failed");
                Console.WriteLine("Exception: " + exception.Message);
            }
        }

        public async Task GetBalance(string accountId)
        {
            Server server = new Server(HorizonUrl);

            //Generate a keypair from the account id.
            KeyPair keypair = KeyPair.FromAccountId(accountId);

            //Load the account
            AccountResponse accountResponse = await server.Accounts.Account(keypair);
            //Get the balance
            Balance[] balances = accountResponse.Balances;

            //Show the balance
            for (int i = 0; i < balances.Length; i++)
            {
                Balance asset = balances[i];
                Console.WriteLine("Account: " + accountId);
                Console.WriteLine("Asset Code: " + asset.AssetCode);
                Console.WriteLine("Asset Amount: " + asset.BalanceString);
            }
        }
    }
}
