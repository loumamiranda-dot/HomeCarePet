import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { MdFavorite, MdStar, MdShield, MdPhone, MdFormatQuote, MdPets } from 'react-icons/md'
import { FaWhatsapp } from 'react-icons/fa'
import styles from './Home.module.css'

const SERVICOS = [
  { ico: '🐕', nome: 'Dog Walker', desc: 'Passeios diários com carinho e segurança para o seu cão.' },
  { ico: '🏡', nome: 'Pet Sitting', desc: 'Cuidados no conforto da sua casa enquanto você viaja.' },
  { ico: '🛁', nome: 'Banho & Tosa', desc: 'Higiene completa com produtos premium e muito carinho.' },
]

const PORQUE = [
  { ico: <MdStar size={22}/>, titulo: 'Profissional Certificada', desc: 'Formação em comportamento animal e primeiros socorros para pets.' },
  { ico: <MdShield size={22}/>, titulo: 'Segurança Garantida', desc: 'Acompanhamento em tempo real com fotos e atualizações.' },
  { ico: <MdFavorite size={22}/>, titulo: 'Amor Incondicional', desc: 'Cada pet é tratado como parte da família, com dedicação total.' },
  { ico: <MdPhone size={22}/>, titulo: 'Atendimento Ágil', desc: 'Resposta rápida e agendamento fácil pelo nosso sistema.' },
]

// fallback caso a imagem da logo nao carregue
function LogoFallback({ className }) {
  return (
    <div className={className} style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <MdPets size={48} style={{ color: '#7B2D8B' }} />
    </div>
  )
}

export default function Home() {
  const { usuario } = useAuth()
  const navigate = useNavigate()
  const [logoErro, setLogoErro] = useState(false)

  function irParaApp() {
    if (!usuario) { navigate('/login'); return }
    navigate(usuario.role === 'Administrador' ? '/admin/dashboard' : '/app/dashboard')
  }

  return (
    <>
      <nav className={styles.navbar}>
        {logoErro ? (
          <div className={styles.navBrandFallback}>
            <LogoFallback className={styles.navLogoFallback} />
            <div>
              <p className={styles.navBrandNome}>Vanessa Terceti</p>
              <p className={styles.navBrandSub}>Pet Sitter</p>
            </div>
          </div>
        ) : (
          <img
            src="/logo.png"
            alt="Vanessa Terceti Pet Sitter"
            className={styles.navLogo}
            onError={() => setLogoErro(true)}
          />
        )}
        <div className={styles.navAcoes}>
          {usuario ? (
            <button className="btn-primary" onClick={irParaApp}>
              {usuario.role === 'Administrador' ? 'Painel Admin' : 'Acessar Sistema'}
            </button>
          ) : (
            <>
              <button className={styles.btnOutline} onClick={() => navigate('/login')}>
                Entrar
              </button>
              <button className="btn-primary" onClick={() => navigate('/login')}>
                Cadastrar
              </button>
            </>
          )}
        </div>
      </nav>

      <section className={styles.hero}>
        <div className={styles.heroBg} />
        <div className={styles.heroConteudo}>
          {logoErro ? (
            <>
              <LogoFallback className={styles.heroLogoFallback} />
              <h1 className={styles.heroTitulo}>Vanessa Terceti</h1>
              <p className={styles.heroSub}>Pet Sitter Profissional</p>
            </>
          ) : (
            <img
              src="/logo.png"
              alt="Vanessa Terceti Pet Sitter"
              className={styles.heroLogoImg}
              onError={() => setLogoErro(true)}
            />
          )}

          <p className={styles.heroDesc}>
            Seu pet merece o melhor cuidado quando você não pode estar por perto.
            Passeios, hospedagem, banho e muito carinho — tudo com a segurança
            que você e seu amigo merecem.
          </p>
          <div className={styles.heroBadge}>🐾 Pet Sitter desde 2009</div>
          <div className={styles.heroBotoes}>
            <button className={styles.btnHeroPrimario} onClick={irParaApp}>
              {usuario ? 'Acessar Minha Conta' : 'Agendar Agora'}
            </button>
            <button className={styles.btnHeroSecundario}
              onClick={() => document.getElementById('quem-sou').scrollIntoView({ behavior: 'smooth' })}>
              Quem Sou Eu
            </button>
          </div>
        </div>
        <div className={styles.heroScroll}>
          <span>rolar</span>↓
        </div>
      </section>

      <div className={styles.statsBar}>
        <div className={styles.statItem}>
          <p className={styles.statNum}>500+</p>
          <p className={styles.statLbl}>Pets Atendidos</p>
        </div>
        <div className={styles.statItem}>
          <p className={styles.statNum}>15+</p>
          <p className={styles.statLbl}>Anos de Experiência</p>
        </div>
        <div className={styles.statItem}>
          <p className={styles.statNum}>98%</p>
          <p className={styles.statLbl}>Clientes Satisfeitos</p>
        </div>
        <div className={styles.statItem}>
          <p className={styles.statNum}>7</p>
          <p className={styles.statLbl}>Pets Próprios</p>
        </div>
      </div>

      {/* secao sobre a vanessa */}
      <section id="quem-sou" className={styles.quemSouWrap}>
        <div className={styles.secao}>
          <div className={styles.secaoTitulo}>
            <p className={styles.secaoTag}>Conheça a Vanessa</p>
            <h2>Quem Sou Eu</h2>
          </div>
          <div className={styles.quemSouGrid}>
            <div className={styles.quemSouImgWrap}>
              <img src="/vanessa.png" alt="Vanessa Terceti" className={styles.quemSouImg}/>
              <div className={styles.quemSouImgBadge}>
                <MdPets size={28} style={{ color: '#fff', display: 'block', margin: '0 auto 4px' }} />
                <p>Carinho &amp; Confiança!</p>
              </div>
            </div>
            <div className={styles.quemSouTexto}>
              <div className={styles.quoteIco}><MdFormatQuote size={40}/></div>
              <p>Oi gente! Sou pet sitter desde 2009 e vou contar um pouquinho sobre a decisão de me profissionalizar na profissão.</p>
              <p>A ideia surgiu no final de 2020 e me lembro como se fosse ontem... iria cuidar de 5 cachorros de uma amiga e nasceu a ideia de concretizar uma paixão que tenho desde que me entendo por gente: cuidar de pets! Mas, já parou pra pensar na responsabilidade que é cuidar do amor ou amores da vida de alguém? Com isso em mente, fiz e sempre faço cursos, que por sinal me enriqueceram com bastante conhecimento. Digo que foi de suma importância, pois uma coisa é prestar um serviço, outra bem diferente é <strong>AMAR o que faz</strong> e saber tudo o que está envolvido no pet sitter.</p>
              <p>Acredito que a época da pandemia não foi fácil pra ninguém e quem já teve ou tem depressão/ansiedade sabe que existem dias que é uma luta intensa. Mas, minhas amigas mergulharam de cabeça no meu sonho, sem nem saber a profundidade! O apoio de cada uma delas foi tão importante, que mesmo tentando não consigo descrever o tamanho da gratidão que vou sempre sentir no meu coração!</p>
              <p>Sou tutora de <strong>7 pets, sendo eles 6 gatos e 1 dog</strong>. Minha turminha é muito importante pra mim e da mesma maneira que cuido e zelo pelo bem-estar deles, faço com os animais de meus clientes.</p>
              <p>Se tem uma coisa que não tem preço é ser recebida com festa pelos bichinhos à cada visita, banho, tosa ou passeio — isso me faz sentir realizada e saber que um cliente confia totalmente em mim me deixa honrada! 🐾</p>
              <div className={styles.quemSouTags}>
                <span>🐕 Dog Walker</span>
                <span>🐈 Pet Sitting</span>
                <span>🛁 Banho & Tosa</span>
                <span>📚 Cursos Certificados</span>
                <span>❤️ Amor pelos Animais</span>
              </div>
            </div>
          </div>
        </div>
      </section>

      <div id="servicos" className={styles.servicosWrap}>
        <div className={styles.secao}>
          <div className={styles.secaoTitulo}>
            <p className={styles.secaoTag}>O que oferecemos</p>
            <h2>Serviços com Carinho e Profissionalismo</h2>
            <p>Cada serviço pensado para o bem-estar e felicidade do seu pet</p>
          </div>
          <div className={styles.servicosGrid}>
            {SERVICOS.map(s => (
              <div key={s.nome} className={styles.servicoCard}>
                <div className={styles.servicoIco}>{s.ico}</div>
                <h3>{s.nome}</h3>
                <p>{s.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className={styles.porqueWrap}>
        <div className={styles.secao}>
          <div className={styles.secaoTitulo}>
            <p className={styles.secaoTag}>Nossos diferenciais</p>
            <h2>Por que Escolher a Vanessa?</h2>
            <p>Profissionalismo e afeto que fazem toda a diferença</p>
          </div>
          <div className={styles.porqueGrid}>
            {PORQUE.map(item => (
              <div key={item.titulo} className={styles.porqueItem}>
                <div className={styles.porqueIco}>{item.ico}</div>
                <h4>{item.titulo}</h4>
                <p>{item.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className={styles.ctaWrap}>
        <div className={styles.ctaInner}>
          <h2>Pronto para começar? <MdPets size={40} style={{color:'#F9A8D4', verticalAlign:'middle'}}/></h2>
          <p>Crie sua conta e agende o melhor cuidado para o seu pet hoje mesmo.</p>
          <button className={styles.btnCtaBranco} onClick={irParaApp}>
            {usuario ? 'Ir para o Dashboard' : 'Criar Conta Grátis'}
          </button>
        </div>
      </div>

      <footer className={styles.footer}>
        {!logoErro && (
          <img
            src="/logo.png"
            alt=""
            className={styles.footerLogo}
            style={{ cursor: 'default' }}
            onError={e => { e.currentTarget.style.display = 'none' }}
            onClick={() => navigate('/login')}
          />
        )}
        <p>© 2025 <span>Vanessa Terceti Pet Sitter</span>. Feito com <MdPets size={16} style={{color:'#F9A8D4', verticalAlign:'middle'}}/> e muito amor.</p>
      </footer>

      {/* botao do whatsapp flutuante */}
      <a
        href="https://wa.me/553599206701"
        target="_blank"
        rel="noopener noreferrer"
        className={styles.whatsappBtn}
      >
        <FaWhatsapp size={24} className={styles.whatsappIco} />
        Dúvidas?
      </a>
    </>
  )
}
