import { useEffect, useState } from 'react'
import { servicoAPI } from '../../services/servicoAPI'
import Modal from '../../componentes/Modal/Modal'
import { MdAdd, MdEdit, MdDelete, MdMiscellaneousServices } from 'react-icons/md'
import styles from './Servicos.module.css'

const VAZIO = { nome: '', descricao: '', preco: '', duracaoEmMinutos: '' }

export default function Servicos() {
  const [servicos, setServicos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [modal, setModal] = useState(false)
  const [editando, setEditando] = useState(null)
  const [form, setForm] = useState(VAZIO)
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState('')

  async function carregar() {
    setCarregando(true)
    try { setServicos((await servicoAPI.listar()).data ?? []) }
    finally { setCarregando(false) }
  }
  useEffect(() => { carregar() }, [])

  function abrirCriar() { setEditando(null); setForm(VAZIO); setErro(''); setModal(true) }
  function abrirEditar(s) { setEditando(s); setForm({ nome: s.nome, descricao: s.descricao, preco: s.preco, duracaoEmMinutos: s.duracaoEmMinutos }); setErro(''); setModal(true) }

  async function salvar(e) {
    e.preventDefault(); setSalvando(true); setErro('')
    try {
      const dados = { ...form, preco: Number(form.preco), duracaoEmMinutos: Number(form.duracaoEmMinutos) }
      editando ? await servicoAPI.atualizar(editando.id, dados) : await servicoAPI.criar(dados)
      setModal(false); carregar()
    } catch (err) { setErro(err.response?.data?.message ?? 'Erro ao salvar.') }
    finally { setSalvando(false) }
  }

  async function deletar(id) {
    if (!confirm('Remover serviço?')) return
    try { await servicoAPI.deletar(id); carregar() } catch { alert('Erro ao remover.') }
  }

  const campo = (k) => (e) => setForm(f => ({ ...f, [k]: e.target.value }))

  return (
    <div className="pagina">
      <div className="pagina-header">
        <div>
          <h1 className="pagina-titulo">Serviços</h1>
          <p className="pagina-sub">{servicos.length} serviço(s)</p>
        </div>
        <button className="btn-primary" onClick={abrirCriar}><MdAdd size={20}/> Novo Serviço</button>
      </div>

      {carregando ? <div className="loading-full"><div className="spinner"/></div>
        : servicos.length === 0 ? <div className="vazio"><MdMiscellaneousServices size={44}/><p>Nenhum serviço cadastrado</p></div>
        : (
          <div className={styles.grid}>
            {servicos.map(s => (
              <div key={s.id} className={`${styles.card} ${!s.ativo ? styles.inativo : ''}`}>
                <div className={styles.topo}>
                  <h3>{s.nome}</h3>
                  <span className={`badge ${s.ativo ? 'verde' : 'vermelho'}`}>{s.ativo ? 'Ativo' : 'Inativo'}</span>
                </div>
                <p className={styles.desc}>{s.descricao}</p>
                <div className={styles.rodape}>
                  <span className={styles.preco}>R$ {Number(s.preco).toFixed(2)}</span>
                  <span className={styles.dur}>{s.duracaoEmMinutos} min</span>
                </div>
                <div className="acoes">
                  <button className="btn-ico editar" onClick={() => abrirEditar(s)}><MdEdit size={17}/></button>
                  <button className="btn-ico deletar" onClick={() => deletar(s.id)}><MdDelete size={17}/></button>
                </div>
              </div>
            ))}
          </div>
        )}

      {modal && (
        <Modal titulo={editando ? 'Editar Serviço' : 'Novo Serviço'} onFechar={() => setModal(false)}>
          <form onSubmit={salvar} className="form-modal">
            <label>Nome<input value={form.nome} onChange={campo('nome')} required/></label>
            <label>Descrição<textarea rows={3} value={form.descricao} onChange={campo('descricao')}/></label>
            <div className="form-row">
              <label>Preço (R$)<input type="number" min="0" step="0.01" value={form.preco} onChange={campo('preco')} required/></label>
              <label>Duração (min)<input type="number" min="1" value={form.duracaoEmMinutos} onChange={campo('duracaoEmMinutos')} required/></label>
            </div>
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
