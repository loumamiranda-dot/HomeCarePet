-- ============================================================
-- HomeCare API - Script 02: Stored Procedures
-- Pré-requisito: tabelas já criadas pelo EF Core (migration)
--
-- Tabelas do schema (nomes configurados via Fluent API):
--   dbo.Usuario      (UsuarioId, Nome, Email, SenhaHash, TipoUsuario, Ativo)
--   dbo.Cliente      (ClienteId, Nome, Email, Telefone, Endereco, DataCadastro, Ativo, UsuarioId)
--   dbo.Pet          (PetId, Nome, Tipo, Raca, Idade, Peso, Observacoes, ClienteId)
--   dbo.Servico      (ServicoId, Nome, Descricao, Preco, DuracaoEmMinutos, Ativo)
--   dbo.Agendamento  (AgendamentoId, ClienteId, PetId, ServicoId, DataHora, Status, Observacoes)
--
-- Enum TiposUsuario : Administrador=1, Funcionario=2, Cliente=3
-- Enum TiposPet     : Cachorro=1, Gato=2, Ave=3, Roedor=4, Reptil=5, Outro=6
-- Enum StatusAgendamento : Pendente=1, Confirmado=2, Finalizado=3, Cancelado=4
-- ============================================================

-- ============================================================
-- spCadastrarCliente
-- Cadastra Usuario (TipoUsuario=3) + Cliente em uma transação.
-- A SenhaHash já deve chegar hasheada pela camada de aplicação.
-- Retorna o ClienteId gerado.
-- ============================================================
IF OBJECT_ID('dbo.spCadastrarCliente', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spCadastrarCliente;
GO

CREATE PROCEDURE dbo.spCadastrarCliente
    @Nome      NVARCHAR(100),
    @Email     NVARCHAR(150),
    @SenhaHash NVARCHAR(MAX),
    @Telefone  NVARCHAR(20)  = NULL,
    @Endereco  NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email)
    BEGIN
        RAISERROR('Já existe um usuário cadastrado com este e-mail.', 16, 1);
        RETURN;
    END

    DECLARE @UsuarioId INT;

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.Usuario (Nome, Email, SenhaHash, TipoUsuario, Ativo)
        VALUES (@Nome, @Email, @SenhaHash, 3, 1);   -- TipoUsuario=3 (Cliente)

        SET @UsuarioId = SCOPE_IDENTITY();

        INSERT INTO dbo.Cliente (Nome, Email, Telefone, Endereco, DataCadastro, Ativo, UsuarioId)
        VALUES (@Nome, @Email, @Telefone, @Endereco, GETDATE(), 1, @UsuarioId);

        COMMIT TRANSACTION;

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS ClienteId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================================
-- spCadastrarPet
-- Cadastra um pet vinculado a um cliente ativo.
-- Tipo segue o enum TiposPet (1=Cachorro, 2=Gato, ...).
-- Retorna o PetId gerado.
-- ============================================================
IF OBJECT_ID('dbo.spCadastrarPet', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spCadastrarPet;
GO

CREATE PROCEDURE dbo.spCadastrarPet
    @ClienteId   INT,
    @Nome        NVARCHAR(100),
    @Tipo        INT,
    @Raca        NVARCHAR(80)  = NULL,
    @Idade       INT           = NULL,
    @Peso        DECIMAL(5,2)  = NULL,
    @Observacoes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE ClienteId = @ClienteId AND Ativo = 1)
    BEGIN
        RAISERROR('Cliente não encontrado ou inativo.', 16, 1);
        RETURN;
    END

    IF @Tipo NOT BETWEEN 1 AND 6
    BEGIN
        RAISERROR('Tipo de pet inválido. Use: 1=Cachorro, 2=Gato, 3=Ave, 4=Roedor, 5=Reptil, 6=Outro.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Pet (Nome, Tipo, Raca, Idade, Peso, Observacoes, ClienteId)
    VALUES (@Nome, @Tipo, @Raca, @Idade, @Peso, @Observacoes, @ClienteId);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS PetId;
END
GO

-- ============================================================
-- spCriarAgendamento
-- Cria um agendamento com Status=1 (Pendente).
-- Valida serviço ativo antes de inserir.
-- Retorna o AgendamentoId gerado.
-- ============================================================
IF OBJECT_ID('dbo.spCriarAgendamento', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spCriarAgendamento;
GO

CREATE PROCEDURE dbo.spCriarAgendamento
    @ClienteId   INT,
    @PetId       INT,
    @ServicoId   INT,
    @DataHora    DATETIME2,
    @Observacoes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Servico WHERE ServicoId = @ServicoId AND Ativo = 1)
    BEGIN
        RAISERROR('Serviço não encontrado ou inativo.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Pet WHERE PetId = @PetId AND ClienteId = @ClienteId)
    BEGIN
        RAISERROR('Pet não pertence ao cliente informado.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Agendamento (ClienteId, PetId, ServicoId, DataHora, Status, Observacoes)
    VALUES (@ClienteId, @PetId, @ServicoId, @DataHora, 1, @Observacoes);  -- Status=1 (Pendente)

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS AgendamentoId;
END
GO

-- ============================================================
-- spObterAgendamentosPorCliente
-- Retorna todos os agendamentos de um cliente com dados
-- completos: cliente, pet e serviço.
-- ============================================================
IF OBJECT_ID('dbo.spObterAgendamentosPorCliente', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spObterAgendamentosPorCliente;
GO

CREATE PROCEDURE dbo.spObterAgendamentosPorCliente
    @ClienteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.AgendamentoId,
        a.DataHora,
        a.Status,
        a.Observacoes,
        c.ClienteId,
        c.Nome      AS NomeCliente,
        c.Email     AS EmailCliente,
        c.Telefone,
        p.PetId,
        p.Nome      AS NomePet,
        p.Tipo      AS TipoPet,
        p.Raca,
        p.Peso,
        s.ServicoId,
        s.Nome      AS NomeServico,
        s.Preco,
        s.DuracaoEmMinutos,
        s.Descricao AS DescricaoServico
    FROM       dbo.Agendamento a
    INNER JOIN dbo.Cliente     c ON a.ClienteId = c.ClienteId
    INNER JOIN dbo.Pet         p ON a.PetId     = p.PetId
    INNER JOIN dbo.Servico     s ON a.ServicoId = s.ServicoId
    WHERE a.ClienteId = @ClienteId
    ORDER BY a.DataHora DESC;
END
GO

-- ============================================================
-- spObterAgendamentosPorStatus
-- Retorna agendamentos filtrados por status com dados completos.
-- Status: 1=Pendente, 2=Confirmado, 3=Finalizado, 4=Cancelado
-- ============================================================
IF OBJECT_ID('dbo.spObterAgendamentosPorStatus', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spObterAgendamentosPorStatus;
GO

CREATE PROCEDURE dbo.spObterAgendamentosPorStatus
    @Status INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.AgendamentoId,
        a.DataHora,
        a.Status,
        a.Observacoes,
        c.ClienteId,
        c.Nome      AS NomeCliente,
        c.Email     AS EmailCliente,
        c.Telefone,
        p.PetId,
        p.Nome      AS NomePet,
        p.Tipo      AS TipoPet,
        p.Raca,
        p.Peso,
        s.ServicoId,
        s.Nome      AS NomeServico,
        s.Preco,
        s.DuracaoEmMinutos,
        s.Descricao AS DescricaoServico
    FROM       dbo.Agendamento a
    INNER JOIN dbo.Cliente     c ON a.ClienteId = c.ClienteId
    INNER JOIN dbo.Pet         p ON a.PetId     = p.PetId
    INNER JOIN dbo.Servico     s ON a.ServicoId = s.ServicoId
    WHERE a.Status = @Status
    ORDER BY a.DataHora;
END
GO

-- ============================================================
-- spAtualizarStatusAgendamento
-- Atualiza o status de um agendamento.
-- Retorna a quantidade de registros afetados.
-- ============================================================
IF OBJECT_ID('dbo.spAtualizarStatusAgendamento', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spAtualizarStatusAgendamento;
GO

CREATE PROCEDURE dbo.spAtualizarStatusAgendamento
    @AgendamentoId INT,
    @NovoStatus    INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Agendamento WHERE AgendamentoId = @AgendamentoId)
    BEGIN
        RAISERROR('Agendamento não encontrado.', 16, 1);
        RETURN;
    END

    IF @NovoStatus NOT BETWEEN 1 AND 4
    BEGIN
        RAISERROR('Status inválido. Use: 1=Pendente, 2=Confirmado, 3=Finalizado, 4=Cancelado.', 16, 1);
        RETURN;
    END

    UPDATE dbo.Agendamento
    SET    Status = @NovoStatus
    WHERE  AgendamentoId = @AgendamentoId;

    SELECT @@ROWCOUNT AS RegistrosAfetados;
END
GO

-- ============================================================
-- spCancelarAgendamento
-- Cancela um agendamento (Status=4).
-- Impede cancelar agendamentos já finalizados.
-- ============================================================
IF OBJECT_ID('dbo.spCancelarAgendamento', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spCancelarAgendamento;
GO

CREATE PROCEDURE dbo.spCancelarAgendamento
    @AgendamentoId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Agendamento WHERE AgendamentoId = @AgendamentoId)
    BEGIN
        RAISERROR('Agendamento não encontrado.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.Agendamento WHERE AgendamentoId = @AgendamentoId AND Status = 3)
    BEGIN
        RAISERROR('Não é possível cancelar um agendamento já finalizado.', 16, 1);
        RETURN;
    END

    UPDATE dbo.Agendamento
    SET    Status = 4  -- Cancelado
    WHERE  AgendamentoId = @AgendamentoId;

    SELECT @@ROWCOUNT AS RegistrosAfetados;
END
GO

-- ============================================================
-- spListarServicosAtivos
-- Retorna todos os serviços com Ativo=1 ordenados por nome.
-- ============================================================
IF OBJECT_ID('dbo.spListarServicosAtivos', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spListarServicosAtivos;
GO

CREATE PROCEDURE dbo.spListarServicosAtivos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ServicoId,
        Nome,
        Descricao,
        Preco,
        DuracaoEmMinutos,
        Ativo
    FROM  dbo.Servico
    WHERE Ativo = 1
    ORDER BY Nome;
END
GO

-- ============================================================
-- spObterHistoricoCliente
-- Retorna o histórico completo de agendamentos de um cliente
-- com todos os detalhes do pet e serviço contratado.
-- ============================================================
IF OBJECT_ID('dbo.spObterHistoricoCliente', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spObterHistoricoCliente;
GO

CREATE PROCEDURE dbo.spObterHistoricoCliente
    @ClienteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.AgendamentoId,
        a.DataHora,
        a.Status,
        a.Observacoes,
        c.Nome      AS NomeCliente,
        p.Nome      AS NomePet,
        p.Tipo      AS TipoPet,
        p.Raca,
        s.Nome      AS NomeServico,
        s.Descricao AS DescricaoServico,
        s.Preco,
        s.DuracaoEmMinutos
    FROM       dbo.Agendamento a
    INNER JOIN dbo.Cliente     c ON a.ClienteId = c.ClienteId
    INNER JOIN dbo.Pet         p ON a.PetId     = p.PetId
    INNER JOIN dbo.Servico     s ON a.ServicoId = s.ServicoId
    WHERE a.ClienteId = @ClienteId
    ORDER BY a.DataHora DESC;
END
GO

PRINT '=== Script 02_StoredProcedures concluído com sucesso. ===';
GO
