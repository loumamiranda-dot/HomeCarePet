import { useState } from 'react'
import { Outlet } from 'react-router-dom'
import { MdMenu } from 'react-icons/md'
import Sidebar from '../Sidebar/Sidebar'
import styles from './Layout.module.css'

export default function Layout() {
  const [aberta, setAberta] = useState(false)

  return (
    <div className={styles.layout}>
      <Sidebar aberto={aberta} fechar={() => setAberta(false)} />
      <div className={styles.layoutMain}>
        <header className={styles.topbar}>
          <button className={styles.topbarMenuBtn} onClick={() => setAberta(true)}>
            <MdMenu size={26} />
          </button>
          <img src="/logo.png" alt="Vanessa Terceti Pet Sitter" style={{height:'42px', width:'auto', cursor:'pointer'}} onClick={() => navigate('/')} />
        </header>
        <main className={styles.pageContent}>
          <Outlet />
        </main>
      </div>
    </div>
  )
}
