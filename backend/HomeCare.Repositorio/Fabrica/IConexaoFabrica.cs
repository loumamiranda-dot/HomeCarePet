using System.Data;

namespace HomeCare.Repositorio.Fabrica;

public interface IConexaoFabrica
{
    IDbConnection Criar();
}
