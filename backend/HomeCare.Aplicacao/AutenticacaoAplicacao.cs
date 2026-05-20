using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;
using HomeCare.Repositorio.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeCare.Aplicacao;

public class AutenticacaoAplicacao : IAutenticacaoAplicacao
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IConfiguration _configuracao;

    public AutenticacaoAplicacao(IUsuarioRepositorio usuarioRepositorio, IClienteRepositorio clienteRepositorio, IConfiguration configuracao)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
        _configuracao = configuracao;
    }

    public async Task<string> EntrarAsync(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            throw new Exception("Email e senha são obrigatórios.");

        var usuario = await _usuarioRepositorio.ObterPorEmailAsync(email);
        if (usuario == null || !usuario.Ativo)
            throw new Exception("Credenciais inválidas.");

        // confere a senha com bcrypt
        if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            throw new Exception("Credenciais inválidas.");

        // Console.WriteLine($"Login: {usuario.Email} - {usuario.TipoUsuario}");
        return GerarToken(usuario);
    }

    public async Task<Usuario> RegistrarClienteAsync(string nome, string email, string senha, string telefone, string endereco)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new Exception("Email é obrigatório.");
        if (string.IsNullOrWhiteSpace(senha)) throw new Exception("Senha é obrigatória.");

        var existente = await _usuarioRepositorio.ObterPorEmailAsync(email);
        if (existente != null) throw new Exception("Email já cadastrado.");

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

        await _clienteRepositorio.SalvarAsync(cliente);

        return usuario;
    }

    public async Task<Usuario> RegistrarAdminAsync(string nome, string email, string senha, int tipoUsuario)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new Exception("Email é obrigatório.");
        if (string.IsNullOrWhiteSpace(senha)) throw new Exception("Senha é obrigatória.");

        var existente = await _usuarioRepositorio.ObterPorEmailAsync(email);
        if (existente != null) throw new Exception("Email já cadastrado.");

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
            TipoUsuario = (TiposUsuario)tipoUsuario,
            Ativo = true
        };

        await _usuarioRepositorio.SalvarAsync(usuario);
        return usuario;
    }

    // gera o token jwt pro usuario logado
    private string GerarToken(Usuario usuario)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracao["Jwt:Chave"]!));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuracao["Jwt:Emissor"],
            audience: _configuracao["Jwt:Audiencia"],
            claims: claims,
            expires: DateTime.Now.AddHours(int.Parse(_configuracao["Jwt:ExpiracaoEmHoras"]!)),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
