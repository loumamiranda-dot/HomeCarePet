-- ============================================================
-- HomeCare API - Script 04: Functions
-- Pré-requisito: tabelas já criadas pelo EF Core (migration)
-- ============================================================

-- ============================================================
-- fnTotalAgendamentosCliente
-- Scalar function: total de agendamentos de um cliente
-- (qualquer status). Retorna 0 se não tiver nenhum.
-- ============================================================
IF OBJECT_ID('dbo.fnTotalAgendamentosCliente', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fnTotalAgendamentosCliente;
GO

CREATE FUNCTION dbo.fnTotalAgendamentosCliente
(
    @ClienteId INT
)
RETURNS INT
AS
BEGIN
    DECLARE @Total INT;

    SELECT @Total = COUNT(*)
    FROM   dbo.Agendamento
    WHERE  ClienteId = @ClienteId;

    RETURN COALESCE(@Total, 0);
END
GO

-- ============================================================
-- fnReceitaTotalServico
-- Scalar function: receita gerada por um serviço específico,
-- considerando apenas agendamentos Finalizados (Status=3).
-- Retorna 0.00 se não houver agendamentos finalizados.
-- ============================================================
IF OBJECT_ID('dbo.fnReceitaTotalServico', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fnReceitaTotalServico;
GO

CREATE FUNCTION dbo.fnReceitaTotalServico
(
    @ServicoId INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Receita DECIMAL(10,2);

    SELECT @Receita = SUM(s.Preco)
    FROM       dbo.Agendamento a
    INNER JOIN dbo.Servico     s ON a.ServicoId = s.ServicoId
    WHERE  a.ServicoId = @ServicoId
      AND  a.Status    = 3;   -- Finalizado

    RETURN COALESCE(@Receita, 0.00);
END
GO

-- ============================================================
-- fnAgendamentosNoPeriodo
-- Table-valued function: retorna agendamentos com detalhes
-- dentro de um intervalo de data/hora.
-- Inclui todos os status (filtre após se necessário).
-- ============================================================
IF OBJECT_ID('dbo.fnAgendamentosNoPeriodo', 'TF') IS NOT NULL
    DROP FUNCTION dbo.fnAgendamentosNoPeriodo;
GO

CREATE FUNCTION dbo.fnAgendamentosNoPeriodo
(
    @DataInicio DATETIME2,
    @DataFim    DATETIME2
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        a.AgendamentoId,
        a.DataHora,
        a.Status,
        a.Observacoes,
        c.Nome  AS NomeCliente,
        c.Email AS EmailCliente,
        p.Nome  AS NomePet,
        p.Tipo  AS TipoPet,
        s.Nome  AS NomeServico,
        s.Preco,
        s.DuracaoEmMinutos
    FROM       dbo.Agendamento a
    INNER JOIN dbo.Cliente     c ON a.ClienteId = c.ClienteId
    INNER JOIN dbo.Pet         p ON a.PetId     = p.PetId
    INNER JOIN dbo.Servico     s ON a.ServicoId = s.ServicoId
    WHERE a.DataHora BETWEEN @DataInicio AND @DataFim
);
GO

PRINT '=== Script 04_Functions concluído com sucesso. ===';
GO
