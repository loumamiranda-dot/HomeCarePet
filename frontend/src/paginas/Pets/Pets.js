import { useEffect, useState } from 'react'
import { petAPI } from '../../services/petAPI'
import { clienteAPI } from '../../services/clienteAPI'
import { useAuth } from '../../contexts/AuthContext'
import Modal from '../../componentes/Modal/Modal'
import { MdAdd, MdEdit, MdDelete, MdPets } from 'react-icons/md'
import styles from './Pets.module.css'

const TIPOS = ['Cachorro', 'Gato', 'Ave', 'Roedor', 'Reptil', 'Outro']
const TIPO_ENUM = { Cachorro: 1, Gato: 2, Ave: 3, Roedor: 4, Reptil: 5, Outro: 6 }
const EMOJI = { Cachorro: '🐕', Gato: '🐈', Ave: '🐦', Roedor: '🐭', Reptil: '🦎', Outro: '🐾' }
const VAZIO = { nome: '', tipo: 'Cachorro', raca: '', idade: '', peso: '', observacoes: '', clienteId: '' }

export default function Pets() {
  const { isAdmin, isFuncionario, isCliente } = useAuth()
  const podeVerAdmin = isAdmin || isFuncionario

  const [pets, setPets] = useState([])
  const [clientes, setClientes] = useState([])
  const [clienteIdProprio, setClienteIdProprio] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [busca, setBusca] = useState('')
  const [modal, setModal] = useState(false)
  const [editando, setEditando] = useState(null)
  const [form, setForm] = useState(VAZIO)
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState('')

  async function carregar() {
    setCarregando(true)
    try {
      const petsRes = isCliente ? await petAPI.meusPets() : await petAPI.listar()
      // console.log('pets carregados:', petsRes.data)
      setPets(petsRes.data ?? [])
      if (podeVerAdmin) {
        const clientesRes = await clienteAPI.listar()
        setClientes(clientesRes.data ?? [])
      }
      if (isCliente && !clienteIdProprio) {
        const perfil = await clienteAPI.meuPerfil()
        setClienteIdProprio(perfil.data.id)
      }
    } finally { setCarregando(false) }
  }

  useEffect(() => { carregar() }, [])

  function abrirCriar() { setEditando(null); setForm(VAZIO); setErro(''); setModal(true) }
  function abrirEditar(p) {
    setEditando(p)
    setForm({ nome: p.nome, tipo: p.tipo, raca: p.raca, idade: p.idade, peso: p.peso, observacoes: p.observacoes ?? '', clienteId: p.clienteId })
    setErro(''); setModal(true)
  }

  async function salvar(e) {
    e.preventDefault(); setSalvando(true); setErro('')
    try {
      const cId = isCliente ? clienteIdProprio : Number(form.clienteId)
      const dados = {
        nome: form.nome,
        tipo: TIPO_ENUM[form.tipo] ?? 1,
        raca: form.raca,
        idade: Number(form.idade),
        peso: Number(form.peso),
        observacoes: form.observacoes ?? '',
        clienteId: cId,
      }
      editando ? await petAPI.atualizar(editando.id, dados) : await petAPI.criar(dados)
      setModal(false); carregar()
    } catch (err) {
      const d = err.response?.data
      const msg = d?.message ?? d?.title ?? (typeof d === 'string' ? d : null) ?? 'Erro ao salvar. Verifique os dados.'
      setErro(msg)
    } finally { setSalvando(false) }
  }

  async function deletar(id) {
    if (!confirm('Remover pet?')) return
    try { await petAPI.deletar(id); carregar() } catch { alert('Erro ao remover.') }
  }

  const campo = (k) => (e) => setForm(f => ({ ...f, [k]: e.target.value }))

  const filtrados = pets.filter(p =>
    p.nome?.toLowerCase().includes(busca.toLowerCase()) ||
    p.tipo?.toLowerCase().includes(busca.toLowerCase()) ||
    p.raca?.toLowerCase().includes(busca.toLowerCase())
  )

  return (
    <div className="pagina">
      <div className="pagina-header">
        <div>
          <h1 className="pagina-titulo">Pets</h1>
          <p className="pagina-sub">{pets.length} pet(s)</p>
        </div>
        <button className="btn-primary" onClick={abrirCriar}><MdAdd size={20}/> Novo Pet</button>
      </div>

      <div className="barra-busca">
        <MdPets size={20} className="busca-ico"/>
        <input placeholder="Buscar por nome, tipo ou raça..." value={busca} onChange={e => setBusca(e.target.value)}/>
      </div>

      {carregando ? <div className="loading-full"><div className="spinner"/></div>
        : filtrados.length === 0 ? <div className="vazio"><MdPets size={44}/><p>Nenhum pet encontrado</p></div>
        : (
          <div className={styles.grid}>
            {filtrados.map(p => (
              <div key={p.id} className={styles.card}>
                <div className={styles.emoji}>{EMOJI[p.tipo] ?? '🐾'}</div>
                <div className={styles.info}>
                  <h3>{p.nome}</h3>
                  <p className={styles.tipo}>{p.tipo} · {p.raca}</p>
                  <p className={styles.det}>{p.idade} ano(s) · {p.peso} kg</p>
                  {p.observacoes && <p className={styles.obs}>{p.observacoes}</p>}
                </div>
                <div className="acoes">
                  <button className="btn-ico editar" onClick={() => abrirEditar(p)}><MdEdit size={17}/></button>
                  <button className="btn-ico deletar" onClick={() => deletar(p.id)}><MdDelete size={17}/></button>
                </div>
              </div>
            ))}
          </div>
        )}

      {modal && (
        <Modal titulo={editando ? 'Editar Pet' : 'Novo Pet'} onFechar={() => setModal(false)}>
          <form onSubmit={salvar} className="form-modal">
            <label>Nome<input value={form.nome} onChange={campo('nome')} required/></label>
            <label>Tipo
              <select value={form.tipo} onChange={campo('tipo')}>
                {TIPOS.map(t => <option key={t}>{t}</option>)}
              </select>
            </label>
            <label>Raça<input value={form.raca} onChange={campo('raca')}/></label>
            <div className="form-row">
              <label>Idade (anos)<input type="number" min="0" value={form.idade} onChange={campo('idade')} required/></label>
              <label>Peso (kg)<input type="number" min="0" step="0.1" value={form.peso} onChange={campo('peso')} required/></label>
            </div>
            {podeVerAdmin && (
              <label>Cliente
                <select value={form.clienteId} onChange={campo('clienteId')} required>
                  <option value="">Selecione...</option>
                  {clientes.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
                </select>
              </label>
            )}
            <label>Observações<textarea rows={3} value={form.observacoes} onChange={campo('observacoes')}/></label>
            {erro && <p className="form-erro">{erro}</p>}
            <div className="form-footer">
              <button type="button" className="btn-secondary" onClick={() => setModal(false)}>Cancelar</button>
              <button type="submit" className="btn-primary" disabled={salvando}>{salvando ? 'Salvando...' : 'Salvar'}</button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}
