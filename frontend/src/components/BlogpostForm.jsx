import { useState } from 'react'

// 1. Datum uit de API → datumveld ("2026-09-01")
function naarDatumVeld(datum) {
  return datum ? datum.slice(0, 10) : new Date().toISOString().slice(0, 10)
}

// 2. Eén formulier voor toevoegen én wijzigen
export default function BlogpostForm({ blogpost, busy, fouten, onSave, onCancel }) {
  // 3. Startwaarden
  const [titel, setTitel] = useState(blogpost?.titel ?? '')
  const [inhoud, setInhoud] = useState(blogpost?.inhoud ?? '')
  const [publicatiedatum, setPublicatiedatum] = useState(naarDatumVeld(blogpost?.publicatiedatum))
  const [eigenFouten, setEigenFouten] = useState([])

  function handleSubmit(e) {
    e.preventDefault()

    // 4. Zelf controleren (zelfde regels als BlogpostCreateDto)
    const lijst = []
    if (!titel.trim()) lijst.push('Vul een titel in.')
    else if (titel.trim().length > 200) lijst.push('De titel mag maximaal 200 tekens hebben.')
    if (!inhoud.trim()) lijst.push('Vul de inhoud in.')
    if (!publicatiedatum) lijst.push('Kies een publicatiedatum.')
    setEigenFouten(lijst)
    if (lijst.length > 0) return

    // 5. Doorgeven aan de pagina (die doet POST of PUT)
    onSave({
      titel: titel.trim(),
      inhoud: inhoud.trim(),
      publicatiedatum,
    })
  }

  // 6. Eigen fouten + API-fouten
  const alleFouten = [...eigenFouten, ...fouten]

  return (
    <form onSubmit={handleSubmit} noValidate aria-labelledby="blog-form-kop">
      <h2 id="blog-form-kop">{blogpost ? 'Blogpost wijzigen' : 'Nieuwe blogpost'}</h2>

      <div>
        <label htmlFor="b-titel">Titel</label>
        <input id="b-titel" value={titel} onChange={(e) => setTitel(e.target.value)} required maxLength={200} />
      </div>
      <div>
        <label htmlFor="b-inhoud">Inhoud</label>
        <textarea id="b-inhoud" rows={6} value={inhoud} onChange={(e) => setInhoud(e.target.value)} required />
      </div>
      <div>
        <label htmlFor="b-datum">Publicatiedatum</label>
        <input id="b-datum" type="date" value={publicatiedatum} onChange={(e) => setPublicatiedatum(e.target.value)} required />
      </div>

      {/* 7. Foutmeldingen */}
      {alleFouten.length > 0 && (
        <div role="alert">
          <p>Controleer het formulier:</p>
          <ul>
            {alleFouten.map((f) => <li key={f}>{f}</li>)}
          </ul>
        </div>
      )}

      <button type="submit" disabled={busy}>{busy ? 'Opslaan…' : 'Opslaan'}</button>{' '}
      <button type="button" onClick={onCancel} disabled={busy}>Annuleren</button>
    </form>
  )
}