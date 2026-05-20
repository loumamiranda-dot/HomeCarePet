import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { authAPI } from '../../services/authAPI'
import { MdEmail, MdLock, MdPerson, MdPhone, MdLocationOn, MdVisibility, MdVisibilityOff } from 'react-icons/md'
import styles from './Login.module.css'

export default function Login() {
  const [modo, setModo] = useState('login')
  const [carregando, setCarregando] = useState(false)
  const [erro, setErro] = useState('')
  const [form, setForm] = useState({ nome: '', email: '', senha: '', confirmarSenha: '', telefone: '', endereco: '' })
  const [verSenha, setVerSenha] = useState(false)
  const [verConfirmar, setVerConfirmar] = useState(false)
  const { entrar } = useAuth()
  const navigate = useNavigate()

  function set(campo) {
    return (e) => {
      setForm((f) => ({ ...f, [campo]: e.target.value }))
      setErro('')
    }
  }

  async function handleSubmit(e) {
    e.preventDefault()
    setErro('')

    if (modo === 'cadastro' && form.senha !== form.confirmarSenha) {
      setErro('As senhas não coincidem.')
      return
    }

    setCarregando(true)
    try {
      let usuarioLogado
      if (modo === 'login') {
        usuarioLogado = await entrar(form.email, form.senha)
      } else {
        await authAPI.registrar({ nome: form.nome, email: form.email, senha: form.senha, telefone: form.telefone, endereco: form.endereco })
        usuarioLogado = await entrar(form.email, form.senha)
      }
      navigate(usuarioLogado?.role === 'Administrador' ? '/admin/dashboard' : '/app/dashboard')
    } catch (err) {
      setErro(err.response?.data?.message ?? err.response?.data ?? 'Dados inválidos. Tente novamente.')
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className={styles.page}>

      <div className={styles.esquerda}>
        <div className={styles.logoBloco}>
          <img src="/logo.png" alt="Vanessa Terceti Pet Sitter" className={styles.logoImg} />
          <p className={styles.slogan}>
            Cuidado e amor para o<br/>seu melhor amigo 🐾
          </p>
        </div>
      </div>

      <div className={styles.direita}>
        <div className={styles.card}>
          <div className={styles.tabs}>
            <button
              className={`${styles.tab} ${modo === 'login' ? styles.tabAtivo : ''}`}
              onClick={() => { setModo('login'); setErro('') }}
            >
              Entrar
            </button>
            <button
              className={`${styles.tab} ${modo === 'cadastro' ? styles.tabAtivo : ''}`}
              onClick={() => { setModo('cadastro'); setErro('') }}
            >
              Cadastrar
            </button>
          </div>

          <form onSubmit={handleSubmit} className={styles.form}>
            {modo === 'cadastro' && (
              <div className={styles.inputWrap}>
                <MdPerson className={styles.inputIco} />
                <input type="text" placeholder="Nome completo" value={form.nome} onChange={set('nome')} required />
              </div>
            )}

            <div className={styles.inputWrap}>
              <MdEmail className={styles.inputIco} />
              <input type="email" placeholder="E-mail" value={form.email} onChange={set('email')} required />
            </div>

            <div className={styles.inputWrap}>
              <MdLock className={styles.inputIco} />
              <input
                type={verSenha ? 'text' : 'password'}
                placeholder="Senha"
                value={form.senha}
                onChange={set('senha')}
                required
              />
              <button type="button" className={styles.olhoBtn} onClick={() => setVerSenha(v => !v)}>
                {verSenha ? <MdVisibilityOff size={18}/> : <MdVisibility size={18}/>}
              </button>
            </div>

            {modo === 'cadastro' && (
              <>
                <div className={styles.inputWrap}>
                  <MdLock className={styles.inputIco} />
                  <input
                    type={verConfirmar ? 'text' : 'password'}
                    placeholder="Confirmar senha"
                    value={form.confirmarSenha}
                    onChange={set('confirmarSenha')}
                    required
                  />
                  <button type="button" className={styles.olhoBtn} onClick={() => setVerConfirmar(v => !v)}>
                    {verConfirmar ? <MdVisibilityOff size={18}/> : <MdVisibility size={18}/>}
                  </button>
                </div>

                <div className={styles.inputWrap}>
                  <MdPhone className={styles.inputIco} />
                  <input type="tel" placeholder="Telefone" value={form.telefone} onChange={set('telefone')} required />
                </div>
                <div className={styles.inputWrap}>
                  <MdLocationOn className={styles.inputIco} />
                  <input type="text" placeholder="Endereço" value={form.endereco} onChange={set('endereco')} required />
                </div>
              </>
            )}

            {erro && <p className={styles.erro}>{erro}</p>}

            <button type="submit" className="btn-primary btn-full" disabled={carregando}>
              {carregando ? 'Aguarde...' : modo === 'login' ? 'Entrar' : 'Criar conta'}
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}
