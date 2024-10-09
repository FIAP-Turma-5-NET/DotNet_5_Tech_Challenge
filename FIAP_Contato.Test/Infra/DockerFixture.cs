using Docker.DotNet;
using Docker.DotNet.Models;

using MySql.Data.MySqlClient;

namespace FIAP_Contato.Test.Infra
{
    public class DockerFixture : IDisposable
    {
        private DockerClient _dockerClient;
        private string _containerId;
        private readonly string _environment;

        public DockerFixture()
        {            
            _dockerClient = new DockerClientConfiguration().CreateClient();
            
            var createContainerResponse = _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = "mysql:8.0.32",
                Env = new List<string>
                {
                    "MYSQL_ROOT_PASSWORD=202410",  
                    "MYSQL_DATABASE=FIAPContato"   
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "3306/tcp", new List<PortBinding> { new PortBinding { HostPort = "3306" } } }  // Mapeia a porta do MySQL
                    },
                    PublishAllPorts = true 
                },
                Name = "mysql-fiap-contato-test"
            }).GetAwaiter().GetResult();

            _containerId = createContainerResponse.ID;
            
            _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()).GetAwaiter().GetResult();

            WaitForDatabaseToBeAvailable();
        }

        // Retorna a string de conexão para o MySQL
        public string GetConnectionString()
        {
            //var _connectionString = "Server=localhost;Port=3306;Database=FIAPContato;User Id=root;Password=202410;AllowPublicKeyRetrieval=True;SslMode=None;";
            var _connectionString = "Server=host.docker.internal;port=3306;database=FIAPContato;uid=root;pwd=202410;AllowPublicKeyRetrieval=True;SslMode=None;";
      
            return _connectionString;
        }

        // Método para liberar recursos e parar o contêiner
        public void Dispose()
        {
            // Para e remove o contêiner
            _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).GetAwaiter().GetResult();
            _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters()).GetAwaiter().GetResult();

            // Disposição do cliente Docker
            _dockerClient.Dispose();
        }

        private void WaitForDatabaseToBeAvailable()
        {
            var connectionString = GetConnectionString();

            // Tentativas de conexão
            var maxRetries = 10;
            var delay = TimeSpan.FromSeconds(5);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        break; 
                    }
                }
                catch (Exception)
                {                   
                    Thread.Sleep(delay);
                }
            }
        }
    }
}
