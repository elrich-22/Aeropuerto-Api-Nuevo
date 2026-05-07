export default function 

NavBar({ user, adminView, isAdmin, activeView, cartCount, onAdminView, onNavigate, onCartClick, onLoginClick, onLogout }) {
  return (
    <nav className="site-nav">
      <a className="nav-logo" href="#inicio" onClick={(event) => onNavigate(event, 'inicio')}>
        La <span>Aurora</span>
      </a>
      <div className="nav-links">
        <a className={activeView === 'explorar' ? 'active' : ''} href="#explorar" onClick={(event) => onNavigate(event, 'explorar')}>Explorar</a>
        <a className={activeView === 'rastreo' ? 'active' : ''} href="#rastreo" onClick={(event) => onNavigate(event, 'rastreo')}>Rastreo</a>
        <a className={activeView === 'ubicacion' ? 'active' : ''} href="#ubicacion" onClick={(event) => onNavigate(event, 'ubicacion')}>Ubicacion</a>
        {isAdmin && (
          <>
            <button
              className={adminView === 'reporteria' ? 'nav-admin-link active' : 'nav-admin-link'}
              type="button"
              onClick={() => onAdminView('reporteria')}
            >
              Reporteria
            </button>
            <button
              className={adminView === 'admin' ? 'nav-admin-link active' : 'nav-admin-link'}
              type="button"
              onClick={() => onAdminView('admin')}
            >
              Admin
            </button>
          </>
        )}
        {user ? (
          <>
            <span className="nav-user">{user.nombreCompleto || user.usuario}</span>
            {cartCount > 0 && (
              <button className="nav-session-button cart-nav-button" type="button" onClick={onCartClick}>
                Carrito ({cartCount})
              </button>
            )}
            <button className="nav-session-button" type="button" onClick={onLogout}>Salir</button>
          </>
        ) : (
          <button className="nav-session-button" type="button" onClick={onLoginClick}>Iniciar sesion</button>
        )}
      </div>
    </nav>
  );
}