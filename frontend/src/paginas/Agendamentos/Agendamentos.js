import { useEffect, useState } from 'react'
import { agendamentoAPI } from '../../services/agendamentoAPI'
import { clienteAPI } from '../../services/clienteAPI'
import { petAPI } from '../../services/petAPI'
import { servicoAPI } from '../../services/servicoAPI'
import { useAuth } from '../../contexts/AuthContext'
import Modal from '../../componentes/Modal/Modal'
import { MdAdd, MdEdit, MdDelete, MdCalendarMonth, MdCancel } from 'react-icons/md'

const STATUS_LIST = ['Pendente', 'Confirmado', 'Finalizado', 'Cancelado']
const STATUS_ENUM = { Pendente: 1, Confirmado: 2, Finalizado: 3, Cancelado: 4 }
const COR = { Pendente: '#F59E0B', Confirmado: '#10B981', Finalizado: '#6B7280', Cancelado: '#EF4444' }
const VAZIO = { clienteId: '', petId: '', servicoId: '', data: '', hora: '', observacoes: '', status: 'Pendente' }

// horarios disponiveis de 30 em 30 min
const SLOTS = []
for (let h = 8; h <= 18; h++) {
  SLOTS.push(`${String(h).padStart(2, '0')}:00`)
  SLOTS.push(`${String(h).padStart(2, '0')}:30`)
}

function toLocalDT(dateStr) {
  const d = new Date(dateStr)
  const pad = (n) => String(n).padStart(2, '0')
  return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`
}

function addMinutos(hora, min) {
  const [h, m] = hora.split(':').map(Number)
  const total = h * 60 + m + min
  return `${String(Math.floor(total / 60)).padStart(2, '0')}:${String(total % 60).padStart(2, '0')}`
}

export default function Agendamentos() {
  const { isAdmin, isFuncionario, isCliente } = useAuth()
  const podeVerAdmin = isAdmin || isFuncionario

  const [agendamentos, setAgendamentos] = useState([])
  const [clientes, setClientes] = useState([])
  const [pets, setPets] = useState([])
  const [servicos, setServicos] = useState([])
  const [clienteIdProprio, setClienteIdProprio] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [filtro, setFiltro] = useState('')
  const [modal, setModal] = useState(false)
  const [editando, setEditando] = useState(null)
  const [form, setForm] = useState(VAZIO)
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState('')

  async function carregar() {
    setCarregando(true)
    try {
      const ag = podeVerAdmin ? await agendamentoAPI.listar() : await agendamentoAPI.meusAgendamentos()
      setAgendamentos(ag.data ?? [])
      const [p, s] = await Promise.all([
        podeVerAdmin ? petAPI.listar() : petAPI.meusPets(),
        servicoAPI.listar(),
      ])
      setPets(p.data ?? [])
      setServicos(s.data ?? [])
      if (podeVerAdmin) setClientes((await clienteAPI.listar()).data ?? [])
      if (isCliente && !clienteIdProprio) {
        const perfil = await clienteAPI.meuPerfil()
        setClienteIdProprio(perfil.data.id)
      }
    } finally { setCarregando(false) }
  }
  useEffect(() => { carregar() }, [])

  // pega os horarios que ja estao ocupados naquele dia
  function getSlotsBlockados(data) {
    const blocked = new Set()
    agendamentos
      .filter(a => a.status !== 'Cancelado' && a.id !== editando?.id)
      .filter(a => toLocalDT(a.dataHora).startsWith(data))
      .forEach(a => {
        const hora = toLocalDT(a.dataHora).slice(11, 16)
        blocked.add(hora)
        blocked.add(addMinutos(hora, 30)) // bloqueia o slot seguinte
      })
    return blocked
  }

  const hoje = new Date().toISOString().slice(0, 10)
  const agora = new Date()
  const horaAgora = `${String(agora.getHours()).padStart(2,'0')}:${String(agora.getMinutes()).padStart(2,'0')}`

  const blockedSlots = form.data ? getSlotsBlockados(form.data) : new Set()

  function slotDisponivel(slot) {
    if (blockedSlots.has(slot)) return false
    if (form.data === hoje && slot <= horaAgora) return false
    return true
  }

  const slotsDisponiveis = SLOTS.filter(slotDisponivel)
  const slotsOcupados = SLOTS.filter(s => blockedSlots.has(s))

  function abrirCriar() { setEditando(null); setForm(VAZIO); setErro(''); setModal(true) }
  function abrirEditar(a) {
    const dt = toLocalDT(a.dataHora)
    setEditando(a)
    setForm({
      clienteId: a.clienteId, petId: a.petId, servicoId: a.servicoId,
      data: dt.slice(0, 10),
      hora: dt.slice(11, 16),
      observacoes: a.observacoes ?? '',
      status: a.status,
    })
    setErro(''); setModal(true)
  }

  async function salvar(e) {
    e.preventDefault()
    if (!form.hora) { setErro('Selecione um horário.'); return }
    if (blockedSlots.has(form.hora)) { setErro('Este horário está ocupado. Escolha outro.'); return }
    setSalvando(true); setErro('')
    try {
      const dataHora = `${form.data}T${form.hora}`
      if (editando) {
        await agendamentoAPI.atualizar(editando.id, {
          dataHora,
          status: STATUS_ENUM[form.status] ?? 1,
          observacoes: form.observacoes ?? '',
        })
      } else {
        const cId = isCliente ? clienteIdProprio : Number(form.clienteId)
        await agendamentoAPI.criar({
          clienteId: cId,
          petId: Number(form.petId),
          servicoId: Number(form.servicoId),
          dataHora,
          observacoes: form.observacoes ?? '',
        })
      }
      setModal(false); carregar()
    } catch (err) {
      const d = err.response?.data
      setErro(d?.message ?? d?.title ?? (typeof d === 'string' ? d : null) ?? 'Erro ao salvar.')
    } finally { setSalvando(false) }
  }

  async function cancelar(id) {
    if (!confirm('Cancelar agendamento?')) return
    try { await agendamentoAPI.cancelar(id); carregar() } catch { alert('Erro ao cancelar.') }
  }
  async function deletar(id) {
    if (!confirm('Remover agendamento?')) return
    try { await agendamentoAPI.deletar(id); carregar() } catch { alert('Erro ao remover.') }
  }

  const campo = (k) => (e) => {
    setForm(f => {
      const novo = { ...f, [k]: e.target.value }
      // limpa o horario quando troca a data
      if (k === 'data') novo.hora = ''
      return novo
    })
    setErro('')
  }

  const filtrados = filtro ? agendamentos.filter(a => a.status === filtro) : agendamentos

  return (
    <div className="pagina">
      <div className="pagina-header">
        <div>
          <h1 className="pagina-titulo">Agendamentos</h1>
          <p className="pagina-sub">{agendamentos.length} agendamento(s)</p>
        </div>
        <button className="btn-primary" onClick={abrirCriar}><MdAdd size={20}/> Novo</button>
      </div>

      <div className="filtros-row">
        <button className={`filtro-chip ${filtro === '' ? 'ativo' : ''}`} onClick={() => setFiltro('')}>Todos</button>
        {STATUS_LIST.map(s => (
          <button key={s} className={`filtro-chip ${filtro === s ? 'ativo' : ''}`}
            style={filtro === s ? { background: COR[s], borderColor: COR[s] } : {}}
            onClick={() => setFiltro(s)}>{s}</button>
        ))}
      </div>

      {carregando ? <div className="loading-full"><div className="spinner"/></div>
        : filtrados.length === 0 ? <div className="vazio"><MdCalendarMonth size={44}/><p>Nenhum agendamento</p></div>
        : (
          <div className="table-wrap">
            <table className="tabela">
              <thead>
                <tr><th>#</th><th>Pet</th><th>Cliente</th><th>Serviço</th><th>Data/Hora</th><th>Status</th><th>Ações</th></tr>
              </thead>
              <tbody>
                {filtrados.map(a => (
                  <tr key={a.id}>
                    <td>{a.id}</td>
                    <td><strong>{a.nomePet}</strong></td>
                    <td>{a.nomeCliente}</td>
                    <td>{a.nomeServico}</td>
                    <td>{new Date(a.dataHora).toLocaleString('pt-BR', { day:'2-digit', month:'2-digit', year:'numeric', hour:'2-digit', minute:'2-digit' })}</td>
                    <td><span className="badge" style={{ background: COR[a.status] ?? '#888' }}>{a.status}</span></td>
                    <td>
                      <div className="acoes">
                        {podeVerAdmin && <button className="btn-ico editar" onClick={() => abrirEditar(a)}><MdEdit size={17}/></button>}
                        {isCliente && a.status === 'Pendente' && <button className="btn-ico deletar" onClick={() => cancelar(a.id)}><MdCancel size={17}/></button>}
                        {isAdmin && <button className="btn-ico deletar" onClick={() => deletar(a.id)}><MdDelete size={17}/></button>}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

      {modal && (
        <Modal titulo={editando ? 'Editar Agendamento' : 'Novo Agendamento'} onFechar={() => setModal(false)}>
          <form onSubmit={salvar} className="form-modal">
            {!editando && podeVerAdmin && (
              <label>Cliente
                <select value={form.clienteId} onChange={campo('clienteId')} required>
                  <option value="">Selecione...</option>
                  {clientes.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
                </select>
              </label>
            )}
            {!editando && (
              <>
                <label>Pet
                  <select value={form.petId} onChange={campo('petId')} required>
                    <option value="">Selecione...</option>
                    {pets.map(p => <option key={p.id} value={p.id}>{p.nome} ({p.tipo})</option>)}
                  </select>
                </label>
                <label>Serviço
                  <select value={form.servicoId} onChange={campo('servicoId')} required>
                    <option value="">Selecione...</option>
                    {servicos.map(s => <option key={s.id} value={s.id}>{s.nome} — R$ {Number(s.preco).toFixed(2)}</option>)}
                  </select>
                </label>
              </>
            )}

            <div className="form-row">
              <label>Data
                <input
                  type="date"
                  min={hoje}
                  value={form.data}
                  onChange={campo('data')}
                  required
                />
              </label>
              <label>Horário
                <select
                  value={form.hora}
                  onChange={campo('hora')}
                  required
                  disabled={!form.data}
                >
                  <option value="">{form.data ? 'Selecione...' : 'Escolha a data primeiro'}</option>
                  {SLOTS.map(slot => {
                    const disponivel = slotDisponivel(slot)
                    return (
                      <option key={slot} value={slot} disabled={!disponivel}>
                        {slot}{!disponivel ? ' — ocupado' : ''}
                      </option>
                    )
                  })}
                </select>
              </label>
            </div>

            {form.data && slotsOcupados.length > 0 && (
              <p className="form-aviso">
                🔒 Horários ocupados neste dia: <strong>{slotsOcupados.join('  |  ')}</strong>
              </p>
            )}

            {form.data && slotsDisponiveis.length === 0 && (
              <p className="form-erro">⚠️ Não há horários disponíveis neste dia.</p>
            )}

            {editando && podeVerAdmin && (
              <label>Status
                <select value={form.status} onChange={campo('status')}>
                  {STATUS_LIST.map(s => <option key={s}>{s}</option>)}
                </select>
              </label>
            )}
            <label>Observações<textarea rows={3} value={form.observacoes} onChange={campo('observacoes')}/></label>
            {erro && <p className="form-erro">{erro}</p>}
            <div className="form-footer">
              <button type="button" className="btn-secondary" onClick={() => setModal(false)}>Cancelar</button>
              <button type="submit" className="btn-primary" disabled={salvando || slotsDisponiveis.length === 0}>
                {salvando ? 'Salvando...' : 'Salvar'}
              </button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}
