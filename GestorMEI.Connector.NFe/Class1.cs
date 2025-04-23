namespace GestorMEI.Connector.NFe
{
    public class Class1
    {
        public void Teste()
        {
            var con = new NFeSPService.ConsultaGuia();
            var client = new NFeSPService.LoteNFeAsyncSoapClient(NFeSPService.LoteNFeAsyncSoapClient.EndpointConfiguration.LoteNFeAsyncSoap);
            client.Open();

            client.Close();
        }
    }
}
