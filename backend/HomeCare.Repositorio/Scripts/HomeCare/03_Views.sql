-- ============================================================
-- HomeCare API - Script 03: Views
-- Pré-requisito: tabelas já criadas pelo EF Core (migration)
-- ============================================================

-- ============================================================
-- vwAgendamentosDetalhados
-- Todos os agendamentos com dados completos de cliente,
-- pet e serviço. Usada para consultas gerais de back-office.
-- ============================================================
IF OBJECT_ID('dbo.vwAgendamentosDetalhados', 'V') IS NOT NULL
    DROP VIEW dbo.vwAgendamentosDetalhados;
GO

CREATE VIEW dbo.vwAgendamentosDetalhados
AS
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
INNER JOIN dbo.Servico     s ON a.ServicoId = s.ServicoId;
GO

-- ============================================================
-- vwRelatorioServicos
-- Relatório financeiro por serviço:
--   - Total de agendamentos criados
--   - Total de agendamentos finalizados (Status=3)
--   - Total de agendamentos cancelados (Status=4)
--   - Receita gerada (soma dos agendamentos finalizados)
-- ============================================================
IF OBJECT_ID('dbo.vwRelatorioServicos', 'V') IS NOT NULL
    DROP VIEW dbo.vwRelatorioServicos;
GO

CREATE VIEW dbo.vwRelatorioServicos
AS
SELECT
    s.ServicoId,
    s.Nome,
    s.Preco,
    s.DuracaoEmMinutos,
    COUNT(a.AgendamentoId)                                        AS TotalAgendamentos,
    COUNT(CASE WHEN a.Status = 3 THEN 1 END)                      AS TotalFinalizados,
    COUNT(CASE WHEN a.Status = 4 THEN 1 END)                      AS TotalCancelados,
    COALESCE(SUM(CASE WHEN a.Status = 3 THEN s.Preco END), 0.00)  AS ReceitaTotal
FROM       dbo.Servico     s
LEFT  JOIN dbo.Agendamento a ON s.ServicoId = a.ServicoId
GROUP BY
    s.ServicoId,
    s.Nome,
    s.Preco,
    s.DuracaoEmMinutos;
GO

-- ============================================================
-- vwClientesComPets
-- Clientes ativos com a contagem de pets cadastrados.
-- Útil para listagem e relatórios de carteira de clientes.
-- ============================================================
IF OBJECT_ID('dbo.vwClientesComPets', 'V') IS NOT NULL
    DROP VIEW dbo.vwClientesComPets;
GO

CREATE VIEW dbo.vwClientesComPets
AS
SELECT
    c.ClienteId,
    c.Nome,
    c.Email,
    c.Telefone,
    c.Endereco,
    c.DataCadastro,
    COUNT(p.PetId) AS TotalPets
FROM       dbo.Cliente c
LEFT  JOIN dbo.Pet     p ON c.ClienteId = p.ClienteId
WHERE c.Ativo = 1
GROUP BY
    c.ClienteId,
    c.Nome,
    c.Email,
    c.Telefone,
    c.Endereco,
    c.DataCadastro;
GO

-- ============================================================
-- vwAgendamentosHoje
-- Agenda do dia atual, excluindo cancelados (Status != 4).
-- Usada na tela de recepção e agenda diária.
-- ============================================================
IF OBJECT_ID('dbo.vwAgendamentosHoje', 'V') IS NOT NULL
    DROP VIEW dbo.vwAgendamentosHoje;
GO

CREATE VIEW dbo.vwAgendamentosHoje
AS
SELECT
    a.AgendamentoId,
    a.DataHora,
    a.Status,
    a.Observacoes,
    c.Nome    AS NomeCliente,
    c.Telefone,
    p.Nome    AS NomePet,
    p.Tipo    AS TipoPet,
    s.Nome    AS NomeServico,
    s.DuracaoEmMinutos
FROM       dbo.Agendamento a
INNER JOIN dbo.Cliente     c ON a.ClienteId = c.ClienteId
INNER JOIN dbo.Pet         p ON a.PetId     = p.PetId
INNER JOIN dbo.Servico     s ON a.ServicoId = s.ServicoId
WHERE CAST(a.DataHora AS DATE) = CAST(GETDATE() AS DATE)
  AND a.Status <> 4;   -- Exclui cancelados
GO

PRINT '=== Script 03_Views concluído com sucesso. ===';
GO
