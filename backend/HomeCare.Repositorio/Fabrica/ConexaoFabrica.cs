using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HomeCare.Repositorio.Fabrica;

public class ConexaoFabrica : IConexaoFabrica
{
    private readonly string _connectionString;

    public ConexaoFabrica(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Padrao")
            ?? throw new InvalidOperationException("Connection string 'Padrao' não encontrada na configuração.");
    }

    public IDbConnection Criar() => new SqlConnection(_connectionString);
}
