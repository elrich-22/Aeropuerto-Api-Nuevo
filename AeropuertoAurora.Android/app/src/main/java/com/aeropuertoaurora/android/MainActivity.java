package com.aeropuertoaurora.android;

import android.app.Activity;
import android.graphics.Color;
import android.graphics.Typeface;
import android.graphics.drawable.GradientDrawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.InputType;
import android.view.Gravity;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ScrollView;
import android.widget.Spinner;
import android.widget.TextView;

import org.json.JSONArray;
import org.json.JSONObject;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Locale;
import java.util.Map;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public final class MainActivity extends Activity {
    private static final int DEEP = Color.rgb(6, 14, 28);
    private static final int SKY = Color.rgb(10, 22, 40);
    private static final int PANEL = Color.rgb(14, 31, 55);
    private static final int PANEL_LIGHT = Color.rgb(20, 43, 72);
    private static final int LINE = Color.rgb(43, 65, 94);
    private static final int TEXT = Color.rgb(232, 238, 245);
    private static final int MUTED = Color.rgb(122, 143, 166);
    private static final int TEAL = Color.rgb(30, 184, 200);
    private static final int GOLD = Color.rgb(200, 168, 75);
    private static final int RED = Color.rgb(236, 93, 93);
    private static final double BASE_FARE = 1250.0;

    private final ExecutorService executor = Executors.newSingleThreadExecutor();
    private final Handler mainHandler = new Handler(Looper.getMainLooper());

    private SettingsStore settingsStore;
    private ApiClient apiClient;
    private LinearLayout rootLayout;
    private LinearLayout navLayout;
    private LinearLayout contentLayout;
    private TextView statusText;
    private TextView sessionText;
    private EditText baseUrlInput;
    private EditText apiKeyInput;

    private JSONObject sessionUser;
    private final ArrayList<JSONObject> cartItems = new ArrayList<>();
    private String activeView = "inicio";
    private String lastMessage = "";

    private JSONObject health;
    private JSONArray flights = new JSONArray();
    private JSONArray airports = new JSONArray();
    private JSONArray destinations = new JSONArray();
    private JSONArray severities = new JSONArray();
    private JSONArray baggage = new JSONArray();
    private JSONArray incidents = new JSONArray();
    private JSONArray maintenance = new JSONArray();
    private JSONArray occupancy = new JSONArray();
    private JSONArray tables = new JSONArray();

    private String selectedTable = "";
    private JSONObject adminMetadata;
    private JSONArray adminRows = new JSONArray();
    private JSONObject editingRow;
    private final Map<String, EditText> adminInputs = new HashMap<>();
    private JSONObject checkoutFlight;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        settingsStore = new SettingsStore(this);
        apiClient = new ApiClient(settingsStore.getBaseUrl(), settingsStore.getApiKey());
        loadLocalState();
        setContentView(buildScreen());
        render();
        loadDashboard();
    }

    @Override
    protected void onDestroy() {
        executor.shutdownNow();
        super.onDestroy();
    }

    private View buildScreen() {
        ScrollView scrollView = new ScrollView(this);
        scrollView.setFillViewport(true);
        scrollView.setBackgroundColor(DEEP);

        rootLayout = new LinearLayout(this);
        rootLayout.setOrientation(LinearLayout.VERTICAL);
        rootLayout.setPadding(dp(16), dp(16), dp(16), dp(30));
        scrollView.addView(rootLayout, matchWrap());

        LinearLayout header = new LinearLayout(this);
        header.setOrientation(LinearLayout.VERTICAL);
        header.setPadding(dp(4), dp(4), dp(4), dp(10));
        rootLayout.addView(header, matchWrap());

        TextView title = text("La Aurora", 30, GOLD, Typeface.BOLD);
        header.addView(title);
        TextView subtitle = text("App movil conectada al API del proyecto.", 14, MUTED, Typeface.NORMAL);
        subtitle.setPadding(0, dp(3), 0, dp(10));
        header.addView(subtitle);

        statusText = text("Preparando panel movil...", 13, TEXT, Typeface.BOLD);
        statusText.setPadding(dp(12), dp(10), dp(12), dp(10));
        statusText.setBackground(round(PANEL_LIGHT, LINE, dp(8)));
        header.addView(statusText, matchWrap());

        sessionText = text("", 13, MUTED, Typeface.NORMAL);
        sessionText.setPadding(dp(2), dp(8), 0, 0);
        header.addView(sessionText);

        navLayout = new LinearLayout(this);
        navLayout.setOrientation(LinearLayout.VERTICAL);
        rootLayout.addView(navLayout, matchWrap());

        contentLayout = new LinearLayout(this);
        contentLayout.setOrientation(LinearLayout.VERTICAL);
        rootLayout.addView(contentLayout, matchWrap());

        return scrollView;
    }

    private void loadLocalState() {
        try {
            String session = settingsStore.getSessionJson();
            sessionUser = session == null || session.trim().isEmpty() ? null : new JSONObject(session);
        } catch (Exception ignored) {
            sessionUser = null;
        }

        cartItems.clear();
        try {
            JSONArray stored = new JSONArray(settingsStore.getCartJson());
            for (int i = 0; i < stored.length(); i++) {
                cartItems.add(stored.getJSONObject(i));
            }
        } catch (Exception ignored) {
            settingsStore.saveCartJson("[]");
        }
    }

    private void render() {
        updateSessionText();
        renderNav();
        contentLayout.removeAllViews();

        if (!lastMessage.isEmpty()) {
            addAlert(lastMessage, false);
            lastMessage = "";
        }

        if ("explorar".equals(activeView)) {
            renderExplore();
        } else if ("rastreo".equals(activeView)) {
            renderBoard();
        } else if ("operaciones".equals(activeView)) {
            renderOperations();
        } else if ("reportes".equals(activeView)) {
            renderReports();
        } else if ("carrito".equals(activeView)) {
            renderCart();
        } else if ("checkout".equals(activeView)) {
            renderCheckout();
        } else if ("admin".equals(activeView)) {
            renderAdmin();
        } else {
            renderHome();
        }
    }

    private void renderNav() {
        navLayout.removeAllViews();
        String[] labels = isAdmin() ? new String[]{"Inicio", "Explorar", "Rastreo", "Ops", "Reportes", "Carrito", "Admin"}
                : new String[]{"Inicio", "Explorar", "Rastreo", "Ops", "Reportes", "Carrito"};
        String[] views = isAdmin() ? new String[]{"inicio", "explorar", "rastreo", "operaciones", "reportes", "carrito", "admin"}
                : new String[]{"inicio", "explorar", "rastreo", "operaciones", "reportes", "carrito"};

        for (int i = 0; i < labels.length; i += 3) {
            LinearLayout row = row();
            for (int j = i; j < Math.min(i + 3, labels.length); j++) {
                String view = views[j];
                String label = "carrito".equals(view) ? labels[j] + " (" + cartItems.size() + ")" : labels[j];
                Button button = navButton(label, activeView.equals(view), item -> {
                    checkoutFlight = null;
                    activeView = view;
                    if ("admin".equals(view) && tables.length() == 0) {
                        loadTables();
                    }
                    render();
                });
                row.addView(button, weightedButtonParams());
            }
            navLayout.addView(row);
        }
    }

    private void renderHome() {
        LinearLayout hero = card();
        hero.setBackground(round(SKY, LINE, dp(8)));
        hero.addView(text("Aeropuerto Internacional La Aurora", 24, TEXT, Typeface.BOLD));
        TextView copy = text("Puerta de entrada a Guatemala, con vuelos, compras, rastreo, reporteria y administracion desde Android.", 14, MUTED, Typeface.NORMAL);
        copy.setPadding(0, dp(8), 0, dp(12));
        hero.addView(copy);
        Button explore = primaryButton("Explorar vuelos", v -> {
            activeView = "explorar";
            render();
        });
        hero.addView(explore, matchWrap());
        contentLayout.addView(hero);

        renderConnectionCard();
        renderSessionCard();
        renderStats();
        renderDestinations();
        renderServices();
        renderLocation();
    }

    private void renderConnectionCard() {
        LinearLayout card = card();
        card.addView(sectionTitle("Conexion"));
        baseUrlInput = input("URL de API", settingsStore.getBaseUrl(), false);
        apiKeyInput = input("API key", settingsStore.getApiKey(), false);
        card.addView(baseUrlInput);
        card.addView(apiKeyInput);
        LinearLayout row = row();
        row.addView(primaryButton("Guardar", view -> saveConnection()), weightedButtonParams());
        row.addView(outlineButton("Probar", view -> fetchHealth()), weightedButtonParams());
        card.addView(row);
        contentLayout.addView(card);
    }

    private void renderSessionCard() {
        LinearLayout card = card();
        card.addView(sectionTitle(sessionUser == null ? "Sesion" : "Sesion activa"));
        if (sessionUser != null) {
            card.addView(text(value(sessionUser, "nombreCompleto", value(sessionUser, "usuario", "Usuario")), 16, TEXT, Typeface.BOLD));
            card.addView(text(value(sessionUser, "email", ""), 13, MUTED, Typeface.NORMAL));
            LinearLayout row = row();
            row.addView(outlineButton("Salir", v -> logout()), weightedButtonParams());
            row.addView(primaryButton("Ver carrito", v -> {
                activeView = "carrito";
                render();
            }), weightedButtonParams());
            card.addView(row);
        } else {
            EditText user = input("Usuario o email", "", false);
            user.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS);
            EditText password = input("Contrasena", "", true);
            card.addView(user);
            card.addView(password);
            LinearLayout row = row();
            row.addView(primaryButton("Entrar", v -> login(user.getText().toString(), password.getText().toString())), weightedButtonParams());
            row.addView(outlineButton("Registro", v -> renderRegisterForm()), weightedButtonParams());
            card.addView(row);
        }
        contentLayout.addView(card);
    }

    private void renderRegisterForm() {
        contentLayout.removeAllViews();
        LinearLayout card = card();
        card.addView(sectionTitle("Crear cuenta"));
        EditText usuario = input("Usuario", "", false);
        EditText email = input("Email", "", false);
        EditText contrasena = input("Contrasena", "", true);
        EditText documento = input("Numero de documento", "", false);
        EditText tipoDocumento = input("Tipo documento (DPI/Pasaporte)", "DPI", false);
        EditText primerNombre = input("Primer nombre", "", false);
        EditText segundoNombre = input("Segundo nombre", "", false);
        EditText primerApellido = input("Primer apellido", "", false);
        EditText segundoApellido = input("Segundo apellido", "", false);
        EditText fechaNacimiento = input("Fecha nacimiento yyyy-mm-dd", "", false);
        EditText nacionalidad = input("Nacionalidad", "Guatemala", false);
        EditText sexo = input("Sexo M/F", "", false);
        EditText telefono = input("Telefono", "", false);
        EditText[] fields = {usuario, email, contrasena, documento, tipoDocumento, primerNombre, segundoNombre, primerApellido, segundoApellido, fechaNacimiento, nacionalidad, sexo, telefono};
        for (EditText field : fields) {
            card.addView(field);
        }
        card.addView(primaryButton("Crear cuenta", v -> register(usuario, email, contrasena, documento, tipoDocumento, primerNombre, segundoNombre, primerApellido, segundoApellido, fechaNacimiento, nacionalidad, sexo, telefono)), matchWrap());
        card.addView(outlineButton("Volver", v -> render()), matchWrap());
        contentLayout.addView(card);
    }

    private void renderStats() {
        LinearLayout card = card();
        card.addView(sectionTitle("Resumen operativo"));
        LinearLayout row = row();
        row.addView(metric("Vuelos", flights.length()), weightedButtonParams());
        row.addView(metric("Destinos", destinations.length()), weightedButtonParams());
        card.addView(row);
        LinearLayout row2 = row();
        row2.addView(metric("Equipajes", baggage.length()), weightedButtonParams());
        row2.addView(metric("Incidentes", openIncidents()), weightedButtonParams());
        card.addView(row2);
        Button refresh = outlineButton("Actualizar datos", v -> loadDashboard());
        card.addView(refresh, matchWrap());
        contentLayout.addView(card);
    }

    private void renderDestinations() {
        LinearLayout card = card();
        card.addView(sectionTitle("Destinos mas buscados"));
        forEach(destinations, 5, item -> {
            LinearLayout itemCard = compactCard();
            itemCard.addView(text(value(item, "aeropuerto", "Destino"), 16, TEXT, Typeface.BOLD));
            itemCard.addView(text("Busquedas " + value(item, "totalBusquedas", "0") + "  Clicks " + value(item, "totalClicks", "0"), 13, MUTED, Typeface.NORMAL));
            itemCard.setOnClickListener(v -> destinationClick(item));
            card.addView(itemCard);
        });
        if (destinations.length() == 0) {
            card.addView(text("Aun no hay ranking cargado.", 13, MUTED, Typeface.NORMAL));
        }
        contentLayout.addView(card);
    }

    private void renderServices() {
        LinearLayout card = card();
        card.addView(sectionTitle("Servicios"));
        String[][] services = {
                {"Tiendas y Duty Free", "Compras, recuerdos y productos de viaje."},
                {"Restaurantes", "Opciones rapidas antes de abordar."},
                {"Parqueo y Transporte", "Taxis autorizados, parqueo y movilidad."},
                {"Sala VIP", "Descanso y espera prioritaria."},
                {"WiFi Gratuito", "Conexion para pasajeros dentro de terminal."},
                {"Servicio Medico", "Primeros auxilios y apoyo ante emergencias."}
        };
        for (String[] service : services) {
            LinearLayout item = compactCard();
            item.addView(text(service[0], 15, TEXT, Typeface.BOLD));
            item.addView(text(service[1], 13, MUTED, Typeface.NORMAL));
            card.addView(item);
        }
        contentLayout.addView(card);
    }

    private void renderLocation() {
        LinearLayout card = card();
        card.addView(sectionTitle("Ubicacion"));
        card.addView(text("Ciudad de Guatemala - Codigo IATA: GUA - Codigo ICAO: MGGT", 14, MUTED, Typeface.NORMAL));
        card.addView(text("Referencia: zona 13, acceso principal a terminal internacional.", 14, MUTED, Typeface.NORMAL));
        contentLayout.addView(card);
    }

    private void renderExplore() {
        LinearLayout card = card();
        card.addView(sectionTitle("Buscar vuelos"));
        EditText origin = input("Origen", "La Aurora", false);
        EditText destination = input("Destino", "", false);
        EditText date = input("Fecha yyyy-mm-dd", "", false);
        EditText passengers = input("Pasajeros", "1", false);
        passengers.setInputType(InputType.TYPE_CLASS_NUMBER);
        card.addView(origin);
        card.addView(destination);
        card.addView(date);
        card.addView(passengers);
        card.addView(primaryButton("Buscar", v -> showTravelResults(origin.getText().toString(), destination.getText().toString(), date.getText().toString(), parseInt(passengers.getText().toString(), 1))), matchWrap());
        contentLayout.addView(card);
        showTravelResults("", "", "", 1);
    }

    private void showTravelResults(String origin, String destination, String date, int passengers) {
        if (contentLayout.getChildCount() > 1) {
            contentLayout.removeViews(1, contentLayout.getChildCount() - 1);
        }
        registerFlightSearch(origin, destination, date, passengers);
        LinearLayout results = card();
        results.addView(sectionTitle("Resultados"));
        int shown = 0;
        for (int i = 0; i < flights.length(); i++) {
            JSONObject flight = flights.optJSONObject(i);
            if (flight == null || !canPurchase(flight)) {
                continue;
            }
            if (!matchesFlight(flight, origin, destination, date)) {
                continue;
            }
            shown++;
            results.addView(flightCard(flight, passengers, true));
            if (shown >= 30) {
                break;
            }
        }
        if (shown == 0) {
            results.addView(text("No hay vuelos programados con esos filtros.", 13, MUTED, Typeface.NORMAL));
        }
        contentLayout.addView(results);
    }

    private void renderBoard() {
        LinearLayout search = card();
        search.addView(sectionTitle("Rastreo de vuelos"));
        EditText term = input("Numero, aerolinea, origen, destino o estado", "", false);
        search.addView(term);
        search.addView(primaryButton("Filtrar", v -> showBoardResults(term.getText().toString())), matchWrap());
        contentLayout.addView(search);
        showBoardResults("");
    }

    private void showBoardResults(String term) {
        if (contentLayout.getChildCount() > 1) {
            contentLayout.removeViews(1, contentLayout.getChildCount() - 1);
        }
        LinearLayout board = card();
        board.addView(sectionTitle("Tablero"));
        int shown = 0;
        for (int i = 0; i < flights.length(); i++) {
            JSONObject flight = flights.optJSONObject(i);
            if (flight == null || !containsFlight(flight, term)) {
                continue;
            }
            shown++;
            board.addView(flightCard(flight, 1, canPurchase(flight)));
            if (shown >= 60) {
                break;
            }
        }
        if (shown == 0) {
            board.addView(text("No hay vuelos para mostrar.", 13, MUTED, Typeface.NORMAL));
        }
        contentLayout.addView(board);
    }

    private View flightCard(JSONObject flight, int passengers, boolean allowBuy) {
        LinearLayout item = compactCard();
        item.addView(text(value(flight, "numeroVuelo", "Vuelo"), 17, TEXT, Typeface.BOLD));
        item.addView(text(value(flight, "origen", "-") + " -> " + value(flight, "destino", "-"), 14, TEXT, Typeface.NORMAL));
        item.addView(text("Aerolinea: " + value(flight, "aerolinea", "-") + "  Avion: " + value(flight, "matriculaAvion", "-"), 13, MUTED, Typeface.NORMAL));
        item.addView(text("Fecha: " + shortDate(value(flight, "fechaVuelo", "-")) + "  Estado: " + value(flight, "estado", "-"), 13, statusColor(value(flight, "estado", "")), Typeface.BOLD));
        item.addView(text("Ocupadas/disponibles: " + value(flight, "plazasOcupadas", "0") + "/" + value(flight, "plazasDisponibles", "0"), 13, MUTED, Typeface.NORMAL));
        if (allowBuy) {
            LinearLayout fares = row();
            fares.addView(outlineButton("Turista " + money(fare("economica", passengers)), v -> addCart(flight, "economica", passengers)), weightedButtonParams());
            fares.addView(outlineButton("Ejecutiva", v -> addCart(flight, "ejecutiva", passengers)), weightedButtonParams());
            fares.addView(outlineButton("Primera", v -> addCart(flight, "primera", passengers)), weightedButtonParams());
            item.addView(fares);
        }
        return item;
    }

    private void renderOperations() {
        contentLayout.addView(listSection("Equipaje", baggage, 20, item -> new String[]{
                value(item, "codigoBarras", "Equipaje"),
                "Pasajero: " + value(item, "pasajero", "-"),
                "Vuelo: " + value(item, "numeroVuelo", "-"),
                "Peso: " + value(item, "pesoKg", "0") + " kg  Estado: " + value(item, "estado", "-")
        }));
        contentLayout.addView(listSection("Incidentes", incidents, 20, item -> new String[]{
                value(item, "tipoIncidente", "Incidente"),
                "Fecha: " + shortDate(value(item, "fechaIncidente", "-")),
                "Ubicacion: " + value(item, "ubicacion", "-"),
                "Severidad: " + value(item, "severidad", "-") + "  Estado: " + value(item, "estado", "-"),
                value(item, "descripcion", "")
        }));
        contentLayout.addView(listSection("Mantenimientos", maintenance, 20, item -> new String[]{
                value(item, "matriculaAvion", "Avion"),
                "Tipo: " + value(item, "tipoMantenimiento", "-"),
                "Inicio: " + shortDate(value(item, "fechaInicio", "-")) + "  Fin: " + shortDate(value(item, "fechaFin", "-")),
                "Costo: " + money(parseDouble(value(item, "costo", "0"))) + "  Estado: " + value(item, "estado", "-")
        }));
    }

    private void renderReports() {
        contentLayout.addView(listSection("Destinos mas buscados", destinations, 20, item -> new String[]{
                value(item, "aeropuerto", "Destino"),
                "Busquedas: " + value(item, "totalBusquedas", "0"),
                "Clicks: " + value(item, "totalClicks", "0"),
                "Pasajeros: " + value(item, "totalPasajeros", "0")
        }));
        contentLayout.addView(listSection("Ocupacion de vuelos", occupancy, 20, item -> new String[]{
                value(item, "numeroVuelo", "Vuelo"),
                "Fecha: " + shortDate(value(item, "fechaVuelo", "-")),
                "Ocupacion: " + value(item, "porcentajeOcupacion", "0") + "%",
                "Ocupadas/disponibles: " + value(item, "plazasOcupadas", "0") + "/" + value(item, "plazasDisponibles", "0"),
                "Estado: " + value(item, "estado", "-")
        }));
        contentLayout.addView(listSection("Incidentes por severidad", severities, 20, item -> new String[]{
                value(item, "severidad", "Severidad"),
                "Total: " + value(item, "totalIncidentes", "0"),
                "Abiertos: " + value(item, "abiertos", "0") + "  Cerrados: " + value(item, "cerrados", "0")
        }));
    }

    private void renderCart() {
        LinearLayout card = card();
        card.addView(sectionTitle("Carrito"));
        if (cartItems.isEmpty()) {
            card.addView(text("No hay vuelos en el carrito.", 13, MUTED, Typeface.NORMAL));
        }
        for (JSONObject item : cartItems) {
            LinearLayout rowCard = compactCard();
            rowCard.addView(text(value(item, "numeroVuelo", "Vuelo") + " - " + classLabel(value(item, "selectedClass", "economica")), 16, TEXT, Typeface.BOLD));
            rowCard.addView(text(value(item, "origen", "-") + " -> " + value(item, "destino", "-"), 13, MUTED, Typeface.NORMAL));
            rowCard.addView(text("Pasajeros: " + value(item, "passengerCount", "1") + "  Tarifa: " + money(fare(value(item, "selectedClass", "economica"), parseInt(value(item, "passengerCount", "1"), 1))), 13, TEXT, Typeface.NORMAL));
            LinearLayout actions = row();
            actions.addView(primaryButton("Comprar", v -> startCheckout(item)), weightedButtonParams());
            actions.addView(outlineButton("Quitar", v -> removeCartItem(value(item, "cartId", ""))), weightedButtonParams());
            rowCard.addView(actions);
            card.addView(rowCard);
        }
        contentLayout.addView(card);
        if (sessionUser == null) {
            renderSessionCard();
        }
    }

    private void renderCheckout() {
        if (checkoutFlight == null) {
            activeView = "carrito";
            render();
            return;
        }
        if (sessionUser == null) {
            addAlert("Inicia sesion para confirmar la compra.", true);
            renderSessionCard();
            return;
        }

        LinearLayout card = card();
        card.addView(sectionTitle("Checkout"));
        card.addView(text(value(checkoutFlight, "numeroVuelo", "Vuelo"), 20, TEXT, Typeface.BOLD));
        card.addView(text(value(checkoutFlight, "origen", "-") + " -> " + value(checkoutFlight, "destino", "-"), 14, MUTED, Typeface.NORMAL));

        Spinner classSpinner = spinner(new String[]{"economica", "ejecutiva", "primera"}, value(checkoutFlight, "selectedClass", "economica"));
        EditText bags = input("Equipaje facturado", "1", false);
        bags.setInputType(InputType.TYPE_CLASS_NUMBER);
        EditText weight = input("Peso equipaje kg", "23", false);
        weight.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL);
        Spinner method = spinner(new String[]{"1 - Tarjeta de credito", "2 - Tarjeta de debito", "3 - Transferencia"}, "1 - Tarjeta de credito");

        card.addView(label("Clase"));
        card.addView(classSpinner);
        card.addView(bags);
        card.addView(weight);
        card.addView(label("Metodo de pago"));
        card.addView(method);
        card.addView(primaryButton("Confirmar compra", v -> confirmPurchase(classSpinner, bags, weight, method)), matchWrap());
        card.addView(outlineButton("Volver al carrito", v -> {
            activeView = "carrito";
            render();
        }), matchWrap());
        contentLayout.addView(card);
    }

    private void renderAdmin() {
        if (!isAdmin()) {
            addAlert("Tu usuario no tiene permisos de administrador.", true);
            return;
        }
        LinearLayout selector = card();
        selector.addView(sectionTitle("Panel administrativo"));
        selector.addView(text("CRUD dinamico de las tablas habilitadas en /api/tablas.", 13, MUTED, Typeface.NORMAL));
        selector.addView(primaryButton("Cargar tablas", v -> loadTables()), matchWrap());

        int count = Math.min(tables.length(), 30);
        for (int i = 0; i < count; i += 2) {
            LinearLayout row = row();
            for (int j = i; j < Math.min(i + 2, count); j++) {
                JSONObject table = tables.optJSONObject(j);
                if (table == null) {
                    continue;
                }
                String alias = value(table, "alias", "");
                row.addView(outlineButton(prettify(value(table, "nombre", alias)), v -> {
                    selectedTable = alias;
                    loadAdminTable();
                }), weightedButtonParams());
            }
            selector.addView(row);
        }
        contentLayout.addView(selector);

        if (selectedTable.isEmpty()) {
            return;
        }

        LinearLayout workspace = card();
        workspace.addView(sectionTitle("Tabla " + prettify(selectedTable)));
        workspace.addView(outlineButton("Actualizar tabla", v -> loadAdminTable()), matchWrap());
        if (adminMetadata == null) {
            workspace.addView(text("Selecciona o carga una tabla para empezar.", 13, MUTED, Typeface.NORMAL));
            contentLayout.addView(workspace);
            return;
        }

        buildAdminForm(workspace);
        buildAdminRows(workspace);
        contentLayout.addView(workspace);
    }

    private void buildAdminForm(LinearLayout parent) {
        adminInputs.clear();
        JSONArray columns = array(adminMetadata, "columnas");
        LinearLayout form = compactCard();
        form.addView(text(editingRow == null ? "Crear registro" : "Editar registro", 16, TEXT, Typeface.BOLD));
        for (int i = 0; i < columns.length(); i++) {
            JSONObject column = columns.optJSONObject(i);
            if (column == null || bool(column, "esIdentidad") || (editingRow != null && bool(column, "esLlavePrimaria"))) {
                continue;
            }
            String name = value(column, "nombre", "");
            EditText input = input(name + " (" + value(column, "tipoDato", "") + ")", editingRow == null ? "" : value(editingRow, name, ""), false);
            if (value(column, "tipoDato", "").toUpperCase(Locale.ROOT).contains("NUMBER")) {
                input.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL | InputType.TYPE_NUMBER_FLAG_SIGNED);
            }
            adminInputs.put(name, input);
            form.addView(input);
        }
        LinearLayout actions = row();
        actions.addView(primaryButton(editingRow == null ? "Crear" : "Guardar", v -> saveAdminRow()), weightedButtonParams());
        actions.addView(outlineButton("Limpiar", v -> {
            editingRow = null;
            render();
        }), weightedButtonParams());
        form.addView(actions);
        parent.addView(form);
    }

    private void buildAdminRows(LinearLayout parent) {
        JSONArray columns = array(adminMetadata, "columnas");
        int visibleColumns = Math.min(columns.length(), 8);
        for (int i = 0; i < Math.min(adminRows.length(), 40); i++) {
            JSONObject row = adminRows.optJSONObject(i);
            if (row == null) {
                continue;
            }
            LinearLayout item = compactCard();
            String keyName = value(adminMetadata, "llavePrimaria", "");
            item.addView(text(keyName.isEmpty() ? "Registro" : keyName + ": " + value(row, keyName, "-"), 15, TEXT, Typeface.BOLD));
            for (int c = 0; c < visibleColumns; c++) {
                JSONObject column = columns.optJSONObject(c);
                if (column != null) {
                    String name = value(column, "nombre", "");
                    item.addView(text(name + ": " + value(row, name, "-"), 12, MUTED, Typeface.NORMAL));
                }
            }
            LinearLayout actions = row();
            actions.addView(outlineButton("Editar", v -> {
                editingRow = row;
                render();
            }), weightedButtonParams());
            actions.addView(outlineButton("Borrar", v -> deleteAdminRow(row)), weightedButtonParams());
            item.addView(actions);
            parent.addView(item);
        }
        if (adminRows.length() == 0) {
            parent.addView(text("No hay registros cargados.", 13, MUTED, Typeface.NORMAL));
        }
    }

    private void saveConnection() {
        settingsStore.save(baseUrlInput.getText().toString(), apiKeyInput.getText().toString());
        apiClient = new ApiClient(settingsStore.getBaseUrl(), settingsStore.getApiKey());
        statusText.setText("Conexion guardada.");
    }

    private void fetchHealth() {
        saveConnection();
        runTask("Probando conexion...", () -> apiClient.get("/api/health"), json -> {
            health = new JSONObject(json);
            statusText.setText("API conectada: " + value(health, "status", "OK"));
            lastMessage = "La API respondio correctamente.";
            render();
        });
    }

    private void loadDashboard() {
        saveConnectionIfReady();
        statusText.setText("Cargando datos del API...");
        executor.submit(() -> {
            String error = "";
            JSONObject nextHealth = null;
            JSONArray nextFlights = new JSONArray();
            JSONArray nextAirports = new JSONArray();
            JSONArray nextDestinations = new JSONArray();
            JSONArray nextSeverities = new JSONArray();
            JSONArray nextBaggage = new JSONArray();
            JSONArray nextIncidents = new JSONArray();
            JSONArray nextMaintenance = new JSONArray();
            JSONArray nextOccupancy = new JSONArray();

            try {
                nextHealth = new JSONObject(apiClient.get("/api/health"));
            } catch (Exception exception) {
                error = exception.getMessage();
            }
            try { nextFlights = new JSONArray(apiClient.get("/api/vuelos?limit=1000")); } catch (Exception exception) { error = firstError(error, exception); }
            try { nextAirports = new JSONArray(apiClient.get("/api/aeropuertos?limit=500")); } catch (Exception exception) { error = firstError(error, exception); }
            try { nextDestinations = new JSONArray(apiClient.get("/api/reportes/destinos-mas-buscados?limit=10")); } catch (Exception exception) { error = firstError(error, exception); }
            try { nextSeverities = new JSONArray(apiClient.get("/api/reportes/incidentes-por-severidad")); } catch (Exception exception) { error = firstError(error, exception); }
            try { nextBaggage = new JSONArray(apiClient.get("/api/operaciones/equipaje?limit=20")); } catch (Exception exception) { error = firstError(error, exception); }
            try { nextIncidents = new JSONArray(apiClient.get("/api/operaciones/incidentes?limit=20")); } catch (Exception exception) { error = firstError(error, exception); }
            try { nextMaintenance = new JSONArray(apiClient.get("/api/operaciones/mantenimientos?limit=20")); } catch (Exception exception) { error = firstError(error, exception); }
            try { nextOccupancy = new JSONArray(apiClient.get("/api/reportes/ocupacion-vuelos?limit=20")); } catch (Exception exception) { error = firstError(error, exception); }

            JSONObject finalHealth = nextHealth;
            JSONArray finalFlights = nextFlights;
            JSONArray finalAirports = nextAirports;
            JSONArray finalDestinations = nextDestinations;
            JSONArray finalSeverities = nextSeverities;
            JSONArray finalBaggage = nextBaggage;
            JSONArray finalIncidents = nextIncidents;
            JSONArray finalMaintenance = nextMaintenance;
            JSONArray finalOccupancy = nextOccupancy;
            String finalError = error;
            mainHandler.post(() -> {
                health = finalHealth;
                flights = finalFlights;
                airports = finalAirports;
                destinations = finalDestinations;
                severities = finalSeverities;
                baggage = finalBaggage;
                incidents = finalIncidents;
                maintenance = finalMaintenance;
                occupancy = finalOccupancy;
                statusText.setText(finalError.isEmpty() ? "Datos actualizados." : "Datos parciales: " + finalError);
                render();
            });
        });
    }

    private void login(String user, String password) {
        saveConnectionIfReady();
        try {
            JSONObject body = new JSONObject()
                    .put("usuarioOEmail", user.trim())
                    .put("contrasena", password);
            runTask("Iniciando sesion...", () -> apiClient.post("/api/auth/login", body.toString()), json -> {
                sessionUser = new JSONObject(json);
                settingsStore.saveSessionJson(sessionUser.toString());
                statusText.setText("Sesion iniciada.");
                lastMessage = "Bienvenido " + value(sessionUser, "nombreCompleto", value(sessionUser, "usuario", "usuario")) + ".";
                render();
            });
        } catch (Exception exception) {
            showError(exception);
        }
    }

    private void register(EditText usuario, EditText email, EditText contrasena, EditText documento, EditText tipoDocumento, EditText primerNombre, EditText segundoNombre, EditText primerApellido, EditText segundoApellido, EditText fechaNacimiento, EditText nacionalidad, EditText sexo, EditText telefono) {
        saveConnectionIfReady();
        try {
            JSONObject body = new JSONObject()
                    .put("usuario", usuario.getText().toString().trim())
                    .put("email", email.getText().toString().trim())
                    .put("contrasena", contrasena.getText().toString())
                    .put("numeroDocumento", documento.getText().toString().trim())
                    .put("tipoDocumento", tipoDocumento.getText().toString().trim())
                    .put("primerNombre", primerNombre.getText().toString().trim())
                    .put("segundoNombre", segundoNombre.getText().toString().trim())
                    .put("primerApellido", primerApellido.getText().toString().trim())
                    .put("segundoApellido", segundoApellido.getText().toString().trim())
                    .put("fechaNacimiento", fechaNacimiento.getText().toString().trim() + "T00:00:00")
                    .put("nacionalidad", nacionalidad.getText().toString().trim())
                    .put("sexo", sexo.getText().toString().trim())
                    .put("telefono", telefono.getText().toString().trim());
            runTask("Creando cuenta...", () -> apiClient.post("/api/auth/register", body.toString()), json -> {
                sessionUser = new JSONObject(json);
                settingsStore.saveSessionJson(sessionUser.toString());
                statusText.setText("Cuenta creada.");
                activeView = "inicio";
                lastMessage = "Cuenta creada e iniciada.";
                render();
            });
        } catch (Exception exception) {
            showError(exception);
        }
    }

    private void logout() {
        sessionUser = null;
        settingsStore.clearSession();
        activeView = "inicio";
        statusText.setText("Sesion cerrada.");
        render();
    }

    private void destinationClick(JSONObject destination) {
        String id = value(destination, "aeropuertoId", "");
        String name = value(destination, "aeropuerto", "");
        if (!id.isEmpty()) {
            try {
                JSONObject body = new JSONObject()
                        .put("sesionId", JSONObject.NULL)
                        .put("aeropuertoDestinoId", parseInt(id, 0))
                        .put("fechaClick", nowIso())
                        .put("origenBusqueda", "android")
                        .put("fechaViajeBuscada", JSONObject.NULL)
                        .put("numeroPasajeros", JSONObject.NULL)
                        .put("claseBuscada", JSONObject.NULL);
                executor.submit(() -> {
                    try {
                        apiClient.post("/api/clicks-destino", body.toString());
                    } catch (Exception ignored) {
                    }
                });
            } catch (Exception ignored) {
            }
        }
        activeView = "explorar";
        render();
        showTravelResults("", name, "", 1);
    }

    private void registerFlightSearch(String origin, String destination, String date, int passengers) {
        if (destination == null || destination.trim().isEmpty()) {
            return;
        }
        int originId = findAirportId(origin.isEmpty() ? "La Aurora" : origin);
        int destinationId = findAirportId(destination);
        if (originId <= 0 || destinationId <= 0) {
            return;
        }
        try {
            JSONObject body = new JSONObject()
                    .put("sesionId", JSONObject.NULL)
                    .put("aeropuertoOrigenId", originId)
                    .put("aeropuertoDestinoId", destinationId)
                    .put("fechaIda", (date == null || date.trim().isEmpty() ? "2026-05-05" : date.trim()) + "T00:00:00")
                    .put("fechaVuelta", JSONObject.NULL)
                    .put("numeroPasajeros", Math.max(1, passengers))
                    .put("clase", JSONObject.NULL)
                    .put("fechaBusqueda", nowIso())
                    .put("seConvirtioCompra", "N");
            executor.submit(() -> {
                try {
                    apiClient.post("/api/busquedas-vuelo", body.toString());
                } catch (Exception ignored) {
                }
            });
        } catch (Exception ignored) {
        }
    }

    private void addCart(JSONObject flight, String className, int passengers) {
        try {
            JSONObject item = new JSONObject(flight.toString())
                    .put("cartId", value(flight, "id", "0") + "-" + System.currentTimeMillis())
                    .put("selectedClass", className)
                    .put("passengerCount", Math.max(1, passengers));
            cartItems.add(item);
            saveCart();
            activeView = "carrito";
            lastMessage = "Vuelo agregado al carrito.";
            render();
        } catch (Exception exception) {
            showError(exception);
        }
    }

    private void startCheckout(JSONObject item) {
        checkoutFlight = item;
        activeView = "checkout";
        render();
    }

    private void removeCartItem(String cartId) {
        for (int i = cartItems.size() - 1; i >= 0; i--) {
            if (value(cartItems.get(i), "cartId", "").equals(cartId)) {
                cartItems.remove(i);
            }
        }
        saveCart();
        render();
    }

    private void confirmPurchase(Spinner classSpinner, EditText bags, EditText weight, Spinner method) {
        if (checkoutFlight == null || sessionUser == null) {
            return;
        }
        String className = classSpinner.getSelectedItem().toString();
        int passengers = parseInt(value(checkoutFlight, "passengerCount", "1"), 1);
        double fare = fare(className, passengers);
        int methodId = parseInt(method.getSelectedItem().toString().substring(0, 1), 1);
        try {
            JSONObject body = new JSONObject()
                    .put("usuarioId", parseInt(value(sessionUser, "usuarioId", "0"), 0))
                    .put("pasajeroId", parseInt(value(sessionUser, "pasajeroId", "0"), 0))
                    .put("vueloId", parseInt(value(checkoutFlight, "id", "0"), 0))
                    .put("clase", className)
                    .put("equipajeFacturado", parseInt(bags.getText().toString(), 0))
                    .put("pesoEquipaje", parseDouble(weight.getText().toString()))
                    .put("tarifaPagada", fare)
                    .put("metodoPagoId", methodId);
            runTask("Confirmando compra...", () -> apiClient.post("/api/compras/vuelos", body.toString()), json -> {
                JSONObject response = new JSONObject(json);
                removeCartItem(value(checkoutFlight, "cartId", ""));
                checkoutFlight = null;
                activeView = "carrito";
                lastMessage = "Compra confirmada. Reserva " + value(response, "codigoReserva", "-") + ". Total " + money(parseDouble(value(response, "total", "0"))) + ".";
                loadDashboard();
            });
        } catch (Exception exception) {
            showError(exception);
        }
    }

    private void loadTables() {
        runTask("Cargando tablas...", () -> apiClient.get("/api/tablas"), json -> {
            tables = new JSONArray(json);
            if (selectedTable.isEmpty() && tables.length() > 0) {
                selectedTable = value(tables.getJSONObject(0), "alias", "");
                loadAdminTable();
                return;
            }
            statusText.setText("Tablas cargadas.");
            render();
        });
    }

    private void loadAdminTable() {
        if (selectedTable.isEmpty()) {
            return;
        }
        runTask("Cargando tabla " + selectedTable + "...", () -> {
            String metadata = apiClient.get("/api/tablas/" + encode(selectedTable) + "/metadata");
            String rows = apiClient.get("/api/tablas/" + encode(selectedTable) + "?limit=100");
            return new JSONObject().put("metadata", new JSONObject(metadata)).put("rows", new JSONObject(rows)).toString();
        }, json -> {
            JSONObject payload = new JSONObject(json);
            adminMetadata = payload.getJSONObject("metadata");
            adminRows = array(payload.getJSONObject("rows"), "filas");
            editingRow = null;
            statusText.setText("Tabla cargada.");
            render();
        });
    }

    private void saveAdminRow() {
        if (adminMetadata == null) {
            return;
        }
        try {
            JSONObject body = new JSONObject();
            for (Map.Entry<String, EditText> input : adminInputs.entrySet()) {
                body.put(input.getKey(), input.getValue().getText().toString());
            }
            Callable<String> call;
            if (editingRow == null) {
                call = () -> apiClient.post("/api/tablas/" + encode(selectedTable), body.toString());
            } else {
                String key = value(editingRow, value(adminMetadata, "llavePrimaria", ""), "");
                call = () -> apiClient.put("/api/tablas/" + encode(selectedTable) + "/" + encode(key), body.toString());
            }
            runTask("Guardando registro...", call, json -> {
                editingRow = null;
                lastMessage = "Registro guardado.";
                loadAdminTable();
            });
        } catch (Exception exception) {
            showError(exception);
        }
    }

    private void deleteAdminRow(JSONObject row) {
        String keyName = value(adminMetadata, "llavePrimaria", "");
        String key = value(row, keyName, "");
        if (keyName.isEmpty() || key.isEmpty()) {
            lastMessage = "Esta tabla no tiene llave primaria para borrar.";
            render();
            return;
        }
        runTask("Borrando registro...", () -> apiClient.delete("/api/tablas/" + encode(selectedTable) + "/" + encode(key)), json -> {
            lastMessage = "Registro eliminado.";
            loadAdminTable();
        });
    }

    private void runTask(String message, Callable<String> task, ResponseRenderer renderer) {
        statusText.setText(message);
        executor.submit(() -> {
            try {
                String json = task.call();
                mainHandler.post(() -> {
                    try {
                        renderer.render(json);
                    } catch (Exception exception) {
                        showError(exception);
                    }
                });
            } catch (Exception exception) {
                mainHandler.post(() -> showError(exception));
            }
        });
    }

    private void showError(Exception exception) {
        statusText.setText("No se pudo completar la accion.");
        lastMessage = exception.getMessage() == null ? exception.toString() : exception.getMessage();
        render();
    }

    private LinearLayout listSection(String title, JSONArray items, int limit, RowFormatter formatter) {
        LinearLayout section = card();
        section.addView(sectionTitle(title));
        forEach(items, limit, item -> {
            String[] lines = formatter.lines(item);
            LinearLayout itemCard = compactCard();
            itemCard.addView(text(lines.length == 0 ? "Registro" : lines[0], 16, TEXT, Typeface.BOLD));
            for (int i = 1; i < lines.length; i++) {
                if (lines[i] != null && !lines[i].trim().isEmpty()) {
                    itemCard.addView(text(lines[i], 13, MUTED, Typeface.NORMAL));
                }
            }
            section.addView(itemCard);
        });
        if (items.length() == 0) {
            section.addView(text("Sin datos cargados.", 13, MUTED, Typeface.NORMAL));
        }
        return section;
    }

    private TextView metric(String label, int value) {
        TextView view = text(value + "\n" + label, 13, TEXT, Typeface.BOLD);
        view.setGravity(Gravity.CENTER);
        view.setPadding(dp(8), dp(12), dp(8), dp(12));
        view.setBackground(round(PANEL_LIGHT, LINE, dp(8)));
        return view;
    }

    private void addAlert(String message, boolean error) {
        TextView alert = text(message, 13, error ? RED : TEXT, Typeface.BOLD);
        alert.setPadding(dp(12), dp(10), dp(12), dp(10));
        alert.setBackground(round(error ? Color.rgb(62, 30, 40) : PANEL_LIGHT, error ? RED : LINE, dp(8)));
        LinearLayout.LayoutParams params = matchWrap();
        params.setMargins(0, dp(12), 0, 0);
        contentLayout.addView(alert, params);
    }

    private LinearLayout card() {
        LinearLayout layout = new LinearLayout(this);
        layout.setOrientation(LinearLayout.VERTICAL);
        layout.setPadding(dp(14), dp(14), dp(14), dp(14));
        layout.setBackground(round(PANEL, LINE, dp(8)));
        LinearLayout.LayoutParams params = matchWrap();
        params.setMargins(0, dp(12), 0, 0);
        layout.setLayoutParams(params);
        return layout;
    }

    private LinearLayout compactCard() {
        LinearLayout layout = card();
        layout.setPadding(dp(12), dp(12), dp(12), dp(12));
        layout.setBackground(round(SKY, LINE, dp(8)));
        return layout;
    }

    private LinearLayout row() {
        LinearLayout layout = new LinearLayout(this);
        layout.setOrientation(LinearLayout.HORIZONTAL);
        layout.setGravity(Gravity.CENTER);
        layout.setPadding(0, dp(8), 0, 0);
        return layout;
    }

    private TextView sectionTitle(String text) {
        TextView view = text(text, 18, TEXT, Typeface.BOLD);
        view.setPadding(0, 0, 0, dp(8));
        return view;
    }

    private TextView label(String text) {
        TextView view = text(text, 12, GOLD, Typeface.BOLD);
        view.setPadding(0, dp(10), 0, dp(4));
        return view;
    }

    private EditText input(String hint, String value, boolean password) {
        EditText editText = new EditText(this);
        editText.setHint(hint);
        editText.setHintTextColor(MUTED);
        editText.setSingleLine(true);
        editText.setText(value);
        editText.setTextSize(14);
        editText.setTextColor(TEXT);
        editText.setPadding(dp(12), 0, dp(12), 0);
        editText.setMinHeight(dp(48));
        editText.setBackground(round(Color.rgb(9, 19, 34), LINE, dp(8)));
        editText.setInputType(password
                ? InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_PASSWORD
                : InputType.TYPE_CLASS_TEXT);
        LinearLayout.LayoutParams params = matchWrap();
        params.setMargins(0, dp(8), 0, 0);
        editText.setLayoutParams(params);
        return editText;
    }

    private Spinner spinner(String[] options, String selected) {
        Spinner spinner = new Spinner(this);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, options);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);
        for (int i = 0; i < options.length; i++) {
            if (options[i].equalsIgnoreCase(selected)) {
                spinner.setSelection(i);
                break;
            }
        }
        LinearLayout.LayoutParams params = matchWrap();
        params.setMargins(0, dp(6), 0, dp(4));
        spinner.setLayoutParams(params);
        return spinner;
    }

    private Button primaryButton(String label, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(DEEP);
        button.setBackground(round(TEAL, TEAL, dp(8)));
        return button;
    }

    private Button outlineButton(String label, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(GOLD);
        button.setBackground(round(Color.TRANSPARENT, GOLD, dp(8)));
        return button;
    }

    private Button navButton(String label, boolean active, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(active ? DEEP : TEXT);
        button.setBackground(round(active ? GOLD : Color.rgb(9, 19, 34), active ? GOLD : LINE, dp(8)));
        return button;
    }

    private Button baseButton(String label, View.OnClickListener listener) {
        Button button = new Button(this);
        button.setText(label);
        button.setTextSize(12);
        button.setAllCaps(false);
        button.setMinHeight(dp(44));
        button.setOnClickListener(listener);
        return button;
    }

    private TextView text(String text, int sp, int color, int style) {
        TextView view = new TextView(this);
        view.setText(text);
        view.setTextSize(sp);
        view.setTextColor(color);
        view.setTypeface(Typeface.DEFAULT, style);
        view.setLineSpacing(dp(2), 1.0f);
        return view;
    }

    private GradientDrawable round(int fillColor, int strokeColor, int radius) {
        GradientDrawable drawable = new GradientDrawable();
        drawable.setColor(fillColor);
        drawable.setCornerRadius(radius);
        drawable.setStroke(dp(1), strokeColor);
        return drawable;
    }

    private LinearLayout.LayoutParams matchWrap() {
        return new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
    }

    private LinearLayout.LayoutParams weightedButtonParams() {
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f);
        params.setMargins(dp(4), 0, dp(4), 0);
        return params;
    }

    private int dp(int value) {
        return (int) (value * getResources().getDisplayMetrics().density + 0.5f);
    }

    private void saveConnectionIfReady() {
        if (baseUrlInput != null && apiKeyInput != null) {
            saveConnection();
        }
    }

    private void updateSessionText() {
        sessionText.setText(sessionUser == null
                ? "Sin sesion activa."
                : "Sesion: " + value(sessionUser, "nombreCompleto", value(sessionUser, "usuario", "Usuario")));
    }

    private void saveCart() {
        JSONArray array = new JSONArray();
        for (JSONObject item : cartItems) {
            array.put(item);
        }
        settingsStore.saveCartJson(array.toString());
    }

    private boolean isAdmin() {
        if (sessionUser == null) {
            return false;
        }
        String combined = (value(sessionUser, "usuario", "") + " " + value(sessionUser, "email", "") + " " + value(sessionUser, "nombreCompleto", "")).toLowerCase(Locale.ROOT);
        return combined.contains("admin.aurora") || combined.contains("administrador");
    }

    private boolean canPurchase(JSONObject flight) {
        return normalize(value(flight, "estado", "")).equals("programado");
    }

    private boolean matchesFlight(JSONObject flight, String origin, String destination, String date) {
        return contains(value(flight, "origen", ""), origin)
                && contains(value(flight, "destino", ""), destination)
                && (date == null || date.trim().isEmpty() || value(flight, "fechaVuelo", "").startsWith(date.trim()));
    }

    private boolean containsFlight(JSONObject flight, String term) {
        String value = value(flight, "numeroVuelo", "") + " " + value(flight, "aerolinea", "") + " " + value(flight, "origen", "") + " " + value(flight, "destino", "") + " " + value(flight, "estado", "") + " " + value(flight, "matriculaAvion", "");
        return contains(value, term);
    }

    private boolean contains(String value, String term) {
        return term == null || term.trim().isEmpty() || normalize(value).contains(normalize(term));
    }

    private String normalize(String value) {
        return value == null ? "" : value.trim().toLowerCase(Locale.ROOT);
    }

    private String value(JSONObject item, String key, String fallback) {
        if (item == null || key == null || key.isEmpty()) {
            return fallback;
        }
        if (item.has(key) && !item.isNull(key)) {
            return item.optString(key, fallback);
        }
        String pascal = key.substring(0, 1).toUpperCase(Locale.ROOT) + key.substring(1);
        if (item.has(pascal) && !item.isNull(pascal)) {
            return item.optString(pascal, fallback);
        }
        return fallback;
    }

    private JSONArray array(JSONObject item, String key) {
        if (item == null) {
            return new JSONArray();
        }
        JSONArray array = item.optJSONArray(key);
        if (array != null) {
            return array;
        }
        String pascal = key.substring(0, 1).toUpperCase(Locale.ROOT) + key.substring(1);
        array = item.optJSONArray(pascal);
        return array == null ? new JSONArray() : array;
    }

    private boolean bool(JSONObject item, String key) {
        if (item == null) {
            return false;
        }
        if (item.has(key)) {
            return item.optBoolean(key, false);
        }
        String pascal = key.substring(0, 1).toUpperCase(Locale.ROOT) + key.substring(1);
        return item.optBoolean(pascal, false);
    }

    private void forEach(JSONArray array, int limit, JsonConsumer consumer) {
        for (int i = 0; i < Math.min(array.length(), limit); i++) {
            JSONObject item = array.optJSONObject(i);
            if (item != null) {
                consumer.accept(item);
            }
        }
    }

    private String shortDate(String value) {
        if (value == null || value.equals("-") || value.equals("null")) {
            return "-";
        }
        String clean = value.replace("T", " ");
        return clean.length() > 16 ? clean.substring(0, 16) : clean;
    }

    private int statusColor(String status) {
        String value = normalize(status);
        if (value.contains("cancel") || value.contains("demor") || value.contains("retras")) {
            return RED;
        }
        if (value.contains("program") || value.contains("abord")) {
            return TEAL;
        }
        if (value.contains("final") || value.contains("complet") || value.contains("activo")) {
            return GOLD;
        }
        return MUTED;
    }

    private int openIncidents() {
        int total = 0;
        for (int i = 0; i < severities.length(); i++) {
            total += parseInt(value(severities.optJSONObject(i), "abiertos", "0"), 0);
        }
        return total;
    }

    private int findAirportId(String query) {
        String term = normalize(query);
        for (int i = 0; i < airports.length(); i++) {
            JSONObject airport = airports.optJSONObject(i);
            if (airport == null) {
                continue;
            }
            String name = value(airport, "nombre", "");
            String country = value(airport, "pais", "");
            String city = value(airport, "ciudad", "");
            if (normalize(name).contains(term) || term.contains(normalize(name)) || normalize(country).contains(term) || normalize(city).contains(term)) {
                return parseInt(value(airport, "id", "0"), 0);
            }
        }
        return 0;
    }

    private double fare(String className, int passengers) {
        double multiplier = "primera".equals(className) ? 1.68 : "ejecutiva".equals(className) ? 1.32 : 1.0;
        return Math.round(BASE_FARE * multiplier * Math.max(1, passengers) * 100.0) / 100.0;
    }

    private String classLabel(String className) {
        if ("primera".equals(className)) {
            return "Primera clase";
        }
        if ("ejecutiva".equals(className)) {
            return "Ejecutiva";
        }
        return "Turista";
    }

    private String money(double value) {
        NumberFormat format = NumberFormat.getCurrencyInstance(new Locale("es", "GT"));
        return format.format(value);
    }

    private int parseInt(String value, int fallback) {
        try {
            return Integer.parseInt(value == null ? "" : value.trim());
        } catch (Exception ignored) {
            return fallback;
        }
    }

    private double parseDouble(String value) {
        try {
            return Double.parseDouble(value == null ? "" : value.trim());
        } catch (Exception ignored) {
            return 0;
        }
    }

    private String prettify(String value) {
        return value == null ? "" : value.replace("AER_", "").replace("_", " ").toLowerCase(Locale.ROOT);
    }

    private String encode(String value) {
        return URLEncoder.encode(value == null ? "" : value, StandardCharsets.UTF_8);
    }

    private String nowIso() {
        return new java.text.SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.US).format(new java.util.Date());
    }

    private String firstError(String current, Exception exception) {
        return current == null || current.isEmpty() ? (exception.getMessage() == null ? exception.toString() : exception.getMessage()) : current;
    }

    private interface ResponseRenderer {
        void render(String json) throws Exception;
    }

    private interface JsonConsumer {
        void accept(JSONObject item);
    }

    private interface RowFormatter {
        String[] lines(JSONObject item);
    }
}
