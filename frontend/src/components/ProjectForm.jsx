import { useState } from 'react'

// 1. Datum uit de API ("2026-10-09T00:00:00") → datumveld ("2026-10-09")
function naarDatumVeld(datum) {
  return datum ? datum.slice(0, 10) : new Date().toISOString().slice(0, 10)
}

// 2. Eén formulier voor toevoegen én wijzigen
//    project = null → nieuw, project = {...} → wijzigen
export default function ProjectForm({ project, busy, fouten, onSave, onCancel }) {
  // 3. Startwaarden: leeg of de huidige waarden van het project
  const [titel, setTitel] = useState(project?.titel ?? '')
  const [beschrijving, setBeschrijving] = useState(project?.beschrijving ?? '')
  const [categorie, setCategorie] = useState(project?.categorie ?? '')
  const [githubUrl, setGithubUrl] = useState(project?.githubUrl ?? '')
  const [datum, setDatum] = useState(naarDatumVeld(project?.datum))
  const [eigenFouten, setEigenFouten] = useState([])

  function handleSubmit(e) {
    e.preventDefault()

    // 4. Eerst zelf controleren (zelfde regels als de DTO in de API)
    const lijst = []
    if (!titel.trim()) lijst.push('Vul een titel in.')
    else if (titel.trim().length > 200) lijst.push('De titel mag maximaal 200 tekens hebben.')
    if (!beschrijving.trim()) lijst.push('Vul een beschrijving in.')
    if (!categorie.trim()) lijst.push('Vul een categorie in.')
    if (!datum) lijst.push('Kies een datum.')
    setEigenFouten(lijst)
    if (lijst.length > 0) return

    // 5. Goed? Dan de gegevens doorgeven aan de pagina (die doet POST of PUT)
    onSave({
      titel: titel.trim(),
      beschrijving: beschrijving.trim(),
      categorie: categorie.trim(),
      githubUrl: githubUrl.trim(),
      datum,
    })
  }

  // 6. Eigen fouten + fouten van de API samen tonen
  const alleFouten = [...eigenFouten, ...fouten]

  return (
    <form onSubmit={handleSubmit} noValidate aria-labelledby="project-form-kop">
      <h2 id="project-form-kop">{project ? 'Project wijzigen' : 'Nieuw project'}</h2>

      <div>
        <label htmlFor="p-titel">Titel</label>
        <input id="p-titel" value={titel} onChange={(e) => setTitel(e.target.value)} required maxLength={200} />
      </div>
      <div>
        <label htmlFor="p-beschrijving">Beschrijving</label>
        <textarea id="p-beschrijving" rows={4} value={beschrijving} onChange={(e) => setBeschrijving(e.target.value)} required />
      </div>
      <div>
        <label htmlFor="p-categorie">Categorie</label>
        <input id="p-categorie" value={categorie} onChange={(e) => setCategorie(e.target.value)} required />
      </div>
      <div>
        <label htmlFor="p-github">GitHub-link (optioneel)</label>
        <input id="p-github" type="url" value={githubUrl} onChange={(e) => setGithubUrl(e.target.value)} />
      </div>
      <div>
        <label htmlFor="p-datum">Datum</label>
        <input id="p-datum" type="date" value={datum} onChange={(e) => setDatum(e.target.value)} required />
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