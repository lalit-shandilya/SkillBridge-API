using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Domain
{
    public class CosmosDb
    {
        public string AccountEndpoint { get; set; }= "https://skillbridge.documents.azure.com:443/";
        public string AccountKey { get; set; } = "6X4ZSpdV8YuzcW4rIExTHIV8eNMH6jYTkMVBVdBRDfUxQoLO0aN6U0pvwefF1Gq635m18ExMk7ESACDbUIRc3g==";
        public string ContainerName { get; set; }= "SB_Container";
        public string DatabaseName { get; set; }= "SB_database";
    }
}
