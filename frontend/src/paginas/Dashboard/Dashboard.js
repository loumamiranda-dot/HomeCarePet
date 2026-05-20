import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { agendamentoAPI } from '../../services/agendamentoAPI'
import { clienteAPI } from '../../services/clienteAPI'
import { petAPI } from '../../services/petAPI'
import { servicoAPI } from '../../services/servicoAPI'
import { sugestaoAPI } from '../../services/sugestaoAPI'
import { MdPeople, MdPets, MdCalendarToday, MdMiscellaneousServices, MdEdit, MdCancel, MdAutoAwesome, MdRefresh, MdCheckCircle } from 'react-icons/md'

const COR_STATUS = { Pendente: '#F59E0B', Confirmado: '#10B981', Finalizado: '#6B7280', Cancelado: '#EF4444' }
const STATUS_LIST = ['Pendente', 'Confirmado', 'Finalizado', 'Cancelado']
const STATUS_ENUM = { Pendente: 1, Confirmado: 2, Finalizado: 3, Cancelado: 4 }

const todayStr = new Date().toDateString()

function isHoje(dataHora) {
  return new Date(dataHora).toDateString() === todayStr
}

export default function Dashboard() {
  const { usuario, isAdmin, isFuncionario, isCliente } = useAuth()
  const podeVerAdmin = isAdmin || isFuncionario
  const navigate = useNavigate()

  const [resumo, setResumo] = useState({ clientes: 0, pets: 0, agendamentos: 0, servicos: 0 })
  const [agendamentos, setAgendamentos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [alterando, setAlterando] = useState(null)
  const [sugestoes, setSugestoes] = useState(null)
  const [carregandoIA, setCarregandoIA] = useState(false)
  const [erroIA, setErroIA] = useState('')

  // carrega os dados do dashboard
  async function carregar() {
    setCarregando(true)
    try {
      if (podeVerAdmin) {
        const [c, p, a, s] = await Promise.allSettled([
          clienteAPI.listar(1, 1000),
          petAPI.listar(),
          agendamentoAPI.listar(1, 1000),
          servicoAPI.listar(),
        ])
        const ags = a.value?.data ?? []
        setResumo({
          clientes: c.value?.data?.length ?? 0,
          pets: p.value?.data?.length ?? 0,
          agendamentos: ags.length,
          servicos: s.value?.data?.length ?? 0,
        })
        setAgendamentos(ags)
      } else {
        const [p, a] = await Promise.allSettled([
          petAPI.meusPets(),
          agendamentoAPI.meusAgendamentos(),
        ])
        const ags = a.value?.data ?? []
        setResumo({ pets: p.value?.data?.length ?? 0, agendamentos: ags.length })
        setAgendamentos(ags)
      }
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregar()
    if (isCliente) buscarSugestoes() // so busca sugestoes se for cliente
  }, [podeVerAdmin]) // eslint-disable-line

  async function buscarSugestoes() {
    setCarregandoIA(true); setErroIA('')
    try {
      const res = await sugestaoAPI.minhasSugestoes()
      console.log('sugestoes recebidas:', res.data)
      setSugestoes(res.data)
    } catch (err) {
      console.log('erro ao buscar sugestoes', err)
      setErroIA(err.response?.data?.message ?? 'Não foi possível gerar sugestões agora.')
    } finally { setCarregandoIA(false) }
  }

  async function alterarStatus(ag, novoStatus) {
    setAlterando(ag.id)
    try {
      const dt = new Date(ag.dataHora)
      const pad = n => String(n).padStart(2, '0')
      const dataHora = `${dt.getFullYear()}-${pad(dt.getMonth()+1)}-${pad(dt.getDate())}T${pad(dt.getHours())}:${pad(dt.getMinutes())}`
      await agendamentoAPI.atualizar(ag.id, {
        dataHora,
        status: STATUS_ENUM[novoStatus] ?? 1,
        observacoes: ag.observacoes ?? '',
      })
      carregar()
    } catch { alert('Erro ao alterar status.') }
    finally { setAlterando(null) }
  }

  async function cancelar(id) {
    if (!confirm('Cancelar este agendamento?')) return
    try { await agendamentoAPI.cancelar(id); carregar() }
    catch { alert('Erro ao cancelar.') }
  }

  if (carregando) return <div className="loading-full"><div className="spinner" /></div>

  const ordenados = [...agendamentos].sort((a, b) => new Date(a.dataHora) - new Date(b.dataHora))

  const prefixo = isAdmin ? '/admin' : '/app'

  return (
    <div className="pagina">
      <div className="pagina-header">
        <div>
          <h1 className="pagina-titulo">Olá, {usuario?.nome?.split(' ')[0]} 👋</h1>
          <p className="pagina-sub">Bem-vindo ao sistema Vanessa Terceti Pet Sitter</p>
        </div>
      </div>

      {/* cards com os numeros */}
      <div className="stat-grid">
        {podeVerAdmin && (
          <div className="stat-card purple" style={{ cursor: 'pointer' }} onClick={() => navigate(`${prefixo}/clientes`)}>
            <MdPeople size={30}/>
            <div>
              <p className="stat-numero">{resumo.clientes}</p>
              <p className="stat-label">Clientes</p>
            </div>
          </div>
        )}
        <div className="stat-card pink" style={{ cursor: 'pointer' }} onClick={() => navigate(`${prefixo}/pets`)}>
          <MdPets size={30}/>
          <div>
            <p className="stat-numero">{resumo.pets}</p>
            <p className="stat-label">{isCliente ? 'Meus Pets' : 'Pets'}</p>
          </div>
        </div>
        <div className="stat-card purple-light" style={{ cursor: 'pointer' }} onClick={() => navigate(`${prefixo}/agendamentos`)}>
          <MdCalendarToday size={30}/>
          <div>
            <p className="stat-numero">{resumo.agendamentos}</p>
            <p className="stat-label">{isCliente ? 'Meus Agendamentos' : 'Agendamentos'}</p>
          </div>
        </div>
        {podeVerAdmin && (
          <div className="stat-card pink-light" style={{ cursor: 'pointer' }} onClick={() => navigate(`${prefixo}/servicos`)}>
            <MdMiscellaneousServices size={30}/>
            <div>
              <p className="stat-numero">{resumo.servicos}</p>
              <p className="stat-label">Serviços</p>
            </div>
          </div>
        )}
      </div>

      {/* sugestoes da ia - so aparece pro cliente */}
      {isCliente && (
        <div className="secao" style={{ marginBottom: 24 }}>
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 16 }}>
            <h2 className="secao-titulo" style={{ margin: 0 }}>
              <MdAutoAwesome size={18} style={{ color: '#D63FA0' }}/> Pacotes Recomendados
            </h2>
            <button className="btn-secondary" onClick={buscarSugestoes} disabled={carregandoIA} style={{ padding: '6px 12px', fontSize: 12 }}>
              <MdRefresh size={15}/>
              {carregandoIA ? 'Gerando...' : 'Atualizar'}
            </button>
          </div>

          {carregandoIA && (
            <div style={{ textAlign: 'center', padding: '32px 0', color: 'var(--texto-sub)' }}>
              <div className="spinner" style={{ margin: '0 auto 12px' }}/>
              <p style={{ fontSize: 13 }}>Carregando sugestoes...</p>
            </div>
          )}

          {!carregandoIA && erroIA && (
            <p style={{ color: '#EF4444', fontSize: 13 }}>{erroIA}</p>
          )}

          {!carregandoIA && sugestoes && (
            <>
              {sugestoes.observacoes && (
                <p style={{ background: 'var(--roxo-claro)', borderRadius: 8, padding: '12px 16px', marginBottom: 16, fontSize: 13, color: 'var(--roxo)' }}>
                  <strong>Analise:</strong> {sugestoes.observacoes}
                </p>
              )}
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: 14 }}>
                {sugestoes.pacotes?.map((p, i) => (
                  <div key={i} className="secao" style={{ padding: 16, borderTop: '3px solid var(--roxo)' }}>
                    <h4 style={{ fontSize: 14, fontWeight: 700, marginBottom: 8 }}>{p.nomePacote}</h4>
                    <p style={{ fontSize: 11, color: 'var(--texto-sub)', marginBottom: 8 }}>{p.frequenciaRecomendada}</p>

                    <div style={{ marginBottom: 8 }}>
                      {p.servicosIncluidos?.map(s => (
                        <span key={s} style={{ display: 'inline-block', background: 'var(--cinza-bg)', borderRadius: 6, padding: '3px 8px', fontSize: 12, marginRight: 4, marginBottom: 4 }}>
                          <MdCheckCircle size={12} style={{ color: '#10B981', verticalAlign: 'middle' }}/> {s}
                        </span>
                      ))}
                    </div>

                    <p style={{ fontSize: 12, color: 'var(--texto-sub)', marginBottom: 10 }}>{p.justificativa}</p>

                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <strong style={{ color: 'var(--roxo)' }}>R$ {Number(p.precoEstimado).toFixed(2)}</strong>
                      <button className="btn-primary" style={{ padding: '6px 14px', fontSize: 12 }} onClick={() => navigate('/app/agendamentos')}>
                        Agendar
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}
        </div>
      )}

      {/* tabela com todos os agendamentos */}
      <div className="secao">
        <h2 className="secao-titulo"><MdCalendarToday size={18}/> Todos os Agendamentos</h2>
        {ordenados.length === 0 ? (
          <div className="vazio"><MdPets size={44}/><p>Nenhum agendamento encontrado</p></div>
        ) : (
          <div className="table-wrap">
            <table className="tabela">
              <thead>
                <tr>
                  <th>Pet</th>
                  {podeVerAdmin && <th>Cliente</th>}
                  <th>Serviço</th>
                  <th>Data</th>
                  <th>Horário</th>
                  <th>Status</th>
                  {podeVerAdmin && <th>Ações</th>}
                </tr>
              </thead>
              <tbody>
                {ordenados.map(a => (
                  <tr key={a.id} style={isHoje(a.dataHora) ? { background: '#FDF0F7' } : {}}>
                    <td><strong>{a.nomePet}</strong></td>
                    {podeVerAdmin && <td>{a.nomeCliente}</td>}
                    <td>{a.nomeServico}</td>
                    <td>
                      {new Date(a.dataHora).toLocaleDateString('pt-BR')}
                      {isHoje(a.dataHora) && (
                        <span style={{ marginLeft: 6, fontSize: 10, fontWeight: 700, background: '#7B2D8B', color: '#fff', borderRadius: 99, padding: '2px 7px' }}>HOJE</span>
                      )}
                    </td>
                    <td>{new Date(a.dataHora).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}</td>
                    <td>
                      {podeVerAdmin && a.status !== 'Cancelado' ? (
                        <select
                          value={a.status}
                          disabled={alterando === a.id}
                          onChange={e => alterarStatus(a, e.target.value)}
                          style={{
                            background: COR_STATUS[a.status] || '#888',
                            color: '#fff', border: 'none', borderRadius: 99,
                            padding: '3px 10px', fontSize: 11, fontWeight: 600,
                            cursor: 'pointer', fontFamily: 'Poppins, sans-serif',
                          }}
                        >
                          {STATUS_LIST.filter(s => s !== 'Cancelado').map(s => (
                            <option key={s} value={s} style={{ background: '#fff', color: '#333' }}>{s}</option>
                          ))}
                        </select>
                      ) : (
                        <span className="badge" style={{ background: COR_STATUS[a.status] || '#888' }}>{a.status}</span>
                      )}
                    </td>
                    {podeVerAdmin && (
                      <td>
                        <div className="acoes">
                          <button
                            className="btn-ico editar"
                            title="Editar"
                            onClick={() => navigate(`${prefixo}/agendamentos`)}
                          >
                            <MdEdit size={16}/>
                          </button>
                          {a.status !== 'Cancelado' && a.status !== 'Finalizado' && (
                            <button
                              className="btn-ico deletar"
                              title="Cancelar"
                              onClick={() => cancelar(a.id)}
                            >
                              <MdCancel size={16}/>
                            </button>
                          )}
                        </div>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
