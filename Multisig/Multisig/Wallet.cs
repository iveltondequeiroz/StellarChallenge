using System;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using System.Threading.Tasks;

    
namespace Multisig
{
    class Wallet
    {
        private readonly string HorizonUrl = "https://horizon-testnet.stellar.org";
        private readonly string masterAccountId = "GAV3PAFSQMAUZWWCWA6HQOIIQ3RSHSOGLLGXMQ3TDXKRVMGKSATSR5IY";
        private readonly string masterSecret = "SA4UNSKZ2R3K743F4QE2QT4RBNTTVB6YOVRRJRLUNGNQBUCWUGHTENIL";
        private readonly string signerAccountId = "GDTC4V62FV3FTXPBUQQQZ66WD43UIPYXCY4JH2WJYKNGDSASCLJ4EFRQ";
        private readonly string signerSecret = "SA47CHHU2C2N727UEWINYE7TCXDHKVJMZDML5I26AMFEJ5QO4LSUATLZ";
        private readonly string destinationAccountId = "GDQDDVIQ3IDX3STTCWUDZTOBOLMNL4MS2PE67SCKJNA6QFRGJLAS7J4Q";


        // Generates a multi-signed transaction 
        // and transfer amount to a 3rd account,  if signed correctly
        public async Task MultiSigTransfer()
        {
            // Horizon settings
            Network.UseTestNetwork();
            Server server = new Server(HorizonUrl);

            // master account
            Console.WriteLine("Generating key pairs...");
            KeyPair masterKeyPair = KeyPair.FromSecretSeed(masterSecret);
            AccountResponse masterAccountResponse = await server.Accounts.Account(masterKeyPair);
            Account masterAccount = new Account(masterAccountResponse.KeyPair, masterAccountResponse.SequenceNumber);

            // signer account
            KeyPair signerKeyPair = KeyPair.FromAccountId(signerAccountId);
            var signerKey = stellar_dotnet_sdk.Signer.Ed25519PublicKey(signerKeyPair);
            KeyPair signerSecretKeyPair = KeyPair.FromSecretSeed(signerSecret); ;

            // destination account
            KeyPair destinationKeyPair = KeyPair.FromAccountId(destinationAccountId);

            // set signer operation
            SetOptionsOperation signerOperation = new SetOptionsOperation.Builder().SetSigner(signerKey, 1).Build();

            // set flag
            SetOptionsOperation flagOperation = new SetOptionsOperation.Builder().SetSetFlags(1).Build();

            // for clearing flags
            // SetOptionsOperation flagOperation = new SetOptionsOperation.Builder().SetClearFlags(1).Build();

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

                await this.GetBalance(masterAccountId);
                await this.GetBalance(signerAccountId);
                await this.GetBalance(destinationAccountId);
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
