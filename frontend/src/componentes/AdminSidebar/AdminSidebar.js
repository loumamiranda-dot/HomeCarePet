import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import {
  MdDashboard, MdPeople, MdPets,
  MdCalendarMonth, MdMiscellaneousServices, MdLogout, MdClose, MdAdminPanelSettings,
} from 'react-icons/md'
import styles from '../Sidebar/Sidebar.module.css'

export default function AdminSidebar({ aberto, fechar }) {
  const { usuario, sair } = useAuth()
  const navigate = useNavigate()

  function handleSair() {
    sair()
    navigate('/')
  }

  const link = (to) => ({ isActive }) =>
    `${styles.navLink} ${isActive ? styles.navLinkAtivo : ''}`

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
            <p className={styles.usuarioRole}><MdAdminPanelSettings size={12}/> Administrador</p>
          </div>
        </div>

        <nav className={styles.nav}>
          <NavLink to="/admin/dashboard" className={link('/admin/dashboard')} onClick={fechar}>
            <MdDashboard size={19}/><span>Dashboard</span>
          </NavLink>
          <NavLink to="/admin/clientes" className={link('/admin/clientes')} onClick={fechar}>
            <MdPeople size={19}/><span>Clientes</span>
          </NavLink>
          <NavLink to="/admin/pets" className={link('/admin/pets')} onClick={fechar}>
            <MdPets size={19}/><span>Pets</span>
          </NavLink>
          <NavLink to="/admin/agendamentos" className={link('/admin/agendamentos')} onClick={fechar}>
            <MdCalendarMonth size={19}/><span>Agendamentos</span>
          </NavLink>
          <NavLink to="/admin/servicos" className={link('/admin/servicos')} onClick={fechar}>
            <MdMiscellaneousServices size={19}/><span>Serviços</span>
          </NavLink>
        </nav>

        <button className={styles.sairBtn} onClick={handleSair}>
          <MdLogout size={18}/><span>Sair</span>
        </button>
      </aside>
    </>
  )
}
