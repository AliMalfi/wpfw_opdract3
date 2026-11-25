import { Link } from 'react-router-dom'

// 1. Homepagina met tekst en profielfoto (zoals op mijn oude site)
export default function HomePage() {
  return (
    <section className="hero">
      <div className="hero-tekst">
        {/* 2. Klein label boven de titel */}
        <p className="hero-label">Software Engineering · De Haagse Hogeschool</p>

        <h1>Hallo, ik ben Ali Malfi</h1>

        <p className="hero-intro">
          Gedreven <strong>Software Engineering student</strong> met een passie voor
          full-stack webontwikkeling en betrouwbare databases. Ik maak van complexe
          problemen schone, werkende software.
        </p>

        {/* 3. Twee knoppen naar de belangrijkste pagina's */}
        <p className="hero-knoppen">
          <Link className="knop-link" to="/projecten">Bekijk mijn projecten</Link>
          <Link className="knop-link secundair" to="/blog">Lees mijn blog</Link>
        </p>
      </div>

      <img src="/img/profiel.jpeg" alt="Portretfoto van Ali Malfi" />
    </section>
  )
}