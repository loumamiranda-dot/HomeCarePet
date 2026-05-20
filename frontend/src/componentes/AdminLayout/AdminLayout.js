


import { useState } from 'react'
import { Outlet, useNavigate } from 'react-router-dom'
import { MdMenu, MdAdminPanelSettings } from 'react-icons/md'
import AdminSidebar from '../AdminSidebar/AdminSidebar'
import styles from '../Layout/Layout.module.css'
import adminStyles from './AdminLayout.module.css'

export default function AdminLayout() {
  const [aberta, setAberta] = useState(false)
  const navigate = useNavigate()

  return (
    <div className={styles.layout}>
      <AdminSidebar aberto={aberta} fechar={() => setAberta(false)} />
      <div className={styles.layoutMain}>
        <header className={styles.topbar}>
          <button className={styles.topbarMenuBtn} onClick={() => setAberta(true)}>
            <MdMenu size={26} />
          </button>
          <img src="/logo.png" alt="Vanessa Terceti Pet Sitter" style={{height:'42px', width:'auto', cursor:'pointer'}} onClick={() => navigate('/')} />
          <span className={adminStyles.adminBadge}>
            <MdAdminPanelSettings size={15}/> Administrador
          </span>
        </header>
        <main className={styles.pageContent}>
          <Outlet />
        </main>
      </div>
    </div>
  )
}
