import { useEffect, useState } from 'react'
import { clienteAPI } from '../../services/clienteAPI'
import Modal from '../../componentes/Modal/Modal'
import { MdAdd, MdEdit, MdDelete, MdPeople, MdSearch } from 'react-icons/md'

const VAZIO = { nome: '', email: '', telefone: '', endereco: '' }

export default function Clientes() {
  const [clientes, setClientes] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [busca, setBusca] = useState('')
  const [modal, setModal] = useState(false)
  const [editando, setEditando] = useState(null)
  const [form, setForm] = useState(VAZIO)
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState('')

  async function carregar() {
    setCarregando(true)
    try { setClientes((await clienteAPI.listar()).data ?? []) }
    finally { setCarregando(false) }
  }
  useEffect(() => { carregar() }, [])

  function abrirCriar() { setEditando(null); setForm(VAZIO); setErro(''); setModal(true) }
  function abrirEditar(c) { setEditando(c); setForm({ nome: c.nome, email: c.email, telefone: c.telefone, endereco: c.endereco }); setErro(''); setModal(true) }

  async function salvar(e) {
    e.preventDefault(); setSalvando(true); setErro('')
    try {
      editando ? await clienteAPI.atualizar(editando.id, form) : await clienteAPI.criar(form)
      setModal(false); carregar()
    } catch (err) { setErro(err.response?.data?.message ?? 'Erro ao salvar.') }
    finally { setSalvando(false) }
  }

  async function deletar(id) {
    if (!confirm('Remover cliente?')) return
    try { await clienteAPI.deletar(id); carregar() }
    catch { alert('Erro ao remover.') }
  }

  const campo = (k) => (e) => setForm(f => ({ ...f, [k]: e.target.value }))

  const filtrados = clientes.filter(c =>
    c.nome.toLowerCase().includes(busca.toLowerCase()) ||
    c.email.toLowerCase().includes(busca.toLowerCase())
  )

  return (
    <div className="pagina">
      <div className="pagina-header">
        <div>
          <h1 className="pagina-titulo">Clientes</h1>
          <p className="pagina-sub">{clientes.length} cadastrado(s)</p>
        </div>
        <button className="btn-primary" onClick={abrirCriar}><MdAdd size={20}/> Novo Cliente</button>
      </div>

      <div className="barra-busca">
        <MdSearch size={20} className="busca-ico"/>
        <input placeholder="Buscar por nome ou e-mail..." value={busca} onChange={e => setBusca(e.target.value)}/>
      </div>

      {carregando ? <div className="loading-full"><div className="spinner"/></div>
        : filtrados.length === 0 ? <div className="vazio"><MdPeople size={44}/><p>Nenhum cliente encontrado</p></div>
        : (
          <div className="table-wrap">
            <table className="tabela">
              <thead>
                <tr><th>#</th><th>Nome</th><th>E-mail</th><th>Telefone</th><th>Endereço</th><th>Situação</th><th>Ações</th></tr>
              </thead>
              <tbody>
                {filtrados.map(c => (
                  <tr key={c.id}>
                    <td>{c.id}</td>
                    <td><strong>{c.nome}</strong></td>
                    <td>{c.email}</td>
                    <td>{c.telefone}</td>
                    <td>{c.endereco}</td>
                    <td><span className={`badge ${c.ativo ? 'verde' : 'vermelho'}`}>{c.ativo ? 'Ativo' : 'Inativo'}</span></td>
                    <td>
                      <div className="acoes">
                        <button className="btn-ico editar" onClick={() => abrirEditar(c)}><MdEdit size={17}/></button>
                        <button className="btn-ico deletar" onClick={() => deletar(c.id)}><MdDelete size={17}/></button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

      {modal && (
        <Modal titulo={editando ? 'Editar Cliente' : 'Novo Cliente'} onFechar={() => setModal(false)}>
          <form onSubmit={salvar} className="form-modal">
            <label>Nome<input value={form.nome} onChange={campo('nome')} required/></label>
            <label>E-mail<input type="email" value={form.email} onChange={campo('email')} required/></label>
            <label>Telefone<input value={form.telefone} onChange={campo('telefone')} required/></label>
            <label>Endereço<input value={form.endereco} onChange={campo('endereco')} required/></label>
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
