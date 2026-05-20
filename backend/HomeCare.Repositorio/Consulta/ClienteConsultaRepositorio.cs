using System.Data;
using Dapper;
using HomeCare.Dominio.DTOs.HomeCare;
using HomeCare.Repositorio.Fabrica;
using HomeCare.Repositorio.Interfaces.Consulta;

namespace HomeCare.Repositorio.Consulta;

public class ClienteConsultaRepositorio : IClienteConsultaRepositorio
{
    private readonly IConexaoFabrica _fabrica;

    public ClienteConsultaRepositorio(IConexaoFabrica fabrica)
    {
        _fabrica = fabrica;
    }

    // ── Stored Procedures ──────────────────────────────────────

    public async Task<int> CadastrarAsync(CriarClienteDto dto)
    {
        using var conexao = _fabrica.Criar();

        var parametros = new DynamicParameters();
        parametros.Add("@Nome",      dto.Nome,      DbType.String);
        parametros.Add("@Email",     dto.Email,     DbType.String);
        parametros.Add("@SenhaHash", dto.SenhaHash, DbType.String);
        parametros.Add("@Telefone",  dto.Telefone ?? string.Empty,  DbType.String);
        parametros.Add("@Endereco",  dto.Endereco ?? string.Empty,  DbType.String);

        var resultado = await conexao.QuerySingleAsync<dynamic>(
            "dbo.spCadastrarCliente",
            parametros,
            commandType: CommandType.StoredProcedure);

        return (int)resultado.ClienteId;
    }

    public async Task<int> CadastrarPetAsync(CriarPetDto dto)
    {
        using var conexao = _fabrica.Criar();

        var parametros = new DynamicParameters();
        parametros.Add("@ClienteId",   dto.ClienteId,   DbType.Int32);
        parametros.Add("@Nome",        dto.Nome,        DbType.String);
        parametros.Add("@Tipo",        dto.Tipo,        DbType.Int32);
        parametros.Add("@Raca",        dto.Raca ?? string.Empty,        DbType.String);
        parametros.Add("@Idade",       dto.Idade,       DbType.Int32);
        parametros.Add("@Peso",        dto.Peso,        DbType.Decimal);
        parametros.Add("@Observacoes", dto.Observacoes ?? string.Empty, DbType.String);

        var resultado = await conexao.QuerySingleAsync<dynamic>(
            "dbo.spCadastrarPet",
            parametros,
            commandType: CommandType.StoredProcedure);

        return (int)resultado.PetId;
    }

    // ── Views ──────────────────────────────────────────────────

    public async Task<IEnumerable<ClienteComPetsDto>> ObterClientesComPetsAsync()
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QueryAsync<ClienteComPetsDto>(
            "SELECT * FROM dbo.vwClientesComPets ORDER BY Nome");
    }
}
