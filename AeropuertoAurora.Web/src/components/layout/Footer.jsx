export default function Footer({
  currency,
  setCurrency,
  CURRENCIES
}) {
  return (
<footer>
        <div className="footer-logo">Aeropuerto Internacional La Aurora</div>
        <p>Ciudad de Guatemala - Codigo IATA: GUA - Codigo ICAO: MGGT</p>
        <p>Frontend React conectado al API del proyecto.</p>
        <label className="footer-currency">
          <span>Moneda</span>
          <select value={currency} onChange={(event) => setCurrency(event.target.value)}>
            {CURRENCIES.map((item) => <option value={item} key={item}>{item}</option>)}
          </select>
        </label>
      </footer>
        );
}