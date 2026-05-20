import { MdClose } from 'react-icons/md'
import styles from './Modal.module.css'

export default function Modal({ titulo, children, onFechar }) {
  return (
    <div className={styles.overlay} onClick={onFechar}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.header}>
          <h2 className={styles.titulo}>{titulo}</h2>
          <button className={styles.fecharBtn} onClick={onFechar}>
            <MdClose size={22} />
          </button>
        </div>
        <div className={styles.body}>{children}</div>
      </div>
    </div>
  )
}
