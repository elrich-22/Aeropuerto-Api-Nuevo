package com.aeropuertoaurora.android;

import android.app.Activity;
import android.app.DatePickerDialog;
import android.graphics.Color;
import android.graphics.Typeface;
import android.graphics.drawable.GradientDrawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.InputType;
import android.text.InputFilter;
import android.text.SpannableString;
import android.text.Spanned;
import android.text.TextWatcher;
import android.text.style.ImageSpan;
import android.view.Gravity;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.NumberPicker;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.PopupMenu;
import android.widget.ScrollView;
import android.widget.Spinner;
import android.widget.TextView;

import org.json.JSONArray;
import org.json.JSONObject;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.text.NumberFormat;
import java.text.Normalizer;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.HashMap;
import java.util.Locale;
import java.util.Map;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public final class MainActivity extends Activity {
    private static final int DEEP = Color.rgb(15, 23, 42);
    private static final int SKY = Color.rgb(245, 249, 252);
    private static final int PANEL = Color.rgb(255, 255, 255);
    private static final int PANEL_LIGHT = Color.rgb(239, 249, 253);
    private static final int LINE = Color.rgb(226, 232, 240);
    private static final int TEXT = Color.rgb(15, 23, 42);
    private static final int MUTED = Color.rgb(100, 116, 139);
    private static final int TEAL = Color.rgb(0, 180, 216);
    private static final int GOLD = Color.rgb(0, 119, 182);
    private static final int PRIMARY_DARK = Color.rgb(2, 62, 138);
    private static final int RED = Color.rgb(221, 35, 45);
    private static final int GREEN = Color.rgb(24, 157, 65);
    private static final int SOFT_GREEN = Color.rgb(230, 248, 234);
    private static final int APP_BG = Color.rgb(245, 249, 252);
    private static final double BASE_FARE = 1250.0;
    private static final double SERVICE_SEAT = 120.0;
    private static final double SERVICE_BAG = 280.0;
    private static final double SERVICE_VIP = 360.0;
    private static final double SERVICE_PRIORITY = 95.0;
    private static final String TRANSFER_ACCOUNT_NAME = "Aeropuerto La Aurora";
    private static final String TRANSFER_ACCOUNT_NUMBER = "001-445889-7";

    private final ExecutorService executor = Executors.newSingleThreadExecutor();
    private final Handler mainHandler = new Handler(Looper.getMainLooper());

    private SettingsStore settingsStore;
    private ApiClient apiClient;
    private ScrollView screenScrollView;
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
    private JSONObject lastPurchaseSuccess;

    private JSONObject health;
    private JSONArray flights = new JSONArray();
    private JSONArray airports = new JSONArray();
    private JSONArray destinations = new JSONArray();
    private JSONArray severities = new JSONArray();
    private JSONArray baggage = new JSONArray();
    private JSONArray incidents = new JSONArray();
    private JSONArray maintenance = new JSONArray();
    private JSONArray occupancy = new JSONArray();
    private JSONArray lostObjects = new JSONArray();
    private JSONArray myReservations = new JSONArray();
    private JSONArray myCheckIns = new JSONArray();
    private JSONArray myBoardingPasses = new JSONArray();
    private JSONArray tables = new JSONArray();

    private String selectedTable = "";
    private JSONObject adminMetadata;
    private JSONArray adminRows = new JSONArray();
    private JSONObject editingRow;
    private final Map<String, EditText> adminInputs = new HashMap<>();
    private JSONObject checkoutFlight;
    private JSONObject fareSelectionFlight;
    private String fareSelectionLeg = "ida";
    private JSONObject selectedOutboundFlight;
    private JSONObject selectedReturnFlight;
    private String[] savedPassengerNames = new String[0];
    private String[] savedPassengerDocs = new String[0];
    private String savedHolderName = "";
    private String savedHolderEmail = "";
    private boolean selectedSeat;
    private boolean selectedBag;
    private boolean selectedVip;
    private boolean selectedPriority;
    private String criteriaTripType = "roundtrip";
    private String criteriaOrigin = "";
    private String criteriaDestination = "";
    private String criteriaDepartureDate = "";
    private String criteriaReturnDate = "";
    private String datePickerMode = "departure";
    private int travelCalendarYear = 2026;
    private int travelCalendarMonth = 5;
    private int criteriaAdults = 1;
    private int criteriaYouth = 0;
    private int criteriaChildren = 0;
    private int criteriaBabies = 0;
    private int dynamicContentStartIndex = 1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        settingsStore = new SettingsStore(this);
        apiClient = new ApiClient(settingsStore.getBaseUrl(), settingsStore.getApiKey());
        loadLocalState();
        setContentView(buildScreen());
        render();
        loadDashboard();
        if (sessionUser != null) {
            syncCartFromServer(false);
        }
    }

    @Override
    protected void onDestroy() {
        executor.shutdownNow();
        super.onDestroy();
    }

    @Override
    public void onBackPressed() {
        String previous = previousViewFor(activeView);
        if (previous == null) {
            return;
        }
        navigateTo(previous);
    }

    private View buildScreen() {
        ScrollView scrollView = new ScrollView(this);
        screenScrollView = scrollView;
        scrollView.setFillViewport(true);
        scrollView.setBackgroundColor(APP_BG);

        rootLayout = new LinearLayout(this);
        rootLayout.setOrientation(LinearLayout.VERTICAL);
        rootLayout.setPadding(dp(12), dp(12), dp(12), dp(30));
        scrollView.addView(rootLayout, matchWrap());

        LinearLayout header = new LinearLayout(this);
        header.setOrientation(LinearLayout.VERTICAL);
        header.setPadding(dp(4), 0, dp(4), 0);
        rootLayout.addView(header, matchWrap());

        statusText = text("Preparando panel movil...", 13, TEXT, Typeface.BOLD);
        statusText.setPadding(dp(12), dp(10), dp(12), dp(10));
        statusText.setBackground(round(SKY, LINE, dp(14)));
        statusText.setVisibility(View.GONE);
        header.addView(statusText, matchWrap());

        sessionText = text("", 13, MUTED, Typeface.NORMAL);
        sessionText.setPadding(dp(2), dp(8), 0, 0);
        sessionText.setVisibility(View.GONE);
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
        } else if ("travel_results".equals(activeView)) {
            renderTravelResults();
        } else if ("select_origin".equals(activeView)) {
            renderAirportPicker(true, "");
        } else if ("select_destination".equals(activeView)) {
            renderAirportPicker(false, "");
        } else if ("select_dates".equals(activeView)) {
            renderDatePicker();
        } else if ("select_passengers".equals(activeView)) {
            renderPassengerSelector();
        } else if ("select_fare".equals(activeView)) {
            renderFareSelection();
        } else if ("trip_review".equals(activeView)) {
            renderTripReview();
        } else if ("passenger_info".equals(activeView)) {
            renderPassengerInfoStep();
        } else if ("holder_info".equals(activeView)) {
            renderHolderInfoStep();
        } else if ("trip_options".equals(activeView)) {
            renderTripOptionsStep();
        } else if ("payment".equals(activeView)) {
            renderPaymentStep();
        } else if ("rastreo".equals(activeView)) {
            renderBoard();
        } else if ("menu_more".equals(activeView)) {
            renderMoreInformation();
        } else if ("settings".equals(activeView)) {
            renderConnectionCard();
        } else if ("login".equals(activeView)) {
            renderSessionCard();
        } else if ("mis_viajes".equals(activeView)) {
            renderMyTrips();
        } else if ("checkin".equals(activeView)) {
            renderCheckIn();
        } else if ("objetos".equals(activeView)) {
            renderLostObjects();
        } else if ("operaciones".equals(activeView)) {
            renderOperations();
        } else if ("reportes".equals(activeView)) {
            renderReports();
        } else if ("carrito".equals(activeView)) {
            renderCart();
        } else if ("checkout".equals(activeView)) {
            renderCheckout();
        } else if ("purchase_success".equals(activeView)) {
            renderPurchaseSuccess();
        } else if ("admin".equals(activeView)) {
            renderAdmin();
        } else {
            renderHome();
        }
    }

    private void renderNav() {
        navLayout.removeAllViews();
        if ("menu_more".equals(activeView)) {
            return;
        }
        LinearLayout topbar = new LinearLayout(this);
        topbar.setOrientation(LinearLayout.HORIZONTAL);
        topbar.setGravity(Gravity.CENTER_VERTICAL);
        topbar.setPadding(0, 0, 0, dp(8));

        LinearLayout brand = new LinearLayout(this);
        brand.setOrientation(LinearLayout.VERTICAL);
        TextView logo = text("a\u2708", 30, GOLD, Typeface.BOLD);
        TextView subtitle = text("La Aurora", 12, MUTED, Typeface.BOLD);
        brand.addView(logo);
        brand.addView(subtitle);
        brand.setOnClickListener(v -> navigateTo("inicio"));
        topbar.addView(brand, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f));

        Button account = accountButton(sessionUser == null ? "Iniciar sesión" : "Cuenta", v -> {
        });
        account.setOnClickListener(v -> {
            if (sessionUser == null) {
                navigateTo("login");
            } else {
                showAccountMenu(account);
            }
        });
        topbar.addView(account, iconParams());
        topbar.addView(iconButton("menu_more".equals(activeView) ? "X" : "\u2630", v -> {
            if ("menu_more".equals(activeView)) {
                navigateTo("inicio");
            } else {
                navigateTo("menu_more");
            }
        }), iconParams());

        navLayout.addView(topbar, matchWrap());
    }

    private void renderHome() {
        LinearLayout hero = card();
        hero.setPadding(dp(20), dp(22), dp(20), dp(24));
        hero.setBackground(gradientRound(GOLD, TEAL, dp(28)));
        hero.addView(text("Hola,", 28, Color.WHITE, Typeface.NORMAL));
        hero.addView(text("a donde quieres viajar?", 28, Color.WHITE, Typeface.BOLD));

        TextView search = text("\uD83D\uDD0D   Buscar un vuelo", 17, MUTED, Typeface.NORMAL);
        search.setPadding(dp(18), dp(16), dp(18), dp(16));
        search.setMinHeight(dp(62));
        search.setBackground(round(Color.WHITE, Color.TRANSPARENT, dp(32)));
        search.setOnClickListener(v -> navigateTo("explorar"));
        LinearLayout.LayoutParams searchParams = matchWrap();
        searchParams.setMargins(0, dp(24), 0, 0);
        hero.addView(search, searchParams);
        contentLayout.addView(hero);

        renderDestinations();
    }

    private void renderMoreInformation() {
        LinearLayout titleRow = new LinearLayout(this);
        titleRow.setOrientation(LinearLayout.HORIZONTAL);
        titleRow.setGravity(Gravity.CENTER_VERTICAL);
        TextView title = text("Más información", 22, TEXT, Typeface.BOLD);
        title.setGravity(Gravity.CENTER);
        titleRow.addView(title, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f));
        titleRow.addView(iconButton("X", v -> navigateTo("inicio")), iconParams());
        contentLayout.addView(titleRow, matchWrap());

        LinearLayout trip = card();
        trip.addView(sectionTitle("Menu"));
        if (sessionUser == null) {
            trip.addView(menuOption("\u25EF", "Iniciar sesión", "Entrar o crear cuenta", "login"));
        }
        trip.addView(menuOption("\u2708", "Explorar", "Busca y reserva vuelos", "explorar"));
        trip.addView(menuOption("\u25CE", "Rastreo", "Tracking de aviones en tiempo real", "rastreo"));
        trip.addView(menuOption("\u25A3", "Mis viajes", "Reservas, check-in y tarjetas", "mis_viajes"));
            trip.addView(menuOption("\u2713", "Check-in", "Confirma tu reserva por vuelo o código", "checkin"));
        trip.addView(menuOption("?", "Objetos perdidos", "Reporta una perdida o revisa encontrados", "objetos"));
        trip.addView(menuOption("\u25C9", "Ubicación", "Ver donde está el aeropuerto", "ubicacion"));
        contentLayout.addView(trip);
    }

    private void renderPurchaseSuccess() {
        JSONObject success = lastPurchaseSuccess == null ? new JSONObject() : lastPurchaseSuccess;
        LinearLayout card = card();
        card.addView(text("Pago exitoso", 14, TEAL, Typeface.BOLD));
        card.addView(text("Tu reserva esta confirmada", 28, TEXT, Typeface.BOLD));
        card.addView(text("Número de reserva", 14, MUTED, Typeface.NORMAL));
        card.addView(text(value(success, "reservas", "-"), 20, GOLD, Typeface.BOLD));
        card.addView(text("Total pagado: " + money(parseDouble(value(success, "total", "0"))), 18, TEXT, Typeface.BOLD));
        card.addView(text("Pasajeros: " + value(success, "pasajeros", "1"), 14, MUTED, Typeface.NORMAL));

        JSONArray purchasedFlights = success.optJSONArray("vuelos");
        if (purchasedFlights != null) {
            for (int i = 0; i < purchasedFlights.length(); i++) {
                JSONObject item = purchasedFlights.optJSONObject(i);
                if (item == null) {
                    continue;
                }
                LinearLayout flightCard = compactCard();
                flightCard.addView(text(value(item, "etiqueta", "Vuelo"), 12, TEAL, Typeface.BOLD));
                flightCard.addView(text(value(item, "numeroVuelo", "-"), 18, TEXT, Typeface.BOLD));
                flightCard.addView(text(value(item, "ruta", "-"), 14, MUTED, Typeface.NORMAL));
                flightCard.addView(text("Quedan " + value(item, "plazasDisponibles", "0") + " plazas disponibles", 13, GOLD, Typeface.BOLD));
                card.addView(flightCard);
            }
        }

        LinearLayout actions = row();
        actions.addView(outlineButton("Inicio", v -> {
            lastPurchaseSuccess = null;
            activeView = "inicio";
            render();
        }), weightedButtonParams());
        actions.addView(primaryButton("Buscar otro vuelo", v -> {
            lastPurchaseSuccess = null;
            activeView = "explorar";
            render();
        }), weightedButtonParams());
        card.addView(actions);
        contentLayout.addView(card);
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
        card.addView(sectionTitle(sessionUser == null ? "Iniciar sesión" : "Cuenta"));
        if (sessionUser != null) {
            card.addView(text(value(sessionUser, "nombreCompleto", value(sessionUser, "usuario", "Usuario")), 16, TEXT, Typeface.BOLD));
            card.addView(text(value(sessionUser, "email", ""), 13, MUTED, Typeface.NORMAL));
            LinearLayout row = row();
            row.addView(outlineButton("Salir", v -> logout()), weightedButtonParams());
            card.addView(row);
        } else {
            EditText user = input("Usuario o email", "", false);
            user.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS);
            PasswordInput password = passwordInput("Contraseña");
            card.addView(user);
            card.addView(password.layout);
            LinearLayout row = row();
            row.addView(primaryButton("Entrar", v -> login(user.getText().toString(), password.field.getText().toString())), weightedButtonParams());
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
        PasswordInput contrasena = passwordInput("Contraseña");
        EditText documento = input("Número de documento", "", false);
        EditText tipoDocumento = input("Tipo documento (DPI/Pasaporte)", "DPI", false);
        EditText primerNombre = input("Primer nombre", "", false);
        EditText segundoNombre = input("Segundo nombre", "", false);
        EditText primerApellido = input("Primer apellido", "", false);
        EditText segundoApellido = input("Segundo apellido", "", false);
        EditText fechaNacimiento = dateInput("Fecha de nacimiento");
        EditText nacionalidad = input("Nacionalidad", "Guatemala", false);
        Spinner sexo = spinner(new String[]{"Selecciona sexo", "Masculino", "Femenino"}, "Selecciona sexo");
        EditText telefono = input("Teléfono", "", false);
        EditText[] fields = {usuario, email, contrasena.field, documento, tipoDocumento, primerNombre, segundoNombre, primerApellido, segundoApellido, fechaNacimiento, nacionalidad, telefono};
        applyDangerousCharacterFilter(fields);
        card.addView(usuario);
        card.addView(email);
        card.addView(contrasena.layout);
        card.addView(documento);
        card.addView(tipoDocumento);
        card.addView(primerNombre);
        card.addView(segundoNombre);
        card.addView(primerApellido);
        card.addView(segundoApellido);
        card.addView(fechaNacimiento);
        card.addView(nacionalidad);
        card.addView(label("Sexo"));
        card.addView(sexo);
        card.addView(telefono);
        card.addView(text("La contraseña debe tener mínimo 8 caracteres, una mayúscula, un número y un símbolo seguro.", 13, MUTED, Typeface.NORMAL));
        card.addView(primaryButton("Crear cuenta", v -> register(usuario, email, contrasena.field, documento, tipoDocumento, primerNombre, segundoNombre, primerApellido, segundoApellido, fechaNacimiento, nacionalidad, sexo, telefono)), matchWrap());
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
        card.addView(sectionTitle("Destinos más buscados"));
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
        card.addView(sectionTitle("Ubicación"));
        card.addView(text("Ciudad de Guatemala - Codigo IATA: GUA - Codigo ICAO: MGGT", 14, MUTED, Typeface.NORMAL));
        card.addView(text("Referencia: zona 13, acceso principal a terminal internacional.", 14, MUTED, Typeface.NORMAL));
        contentLayout.addView(card);
    }

    private void renderExplore() {
        LinearLayout card = card();
        card.addView(sectionTitle("Reservar vuelo"));
        LinearLayout typeRow = row();
        typeRow.addView("roundtrip".equals(criteriaTripType)
                ? primaryButton("Ida y vuelta", v -> {
                })
                : outlineButton("Ida y vuelta", v -> {
                    criteriaTripType = "roundtrip";
                    render();
                }), weightedButtonParams());
        typeRow.addView("oneway".equals(criteriaTripType)
                ? primaryButton("Solo ida", v -> {
                })
                : outlineButton("Solo ida", v -> {
                    criteriaTripType = "oneway";
                    criteriaReturnDate = "";
                    datePickerMode = "departure";
                    render();
                }), weightedButtonParams());
        card.addView(typeRow);

        card.addView(label("Ruta"));
        card.addView(selectionField("Origen", criteriaOrigin.isEmpty() ? "Ciudad de Guatemala (GUA)" : airportDisplay(criteriaOrigin), v -> {
            activeView = "select_origin";
            render();
        }));
        card.addView(selectionField("Destino", criteriaDestination.isEmpty() ? "Seleccionar destino" : airportDisplay(criteriaDestination), v -> {
            activeView = "select_destination";
            render();
        }));

        card.addView(label("Fechas"));
        String dateSummary = criteriaDepartureDate.isEmpty() ? "Salida" : displayDate(criteriaDepartureDate);
        if ("roundtrip".equals(criteriaTripType)) {
            dateSummary += "  -  " + (criteriaReturnDate.isEmpty() ? "Vuelta" : displayDate(criteriaReturnDate));
        }
        card.addView(selectionField("Fecha de viaje", dateSummary, v -> {
            datePickerMode = "departure";
            setTravelCalendarFromDate(criteriaDepartureDate);
            activeView = "select_dates";
            render();
        }));

        card.addView(label("Quienes vuelan"));
        card.addView(selectionField("Pasajeros", passengerSummary(), v -> {
            activeView = "select_passengers";
            render();
        }));

        card.addView(primaryButton("Explorar", v -> {
            if (criteriaOrigin.trim().isEmpty()) {
                criteriaOrigin = "La Aurora";
            }
            selectedOutboundFlight = null;
            selectedReturnFlight = null;
            activeView = "travel_results";
            render();
        }), matchWrap());
        contentLayout.addView(card);
    }

    private void renderAirportPicker(boolean originPicker, String term) {
        contentLayout.addView(screenHeader(originPicker ? "Seleccionar origen" : "Seleccionar destino", "Busca por ciudad, país o código de aeropuerto.", "explorar", "", null));

        LinearLayout card = card();

        EditText search = input(originPicker ? "Buscar origen" : "Buscar destino", term, false);
        card.addView(search);
        contentLayout.addView(card);

        LinearLayout list = card();
        populateAirportList(list, originPicker, term);
        search.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence value, int start, int count, int after) {
            }

            @Override
            public void onTextChanged(CharSequence value, int start, int before, int count) {
                populateAirportList(list, originPicker, value == null ? "" : value.toString());
            }

            @Override
            public void afterTextChanged(Editable editable) {
            }
        });
        contentLayout.addView(fixedScroll(list, 560));
    }

    private void populateAirportList(LinearLayout list, boolean originPicker, String term) {
        list.removeAllViews();
        list.addView(sectionTitle("Aeropuertos disponibles"));
        int shown = 0;
        for (int i = 0; i < airports.length(); i++) {
            JSONObject airport = airports.optJSONObject(i);
            if (airport == null || !matchesAirport(airport, term)) {
                continue;
            }
            shown++;
            list.addView(airportRow(airport, originPicker));
        }
        if (shown == 0) {
            JSONObject fallback = fallbackAirport(term);
            if (fallback != null) {
                list.addView(airportRow(fallback, originPicker));
            } else {
                list.addView(text("No encontramos aeropuertos con ese texto.", 13, MUTED, Typeface.NORMAL));
            }
        }
    }

    private View airportRow(JSONObject airport, boolean originPicker) {
        LinearLayout item = compactCard();
        item.setMinimumHeight(dp(118));
        item.setOnClickListener(v -> selectAirport(airport, originPicker));
        LinearLayout line = new LinearLayout(this);
        line.setOrientation(LinearLayout.HORIZONTAL);
        line.setGravity(Gravity.CENTER_VERTICAL);
        TextView names = text(airportCityCountry(airport) + "\n" + airportTitle(airport), 16, TEXT, Typeface.BOLD);
        names.setMaxLines(3);
        names.setEllipsize(android.text.TextUtils.TruncateAt.END);
        TextView code = text(airportCodeFromAirport(airport), 24, TEXT, Typeface.NORMAL);
        code.setGravity(Gravity.RIGHT | Gravity.CENTER_VERTICAL);
        line.addView(names, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f));
        line.addView(code, new LinearLayout.LayoutParams(dp(68), LinearLayout.LayoutParams.WRAP_CONTENT));
        item.addView(line);
        return item;
    }

    private void selectAirport(JSONObject airport, boolean originPicker) {
        String name = airportTitle(airport);
        if (originPicker) {
            criteriaOrigin = name;
        } else {
            criteriaDestination = name;
            registerDestinationClick(airport);
        }
        activeView = "explorar";
        render();
    }

    private void renderDatePicker() {
        contentLayout.addView(screenHeader("Fecha de viaje", "Elige salida y vuelta con tarifas disponibles.", "explorar", "", null));

        LinearLayout card = card();

        LinearLayout typeRow = row();
        typeRow.addView("roundtrip".equals(criteriaTripType)
                ? primaryButton("Ida y vuelta", v -> {
                })
                : outlineButton("Ida y vuelta", v -> {
                    criteriaTripType = "roundtrip";
                    render();
                }), weightedButtonParams());
        typeRow.addView("oneway".equals(criteriaTripType)
                ? primaryButton("Solo ida", v -> {
                })
                : outlineButton("Solo ida", v -> {
                    criteriaTripType = "oneway";
                    criteriaReturnDate = "";
                    datePickerMode = "departure";
                    render();
                }), weightedButtonParams());
        card.addView(typeRow);

        if ("roundtrip".equals(criteriaTripType)) {
            LinearLayout modeRow = row();
            modeRow.addView("departure".equals(datePickerMode)
                    ? primaryButton("Salida", v -> {
                    })
                    : outlineButton("Salida", v -> {
                        datePickerMode = "departure";
                        setTravelCalendarFromDate(criteriaDepartureDate);
                        render();
                    }), weightedButtonParams());
            modeRow.addView("return".equals(datePickerMode)
                    ? primaryButton("Vuelta", v -> {
                    })
                    : outlineButton("Vuelta", v -> {
                        datePickerMode = "return";
                        setTravelCalendarFromDate(criteriaReturnDate.isEmpty() ? criteriaDepartureDate : criteriaReturnDate);
                        render();
                    }), weightedButtonParams());
            card.addView(modeRow);
        }

        card.addView(text("Salida: " + (criteriaDepartureDate.isEmpty() ? "-" : displayDate(criteriaDepartureDate)), 14, MUTED, Typeface.NORMAL));
        if ("roundtrip".equals(criteriaTripType)) {
            card.addView(text("Vuelta: " + (criteriaReturnDate.isEmpty() ? "-" : displayDate(criteriaReturnDate)), 14, MUTED, Typeface.NORMAL));
        }
        contentLayout.addView(card);

        clampTravelCalendarMonth();
        LinearLayout calendar = card();
        calendar.setBackground(round(PANEL, LINE, dp(22)));
        LinearLayout monthControls = row();
        Button previous = outlineButton("<", v -> {
            moveTravelCalendarMonth(-1);
            render();
        });
        previous.setEnabled(canMoveTravelCalendarMonth(-1));
        Button next = outlineButton(">", v -> {
            moveTravelCalendarMonth(1);
            render();
        });
        next.setEnabled(canMoveTravelCalendarMonth(1));
        monthControls.addView(previous, new LinearLayout.LayoutParams(dp(58), LinearLayout.LayoutParams.WRAP_CONTENT));
        TextView currentMonth = text(monthTitle(travelCalendarYear, travelCalendarMonth), 18, PRIMARY_DARK, Typeface.BOLD);
        currentMonth.setGravity(Gravity.CENTER);
        monthControls.addView(currentMonth, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f));
        monthControls.addView(next, new LinearLayout.LayoutParams(dp(58), LinearLayout.LayoutParams.WRAP_CONTENT));
        calendar.addView(monthControls);
        addCalendarMonth(calendar, travelCalendarYear, travelCalendarMonth, faresForMonth(travelCalendarYear, travelCalendarMonth));
        contentLayout.addView(calendar);
    }

    private void addCalendarMonth(LinearLayout parent, int year, int month, Map<String, Double> dailyFares) {
        java.util.Calendar calendar = java.util.Calendar.getInstance();
        calendar.set(year, month - 1, 1);
        String[] weekdays = {"Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom"};
        LinearLayout week = row();
        for (String weekday : weekdays) {
            TextView day = text(weekday, 12, MUTED, Typeface.BOLD);
            day.setGravity(Gravity.CENTER);
            week.addView(day, calendarCellParams());
        }
        parent.addView(week);

        int firstDay = calendar.get(java.util.Calendar.DAY_OF_WEEK);
        int offset = firstDay == java.util.Calendar.SUNDAY ? 6 : firstDay - 2;
        int maxDay = calendar.getActualMaximum(java.util.Calendar.DAY_OF_MONTH);
        int day = 1;
        for (int rowIndex = 0; rowIndex < 6 && day <= maxDay; rowIndex++) {
            LinearLayout row = row();
            for (int column = 0; column < 7; column++) {
                if ((rowIndex == 0 && column < offset) || day > maxDay) {
                    TextView blank = text("", 16, TEXT, Typeface.NORMAL);
                    row.addView(blank, calendarCellParams());
                    continue;
                }
                String value = String.format(Locale.US, "%04d-%02d-%02d", year, month, day);
                double price = dailyFares.containsKey(value) ? dailyFares.get(value) : 0;
                boolean invalidReturnDate = isInvalidReturnDate(value);
                String label = price > 0 ? day + "\n" + compactMoney(price) : day + "\n-";
                Button dateButton = baseButton(label, v -> selectDate(value));
                dateButton.setTextColor(dateSelected(value) ? Color.WHITE : TEXT);
                dateButton.setTextSize(12);
                dateButton.setMinHeight(dp(62));
                dateButton.setMinWidth(0);
                dateButton.setMinimumWidth(0);
                dateButton.setPadding(0, dp(4), 0, dp(4));
                dateButton.setIncludeFontPadding(false);
                dateButton.setEnabled(!invalidReturnDate && (price > 0 || criteriaDestination.trim().isEmpty()));
                dateButton.setBackground(round(dateSelected(value) ? GOLD : (price > 0 ? PANEL_LIGHT : Color.rgb(241, 245, 249)), dateSelected(value) ? GOLD : LINE, dp(18)));
                row.addView(dateButton, calendarCellParams());
                day++;
            }
            parent.addView(row);
        }
    }

    private Map<String, Double> faresForMonth(int year, int month) {
        Map<String, Double> fares = new HashMap<>();
        String prefix = String.format(Locale.US, "%04d-%02d", year, month);
        int passengers = passengerCountFromCriteria();
        for (int i = 0; i < flights.length(); i++) {
            JSONObject flight = flights.optJSONObject(i);
            String flightDate = value(flight, "fechaVuelo", "");
            if (flight == null || !canPurchase(flight) || flightDate.length() < 10 || !flightDate.startsWith(prefix)) {
                continue;
            }
            String date = flightDate.substring(0, 10);
            if (!matchesCalendarFlight(flight, date)) {
                continue;
            }
            double price = flightFare(flight, "economica", passengers);
            if (!fares.containsKey(date) || price < fares.get(date)) {
                fares.put(date, price);
            }
        }
        return fares;
    }

    private String monthTitle(int year, int month) {
        Calendar calendar = Calendar.getInstance();
        calendar.set(year, month - 1, 1);
        return new java.text.SimpleDateFormat("MMMM yyyy", new Locale("es", "GT")).format(calendar.getTime());
    }

    private void setTravelCalendarFromDate(String date) {
        if (date != null && date.matches("\\d{4}-\\d{2}-\\d{2}")) {
            travelCalendarYear = parseInt(date.substring(0, 4), travelCalendarYear);
            travelCalendarMonth = parseInt(date.substring(5, 7), travelCalendarMonth);
        }
        clampTravelCalendarMonth();
    }

    private void moveTravelCalendarMonth(int delta) {
        int index = travelCalendarYear * 12 + travelCalendarMonth - 1 + delta;
        travelCalendarYear = index / 12;
        travelCalendarMonth = index % 12 + 1;
        clampTravelCalendarMonth();
    }

    private boolean canMoveTravelCalendarMonth(int delta) {
        int index = travelCalendarYear * 12 + travelCalendarMonth - 1 + delta;
        return index >= travelCalendarStartIndex() && index <= travelCalendarEndIndex();
    }

    private void clampTravelCalendarMonth() {
        int index = travelCalendarYear * 12 + travelCalendarMonth - 1;
        if (index < travelCalendarStartIndex()) {
            travelCalendarYear = 2026;
            travelCalendarMonth = 5;
        } else if (index > travelCalendarEndIndex()) {
            travelCalendarYear = 2026;
            travelCalendarMonth = 12;
        }
    }

    private int travelCalendarStartIndex() {
        return 2026 * 12 + 5 - 1;
    }

    private int travelCalendarEndIndex() {
        return 2026 * 12 + 12 - 1;
    }

    private void selectDate(String value) {
        if ("return".equals(datePickerMode)) {
            if (isInvalidReturnDate(value)) {
                statusText.setText("La fecha de regreso debe ser posterior a la fecha de ida.");
                return;
            }
            criteriaReturnDate = value;
            activeView = "explorar";
        } else {
            criteriaDepartureDate = value;
            if (!criteriaReturnDate.isEmpty() && criteriaReturnDate.compareTo(value) <= 0) {
                criteriaReturnDate = "";
            }
            if ("roundtrip".equals(criteriaTripType)) {
                datePickerMode = "return";
                activeView = "select_dates";
            } else {
                activeView = "explorar";
            }
        }
        render();
    }

    private void renderPassengerSelector() {
        contentLayout.addView(screenHeader("¿Quiénes vuelan?", "Selecciona los pasajeros para esta búsqueda.", "explorar", "", null));
        LinearLayout card = card();
        card.addView(counterRow("Adultos", "Pasajero adulto", criteriaAdults, value -> criteriaAdults = Math.max(1, value)));
        card.addView(counterRow("Jóvenes", "Pasajero joven", criteriaYouth, value -> criteriaYouth = Math.max(0, value)));
        card.addView(counterRow("Niños", "Pasajero menor", criteriaChildren, value -> criteriaChildren = Math.max(0, value)));
        card.addView(counterRow("Bebés", "Bebé en brazos", criteriaBabies, value -> criteriaBabies = Math.max(0, value)));
        card.addView(primaryButton("Listo", v -> {
            activeView = "explorar";
            render();
        }), matchWrap());
        contentLayout.addView(card);
    }

    private View counterRow(String title, String subtitle, int value, CounterSetter setter) {
        LinearLayout item = compactCard();
        LinearLayout line = new LinearLayout(this);
        line.setOrientation(LinearLayout.HORIZONTAL);
        TextView label = text(title + "\n" + subtitle, 16, TEXT, Typeface.BOLD);
        TextView number = text(String.valueOf(value), 20, TEXT, Typeface.BOLD);
        number.setGravity(Gravity.CENTER);
        Button minus = outlineButton("-", v -> {
            setter.set(value - 1);
            render();
        });
        Button plus = outlineButton("+", v -> {
            setter.set(value + 1);
            render();
        });
        line.addView(label, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f));
        line.addView(minus, new LinearLayout.LayoutParams(dp(58), LinearLayout.LayoutParams.WRAP_CONTENT));
        line.addView(number, new LinearLayout.LayoutParams(dp(48), LinearLayout.LayoutParams.WRAP_CONTENT));
        line.addView(plus, new LinearLayout.LayoutParams(dp(58), LinearLayout.LayoutParams.WRAP_CONTENT));
        item.addView(line);
        return item;
    }

    private void showTravelResults(String origin, String destination, String date, int passengers) {
        if (contentLayout.getChildCount() > dynamicContentStartIndex) {
            contentLayout.removeViews(dynamicContentStartIndex, contentLayout.getChildCount() - dynamicContentStartIndex);
        }
        registerFlightSearch(origin, destination, date, passengers);
        LinearLayout results = card();
        results.addView(text(routeLabel(origin, destination), 24, TEXT, Typeface.BOLD));
        results.addView(text((date == null || date.isEmpty() ? "Fecha flexible" : displayDate(date)) + "  |  " + passengers + " pasajero(s)", 14, MUTED, Typeface.NORMAL));
        if ("roundtrip".equals(criteriaTripType)) {
            if (selectedOutboundFlight != null) {
                results.addView(selectedLegSummary("Ida seleccionada", selectedOutboundFlight));
                addFlightResultSection(results, "Vuelo de vuelta", destination, origin, criteriaReturnDate, passengers, "vuelta");
            } else {
                if (selectedReturnFlight != null) {
                    results.addView(selectedLegSummary("Vuelta seleccionada", selectedReturnFlight));
                }
                addFlightResultSection(results, "Vuelo de ida", origin, destination, date, passengers, "ida");
            }
        } else {
            addFlightResultSection(results, "Vuelo de ida", origin, destination, date, passengers, "ida");
        }
        contentLayout.addView(results);
    }

    private View selectedLegSummary(String title, JSONObject flight) {
        LinearLayout selected = compactCard();
        selected.setBackground(round(Color.rgb(232, 249, 252), TEAL, dp(14)));
        selected.addView(text(title, 13, TEAL, Typeface.BOLD));
        selected.addView(text(value(flight, "numeroVuelo", "-") + " - " + value(flight, "origen", "-") + " a " + value(flight, "destino", "-"), 16, TEXT, Typeface.BOLD));
        selected.addView(text(classLabel(value(flight, "selectedClass", "economica")), 13, MUTED, Typeface.NORMAL));
        selected.addView(outlineButton("Cambiar", v -> {
            if ("Ida seleccionada".equals(title)) {
                selectedOutboundFlight = null;
            } else {
                selectedReturnFlight = null;
            }
            activeView = "travel_results";
            render();
        }), matchWrap());
        return selected;
    }

    private void renderTravelResults() {
        if (criteriaOrigin.trim().isEmpty()) {
            criteriaOrigin = "La Aurora";
        }

        LinearLayout header = screenHeader("Vuelos disponibles", "Selecciona el tramo y luego la tarifa para continuar.", "", "Editar", v -> {
            activeView = "explorar";
            render();
        });
        header.addView(bookingProgress(1));
        contentLayout.addView(header);

        dynamicContentStartIndex = contentLayout.getChildCount();
        showTravelResults(criteriaOrigin, criteriaDestination, criteriaDepartureDate, passengerCountFromCriteria());
    }

    private void addFlightResultSection(LinearLayout results, String title, String origin, String destination, String date, int passengers, String leg) {
        results.addView(sectionTitle(title));
        results.addView(text(routeLabel(origin, destination) + " - " + (date == null || date.isEmpty() ? "fecha flexible" : displayDate(date)), 13, MUTED, Typeface.NORMAL));
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
            results.addView(flightCard(flight, passengers, true, leg));
            if (shown >= 20) {
                break;
            }
        }
        if (shown == 0) {
            results.addView(text("No hay vuelos programados para este tramo.", 13, MUTED, Typeface.NORMAL));
        }
    }

    private void renderBoard() {
        LinearLayout search = card();
        search.addView(sectionTitle("Rastreo de vuelos"));
        EditText term = input("Número, aerolínea, origen, destino o estado", "", false);
        search.addView(term);
        search.addView(primaryButton("Filtrar", v -> showBoardResults(term.getText().toString())), matchWrap());
        contentLayout.addView(search);
        dynamicContentStartIndex = contentLayout.getChildCount();
        showBoardResults("");
    }

    private void showBoardResults(String term) {
        if (contentLayout.getChildCount() > dynamicContentStartIndex) {
            contentLayout.removeViews(dynamicContentStartIndex, contentLayout.getChildCount() - dynamicContentStartIndex);
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
        return flightCard(flight, passengers, allowBuy, "ida");
    }

    private View flightCard(JSONObject flight, int passengers, boolean allowBuy, String leg) {
        LinearLayout item = compactCard();
        LinearLayout times = new LinearLayout(this);
        times.setOrientation(LinearLayout.HORIZONTAL);
        TextView left = text(timeOnly(value(flight, "fechaVuelo", "")) + "\n" + airportCode(value(flight, "origen", "")), 24, TEXT, Typeface.BOLD);
        left.setGravity(Gravity.LEFT);
        TextView center = text((hasTechnicalStop(flight) ? "1 Stop" : "Directo") + "\n------ ✈ ------", 15, TEAL, Typeface.BOLD);
        center.setGravity(Gravity.CENTER);
        TextView right = text(timeOnly(estimatedArrival(value(flight, "fechaVuelo", ""), estimateDurationMinutes(flight))) + "\n" + airportCode(value(flight, "destino", "")), 24, TEXT, Typeface.BOLD);
        right.setGravity(Gravity.RIGHT);
        times.addView(left, weightedButtonParams());
        times.addView(center, weightedButtonParams());
        times.addView(right, weightedButtonParams());
        item.addView(times);
        item.addView(text(value(flight, "numeroVuelo", "Vuelo") + " • " + estimateDurationMinutes(flight) + " min", 13, MUTED, Typeface.NORMAL));
        item.addView(text("Operado por " + value(flight, "aerolinea", "-"), 13, MUTED, Typeface.NORMAL));
        item.addView(text("Desde " + money(flightFare(flight, "economica", passengers)), 24, TEXT, Typeface.BOLD));
        if (allowBuy) {
            item.setOnClickListener(v -> openFareSelection(flight, leg));
        }
        return item;
    }

    private void openFareSelection(JSONObject flight, String leg) {
        fareSelectionFlight = flight;
        fareSelectionLeg = leg;
        activeView = "select_fare";
        render();
    }

    private void renderFareSelection() {
        if (fareSelectionFlight == null) {
            activeView = "explorar";
            render();
            return;
        }

        int passengers = passengerCountFromCriteria();
        String from = airportCode(value(fareSelectionFlight, "origen", ""));
        String to = airportCode(value(fareSelectionFlight, "destino", ""));
        String times = timeOnly(value(fareSelectionFlight, "fechaVuelo", "")) + " - " + timeOnly(estimatedArrival(value(fareSelectionFlight, "fechaVuelo", ""), estimateDurationMinutes(fareSelectionFlight)));
        LinearLayout header = screenHeader("Selecciona tu tarifa", ("vuelta".equals(fareSelectionLeg) ? "Vuelo de vuelta" : "Vuelo de ida") + " - " + passengers + " pasajero(s)", "travel_results", "", null);
        header.addView(text(from + "  " + times + "  " + to + "  |  " + displayDate(value(fareSelectionFlight, "fechaVuelo", "").length() >= 10 ? value(fareSelectionFlight, "fechaVuelo", "").substring(0, 10) : ""), 15, TEXT, Typeface.BOLD));
        contentLayout.addView(header);

        contentLayout.addView(tariffCard(fareSelectionFlight, "economica", "Económica", "La opción más simple para viajar ligero.", passengers, "1 artículo personal", "Cambios con cargo", false));
        contentLayout.addView(tariffCard(fareSelectionFlight, "turista", "Turista", "Mejor balance entre precio y beneficios.", passengers, "Artículo personal y equipaje de mano", "Selección de asiento estándar", true));
        contentLayout.addView(tariffCard(fareSelectionFlight, "primera", "Primera clase", "Mas comodidad y flexibilidad para tu viaje.", passengers, "Equipaje completo incluido", "Cambios y prioridad antes del vuelo", false));
    }

    private View tariffCard(JSONObject flight, String className, String name, String tagline, int passengers, String benefitOne, String benefitTwo, boolean recommended) {
        LinearLayout card = compactCard();
        card.setBackground(round(PANEL, LINE, dp(22)));
        if (recommended) {
            TextView badge = text("RECOMENDADA", 12, Color.WHITE, Typeface.BOLD);
            badge.setGravity(Gravity.CENTER);
            badge.setPadding(dp(12), dp(6), dp(12), dp(6));
            badge.setBackground(round(GOLD, GOLD, dp(18)));
            card.addView(badge, new LinearLayout.LayoutParams(dp(142), LinearLayout.LayoutParams.WRAP_CONTENT));
        }
        card.addView(text(name, 27, fareColor(className), Typeface.BOLD));
        card.addView(text(tagline, 14, MUTED, Typeface.NORMAL));
        card.addView(text("- " + benefitOne, 14, TEXT, Typeface.NORMAL));
        card.addView(text("- " + benefitTwo, 14, TEXT, Typeface.NORMAL));
        card.addView(text("- Check-in en el aeropuerto", 14, TEXT, Typeface.NORMAL));
        card.addView(text("Info de restricciones de tarifa", 14, TEAL, Typeface.NORMAL));
        card.addView(primaryButton(money(flightFare(flight, className, passengers)), v -> chooseFare(flight, className, passengers)), matchWrap());
        TextView perPassenger = text("Precio por pasajero", 12, MUTED, Typeface.NORMAL);
        perPassenger.setGravity(Gravity.CENTER);
        card.addView(perPassenger);
        return card;
    }

    private void chooseFare(JSONObject flight, String className, int passengers) {
        String upgrade = upgradeClass(className);
        if (!upgrade.isEmpty()) {
            showUpgradePrompt(flight, className, upgrade, passengers);
            return;
        }

        selectTripFare(flight, className, passengers);
    }

    private void selectTripFare(JSONObject flight, String className, int passengers) {
        try {
            JSONObject selected = new JSONObject(flight.toString())
                    .put("selectedClass", className)
                    .put("passengerCount", Math.max(1, passengers))
                    .put("tripLeg", fareSelectionLeg);
            if ("vuelta".equals(fareSelectionLeg)) {
                selectedReturnFlight = selected;
            } else {
                selectedOutboundFlight = selected;
            }

            if ("roundtrip".equals(criteriaTripType)) {
                if (selectedOutboundFlight == null) {
                    activeView = "travel_results";
                    lastMessage = "Ahora selecciona el vuelo de ida.";
                    render();
                    return;
                }
                if (selectedReturnFlight == null) {
                    activeView = "travel_results";
                    lastMessage = "Listo el vuelo de ida. Ahora selecciona el vuelo de vuelta.";
                    render();
                    return;
                }
            }

            activeView = "passenger_info";
            render();
        } catch (Exception exception) {
            showError(exception);
        }
    }

    private void showUpgradePrompt(JSONObject flight, String selectedClass, String upgradeClass, int passengers) {
        String selectedLabel = classLabel(selectedClass);
        String upgradeLabel = classLabel(upgradeClass);
        String message = "Por " + money(flightFare(flight, upgradeClass, passengers) - flightFare(flight, selectedClass, passengers))
                + " más puedes mejorar a " + upgradeLabel + " y viajar con mejores beneficios.";
        LinearLayout dialogView = new LinearLayout(this);
        dialogView.setOrientation(LinearLayout.VERTICAL);
        dialogView.setPadding(dp(20), dp(20), dp(20), dp(16));
        dialogView.setBackground(round(PANEL, LINE, dp(22)));
        dialogView.addView(text("Mejorar a " + upgradeLabel, 22, TEXT, Typeface.BOLD));
        TextView copy = text(message, 15, MUTED, Typeface.NORMAL);
        copy.setPadding(0, dp(8), 0, dp(12));
        dialogView.addView(copy);
        LinearLayout actions = row();
        android.app.AlertDialog dialog = new android.app.AlertDialog.Builder(this).create();
        actions.addView(outlineButton("Seguir con " + selectedLabel, v -> {
            dialog.dismiss();
            selectTripFare(flight, selectedClass, passengers);
        }), weightedButtonParams());
        actions.addView(primaryButton("Mejorar a " + upgradeLabel, v -> {
            dialog.dismiss();
            selectTripFare(flight, upgradeClass, passengers);
        }), weightedButtonParams());
        dialogView.addView(actions);
        dialog.setView(dialogView);
        dialog.setOnShowListener(item -> {
            if (dialog.getWindow() != null) {
                dialog.getWindow().setBackgroundDrawableResource(android.R.color.transparent);
            }
        });
        dialog.show();
    }

    private void renderTripReview() {
        LinearLayout card = card();
        card.addView(bookingProgress(1));
        card.addView(text(routeLabel(criteriaOrigin, criteriaDestination), 28, TEXT, Typeface.BOLD));
        card.addView(text(reviewDateSummary() + "  •  " + passengerCountFromCriteria() + " pasajero(s)", 14, MUTED, Typeface.NORMAL));
        card.addView(sectionTitle("Resumen del viaje"));
        card.addView(outlineButton("Editar seleccion", v -> {
            activeView = "explorar";
            render();
        }), matchWrap());

        if (selectedOutboundFlight != null) {
            card.addView(reviewFlightCard(selectedOutboundFlight, "Vuelo de ida"));
        }
        if ("roundtrip".equals(criteriaTripType) && selectedReturnFlight != null) {
            card.addView(reviewFlightCard(selectedReturnFlight, "Vuelo de vuelta"));
        }
        card.addView(primaryButton("Siguiente", v -> {
            activeView = "passenger_info";
            render();
        }), matchWrap());
        contentLayout.addView(card);
    }

    private View reviewFlightCard(JSONObject flight, String title) {
        LinearLayout item = compactCard();
        item.setBackground(round(Color.WHITE, GREEN, dp(18)));
        item.addView(text("✓", 22, GREEN, Typeface.BOLD));
        item.addView(text(title + " " + shortAirportName(value(flight, "origen", "")) + " to " + shortAirportName(value(flight, "destino", "")), 20, TEXT, Typeface.BOLD));
        item.addView(text(displayDate(value(flight, "fechaVuelo", "").length() >= 10 ? value(flight, "fechaVuelo", "").substring(0, 10) : ""), 14, MUTED, Typeface.NORMAL));
        item.addView(flightMiniTimeline(flight));
        item.addView(text("Incluye vuelo operado por " + value(flight, "aerolinea", "-"), 13, MUTED, Typeface.NORMAL));
        item.addView(text(classLabel(value(flight, "selectedClass", "economica")) + "  " + money(flightFare(flight, value(flight, "selectedClass", "economica"), parseInt(value(flight, "passengerCount", "1"), 1))), 18, TEXT, Typeface.BOLD));
        return item;
    }

    private View flightMiniTimeline(JSONObject flight) {
        LinearLayout times = new LinearLayout(this);
        times.setOrientation(LinearLayout.HORIZONTAL);
        TextView left = text(timeOnly(value(flight, "fechaVuelo", "")) + "\n" + airportCode(value(flight, "origen", "")), 22, TEXT, Typeface.BOLD);
        TextView center = text((hasTechnicalStop(flight) ? "1 Stop" : "Directo") + "\n------ ✈ ------", 14, TEAL, Typeface.BOLD);
        TextView right = text(timeOnly(estimatedArrival(value(flight, "fechaVuelo", ""), estimateDurationMinutes(flight))) + "\n" + airportCode(value(flight, "destino", "")), 22, TEXT, Typeface.BOLD);
        left.setGravity(Gravity.LEFT);
        center.setGravity(Gravity.CENTER);
        right.setGravity(Gravity.RIGHT);
        times.addView(left, weightedButtonParams());
        times.addView(center, weightedButtonParams());
        times.addView(right, weightedButtonParams());
        return times;
    }

    private void renderPassengerInfoStep() {
        if (!ensureTripSelection()) {
            return;
        }
        if (sessionUser == null) {
            addAlert("Inicia sesión para continuar con la información de pasajeros.", true);
            renderSessionCard();
            return;
        }

        int passengerCount = passengerCountFromCriteria();
        LinearLayout card = card();
        card.addView(bookingProgress(2));
        card.addView(outlineButton("Volver a vuelos", v -> {
            activeView = "travel_results";
            render();
        }), matchWrap());
        card.addView(sectionTitle("Información de pasajeros"));

        EditText[] names = new EditText[passengerCount];
        EditText[] docs = new EditText[passengerCount];
        for (int i = 0; i < passengerCount; i++) {
            LinearLayout passengerCard = compactCard();
            passengerCard.addView(text(i == 0 ? "Pasajero principal" : "Pasajero " + (i + 1), 24, TEXT, Typeface.NORMAL));
            names[i] = input("Nombre *", defaultSaved(savedPassengerNames, i, i == 0 ? value(sessionUser, "nombreCompleto", "") : ""), false);
            docs[i] = input("Documento *", defaultSaved(savedPassengerDocs, i, i == 0 ? value(sessionUser, "numeroDocumento", "") : ""), false);
            if (i == 0) {
                passengerCard.addView(text("Usaremos los datos de tu cuenta para el titular del viaje.", 14, MUTED, Typeface.NORMAL));
                passengerCard.addView(text(value(sessionUser, "nombreCompleto", value(sessionUser, "usuario", "Usuario")), 16, TEXT, Typeface.BOLD));
            } else {
                passengerCard.addView(text("Ingresa nombre y documento tal como aparecen en su identificación.", 14, TEXT, Typeface.NORMAL));
                passengerCard.addView(names[i]);
                passengerCard.addView(docs[i]);
                passengerCard.addView(spinner(new String[]{"Pasaporte", "DPI", "Licencia"}, "Pasaporte"));
                passengerCard.addView(input("Nacionalidad del documento *", "Guatemala", false));
            }
            card.addView(passengerCard);
        }

        card.addView(primaryButton("Siguiente", v -> savePassengerStep(names, docs)), matchWrap());
        card.addView(tripSummaryBar());
        contentLayout.addView(card);
    }

    private void savePassengerStep(EditText[] names, EditText[] docs) {
        for (int i = 1; i < names.length; i++) {
            if (isBlank(names[i]) || isBlank(docs[i])) {
                markInvalid(names[i]);
                markInvalid(docs[i]);
                statusText.setText("Completa la información del pasajero.");
                return;
            }
        }
        savedPassengerNames = textValues(names);
        savedPassengerDocs = textValues(docs);
        activeView = "holder_info";
        render();
    }

    private void renderHolderInfoStep() {
        if (!ensureTripSelection()) {
            return;
        }
        if (sessionUser == null) {
            addAlert("Inicia sesión para continuar con el titular de reserva.", true);
            renderSessionCard();
            return;
        }
        LinearLayout card = card();
        card.addView(bookingProgress(2));
        card.addView(sectionTitle("Titular de reserva"));
        card.addView(outlineButton("Volver a pasajeros", v -> {
            activeView = "passenger_info";
            render();
        }), matchWrap());
        EditText holderName = input("Titular de reserva", savedHolderName.isEmpty() ? value(sessionUser, "nombreCompleto", "") : savedHolderName, false);
        EditText holderEmail = input("Email *", savedHolderEmail.isEmpty() ? value(sessionUser, "email", "") : savedHolderEmail, false);
        holderEmail.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS);
        card.addView(holderName);
        card.addView(holderEmail);
        card.addView(text("Al continuar aceptas el procesamiento de tus datos personales para gestionar la reserva.", 13, MUTED, Typeface.NORMAL));
        card.addView(primaryButton("Siguiente", v -> saveHolderStep(holderName, holderEmail)), matchWrap());
        card.addView(tripSummaryBar());
        contentLayout.addView(card);
    }

    private void saveHolderStep(EditText holderName, EditText holderEmail) {
        if (isBlank(holderName) || isBlank(holderEmail)) {
            markInvalid(holderName);
            markInvalid(holderEmail);
            statusText.setText("Completa los datos del titular.");
            return;
        }
        savedHolderName = holderName.getText().toString().trim();
        savedHolderEmail = holderEmail.getText().toString().trim();
        activeView = "trip_options";
        render();
    }

    private void renderTripOptionsStep() {
        if (!ensureTripSelection()) {
            return;
        }
        LinearLayout card = card();
        card.addView(sectionTitle("Personaliza tu viaje"));
        card.addView(outlineButton("Volver al titular", v -> {
            activeView = "holder_info";
            render();
        }), matchWrap());
        boolean[] selectedServices = {selectedSeat, selectedBag, selectedVip, selectedPriority};
        LinearLayout options = card();
        options.addView(serviceOptionCard(selectedServices, 0, "Elige tu asiento", "Selecciona tu asiento favorito antes de llegar al aeropuerto.", "Desde " + money(SERVICE_SEAT)));
        options.addView(serviceOptionCard(selectedServices, 1, "Equipaje adicional", "Agrega equipaje de bodega y ahorra tiempo en mostrador.", "Desde " + money(SERVICE_BAG)));
        options.addView(serviceOptionCard(selectedServices, 2, "Asistencia de viaje", "Cobertura medica y apoyo ante imprevistos del viaje.", "Desde " + money(SERVICE_VIP)));
        options.addView(serviceOptionCard(selectedServices, 3, "Abordaje prioritario", "Entra primero al avion y evita filas largas.", "Desde " + money(SERVICE_PRIORITY)));
        card.addView(fixedScroll(options, 520));
        card.addView(primaryButton("Siguiente", v -> {
            selectedSeat = selectedServices[0];
            selectedBag = selectedServices[1];
            selectedVip = selectedServices[2];
            selectedPriority = selectedServices[3];
            activeView = "payment";
            render();
        }), matchWrap());
        card.addView(tripSummaryBar());
        contentLayout.addView(card);
    }

    private void renderPaymentStep() {
        if (!ensureTripSelection()) {
            return;
        }
        LinearLayout header = screenHeader("Método de pago", "Confirma los datos de tarjeta para completar la compra.", "trip_options", "", null);
        header.addView(bookingProgress(3));
        contentLayout.addView(header);

        LinearLayout paymentCard = compactCard();
        paymentCard.setBackground(round(PANEL, LINE, dp(22)));
        paymentCard.addView(text("Tarjeta de crédito o débito", 20, TEXT, Typeface.BOLD));
        paymentCard.addView(text("Discover  Visa  Mastercard  AMEX", 12, MUTED, Typeface.BOLD));
        paymentCard.addView(label("Información de tarjeta"));
        Spinner method = spinner(new String[]{"1 - Tarjeta de crédito", "2 - Tarjeta de débito", "3 - Transferencia"}, "1 - Tarjeta de crédito");
        EditText cardNumber = input("Número de tarjeta", "", false);
        cardNumber.setInputType(InputType.TYPE_CLASS_NUMBER);
        NumberPicker cardMonth = picker(1, 12, Calendar.getInstance().get(Calendar.MONTH) + 1, true);
        NumberPicker cardYear = picker(Calendar.getInstance().get(Calendar.YEAR), Calendar.getInstance().get(Calendar.YEAR) + 15, Calendar.getInstance().get(Calendar.YEAR), false);
        paymentCard.addView(method);
        LinearLayout cardDetails = new LinearLayout(this);
        cardDetails.setOrientation(LinearLayout.VERTICAL);
        paymentCard.addView(cardDetails);
        cardDetails.addView(cardNumber);
        LinearLayout cardRow = row();
        cardRow.addView(pickerBox("Mes", cardMonth), weightedButtonParams());
        cardRow.addView(pickerBox("Año", cardYear), weightedButtonParams());
        cardDetails.addView(cardRow);
        cardDetails.addView(text("Solo escribe el número de tarjeta. Mes y año se seleccionan con scroll.", 13, MUTED, Typeface.NORMAL));
        LinearLayout transferInfo = transferInfoPanel();
        paymentCard.addView(transferInfo);
        bindPaymentMethodToggle(method, cardDetails, transferInfo);
        contentLayout.addView(paymentCard);
        contentLayout.addView(primaryButton("Confirmar compra", v -> confirmFlowPurchase(method, cardNumber, cardMonth, cardYear)), matchWrap());
        contentLayout.addView(tripSummaryBar());
    }

    private void confirmFlowPurchase(Spinner method, EditText cardNumber, NumberPicker cardMonth, NumberPicker cardYear) {
        if (sessionUser == null) {
            addAlert("Inicia sesión para confirmar la compra.", true);
            renderSessionCard();
            return;
        }
        boolean requiresCard = !method.getSelectedItem().toString().startsWith("3");
        if (requiresCard && (cardNumber.getText().toString().replaceAll("\\D", "").length() < 13 || isExpiredCard(cardMonth.getValue(), cardYear.getValue()))) {
            markInvalid(cardNumber);
            statusText.setText("Completa correctamente los datos de tarjeta.");
            return;
        }
        double confirmedTotal = tripTotal();
        runTask("Confirmando compra...", () -> {
            JSONArray responses = new JSONArray();
            boolean multipleFlights = "roundtrip".equals(criteriaTripType) && selectedReturnFlight != null;
            if (selectedOutboundFlight != null) {
                responses.put(new JSONObject(apiClient.post("/api/compras/vuelos", purchasePayload(selectedOutboundFlight, method, !multipleFlights).toString())));
            }
            if (multipleFlights) {
                responses.put(new JSONObject(apiClient.post("/api/compras/vuelos", purchasePayload(selectedReturnFlight, method, false).toString())));
                apiClient.post("/api/compras/confirmacion-correo", purchaseSummaryPayload(responses, confirmedTotal).toString());
            }
            return new JSONObject().put("compras", responses).put("total", confirmedTotal).toString();
        }, json -> {
            JSONObject result = new JSONObject(json);
            JSONArray responses = result.optJSONArray("compras");
            JSONArray purchasedFlights = new JSONArray();
            if (selectedOutboundFlight != null) {
                purchasedFlights.put(successFlightSummary(selectedOutboundFlight, responses == null ? null : responses.optJSONObject(0), "Vuelo de ida"));
            }
            if ("roundtrip".equals(criteriaTripType) && selectedReturnFlight != null) {
                purchasedFlights.put(successFlightSummary(selectedReturnFlight, responses == null ? null : responses.optJSONObject(1), "Vuelo de vuelta"));
            }
            lastPurchaseSuccess = new JSONObject()
                    .put("reservas", reservationCodes(responses))
                    .put("total", confirmedTotal)
                    .put("pasajeros", passengerCountFromCriteria())
                    .put("vuelos", purchasedFlights);
            selectedOutboundFlight = null;
            selectedReturnFlight = null;
            activeView = "purchase_success";
            loadDashboard();
        });
    }

    private JSONObject purchasePayload(JSONObject flight, Spinner method, boolean sendConfirmationEmail) throws Exception {
        int passengers = parseInt(value(flight, "passengerCount", "1"), 1);
        double servicesTotal = selectedSeat ? SERVICE_SEAT * passengers : 0;
        servicesTotal += selectedBag ? SERVICE_BAG * passengers : 0;
        servicesTotal += selectedVip ? SERVICE_VIP * passengers : 0;
        servicesTotal += selectedPriority ? SERVICE_PRIORITY * passengers : 0;
        return new JSONObject()
                .put("usuarioId", parseInt(value(sessionUser, "usuarioId", "0"), 0))
                .put("pasajeroId", parseInt(value(sessionUser, "pasajeroId", "0"), 0))
                .put("vueloId", parseInt(value(flight, "id", "0"), 0))
                .put("clase", value(flight, "selectedClass", "economica"))
                .put("numeroPasajeros", passengers)
                .put("equipajeFacturado", selectedBag ? 1 : 0)
                .put("pesoEquipaje", selectedBag ? 23 : JSONObject.NULL)
                .put("tarifaPagada", flightFare(flight, value(flight, "selectedClass", "economica"), passengers) + servicesTotal)
                .put("metodoPagoId", parseInt(method.getSelectedItem().toString().substring(0, 1), 1))
                .put("emailConfirmacion", sendConfirmationEmail ? savedHolderEmail : JSONObject.NULL)
                .put("enviarCorreoConfirmacion", sendConfirmationEmail);
    }

    private JSONObject purchaseSummaryPayload(JSONArray responses, double total) throws Exception {
        JSONArray reservations = new JSONArray();
        JSONObject[] flights = {selectedOutboundFlight, selectedReturnFlight};
        for (int i = 0; i < responses.length() && i < flights.length; i++) {
            JSONObject response = responses.optJSONObject(i);
            JSONObject flight = flights[i];
            if (response == null || flight == null) continue;
            reservations.put(new JSONObject()
                    .put("codigoReserva", value(response, "codigoReserva", "-"))
                    .put("numeroVenta", value(response, "numeroVenta", "-"))
                    .put("numeroVuelo", value(flight, "numeroVuelo", "-"))
                    .put("aerolinea", value(flight, "aerolinea", "-"))
                    .put("origen", value(flight, "origen", "-"))
                    .put("destino", value(flight, "destino", "-"))
                    .put("fechaVuelo", value(flight, "fechaVuelo", ""))
                    .put("clase", value(flight, "selectedClass", "economica"))
                    .put("total", response.optDouble("total", 0)));
        }
        return new JSONObject()
                .put("emailConfirmacion", savedHolderEmail)
                .put("pasajeroNombre", savedHolderName)
                .put("total", total)
                .put("reservas", reservations);
    }

    private JSONObject successFlightSummary(JSONObject flight, JSONObject response, String label) throws Exception {
        int passengers = parseInt(value(flight, "passengerCount", "1"), 1);
        int remainingSeats = response == null
                ? Math.max(0, parseInt(value(flight, "plazasDisponibles", "0"), 0) - passengers)
                : parseInt(value(response, "plazasDisponibles", "0"), 0);
        return new JSONObject()
                .put("etiqueta", label)
                .put("numeroVuelo", value(flight, "numeroVuelo", "-"))
                .put("ruta", value(flight, "origen", "-") + " - " + value(flight, "destino", "-"))
                .put("plazasDisponibles", remainingSeats);
    }

    private String reservationCodes(JSONArray responses) {
        if (responses == null || responses.length() == 0) {
            return "-";
        }
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < responses.length(); i++) {
            JSONObject response = responses.optJSONObject(i);
            if (response == null) {
                continue;
            }
            if (builder.length() > 0) {
                builder.append(" / ");
            }
            builder.append(value(response, "codigoReserva", "-"));
        }
        return builder.length() == 0 ? "-" : builder.toString();
    }

    private void renderMyTrips() {
        contentLayout.addView(screenHeader("Mis viajes", "Reservas, check-in y tarjetas de embarque.", "menu_more", "Actualizar", v -> loadMyTrips()));

        if (sessionUser == null) {
            LinearLayout card = card();
            card.addView(text("Inicia sesión para ver tus viajes.", 15, MUTED, Typeface.NORMAL));
            card.addView(primaryButton("Iniciar sesión", v -> navigateTo("login")), matchWrap());
            contentLayout.addView(card);
            return;
        }

        LinearLayout section = card();
        section.addView(sectionTitle("Reservas"));
        if (myReservations.length() == 0) {
            section.addView(text("Toca Actualizar para cargar tus reservas.", 13, MUTED, Typeface.NORMAL));
        }

        forEach(myReservations, 20, reservation -> {
            int reservationId = parseInt(value(reservation, "id", "0"), 0);
            JSONObject checkIn = findCheckIn(myCheckIns, reservationId);
            JSONObject boardingPass = checkIn == null
                    ? null
                    : findBoardingPass(myBoardingPasses, parseInt(value(checkIn, "id", "0"), 0));

            LinearLayout itemCard = compactCard();
            itemCard.addView(text(value(reservation, "numeroVuelo", "Vuelo"), 18, TEXT, Typeface.BOLD));
            itemCard.addView(text("Reserva: " + value(reservation, "codigo", "-"), 13, MUTED, Typeface.NORMAL));
            itemCard.addView(text("Fecha reserva: " + shortDate(value(reservation, "fechaReserva", "-")), 13, MUTED, Typeface.NORMAL));
            itemCard.addView(text("Clase: " + value(reservation, "clase", "-") + "  Estado: " + value(reservation, "estado", "-"), 13, MUTED, Typeface.NORMAL));
            itemCard.addView(text("Check-in: " + (checkIn == null ? "Pendiente" : value(checkIn, "estado", "Completado")), 13, checkIn == null ? MUTED : GREEN, Typeface.BOLD));

            if (boardingPass != null) {
                itemCard.addView(text("Tarjeta: " + value(boardingPass, "codigoQr", "-"), 13, TEXT, Typeface.BOLD));
                itemCard.addView(text("Grupo " + value(boardingPass, "grupoAbordaje", "-") + "  Zona " + value(boardingPass, "zona", "-"), 13, MUTED, Typeface.NORMAL));
            }
            section.addView(itemCard);
        });
        contentLayout.addView(section);
    }

    private void loadMyTrips() {
        if (sessionUser == null) {
            navigateTo("login");
            return;
        }
        runTask("Cargando tus viajes...", () -> {
            JSONObject result = new JSONObject();
            result.put("reservations", new JSONArray(apiClient.get("/api/reservas?pasajeroId=" + passengerId() + "&limit=200")));
            result.put("checkIns", new JSONArray(apiClient.get("/api/checkin?limit=500")));
            result.put("boardingPasses", new JSONArray(apiClient.get("/api/tarjetas-embarque?limit=500")));
            return result.toString();
        }, json -> {
            JSONObject result = new JSONObject(json);
            myReservations = result.optJSONArray("reservations") == null ? new JSONArray() : result.optJSONArray("reservations");
            myCheckIns = result.optJSONArray("checkIns") == null ? new JSONArray() : result.optJSONArray("checkIns");
            myBoardingPasses = result.optJSONArray("boardingPasses") == null ? new JSONArray() : result.optJSONArray("boardingPasses");
            lastMessage = "Mis viajes actualizados.";
            render();
        });
    }

    private void renderCheckIn() {
        contentLayout.addView(screenHeader("Check-in", "Escribe tu vuelo o código de reserva.", "menu_more", "", null));

        if (sessionUser == null) {
            LinearLayout card = card();
            card.addView(text("Inicia sesión para ver tus reservas y completar el check-in.", 15, MUTED, Typeface.NORMAL));
            card.addView(primaryButton("Iniciar sesión", v -> navigateTo("login")), matchWrap());
            contentLayout.addView(card);
            return;
        }

        LinearLayout card = card();
        card.addView(sectionTitle("Buscar reserva"));
        EditText queryInput = input("Ej. AV2200X o R051...", "", false);
        card.addView(label("Vuelo o reserva"));
        card.addView(queryInput);
        card.addView(text("La tarjeta de embarque se genera al completar el check-in.", 13, MUTED, Typeface.NORMAL));
        card.addView(primaryButton("Completar check-in", v -> completeCheckIn(queryInput.getText().toString())), matchWrap());
        contentLayout.addView(card);
    }

    private void completeCheckIn(String query) {
        String cleanQuery = query == null ? "" : query.trim();
        if (cleanQuery.isEmpty()) {
            lastMessage = "Ingresa un número de vuelo o código de reserva.";
            render();
            return;
        }
        if (sessionUser == null) {
            navigateTo("login");
            return;
        }

        runTask("Completando check-in...", () -> {
            JSONArray reservations = new JSONArray(apiClient.get("/api/reservas?pasajeroId=" + passengerId() + "&limit=200"));
            JSONObject reservation = findReservationForCheckIn(reservations, cleanQuery);
            if (reservation == null) {
                throw new Exception("No encontramos una reserva de tu usuario para " + cleanQuery + ".");
            }

            JSONArray checkIns = new JSONArray(apiClient.get("/api/checkin?limit=500"));
            JSONObject existing = findCheckIn(checkIns, parseInt(value(reservation, "id", "0"), 0));
            if (existing != null) {
                return new JSONObject()
                        .put("message", "Ese vuelo ya tiene check-in completado.")
                        .put("reservation", value(reservation, "codigo", "-"))
                        .put("flight", value(reservation, "numeroVuelo", "-"))
                        .toString();
            }

            JSONObject body = new JSONObject()
                    .put("reservaId", parseInt(value(reservation, "id", "0"), 0))
                    .put("pasajeroId", passengerId())
                    .put("vueloId", parseInt(value(reservation, "vueloId", "0"), 0))
                    .put("fechaHora", nowIso())
                    .put("metodo", "web")
                    .put("estado", "COMPLETADO");
            JSONObject checkIn = new JSONObject(apiClient.post("/api/checkin", body.toString()));

            JSONObject pass = new JSONObject()
                    .put("checkInId", parseInt(value(checkIn, "id", "0"), 0))
                    .put("codigoQr", boardingCode(value(reservation, "id", "0"), String.valueOf(passengerId())))
                    .put("grupoAbordaje", "A")
                    .put("zona", "1")
                    .put("fechaEmision", nowIso());
            apiClient.post("/api/tarjetas-embarque", pass.toString());

            return new JSONObject()
                    .put("message", "Check-in completado y tarjeta emitida.")
                    .put("reservation", value(reservation, "codigo", "-"))
                    .put("flight", value(reservation, "numeroVuelo", "-"))
                    .toString();
        }, json -> {
            JSONObject result = new JSONObject(json);
            lastMessage = value(result, "message", "Check-in completado.") + " " + value(result, "flight", "") + " " + value(result, "reservation", "");
            navigateTo("menu_more");
        });
    }

    private JSONObject findReservationForCheckIn(JSONArray reservations, String query) {
        String normalizedQuery = normalize(query);
        JSONObject containsMatch = null;
        for (int i = 0; i < reservations.length(); i++) {
            JSONObject reservation = reservations.optJSONObject(i);
            if (reservation == null) {
                continue;
            }
            String code = normalize(value(reservation, "codigo", ""));
            String flight = normalize(value(reservation, "numeroVuelo", ""));
            if (code.equals(normalizedQuery) || flight.equals(normalizedQuery)) {
                return reservation;
            }
            if (containsMatch == null && (code.contains(normalizedQuery) || flight.contains(normalizedQuery))) {
                containsMatch = reservation;
            }
        }
        return containsMatch;
    }

    private JSONObject findCheckIn(JSONArray checkIns, int reservationId) {
        for (int i = 0; i < checkIns.length(); i++) {
            JSONObject checkIn = checkIns.optJSONObject(i);
            if (checkIn != null && parseInt(value(checkIn, "reservaId", "0"), 0) == reservationId) {
                return checkIn;
            }
        }
        return null;
    }

    private JSONObject findBoardingPass(JSONArray boardingPasses, int checkInId) {
        for (int i = 0; i < boardingPasses.length(); i++) {
            JSONObject boardingPass = boardingPasses.optJSONObject(i);
            if (boardingPass != null && parseInt(value(boardingPass, "checkInId", "0"), 0) == checkInId) {
                return boardingPass;
            }
        }
        return null;
    }

    private void renderLostObjects() {
        contentLayout.addView(screenHeader("Objetos perdidos", "Reporta una perdida o revisa encontrados recientes.", "menu_more", "Actualizar", v -> loadLostObjects()));

        LinearLayout form = card();
        form.addView(sectionTitle("Reportar perdida"));
        EditText descriptionInput = input("Ej. mochila negra con etiqueta roja", "", false);
        EditText nameInput = input("Nombre y apellido", "", false);
        EditText contactInput = input("Teléfono o email", "", false);
        form.addView(label("Que perdiste?"));
        form.addView(descriptionInput);
        form.addView(label("Tu nombre"));
        form.addView(nameInput);
        form.addView(label("Contacto privado"));
        form.addView(contactInput);
        form.addView(text("El nombre y contacto quedan para seguimiento interno; no se publican.", 13, MUTED, Typeface.NORMAL));
        form.addView(primaryButton("Enviar reporte", v -> reportLostObject(
                descriptionInput.getText().toString(),
                nameInput.getText().toString(),
                contactInput.getText().toString())), matchWrap());
        contentLayout.addView(form);

        contentLayout.addView(listSection("Encontrados recientes", lostObjects, 10, item -> new String[]{
                value(item, "descripcion", "Objeto"),
                (value(item, "estado", "-").equalsIgnoreCase("REPORTADO") ? "Reporte en revisión" : value(item, "ubicacionEncontrado", "Ubicación pendiente")),
                "Fecha: " + shortDate(value(item, "fechaReporte", "-")),
                "Estado: " + value(item, "estado", "-")
        }));
    }

    private void loadLostObjects() {
        runTask("Cargando objetos perdidos...", () -> apiClient.get("/api/objetos-perdidos?limit=30"), json -> {
            lostObjects = new JSONArray(json);
            lastMessage = "Objetos perdidos actualizados.";
            render();
        });
    }

    private void reportLostObject(String description, String name, String contact) {
        String cleanDescription = description == null ? "" : description.trim();
        String cleanName = name == null ? "" : name.trim();
        String cleanContact = contact == null ? "" : contact.trim();

        if (cleanDescription.isEmpty() || cleanName.isEmpty() || cleanContact.isEmpty()) {
            lastMessage = "Completa descripcion, nombre y contacto.";
            render();
            return;
        }

        runTask("Enviando reporte...", () -> {
            String[] parts = cleanName.split("\\s+");
            String firstName = parts.length == 0 ? JSONObject.NULL.toString() : parts[0];
            StringBuilder lastName = new StringBuilder();
            for (int i = 1; i < parts.length; i++) {
                if (lastName.length() > 0) {
                    lastName.append(' ');
                }
                lastName.append(parts[i]);
            }

            JSONObject body = new JSONObject()
                    .put("vueloId", JSONObject.NULL)
                    .put("aeropuertoId", defaultAirportId())
                    .put("descripcion", cleanDescription)
                    .put("fechaReporte", nowIso())
                    .put("ubicacionEncontrado", "Pendiente de revisión")
                    .put("estado", "REPORTADO")
                    .put("reportantePrimerNombre", firstName)
                    .put("reportanteSegundoNombre", JSONObject.NULL)
                    .put("reportantePrimerApellido", lastName.length() == 0 ? JSONObject.NULL : lastName.toString())
                    .put("reportanteSegundoApellido", JSONObject.NULL)
                    .put("contactoReportante", cleanContact)
                    .put("fechaEntrega", JSONObject.NULL)
                    .put("reclamantePrimerNombre", JSONObject.NULL)
                    .put("reclamanteSegundoNombre", JSONObject.NULL)
                    .put("reclamantePrimerApellido", JSONObject.NULL)
                    .put("reclamanteSegundoApellido", JSONObject.NULL);
            apiClient.post("/api/objetos-perdidos", body.toString());
            return apiClient.get("/api/objetos-perdidos?limit=30");
        }, json -> {
            lostObjects = new JSONArray(json);
            lastMessage = "Reporte recibido. Atencion lo revisara y usara el contacto solo para seguimiento.";
            render();
        });
    }

    private int defaultAirportId() {
        JSONObject first = airports.optJSONObject(0);
        return first == null ? 1 : parseInt(value(first, "id", "1"), 1);
    }

    private String boardingCode(String reservationId, String passengerId) {
        return "GUA-" + reservationId + "-" + passengerId;
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
                "Ubicación: " + value(item, "ubicacion", "-"),
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
        contentLayout.addView(listSection("Destinos más buscados", destinations, 20, item -> new String[]{
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
            card.addView(text("Tu carrito está vacío.", 18, TEXT, Typeface.BOLD));
            card.addView(text(sessionUser == null
                    ? "Inicia sesión para sincronizar compras o explora vuelos para agregar una reserva."
                    : "Explora vuelos y agrega una reserva para continuar la compra.", 14, MUTED, Typeface.NORMAL));
            LinearLayout emptyActions = row();
            if (sessionUser == null) {
                emptyActions.addView(primaryButton("Iniciar sesión", v -> navigateTo("login")), weightedButtonParams());
            }
            emptyActions.addView(outlineButton("Explorar vuelos", v -> navigateTo("explorar")), weightedButtonParams());
            card.addView(emptyActions);
        }
        for (JSONObject item : cartItems) {
            LinearLayout rowCard = compactCard();
            rowCard.addView(text(value(item, "numeroVuelo", "Vuelo") + " - " + classLabel(value(item, "selectedClass", "economica")), 16, TEXT, Typeface.BOLD));
            rowCard.addView(text(value(item, "origen", "-") + " -> " + value(item, "destino", "-"), 13, MUTED, Typeface.NORMAL));
            rowCard.addView(text("Pasajeros: " + value(item, "passengerCount", "1") + "  Tarifa: " + money(flightFare(item, value(item, "selectedClass", "economica"), parseInt(value(item, "passengerCount", "1"), 1))), 13, TEXT, Typeface.NORMAL));
            LinearLayout actions = row();
            actions.addView(primaryButton("Comprar", v -> startCheckout(item)), weightedButtonParams());
            actions.addView(outlineButton("Quitar", v -> removeCartItem(value(item, "cartId", ""))), weightedButtonParams());
            rowCard.addView(actions);
            card.addView(rowCard);
        }
        contentLayout.addView(card);
    }

    private void renderCheckout() {
        if (checkoutFlight == null) {
            activeView = "carrito";
            render();
            return;
        }
        if (sessionUser == null) {
            addAlert("Inicia sesión para confirmar la compra.", true);
            renderSessionCard();
            return;
        }

        LinearLayout card = card();
        card.addView(bookingProgress(2));
        card.addView(outlineButton("Volver al carrito", v -> {
            activeView = "carrito";
            render();
        }), matchWrap());
        card.addView(sectionTitle("Información de pasajeros"));
        card.addView(text(value(checkoutFlight, "numeroVuelo", "Vuelo"), 20, TEXT, Typeface.BOLD));
        card.addView(text(value(checkoutFlight, "origen", "-") + " -> " + value(checkoutFlight, "destino", "-"), 14, MUTED, Typeface.NORMAL));
        card.addView(text("Pasos: Pasajeros -> Extras -> Pago. Todo se confirma al final como en el web.", 13, MUTED, Typeface.NORMAL));

        Spinner classSpinner = spinner(new String[]{"economica", "turista", "primera"}, value(checkoutFlight, "selectedClass", "economica"));
        int passengerCount = parseInt(value(checkoutFlight, "passengerCount", "1"), 1);

        card.addView(label("Vuelo seleccionado"));
        LinearLayout summary = compactCard();
        summary.addView(text("Salida: " + shortDate(value(checkoutFlight, "fechaVuelo", "-")), 13, TEXT, Typeface.NORMAL));
        summary.addView(text("Llegada estimada: " + shortDate(estimatedArrival(value(checkoutFlight, "fechaVuelo", ""), estimateDurationMinutes(checkoutFlight))), 13, MUTED, Typeface.NORMAL));
        summary.addView(text("Viajan: " + passengerCount + " pasajero(s) - " + (hasTechnicalStop(checkoutFlight) ? "Con parada" : "Directo"), 13, MUTED, Typeface.NORMAL));
        card.addView(summary);

        card.addView(label("Tarifa"));
        card.addView(classSpinner);

        card.addView(label("Pasajeros"));
        EditText[] passengerNames = new EditText[passengerCount];
        EditText[] passengerDocs = new EditText[passengerCount];
        for (int i = 0; i < passengerCount; i++) {
            LinearLayout passengerCard = compactCard();
            passengerCard.addView(text(i == 0 ? "Pasajero principal" : "Pasajero " + (i + 1), 20, TEXT, Typeface.NORMAL));
            passengerNames[i] = input("Nombre *", i == 0 ? value(sessionUser, "nombreCompleto", "") : "", false);
            passengerDocs[i] = input("Documento *", i == 0 ? value(sessionUser, "numeroDocumento", "") : "", false);
            if (i == 0) {
                passengerCard.addView(text("Usaremos los datos de tu cuenta para el titular del viaje.", 13, MUTED, Typeface.NORMAL));
                passengerCard.addView(text(value(sessionUser, "nombreCompleto", value(sessionUser, "usuario", "Usuario")), 16, TEXT, Typeface.BOLD));
            } else {
                passengerCard.addView(text("Ingresa el nombre y apellido exactamente como aparece en el pasaporte o documento.", 13, MUTED, Typeface.NORMAL));
                passengerCard.addView(passengerNames[i]);
                passengerCard.addView(passengerDocs[i]);
                passengerCard.addView(input("Nacionalidad del documento *", "Guatemala", false));
            }
            card.addView(passengerCard);
        }

        card.addView(label("Titular de reserva"));
        EditText holderName = input("Nombre completo", value(sessionUser, "nombreCompleto", ""), false);
        EditText holderEmail = input("Email", value(sessionUser, "email", ""), false);
        holderEmail.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS);
        card.addView(holderName);
        card.addView(holderEmail);
        card.addView(text("Al continuar aceptas el procesamiento de tus datos personales para gestionar la compra.", 13, MUTED, Typeface.NORMAL));

        card.addView(sectionTitle("Personaliza tu viaje"));
        boolean[] selectedServices = {false, !"economica".equals(value(checkoutFlight, "selectedClass", "economica")), false, false};
        if ("economica".equals(value(checkoutFlight, "selectedClass", "economica"))) {
            TextView warning = text("Tu tarifa Economica no incluye equipaje de bodega. Puedes agregarlo aqui o continuar sin cambios.", 13, GOLD, Typeface.BOLD);
            warning.setPadding(dp(10), dp(10), dp(10), dp(10));
            warning.setBackground(round(Color.rgb(45, 38, 18), GOLD, dp(8)));
            card.addView(warning, matchWrap());
        }
        card.addView(serviceOptionCard(selectedServices, 0, "Elige tu asiento", "Selecciona tu asiento favorito antes de llegar al aeropuerto.", "Desde " + money(SERVICE_SEAT)));
        card.addView(serviceOptionCard(selectedServices, 1, "Equipaje adicional", "Agrega equipaje de bodega y ahorra tiempo en mostrador.", "Desde " + money(SERVICE_BAG)));
        card.addView(serviceOptionCard(selectedServices, 2, "Asistencia de viaje", "Cobertura medica y apoyo ante imprevistos del viaje.", "Desde " + money(SERVICE_VIP)));
        card.addView(serviceOptionCard(selectedServices, 3, "Abordaje prioritario", "Entra primero al avion y evita filas largas.", "Desde " + money(SERVICE_PRIORITY)));

        card.addView(sectionTitle("Selecciona método de pago"));
        LinearLayout paymentCard = compactCard();
        paymentCard.addView(text("Tarjeta de crédito o débito", 20, TEXT, Typeface.NORMAL));
        paymentCard.addView(text("DISCOVER   VISA   Mastercard   AMEX", 13, MUTED, Typeface.BOLD));
        paymentCard.addView(text("Información de tarjeta", 18, TEXT, Typeface.BOLD));
        Spinner method = spinner(new String[]{"1 - Tarjeta de crédito", "2 - Tarjeta de débito", "3 - Transferencia"}, "1 - Tarjeta de crédito");
        paymentCard.addView(method);
        EditText cardNumber = input("Número de tarjeta", "", false);
        cardNumber.setInputType(InputType.TYPE_CLASS_NUMBER);
        NumberPicker cardMonth = picker(1, 12, Calendar.getInstance().get(Calendar.MONTH) + 1, true);
        NumberPicker cardYear = picker(Calendar.getInstance().get(Calendar.YEAR), Calendar.getInstance().get(Calendar.YEAR) + 15, Calendar.getInstance().get(Calendar.YEAR), false);
        LinearLayout cardDetails = new LinearLayout(this);
        cardDetails.setOrientation(LinearLayout.VERTICAL);
        paymentCard.addView(cardDetails);
        cardDetails.addView(cardNumber);
        LinearLayout cardRow = row();
        cardRow.addView(pickerBox("Mes", cardMonth), weightedButtonParams());
        cardRow.addView(pickerBox("Año", cardYear), weightedButtonParams());
        cardDetails.addView(cardRow);
        cardDetails.addView(text("Solo escribe la numeración. Mes y año se seleccionan con scroll.", 13, MUTED, Typeface.NORMAL));
        LinearLayout transferInfo = transferInfoPanel();
        paymentCard.addView(transferInfo);
        bindPaymentMethodToggle(method, cardDetails, transferInfo);
        card.addView(paymentCard);

        LinearLayout purchaseSummary = compactCard();
        purchaseSummary.addView(text("Resumen de compra", 16, TEXT, Typeface.BOLD));
        purchaseSummary.addView(text("Vuelo: " + money(flightFare(checkoutFlight, value(checkoutFlight, "selectedClass", "economica"), passengerCount)), 13, MUTED, Typeface.NORMAL));
        purchaseSummary.addView(text("Extras se calculan al confirmar según lo seleccionado.", 13, MUTED, Typeface.NORMAL));
        purchaseSummary.addView(text("Impuestos: 12%", 13, MUTED, Typeface.NORMAL));
        card.addView(purchaseSummary);
        card.addView(primaryButton("Confirmar compra", v -> confirmPurchase(
                classSpinner,
                passengerNames,
                passengerDocs,
                holderName,
                holderEmail,
                selectedServices[0],
                selectedServices[1],
                selectedServices[2],
                selectedServices[3],
                method,
                cardNumber,
                cardMonth,
                cardYear)), matchWrap());
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
            try { nextFlights = new JSONArray(apiClient.get("/api/vuelos?limit=5000")); } catch (Exception exception) { error = firstError(error, exception); }
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
            runTask("Iniciando sesión...", () -> apiClient.post("/api/auth/login", body.toString()), json -> {
                sessionUser = new JSONObject(json);
                settingsStore.saveSessionJson(sessionUser.toString());
                statusText.setText("Sesión iniciada.");
                activeView = "inicio";
                lastMessage = "Bienvenido " + value(sessionUser, "nombreCompleto", value(sessionUser, "usuario", "usuario")) + ".";
                syncCartFromServer(true);
            });
        } catch (Exception exception) {
            showError(exception);
        }
    }

    private void applyDangerousCharacterFilter(EditText... inputs) {
        InputFilter filter = (source, start, end, dest, dstart, dend) -> {
            StringBuilder clean = new StringBuilder();
            for (int i = start; i < end; i++) {
                char current = source.charAt(i);
                if (!isDangerousCharacter(current)) {
                    clean.append(current);
                }
            }
            return clean.length() == end - start ? null : clean.toString();
        };

        for (EditText input : inputs) {
            input.setFilters(new InputFilter[]{filter});
        }
    }

    private String registerValidationError(EditText usuario, EditText email, EditText contrasena, EditText documento, EditText tipoDocumento, EditText primerNombre, EditText segundoNombre, EditText primerApellido, EditText segundoApellido, EditText fechaNacimiento, EditText nacionalidad, Spinner sexo, EditText telefono) {
        EditText[] required = {usuario, email, contrasena, documento, tipoDocumento, primerNombre, segundoNombre, primerApellido, segundoApellido, fechaNacimiento, nacionalidad, telefono};
        for (EditText input : required) {
            if (isBlank(input)) {
                markInvalid(input);
                return "Completa todos los campos para crear el usuario.";
            }
            if (containsDangerousCharacter(input.getText().toString())) {
                markInvalid(input);
                return "No uses apóstrofes, comillas, slashes ni símbolos peligrosos.";
            }
        }

        String password = contrasena.getText().toString();
        if (!isStrongPassword(password)) {
            markInvalid(contrasena);
            return "La contraseña debe tener mínimo 8 caracteres, una mayúscula, un número y un símbolo seguro.";
        }
        if (!email.getText().toString().trim().matches("[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}")) {
            markInvalid(email);
            return "Usa un email válido.";
        }
        if (!documento.getText().toString().trim().matches("[A-Za-z0-9._ -]+")) {
            markInvalid(documento);
            return "El documento solo puede usar letras, números, espacios, punto, guion y guion bajo.";
        }
        if (!fechaNacimiento.getText().toString().trim().matches("\\d{4}-\\d{2}-\\d{2}")) {
            markInvalid(fechaNacimiento);
            return "Selecciona tu fecha de nacimiento.";
        }
        if (sexo.getSelectedItemPosition() <= 0) {
            return "Selecciona sexo masculino o femenino.";
        }
        if (!telefono.getText().toString().trim().matches("[0-9+ ()-]+")) {
            markInvalid(telefono);
            return "El teléfono solo puede usar números, espacios, +, guion y paréntesis.";
        }

        EditText[] plainText = {usuario, tipoDocumento, primerNombre, segundoNombre, primerApellido, segundoApellido, nacionalidad};
        for (EditText input : plainText) {
            if (!input.getText().toString().trim().matches("[\\p{L}\\p{N} ._-]+")) {
                markInvalid(input);
                return "Usa solo letras, números, espacios, punto, guion y guion bajo.";
            }
        }

        return "";
    }

    private boolean isStrongPassword(String value) {
        return value != null
                && value.length() >= 8
                && value.matches(".*[A-Z].*")
                && value.matches(".*\\d.*")
                && value.matches(".*[!@#$%^&*._?-].*")
                && !containsDangerousCharacter(value);
    }

    private String selectedSexValue(Spinner sexo) {
        Object selected = sexo.getSelectedItem();
        String value = selected == null ? "" : selected.toString();
        return value.toLowerCase(Locale.ROOT).startsWith("m") ? "M" : "F";
    }

    private boolean containsDangerousCharacter(String value) {
        if (value == null) {
            return false;
        }
        for (int i = 0; i < value.length(); i++) {
            if (isDangerousCharacter(value.charAt(i))) {
                return true;
            }
        }
        return false;
    }

    private boolean isDangerousCharacter(char value) {
        return value == '\'' || value == '"' || value == '/' || value == '\\' || value == ';'
                || value == '<' || value == '>' || value == '`' || value == '{' || value == '}'
                || value == '[' || value == ']' || value == '|';
    }

    private void register(EditText usuario, EditText email, EditText contrasena, EditText documento, EditText tipoDocumento, EditText primerNombre, EditText segundoNombre, EditText primerApellido, EditText segundoApellido, EditText fechaNacimiento, EditText nacionalidad, Spinner sexo, EditText telefono) {
        saveConnectionIfReady();
        String validationError = registerValidationError(usuario, email, contrasena, documento, tipoDocumento, primerNombre, segundoNombre, primerApellido, segundoApellido, fechaNacimiento, nacionalidad, sexo, telefono);
        if (!validationError.isEmpty()) {
            statusText.setText(validationError);
            return;
        }

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
                    .put("sexo", selectedSexValue(sexo))
                    .put("telefono", telefono.getText().toString().trim());
            runTask("Creando cuenta...", () -> apiClient.post("/api/auth/register", body.toString()), json -> {
                sessionUser = new JSONObject(json);
                settingsStore.saveSessionJson(sessionUser.toString());
                statusText.setText("Cuenta creada.");
                activeView = "inicio";
                lastMessage = "Cuenta creada e iniciada.";
                syncCartFromServer(true);
            });
        } catch (Exception exception) {
            showError(exception);
        }
    }

    private void logout() {
        sessionUser = null;
        settingsStore.clearSession();
        activeView = "inicio";
        statusText.setText("Sesión cerrada.");
        render();
    }

    private void showAccountMenu(View anchor) {
        PopupMenu menu = new PopupMenu(this, anchor);
        menu.getMenu().add("Cerrar sesión");
        menu.setOnMenuItemClickListener(item -> {
            logout();
            return true;
        });
        menu.show();
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
        activeView = "travel_results";
        criteriaDestination = name;
        criteriaOrigin = criteriaOrigin.isEmpty() ? "La Aurora" : criteriaOrigin;
        selectedOutboundFlight = null;
        selectedReturnFlight = null;
        render();
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
                    .put("tripLeg", fareSelectionLeg)
                    .put("passengerCount", Math.max(1, passengers));

            if (sessionUser != null) {
                JSONObject payload = cartItemPayload(item);
                runTask("Agregando al carrito compartido...", () -> apiClient.post("/api/carritos-compra/pasajero/" + passengerId() + "/items", payload.toString()), json -> {
                    cartItems.add(serverCartItemToFlight(new JSONObject(json)));
                    saveCart();
                    activeView = "carrito";
                    lastMessage = "Vuelo agregado al carrito compartido.";
                    render();
                });
                return;
            }

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
        JSONObject removed = null;
        for (JSONObject item : cartItems) {
            if (value(item, "cartId", "").equals(cartId)) {
                removed = item;
                break;
            }
        }

        if (sessionUser != null && removed != null && !value(removed, "itemCarritoId", "").isEmpty()) {
            JSONObject finalRemoved = removed;
            runTask("Quitando del carrito compartido...", () -> {
                apiClient.delete("/api/carritos-compra/pasajero/" + passengerId() + "/items/" + encode(value(finalRemoved, "itemCarritoId", "")));
                return "";
            }, json -> {
                removeLocalCartItem(cartId);
                render();
            });
            return;
        }

        removeLocalCartItem(cartId);
        render();
    }

    private void removeLocalCartItem(String cartId) {
        for (int i = cartItems.size() - 1; i >= 0; i--) {
            if (value(cartItems.get(i), "cartId", "").equals(cartId)) {
                cartItems.remove(i);
            }
        }
        saveCart();
    }

    private void confirmPurchase(
            Spinner classSpinner,
            EditText[] passengerNames,
            EditText[] passengerDocs,
            EditText holderName,
            EditText holderEmail,
            boolean seat,
            boolean bag,
            boolean vip,
            boolean priority,
            Spinner method,
            EditText cardNumber,
            NumberPicker cardMonth,
            NumberPicker cardYear) {
        if (checkoutFlight == null || sessionUser == null) {
            return;
        }
        String className = classSpinner.getSelectedItem().toString();
        int passengers = parseInt(value(checkoutFlight, "passengerCount", "1"), 1);
        String validationError = checkoutValidationError(passengerNames, passengerDocs, holderName, holderEmail, method, cardNumber, cardMonth, cardYear);
        if (!validationError.isEmpty()) {
            statusText.setText(validationError);
            return;
        }

        double servicesTotal = 0;
        servicesTotal += seat ? SERVICE_SEAT * passengers : 0;
        servicesTotal += bag ? SERVICE_BAG * passengers : 0;
        servicesTotal += vip ? SERVICE_VIP * passengers : 0;
        servicesTotal += priority ? SERVICE_PRIORITY * passengers : 0;
        double fare = flightFare(checkoutFlight, className, passengers) + servicesTotal;
        int methodId = parseInt(method.getSelectedItem().toString().substring(0, 1), 1);
        try {
            String checkoutCartId = value(checkoutFlight, "cartId", "");
            String checkoutItemId = value(checkoutFlight, "itemCarritoId", "");
            JSONObject body = new JSONObject()
                    .put("usuarioId", parseInt(value(sessionUser, "usuarioId", "0"), 0))
                    .put("pasajeroId", parseInt(value(sessionUser, "pasajeroId", "0"), 0))
                    .put("vueloId", parseInt(value(checkoutFlight, "id", "0"), 0))
                    .put("clase", className)
                    .put("numeroPasajeros", passengers)
                    .put("equipajeFacturado", bag ? 1 : 0)
                    .put("pesoEquipaje", bag ? 23 : JSONObject.NULL)
                    .put("tarifaPagada", fare)
                    .put("metodoPagoId", methodId)
                    .put("emailConfirmacion", holderEmail.getText().toString().trim())
                    .put("enviarCorreoConfirmacion", true);
            runTask("Confirmando compra...", () -> {
                String response = apiClient.post("/api/compras/vuelos", body.toString());
                if (!checkoutItemId.isEmpty()) {
                    apiClient.delete("/api/carritos-compra/pasajero/" + passengerId() + "/items/" + encode(checkoutItemId));
                }
                return response;
            }, json -> {
                JSONObject response = new JSONObject(json);
                lastPurchaseSuccess = new JSONObject()
                        .put("reservas", value(response, "codigoReserva", "-"))
                        .put("total", parseDouble(value(response, "total", "0")))
                        .put("pasajeros", passengers)
                        .put("vuelos", new JSONArray().put(successFlightSummary(checkoutFlight, response, "Vuelo")));
                removeLocalCartItem(checkoutCartId);
                checkoutFlight = null;
                activeView = "purchase_success";
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

    private void syncCartFromServer(boolean uploadLocalItems) {
        if (sessionUser == null || passengerId() <= 0) {
            return;
        }

        ArrayList<JSONObject> pendingLocalItems = new ArrayList<>();
        if (uploadLocalItems) {
            for (JSONObject item : cartItems) {
                if (value(item, "itemCarritoId", "").isEmpty()) {
                    pendingLocalItems.add(item);
                }
            }
        }

        runTask("Sincronizando carrito...", () -> {
            for (JSONObject item : pendingLocalItems) {
                apiClient.post("/api/carritos-compra/pasajero/" + passengerId() + "/items", cartItemPayload(item).toString());
            }

            return apiClient.get("/api/carritos-compra/pasajero/" + passengerId() + "/items");
        }, json -> {
            JSONArray serverItems = new JSONArray(json);
            cartItems.clear();
            for (int i = 0; i < serverItems.length(); i++) {
                JSONObject item = serverItems.optJSONObject(i);
                if (item != null) {
                    cartItems.add(serverCartItemToFlight(item));
                }
            }
            saveCart();
            statusText.setText("Carrito sincronizado.");
            render();
        });
    }

    private JSONObject serverCartItemToFlight(JSONObject item) throws Exception {
        String selectedClass = value(item, "selectedClass", "economica");
        int passengerCount = parseInt(value(item, "passengerCount", "1"), 1);

        return new JSONObject()
                .put("id", parseInt(value(item, "vueloId", value(item, "id", "0")), 0))
                .put("itemCarritoId", parseInt(value(item, "id", "0"), 0))
                .put("carritoId", parseInt(value(item, "carritoId", "0"), 0))
                .put("cartId", "server-" + value(item, "id", "0"))
                .put("numeroVuelo", value(item, "numeroVuelo", "Vuelo"))
                .put("aerolinea", value(item, "aerolinea", "-"))
                .put("origen", value(item, "origen", "-"))
                .put("destino", value(item, "destino", "-"))
                .put("fechaVuelo", value(item, "fechaVuelo", ""))
                .put("salidaReal", value(item, "salidaReal", ""))
                .put("llegadaReal", value(item, "llegadaReal", ""))
                .put("plazasOcupadas", parseInt(value(item, "plazasOcupadas", "0"), 0))
                .put("plazasDisponibles", parseInt(value(item, "plazasDisponibles", "0"), 0))
                .put("estado", value(item, "estado", "-"))
                .put("retrasoMinutos", parseInt(value(item, "retrasoMinutos", "0"), 0))
                .put("matriculaAvion", value(item, "matriculaAvion", "-"))
                .put("selectedClass", selectedClass)
                .put("passengerCount", passengerCount)
                .put("precioUnitario", parseDouble(value(item, "precioUnitario", "0")));
    }

    private JSONObject cartItemPayload(JSONObject item) throws Exception {
        String selectedClass = value(item, "selectedClass", "economica");
        int passengers = parseInt(value(item, "passengerCount", "1"), 1);

        return new JSONObject()
                .put("vueloId", parseInt(value(item, "id", value(item, "vueloId", "0")), 0))
                .put("clase", selectedClass)
                .put("precioUnitario", flightFare(item, selectedClass, 1))
                .put("cantidad", Math.max(1, passengers));
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

    private void navigateTo(String view) {
        checkoutFlight = null;
        fareSelectionFlight = null;
        lastPurchaseSuccess = null;
        activeView = view;
        if ("admin".equals(view) && tables.length() == 0) {
            loadTables();
        }
        if ("mis_viajes".equals(view) && sessionUser != null && myReservations.length() == 0) {
            loadMyTrips();
        }
        render();
    }

    private boolean isInvalidReturnDate(String value) {
        return "return".equals(datePickerMode)
                && !criteriaDepartureDate.isEmpty()
                && value.compareTo(criteriaDepartureDate) <= 0;
    }

    private LinearLayout screenHeader(String title, String subtitle, String backView, String actionLabel, View.OnClickListener action) {
        LinearLayout header = card();
        LinearLayout top = new LinearLayout(this);
        top.setOrientation(LinearLayout.HORIZONTAL);
        top.setGravity(Gravity.CENTER_VERTICAL);

        if (backView != null && !backView.trim().isEmpty()) {
            top.addView(smallBackButton(v -> navigateTo(backView)), new LinearLayout.LayoutParams(dp(48), dp(44)));
        }

        LinearLayout copy = new LinearLayout(this);
        copy.setOrientation(LinearLayout.VERTICAL);
        copy.addView(text(title, 22, TEXT, Typeface.BOLD));
        if (subtitle != null && !subtitle.trim().isEmpty()) {
            copy.addView(text(subtitle, 14, MUTED, Typeface.NORMAL));
        }
        top.addView(copy, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f));

        if (actionLabel != null && !actionLabel.trim().isEmpty() && action != null) {
            Button actionButton = outlineButton(actionLabel, action);
            top.addView(actionButton, new LinearLayout.LayoutParams(dp(104), LinearLayout.LayoutParams.WRAP_CONTENT));
        }

        header.addView(top);
        return header;
    }

    private String previousViewFor(String view) {
        if ("inicio".equals(view)) {
            return null;
        }
        if ("travel_results".equals(view) || "select_origin".equals(view) || "select_destination".equals(view)
                || "select_dates".equals(view) || "select_passengers".equals(view)) {
            return "explorar";
        }
        if ("select_fare".equals(view)) {
            return "travel_results";
        }
        if ("passenger_info".equals(view)) {
            return "travel_results";
        }
        if ("holder_info".equals(view)) {
            return "passenger_info";
        }
        if ("trip_options".equals(view)) {
            return "holder_info";
        }
        if ("payment".equals(view)) {
            return "trip_options";
        }
        if ("checkout".equals(view)) {
            return "carrito";
        }
        return "inicio";
    }

    private View infoTile(String icon, String label, View.OnClickListener listener) {
        LinearLayout tile = compactCard();
        tile.setGravity(Gravity.CENTER);
        tile.setMinimumHeight(dp(104));
        tile.setOnClickListener(listener);
        TextView iconView = text(icon, 24, TEXT, Typeface.BOLD);
        iconView.setGravity(Gravity.CENTER);
        tile.addView(iconView, matchWrap());
        TextView labelView = text(label, 16, TEXT, Typeface.NORMAL);
        labelView.setGravity(Gravity.CENTER);
        tile.addView(labelView, matchWrap());
        return tile;
    }

    private View menuOption(String icon, String title, String detail, String view) {
        LinearLayout option = new LinearLayout(this);
        option.setOrientation(LinearLayout.HORIZONTAL);
        option.setGravity(Gravity.CENTER_VERTICAL);
        option.setPadding(0, dp(12), 0, dp(12));
        option.setOnClickListener(v -> navigateTo(view));

        TextView iconView = text(icon, 22, TEXT, Typeface.BOLD);
        iconView.setGravity(Gravity.CENTER);
        option.addView(iconView, new LinearLayout.LayoutParams(dp(42), LinearLayout.LayoutParams.WRAP_CONTENT));

        LinearLayout copy = new LinearLayout(this);
        copy.setOrientation(LinearLayout.VERTICAL);
        copy.addView(text(title, 17, TEXT, Typeface.NORMAL));
        if (detail != null && !detail.trim().isEmpty()) {
            copy.addView(text(detail, 13, MUTED, Typeface.NORMAL));
        }
        option.addView(copy, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f));

        TextView arrow = text(">", 24, TEXT, Typeface.NORMAL);
        arrow.setGravity(Gravity.RIGHT | Gravity.CENTER_VERTICAL);
        option.addView(arrow, new LinearLayout.LayoutParams(dp(26), LinearLayout.LayoutParams.WRAP_CONTENT));
        return option;
    }

    private View destinationMenuOption(JSONObject destination) {
        String name = value(destination, "aeropuerto", "Destino");
        String stats = "Busquedas " + value(destination, "totalBusquedas", "0") + "  Clicks " + value(destination, "totalClicks", "0");
        LinearLayout option = new LinearLayout(this);
        option.setOrientation(LinearLayout.HORIZONTAL);
        option.setGravity(Gravity.CENTER_VERTICAL);
        option.setPadding(0, dp(12), 0, dp(12));
        option.setOnClickListener(v -> destinationClick(destination));

        TextView code = text(airportCode(name), 16, Color.WHITE, Typeface.BOLD);
        code.setGravity(Gravity.CENTER);
        code.setBackground(round(GOLD, GOLD, dp(12)));
        option.addView(code, new LinearLayout.LayoutParams(dp(54), dp(46)));

        LinearLayout copy = new LinearLayout(this);
        copy.setOrientation(LinearLayout.VERTICAL);
        copy.addView(text(shortAirportName(name), 17, TEXT, Typeface.BOLD));
        copy.addView(text(stats, 13, MUTED, Typeface.NORMAL));
        LinearLayout.LayoutParams copyParams = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f);
        copyParams.setMargins(dp(12), 0, 0, 0);
        option.addView(copy, copyParams);

        TextView arrow = text(">", 24, TEXT, Typeface.NORMAL);
        arrow.setGravity(Gravity.RIGHT | Gravity.CENTER_VERTICAL);
        option.addView(arrow, new LinearLayout.LayoutParams(dp(26), LinearLayout.LayoutParams.WRAP_CONTENT));
        return option;
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

    private TextView selectionField(String label, String value, View.OnClickListener listener) {
        TextView view = text(label + "\n" + value, 17, TEXT, Typeface.BOLD);
        view.setPadding(dp(18), dp(12), dp(18), dp(12));
        view.setMinHeight(dp(72));
        view.setBackground(round(Color.WHITE, LINE, dp(16)));
        view.setOnClickListener(listener);
        LinearLayout.LayoutParams params = matchWrap();
        params.setMargins(0, dp(8), 0, 0);
        view.setLayoutParams(params);
        return view;
    }

    private ScrollView fixedScroll(View child, int heightDp) {
        ScrollView scroll = new ScrollView(this);
        scroll.setFillViewport(false);
        scroll.setNestedScrollingEnabled(true);
        scroll.addView(child, matchWrap());
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, dp(heightDp));
        params.setMargins(0, dp(12), 0, 0);
        scroll.setLayoutParams(params);
        return scroll;
    }

    private View bookingProgress(int activeStep) {
        LinearLayout progress = new LinearLayout(this);
        progress.setOrientation(LinearLayout.HORIZONTAL);
        progress.setPadding(0, dp(14), 0, dp(4));
        String[] steps = {"1", "2", "3"};
        for (int i = 0; i < steps.length; i++) {
            boolean done = i + 1 < activeStep;
            boolean active = i + 1 == activeStep;
            TextView step = text(done ? "✓" : steps[i], 16, done || active ? Color.WHITE : MUTED, Typeface.BOLD);
            step.setGravity(Gravity.CENTER);
            step.setPadding(dp(8), dp(8), dp(8), dp(8));
            step.setMinHeight(dp(46));
            step.setBackground(round(done ? TEAL : active ? GOLD : PANEL_LIGHT, done || active ? Color.TRANSPARENT : LINE, dp(28)));
            progress.addView(step, weightedButtonParams());
        }
        return progress;
    }

    private LinearLayout card() {
        LinearLayout layout = new LinearLayout(this);
        layout.setOrientation(LinearLayout.VERTICAL);
        layout.setPadding(dp(14), dp(14), dp(14), dp(14));
        layout.setBackground(round(PANEL, LINE, dp(22)));
        LinearLayout.LayoutParams params = matchWrap();
        params.setMargins(0, dp(12), 0, 0);
        layout.setLayoutParams(params);
        return layout;
    }

    private LinearLayout compactCard() {
        LinearLayout layout = card();
        layout.setPadding(dp(12), dp(12), dp(12), dp(12));
        layout.setBackground(round(SKY, LINE, dp(22)));
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
        editText.setTextSize(16);
        editText.setTextColor(TEXT);
        editText.setPadding(dp(18), 0, dp(18), 0);
        editText.setMinHeight(dp(64));
        editText.setBackground(round(Color.WHITE, LINE, dp(16)));
        editText.setInputType(password
                ? InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_PASSWORD
                : InputType.TYPE_CLASS_TEXT);
        editText.setOnFocusChangeListener((view, hasFocus) -> {
            if (hasFocus && screenScrollView != null) {
                mainHandler.postDelayed(() -> screenScrollView.smoothScrollTo(0, Math.max(0, view.getBottom() - dp(120))), 250);
            }
        });
        LinearLayout.LayoutParams params = matchWrap();
        params.setMargins(0, dp(8), 0, 0);
        editText.setLayoutParams(params);
        return editText;
    }

    private PasswordInput passwordInput(String hint) {
        FrameLayout layout = new FrameLayout(this);
        LinearLayout.LayoutParams layoutParams = matchWrap();
        layoutParams.setMargins(0, dp(8), 0, 0);
        layout.setLayoutParams(layoutParams);

        EditText field = input(hint, "", true);
        field.setPadding(dp(18), 0, dp(58), 0);
        field.setLayoutParams(new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MATCH_PARENT, dp(64)));

        Button toggle = inlineIconButton(R.drawable.ic_visibility_24, v -> {
        });
        toggle.setContentDescription("Mostrar contraseña");
        boolean[] visible = {false};
        toggle.setOnClickListener(v -> {
            visible[0] = !visible[0];
            field.setInputType(InputType.TYPE_CLASS_TEXT | (visible[0] ? InputType.TYPE_TEXT_VARIATION_VISIBLE_PASSWORD : InputType.TYPE_TEXT_VARIATION_PASSWORD));
            field.setTypeface(Typeface.DEFAULT);
            field.setSelection(field.getText().length());
            toggle.setCompoundDrawablesWithIntrinsicBounds(visible[0] ? R.drawable.ic_visibility_off_24 : R.drawable.ic_visibility_24, 0, 0, 0);
            toggle.setContentDescription(visible[0] ? "Ocultar contraseña" : "Mostrar contraseña");
        });

        FrameLayout.LayoutParams toggleParams = new FrameLayout.LayoutParams(dp(52), dp(52), Gravity.RIGHT | Gravity.CENTER_VERTICAL);
        toggleParams.setMargins(0, 0, dp(6), 0);
        layout.addView(field);
        layout.addView(toggle, toggleParams);
        return new PasswordInput(layout, field);
    }

    private EditText dateInput(String hint) {
        EditText editText = input(hint, "", false);
        editText.setFocusable(false);
        editText.setInputType(InputType.TYPE_NULL);
        editText.setOnClickListener(v -> showDatePicker(editText));
        return editText;
    }

    private void showDatePicker(EditText target) {
        Calendar calendar = Calendar.getInstance();
        String current = target.getText().toString().trim();
        if (current.matches("\\d{4}-\\d{2}-\\d{2}")) {
            calendar.set(parseInt(current.substring(0, 4), calendar.get(Calendar.YEAR)),
                    parseInt(current.substring(5, 7), calendar.get(Calendar.MONTH) + 1) - 1,
                    parseInt(current.substring(8, 10), calendar.get(Calendar.DAY_OF_MONTH)));
        }
        DatePickerDialog dialog = new DatePickerDialog(this, (view, year, month, day) ->
                target.setText(String.format(Locale.US, "%04d-%02d-%02d", year, month + 1, day)),
                calendar.get(Calendar.YEAR),
                calendar.get(Calendar.MONTH),
                calendar.get(Calendar.DAY_OF_MONTH));
        dialog.show();
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

    private LinearLayout transferInfoPanel() {
        LinearLayout transferInfo = compactCard();
        transferInfo.setBackground(round(PANEL_LIGHT, LINE, dp(18)));
        transferInfo.addView(text("Datos para transferencia", 18, TEXT, Typeface.BOLD));
        transferInfo.addView(text("Titular: " + TRANSFER_ACCOUNT_NAME, 14, TEXT, Typeface.NORMAL));
        transferInfo.addView(text("Número de cuenta: " + TRANSFER_ACCOUNT_NUMBER, 16, PRIMARY_DARK, Typeface.BOLD));
        return transferInfo;
    }

    private void bindPaymentMethodToggle(Spinner method, View cardDetails, View transferInfo) {
        AdapterView.OnItemSelectedListener listener = new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                boolean requiresCard = !method.getSelectedItem().toString().startsWith("3");
                cardDetails.setVisibility(requiresCard ? View.VISIBLE : View.GONE);
                transferInfo.setVisibility(requiresCard ? View.GONE : View.VISIBLE);
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        };

        method.setOnItemSelectedListener(listener);
        listener.onItemSelected(method, null, method.getSelectedItemPosition(), method.getSelectedItemId());
    }

    private NumberPicker picker(int min, int max, int value, boolean twoDigits) {
        NumberPicker picker = new NumberPicker(this);
        picker.setMinValue(min);
        picker.setMaxValue(max);
        picker.setValue(value);
        picker.setWrapSelectorWheel(false);
        if (twoDigits) {
            picker.setFormatter(number -> String.format(Locale.US, "%02d", number));
        }
        return picker;
    }

    private View pickerBox(String label, NumberPicker picker) {
        LinearLayout box = new LinearLayout(this);
        box.setOrientation(LinearLayout.VERTICAL);
        box.setGravity(Gravity.CENTER);
        box.addView(text(label, 13, MUTED, Typeface.BOLD));
        box.addView(picker);
        return box;
    }

    private View serviceOptionCard(boolean[] selectedServices, int index, String title, String description, String price) {
        LinearLayout option = compactCard();
        TextView illustration = text("▰▰▰", 34, TEAL, Typeface.BOLD);
        illustration.setGravity(Gravity.CENTER);
        illustration.setPadding(0, dp(18), 0, dp(18));
        illustration.setBackground(round(PANEL_LIGHT, PANEL_LIGHT, dp(18)));
        option.addView(illustration, matchWrap());
        option.addView(text(title, 24, TEXT, Typeface.BOLD));
        option.addView(text(description, 15, MUTED, Typeface.NORMAL));
        option.addView(text(price, 18, TEXT, Typeface.BOLD));
        Button toggle = outlineButton(selectedServices[index] ? "Seleccionado" : "Agregar", v -> {
            selectedServices[index] = !selectedServices[index];
            ((Button) v).setText(selectedServices[index] ? "Seleccionado" : "Agregar");
            option.setBackground(round(selectedServices[index] ? Color.rgb(231, 250, 246) : SKY, selectedServices[index] ? TEAL : LINE, dp(22)));
        });
        option.addView(toggle, matchWrap());
        option.setOnClickListener(v -> {
            selectedServices[index] = !selectedServices[index];
            toggle.setText(selectedServices[index] ? "Seleccionado" : "Agregar");
            option.setBackground(round(selectedServices[index] ? Color.rgb(231, 250, 246) : SKY, selectedServices[index] ? TEAL : LINE, dp(22)));
        });
        option.setBackground(round(selectedServices[index] ? Color.rgb(231, 250, 246) : SKY, selectedServices[index] ? TEAL : LINE, dp(22)));
        return option;
    }

    private Button primaryButton(String label, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(Color.WHITE);
        button.setBackground(round(GOLD, GOLD, dp(30)));
        return button;
    }

    private Button outlineButton(String label, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(PRIMARY_DARK);
        button.setBackground(round(Color.TRANSPARENT, LINE, dp(16)));
        return button;
    }

    private Button navButton(String label, boolean active, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(active ? Color.WHITE : TEXT);
        button.setBackground(round(active ? DEEP : Color.WHITE, active ? DEEP : LINE, dp(18)));
        return button;
    }

    private Button iconButton(String label, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(TEXT);
        button.setTextSize(20);
        button.setGravity(Gravity.CENTER);
        button.setMinWidth(0);
        button.setMinimumWidth(0);
        button.setMinHeight(dp(52));
        button.setPadding(0, 0, 0, 0);
        button.setBackground(round(Color.WHITE, LINE, dp(28)));
        return button;
    }

    private Button inlineIconButton(int drawableRes, View.OnClickListener listener) {
        Button button = baseButton("", listener);
        button.setGravity(Gravity.CENTER);
        button.setMinWidth(0);
        button.setMinimumWidth(0);
        button.setMinHeight(dp(52));
        button.setPadding(0, 0, 0, 0);
        button.setCompoundDrawablePadding(0);
        button.setCompoundDrawablesWithIntrinsicBounds(drawableRes, 0, 0, 0);
        button.setBackground(round(Color.TRANSPARENT, Color.TRANSPARENT, dp(26)));
        return button;
    }

    private Button accountButton(String description, View.OnClickListener listener) {
        Button button = iconButton("", listener);
        SpannableString iconText = new SpannableString(" ");
        ImageSpan imageSpan = new ImageSpan(this, R.drawable.ic_account_circle_24, ImageSpan.ALIGN_CENTER);
        iconText.setSpan(imageSpan, 0, 1, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
        button.setText(iconText);
        button.setContentDescription(description);
        return button;
    }

    private Button sessionButton(String label, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(PRIMARY_DARK);
        button.setTextSize(12);
        button.setMinWidth(0);
        button.setMinimumWidth(0);
        button.setMinHeight(dp(52));
        button.setPadding(dp(8), 0, dp(8), 0);
        button.setBackground(round(Color.WHITE, LINE, dp(28)));
        return button;
    }

    private Button smallBackButton(View.OnClickListener listener) {
        Button button = baseButton("<", listener);
        button.setTextColor(PRIMARY_DARK);
        button.setTextSize(20);
        button.setMinWidth(0);
        button.setMinimumWidth(0);
        button.setMinHeight(dp(44));
        button.setPadding(0, 0, 0, 0);
        button.setBackground(round(PANEL_LIGHT, LINE, dp(22)));
        return button;
    }

    private Button baseButton(String label, View.OnClickListener listener) {
        Button button = new Button(this);
        button.setText(label);
        button.setTextSize(14);
        button.setAllCaps(false);
        button.setMinHeight(dp(58));
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

    private GradientDrawable gradientRound(int startColor, int endColor, int radius) {
        GradientDrawable drawable = new GradientDrawable(GradientDrawable.Orientation.TL_BR, new int[]{startColor, endColor});
        drawable.setCornerRadius(radius);
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

    private LinearLayout.LayoutParams iconParams() {
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(dp(52), dp(52));
        params.setMargins(dp(6), 0, 0, 0);
        return params;
    }

    private LinearLayout.LayoutParams calendarCellParams() {
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f);
        params.setMargins(dp(2), 0, dp(2), dp(8));
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
                ? "Sin sesión."
                : "Sesión: " + value(sessionUser, "nombreCompleto", value(sessionUser, "usuario", "Usuario")));
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

    private int passengerId() {
        return parseInt(value(sessionUser, "pasajeroId", "0"), 0);
    }

    private int passengerCountFromCriteria() {
        return Math.max(1, criteriaAdults + criteriaYouth + criteriaChildren + criteriaBabies);
    }

    private boolean ensureTripSelection() {
        if (selectedOutboundFlight == null || ("roundtrip".equals(criteriaTripType) && selectedReturnFlight == null)) {
            activeView = "travel_results";
            lastMessage = "Selecciona los vuelos antes de continuar.";
            render();
            return false;
        }
        return true;
    }

    private String reviewDateSummary() {
        String summary = criteriaDepartureDate.isEmpty() ? "Fecha flexible" : displayDate(criteriaDepartureDate);
        if ("roundtrip".equals(criteriaTripType)) {
            summary += " - " + (criteriaReturnDate.isEmpty() ? "Vuelta" : displayDate(criteriaReturnDate));
        }
        return summary;
    }

    private View tripSummaryBar() {
        LinearLayout summary = compactCard();
        summary.addView(text("Resumen del viaje", 18, TEXT, Typeface.NORMAL));
        summary.addView(text(money(tripTotal()), 20, TEXT, Typeface.BOLD));
        summary.addView(text("Your total booking", 13, MUTED, Typeface.NORMAL));
        return summary;
    }

    private double tripTotal() {
        double total = 0;
        if (selectedOutboundFlight != null) {
            int passengers = parseInt(value(selectedOutboundFlight, "passengerCount", "1"), 1);
            total += flightFare(selectedOutboundFlight, value(selectedOutboundFlight, "selectedClass", "economica"), passengers);
        }
        if ("roundtrip".equals(criteriaTripType) && selectedReturnFlight != null) {
            int passengers = parseInt(value(selectedReturnFlight, "passengerCount", "1"), 1);
            total += flightFare(selectedReturnFlight, value(selectedReturnFlight, "selectedClass", "economica"), passengers);
        }
        int passengers = passengerCountFromCriteria();
        total += selectedSeat ? SERVICE_SEAT * passengers : 0;
        total += selectedBag ? SERVICE_BAG * passengers : 0;
        total += selectedVip ? SERVICE_VIP * passengers : 0;
        total += selectedPriority ? SERVICE_PRIORITY * passengers : 0;
        return Math.round(total * 100.0) / 100.0;
    }

    private String defaultSaved(String[] values, int index, String fallback) {
        return values != null && index >= 0 && index < values.length && values[index] != null && !values[index].isEmpty() ? values[index] : fallback;
    }

    private String[] textValues(EditText[] inputs) {
        String[] values = new String[inputs.length];
        for (int i = 0; i < inputs.length; i++) {
            values[i] = inputs[i].getText().toString().trim();
        }
        return values;
    }

    private String checkoutValidationError(
            EditText[] passengerNames,
            EditText[] passengerDocs,
            EditText holderName,
            EditText holderEmail,
            Spinner method,
            EditText cardNumber,
            NumberPicker cardMonth,
            NumberPicker cardYear) {
        for (int i = 1; i < passengerNames.length; i++) {
            if (isBlank(passengerNames[i]) || isBlank(passengerDocs[i])) {
                markInvalid(passengerNames[i]);
                markInvalid(passengerDocs[i]);
                return "Completa nombre y documento de los pasajeros adicionales.";
            }
        }

        if (isBlank(holderName) || isBlank(holderEmail)) {
            markInvalid(holderName);
            markInvalid(holderEmail);
            return "Completa los datos del titular de la reserva.";
        }

        String email = holderEmail.getText().toString().trim();
        if (!email.contains("@") || !email.contains(".")) {
            markInvalid(holderEmail);
            return "Ingresa un email válido para el titular.";
        }

        boolean requiresCard = !method.getSelectedItem().toString().startsWith("3");
        if (requiresCard) {
            if (cardNumber.getText().toString().replaceAll("\\D", "").length() < 13) {
                markInvalid(cardNumber);
                return "Número de tarjeta incompleto.";
            }
            if (isExpiredCard(cardMonth.getValue(), cardYear.getValue())) {
                return "La tarjeta esta vencida.";
            }
        }

        return "";
    }

    private boolean isBlank(EditText input) {
        return input == null || input.getText().toString().trim().isEmpty();
    }

    private boolean isExpiredCard(int month, int year) {
        Calendar expiration = Calendar.getInstance();
        expiration.set(year, month - 1, expiration.getActualMaximum(Calendar.DAY_OF_MONTH), 23, 59, 59);
        return expiration.before(Calendar.getInstance());
    }

    private void markInvalid(EditText input) {
        if (input != null) {
            input.setBackground(round(Color.rgb(255, 241, 243), RED, dp(12)));
        }
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
        if (value == null) {
            return "";
        }
        String normalized = Normalizer.normalize(value.trim(), Normalizer.Form.NFD)
                .replaceAll("\\p{M}", "");
        return normalized.toLowerCase(Locale.ROOT);
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

    private String dateInputValue(java.util.Calendar calendar) {
        return new java.text.SimpleDateFormat("yyyy-MM-dd", Locale.US).format(calendar.getTime());
    }

    private boolean matchesCalendarFlight(JSONObject flight, String date) {
        if (!canPurchase(flight) || !value(flight, "fechaVuelo", "").startsWith(date)) {
            return false;
        }
        String origin = criteriaOrigin == null || criteriaOrigin.trim().isEmpty() ? "" : criteriaOrigin;
        String destination = criteriaDestination == null || criteriaDestination.trim().isEmpty() ? "" : criteriaDestination;
        return (origin.isEmpty() || contains(value(flight, "origen", ""), origin))
                && (destination.isEmpty() || contains(value(flight, "destino", ""), destination));
    }

    private String compactMoney(double value) {
        if (value <= 0) {
            return "-";
        }
        return "Q " + Math.round(value);
    }

    private String shortAirportName(String value) {
        if (value == null || value.trim().isEmpty()) {
            return "Destino";
        }
        String clean = value
                .replace("Aeropuerto Internacional", "")
                .replace("International Airport", "")
                .replace("Aeropuerto", "")
                .trim();
        return clean.length() > 18 ? clean.substring(0, 18) : clean;
    }

    private String airportDisplay(String value) {
        if (value == null || value.trim().isEmpty()) {
            return "";
        }
        return shortAirportName(value) + " (" + airportCode(value) + ")";
    }

    private String airportTitle(JSONObject airport) {
        String name = value(airport, "nombre", "");
        return name.isEmpty() ? value(airport, "aeropuerto", "Aeropuerto") : name;
    }

    private String airportCityCountry(JSONObject airport) {
        String city = value(airport, "ciudad", "");
        String country = value(airport, "pais", "");
        if (city.isEmpty()) {
            return country.isEmpty() ? "Ciudad" : country;
        }
        return country.isEmpty() ? city : city + ", " + country;
    }

    private String airportCodeFromAirport(JSONObject airport) {
        String code = value(airport, "codigoIata", "");
        if (code.isEmpty() || "null".equalsIgnoreCase(code)) {
            code = value(airport, "codigo", "");
        }
        if (code.isEmpty() || "null".equalsIgnoreCase(code)) {
            code = airportCode(airportTitle(airport));
        }
        return code.toUpperCase(Locale.ROOT);
    }

    private boolean matchesAirport(JSONObject airport, String term) {
        String clean = normalize(term);
        if (clean.isEmpty()) {
            return true;
        }
        return normalize(airportTitle(airport)).contains(clean)
                || normalize(airportCityCountry(airport)).contains(clean)
                || normalize(airportCodeFromAirport(airport)).contains(clean);
    }

    private JSONObject fallbackAirport(String term) {
        String clean = normalize(term);
        try {
            if (clean.contains("canada") || clean.contains("toronto") || clean.contains("yyz")) {
                return new JSONObject()
                        .put("nombre", "Toronto Pearson International Airport")
                        .put("ciudad", "Toronto")
                        .put("pais", "Canadá")
                        .put("codigoIata", "YYZ");
            }
            if (clean.contains("vancouver") || clean.contains("yvr")) {
                return new JSONObject()
                        .put("nombre", "Vancouver International Airport")
                        .put("ciudad", "Vancouver")
                        .put("pais", "Canadá")
                        .put("codigoIata", "YVR");
            }
        } catch (Exception ignored) {
        }
        return null;
    }

    private void registerDestinationClick(JSONObject airport) {
        String id = value(airport, "id", "");
        if (id.isEmpty()) {
            id = value(airport, "aeropuertoId", "");
        }
        if (id.isEmpty()) {
            return;
        }
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

    private String displayDate(String value) {
        if (value == null || value.trim().isEmpty()) {
            return "";
        }
        try {
            java.text.SimpleDateFormat parser = new java.text.SimpleDateFormat("yyyy-MM-dd", Locale.US);
            java.util.Date date = parser.parse(value);
            return date == null ? value : new java.text.SimpleDateFormat("EEE, dd MMM", Locale.US).format(date);
        } catch (Exception ignored) {
            return value;
        }
    }

    private boolean dateSelected(String value) {
        return value.equals(criteriaDepartureDate) || value.equals(criteriaReturnDate);
    }

    private int dateColor(String value) {
        if (dateSelected(value)) {
            return SOFT_GREEN;
        }
        int day = parseInt(value.substring(value.length() - 2), 1);
        if (day % 7 == 0 || day % 11 == 0) {
            return Color.rgb(255, 224, 218);
        }
        if (day % 3 == 0) {
            return Color.rgb(255, 241, 190);
        }
        return Color.rgb(202, 237, 226);
    }

    private String passengerSummary() {
        int total = passengerCountFromCriteria();
        return total + (total == 1 ? " adult" : " passengers");
    }

    private String routeLabel(String origin, String destination) {
        String from = airportCode(origin == null || origin.trim().isEmpty() ? "Guatemala City" : origin);
        String to = airportCode(destination == null || destination.trim().isEmpty() ? "Destino" : destination);
        return from + " ↔ " + to;
    }

    private String airportCode(String value) {
        String normalized = normalize(value);
        if (normalized.contains("miami")) {
            return "MIA";
        }
        if (normalized.contains("fort lauderdale")) {
            return "FLL";
        }
        if (normalized.contains("guatemala") || normalized.contains("aurora")) {
            return "GUA";
        }
        if (normalized.contains("bogota") || normalized.contains("dorado")) {
            return "BOG";
        }
        if (normalized.contains("mexico")) {
            return "MEX";
        }
        if (normalized.contains("cancun")) {
            return "CUN";
        }
        if (normalized.contains("panama")) {
            return "PTY";
        }
        if (normalized.length() >= 3) {
            return normalized.substring(0, 3).toUpperCase(Locale.ROOT);
        }
        return "---";
    }

    private String timeOnly(String value) {
        if (value == null || value.trim().isEmpty()) {
            return "--:--";
        }
        String clean = value.replace("T", " ");
        return clean.length() >= 16 ? clean.substring(11, 16) : "--:--";
    }

    private int estimateDurationMinutes(JSONObject flight) {
        int seed = parseInt(value(flight, "id", "1"), 1) + normalize(value(flight, "destino", "")).length() * 11;
        return 70 + (seed % 5) * 25;
    }

    private boolean hasTechnicalStop(JSONObject flight) {
        return parseInt(value(flight, "retrasoMinutos", "0"), 0) > 0 || parseInt(value(flight, "id", "0"), 0) % 3 == 0;
    }

    private String estimatedArrival(String value, int minutes) {
        if (value == null || value.trim().isEmpty()) {
            return "";
        }

        try {
            java.text.SimpleDateFormat parser = new java.text.SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.US);
            java.util.Date date = parser.parse(value.length() > 19 ? value.substring(0, 19) : value);
            if (date == null) {
                return value;
            }
            java.util.Calendar calendar = java.util.Calendar.getInstance();
            calendar.setTime(date);
            calendar.add(java.util.Calendar.MINUTE, minutes);
            return parser.format(calendar.getTime());
        } catch (Exception ignored) {
            return value;
        }
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

    private double flightFare(JSONObject flight, String className, int passengers) {
        double multiplier = "primera".equals(className) ? 1.68 : ("turista".equals(className) || "ejecutiva".equals(className)) ? 1.24 : 1.0;
        return Math.round(baseFareForFlight(flight) * multiplier * Math.max(1, passengers) * 100.0) / 100.0;
    }

    private double baseFareForFlight(JSONObject flight) {
        int duration = Math.max(55, estimateDurationMinutes(flight));
        int availability = Math.max(0, parseInt(value(flight, "plazasDisponibles", "80"), 80));
        int demand = Math.max(0, 120 - availability) * 3;
        int stopPenalty = hasTechnicalStop(flight) ? 140 : 0;
        int dateSeed = 0;
        String date = value(flight, "fechaVuelo", "");
        if (date.length() >= 10) {
            dateSeed = parseInt(date.substring(8, 10), 1) * 7;
        }
        double routeWeight = Math.max(1, airportCode(value(flight, "destino", "")).charAt(0) - 'A' + 1) * 6;
        return Math.round((740 + duration * 2.8 + demand + stopPenalty + dateSeed + routeWeight) * 100.0) / 100.0;
    }

    private double fare(String className, int passengers) {
        double multiplier = "primera".equals(className) ? 1.68 : ("turista".equals(className) || "ejecutiva".equals(className)) ? 1.24 : 1.0;
        return Math.round(BASE_FARE * multiplier * Math.max(1, passengers) * 100.0) / 100.0;
    }

    private String classLabel(String className) {
        if ("primera".equals(className)) {
            return "Primera clase";
        }
        if ("turista".equals(className) || "ejecutiva".equals(className)) {
            return "Turista";
        }
        return "Económica";
    }

    private String upgradeClass(String className) {
        if ("economica".equals(className)) {
            return "turista";
        }
        if ("turista".equals(className) || "ejecutiva".equals(className)) {
            return "primera";
        }
        return "";
    }

    private int fareColor(String className) {
        if ("primera".equals(className)) {
            return PRIMARY_DARK;
        }
        if ("turista".equals(className) || "ejecutiva".equals(className)) {
            return GOLD;
        }
        return TEAL;
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

    private interface CounterSetter {
        void set(int value);
    }

    private interface RowFormatter {
        String[] lines(JSONObject item);
    }

    private static final class PasswordInput {
        final FrameLayout layout;
        final EditText field;

        PasswordInput(FrameLayout layout, EditText field) {
            this.layout = layout;
            this.field = field;
        }
    }
}
