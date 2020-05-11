using System;
using SimulatedInvesting;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;

namespace SimulatedInvesting.AzureService
{
    public class SimulatedPortfolioAzureStorageHelper
        {
            private CloudBlobClient BlobClient;
            private CloudBlobContainer MainContainer;

            public static SimulatedPortfolioAzureStorageHelper Create(string azure_storage_connection_string, string container_name)
            {
                SimulatedPortfolioAzureStorageHelper ReturnInstance = new SimulatedPortfolioAzureStorageHelper();

                CloudStorageAccount csa;
                CloudStorageAccount.TryParse(azure_storage_connection_string, out csa);
                CloudBlobClient cbc = csa.CreateCloudBlobClient();
                CloudBlobContainer cont = cbc.GetContainerReference(container_name);
                if (cont.Exists() == false)
                {
                    throw new Exception("Container '" + container_name + "' does not exist.");
                }

                ReturnInstance.BlobClient = cbc;
                ReturnInstance.MainContainer = cont;



                return ReturnInstance;
            }

            public void UploadPortfolio(SimulatedPortfolio portfolio)
            {
                string json_data = JsonConvert.SerializeObject(portfolio);
                CloudBlockBlob cbc = MainContainer.GetBlockBlobReference(portfolio.Id.ToString());
                cbc.UploadText(json_data);
            }

            public SimulatedPortfolio DownloadPortfolio(string Id)
            {
                CloudBlockBlob cbc = MainContainer.GetBlockBlobReference(Id);
                if (cbc.Exists() == false)
                {
                    throw new Exception("Unable to find portfolio with Id '" + Id + "'.");
                }
                string cont = cbc.DownloadText();
                SimulatedPortfolio sp = JsonConvert.DeserializeObject<SimulatedPortfolio>(cont);
                return sp;
            }

        }
}