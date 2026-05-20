using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;
using HomeCare.Repositorio.Interfaces;

namespace HomeCare.Aplicacao;

public class ClienteAplicacao : IClienteAplicacao
{
    private readonly IClienteRepositorio _repositorio;
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ClienteAplicacao(IClienteRepositorio repositorio, IUsuarioRepositorio usuarioRepositorio)
    {
        _repositorio = repositorio;
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task<Cliente> CriarAsync(string nome, string email, string senha, string telefone, string endereco)
    {
        // TODO: validar formato do email
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new Exception("Email é obrigatório.");
        if (string.IsNullOrWhiteSpace(senha)) throw new Exception("Senha é obrigatória.");

        var usuarioExistente = await _usuarioRepositorio.ObterPorEmailAsync(email);
        if (usuarioExistente != null) throw new Exception("Email já cadastrado.");

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
            TipoUsuario = TiposUsuario.Cliente,
            Ativo = true
        };

        await _usuarioRepositorio.SalvarAsync(usuario);

        var cliente = new Cliente
        {
            Nome = nome,
            Email = email,
            Telefone = telefone,
            Endereco = endereco,
            DataCadastro = DateTime.Now,
            UsuarioId = usuario.Id
        };

        await _repositorio.SalvarAsync(cliente);
        return cliente;
    }

    public async Task<Cliente> ObterAsync(int id)
    {
        var cliente = await _repositorio.ObterAsync(id);
        if (cliente == null) throw new Exception("Cliente não encontrado.");
        return cliente;
    }

    public async Task<Cliente> ObterPorUsuarioIdAsync(int usuarioId)
    {
        var cliente = await _repositorio.ObterPorUsuarioIdAsync(usuarioId);
        if (cliente == null) throw new Exception("Cliente não encontrado.");
        return cliente;
    }

    public async Task<List<Cliente>> ListarAsync(int pagina, int tamanhoPagina) =>
        await _repositorio.ListarAsync(pagina, tamanhoPagina);

    public async Task AtualizarAsync(int id, string nome, string email, string telefone, string endereco)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new Exception("Email é obrigatório.");

        var cliente = await _repositorio.ObterAsync(id);
        if (cliente == null) throw new Exception("Cliente não encontrado.");

        cliente.Nome = nome;
        cliente.Email = email;
        cliente.Telefone = telefone;
        cliente.Endereco = endereco;
        await _repositorio.AtualizarAsync(cliente);
    }

    public async Task DeletarAsync(int id)
    {
        var cliente = await _repositorio.ObterAsync(id);
        if (cliente == null) throw new Exception("Cliente não encontrado.");
        cliente.Deletar();
        await _repositorio.DeletarAsync(cliente);
    }
}
