import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import {
  MdDashboard, MdPeople, MdPets,
  MdCalendarMonth, MdMiscellaneousServices, MdLogout, MdClose,
} from 'react-icons/md'
import styles from './Sidebar.module.css'

export default function Sidebar({ aberto, fechar }) {
  const { usuario, sair, isAdmin, isFuncionario } = useAuth()
  const navigate = useNavigate()
  const podeVerAdmin = isAdmin || isFuncionario

  function handleSair() {
    sair()
    navigate('/')
  }

  return (
    <>
      {aberto && <div className={styles.overlay} onClick={fechar} />}
      <aside className={`${styles.sidebar} ${aberto ? styles.aberta : ''}`}>

        <div className={styles.logoWrap}>
          <img
            src="/logo.png"
            alt="Vanessa Terceti Pet Sitter"
            className={styles.logoImg}
            style={{ cursor: 'pointer' }}
            onClick={() => { navigate('/'); fechar() }}
          />
          <button className={styles.closeBtn} onClick={fechar}><MdClose size={20}/></button>
        </div>

        <div className={styles.usuarioBox}>
          <div className={styles.avatarCircle}>{usuario?.nome?.charAt(0).toUpperCase()}</div>
          <div>
            <p className={styles.usuarioNome}>{usuario?.nome}</p>
            <p className={styles.usuarioRole}>{usuario?.role}</p>
          </div>
        </div>

        <nav className={styles.nav}>
          <NavLink to="/app/dashboard" className={({ isActive }) => `${styles.navLink} ${isActive ? styles.navLinkAtivo : ''}`} onClick={fechar}>
            <MdDashboard size={19}/><span>Dashboard</span>
          </NavLink>

          {podeVerAdmin && (
            <NavLink to="/app/clientes" className={({ isActive }) => `${styles.navLink} ${isActive ? styles.navLinkAtivo : ''}`} onClick={fechar}>
              <MdPeople size={19}/><span>Clientes</span>
            </NavLink>
          )}

          <NavLink to="/app/pets" className={({ isActive }) => `${styles.navLink} ${isActive ? styles.navLinkAtivo : ''}`} onClick={fechar}>
            <MdPets size={19}/><span>Pets</span>
          </NavLink>

          <NavLink to="/app/agendamentos" className={({ isActive }) => `${styles.navLink} ${isActive ? styles.navLinkAtivo : ''}`} onClick={fechar}>
            <MdCalendarMonth size={19}/><span>Agendamentos</span>
          </NavLink>

          {podeVerAdmin && (
            <NavLink to="/app/servicos" className={({ isActive }) => `${styles.navLink} ${isActive ? styles.navLinkAtivo : ''}`} onClick={fechar}>
              <MdMiscellaneousServices size={19}/><span>Serviços</span>
            </NavLink>
          )}
        </nav>

        <button className={styles.sairBtn} onClick={handleSair}>
          <MdLogout size={18}/><span>Sair</span>
        </button>
      </aside>
    </>
  )
}
