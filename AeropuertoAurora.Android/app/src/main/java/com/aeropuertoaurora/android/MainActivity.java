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
import android.widget.CheckBox;
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
    private static final int DEEP = Color.rgb(24, 24, 24);
    private static final int SKY = Color.rgb(255, 255, 255);
    private static final int PANEL = Color.rgb(255, 255, 255);
    private static final int PANEL_LIGHT = Color.rgb(247, 247, 247);
    private static final int LINE = Color.rgb(218, 218, 218);
    private static final int TEXT = Color.rgb(28, 29, 33);
    private static final int MUTED = Color.rgb(105, 105, 110);
    private static final int TEAL = Color.rgb(21, 139, 142);
    private static final int GOLD = Color.rgb(215, 20, 38);
    private static final int RED = Color.rgb(221, 35, 45);
    private static final int GREEN = Color.rgb(24, 157, 65);
    private static final int SOFT_GREEN = Color.rgb(230, 248, 234);
    private static final int APP_BG = Color.rgb(247, 247, 247);
    private static final double BASE_FARE = 1250.0;
    private static final double SERVICE_SEAT = 120.0;
    private static final double SERVICE_BAG = 280.0;
    private static final double SERVICE_VIP = 360.0;
    private static final double SERVICE_PRIORITY = 95.0;

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
    private String criteriaTripType = "roundtrip";
    private String criteriaOrigin = "";
    private String criteriaDestination = "";
    private String criteriaDepartureDate = "";
    private String criteriaReturnDate = "";
    private String datePickerMode = "departure";
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

    private View buildScreen() {
        ScrollView scrollView = new ScrollView(this);
        scrollView.setFillViewport(true);
        scrollView.setBackgroundColor(APP_BG);

        rootLayout = new LinearLayout(this);
        rootLayout.setOrientation(LinearLayout.VERTICAL);
        rootLayout.setPadding(dp(16), dp(16), dp(16), dp(30));
        scrollView.addView(rootLayout, matchWrap());

        LinearLayout header = new LinearLayout(this);
        header.setOrientation(LinearLayout.VERTICAL);
        header.setPadding(dp(4), dp(4), dp(4), dp(10));
        rootLayout.addView(header, matchWrap());

        TextView title = text("La Aurora", 30, TEXT, Typeface.BOLD);
        header.addView(title);
        TextView subtitle = text("App movil conectada al API del proyecto.", 14, MUTED, Typeface.NORMAL);
        subtitle.setPadding(0, dp(3), 0, dp(10));
        header.addView(subtitle);

        statusText = text("Preparando panel movil...", 13, TEXT, Typeface.BOLD);
        statusText.setPadding(dp(12), dp(10), dp(12), dp(10));
        statusText.setBackground(round(SKY, LINE, dp(14)));
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
        } else if ("select_origin".equals(activeView)) {
            renderAirportPicker(true, "");
        } else if ("select_destination".equals(activeView)) {
            renderAirportPicker(false, "");
        } else if ("select_dates".equals(activeView)) {
            renderDatePicker();
        } else if ("select_passengers".equals(activeView)) {
            renderPassengerSelector();
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
        card.addView(sectionTitle("Book a flight"));
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
                    render();
                }), weightedButtonParams());
        card.addView(typeRow);

        card.addView(label("Ruta"));
        card.addView(selectionField("From", criteriaOrigin.isEmpty() ? "Guatemala City (GUA)" : airportDisplay(criteriaOrigin), v -> {
            activeView = "select_origin";
            render();
        }));
        card.addView(selectionField("To", criteriaDestination.isEmpty() ? "Seleccionar destino" : airportDisplay(criteriaDestination), v -> {
            activeView = "select_destination";
            render();
        }));

        card.addView(label("Fechas"));
        String dateSummary = criteriaDepartureDate.isEmpty() ? "Departure" : displayDate(criteriaDepartureDate);
        if ("roundtrip".equals(criteriaTripType)) {
            dateSummary += "  -  " + (criteriaReturnDate.isEmpty() ? "Return" : displayDate(criteriaReturnDate));
        }
        card.addView(selectionField("When are you flying?", dateSummary, v -> {
            datePickerMode = "departure";
            activeView = "select_dates";
            render();
        }));

        card.addView(label("Quienes vuelan"));
        card.addView(selectionField("Passenger(s)", passengerSummary(), v -> {
            activeView = "select_passengers";
            render();
        }));

        card.addView(primaryButton("Explorar", v -> {
            if (criteriaOrigin.trim().isEmpty()) {
                criteriaOrigin = "La Aurora";
            }
            showTravelResults(criteriaOrigin, criteriaDestination, criteriaDepartureDate, passengerCountFromCriteria());
        }), matchWrap());
        contentLayout.addView(card);
        dynamicContentStartIndex = contentLayout.getChildCount();
        if (!criteriaDestination.trim().isEmpty()) {
            showTravelResults(criteriaOrigin, criteriaDestination, criteriaDepartureDate, passengerCountFromCriteria());
        }
    }

    private void renderAirportPicker(boolean originPicker, String term) {
        LinearLayout card = card();
        card.addView(sectionTitle(originPicker ? "Select origin" : "Select destination"));
        card.addView(outlineButton("Volver", v -> {
            activeView = "explorar";
            render();
        }), matchWrap());

        EditText search = input(originPicker ? "Buscar origen" : "Buscar destino", term, false);
        card.addView(search);
        card.addView(primaryButton("Buscar", v -> {
            contentLayout.removeAllViews();
            renderAirportPicker(originPicker, search.getText().toString());
        }), matchWrap());
        contentLayout.addView(card);

        LinearLayout list = card();
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
            list.addView(text("No encontramos aeropuertos con ese texto.", 13, MUTED, Typeface.NORMAL));
        }
        contentLayout.addView(fixedScroll(list, 560));
    }

    private View airportRow(JSONObject airport, boolean originPicker) {
        LinearLayout item = compactCard();
        item.setOnClickListener(v -> selectAirport(airport, originPicker));
        LinearLayout line = new LinearLayout(this);
        line.setOrientation(LinearLayout.HORIZONTAL);
        TextView names = text(airportCityCountry(airport) + "\n" + airportTitle(airport), 17, TEXT, Typeface.BOLD);
        TextView code = text(airportCodeFromAirport(airport), 24, TEXT, Typeface.NORMAL);
        code.setGravity(Gravity.RIGHT | Gravity.CENTER_VERTICAL);
        line.addView(names, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1f));
        line.addView(code, new LinearLayout.LayoutParams(dp(90), LinearLayout.LayoutParams.WRAP_CONTENT));
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
        LinearLayout card = card();
        card.addView(sectionTitle("When are you flying?"));
        card.addView(outlineButton("Volver", v -> {
            activeView = "explorar";
            render();
        }), matchWrap());

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
                    ? primaryButton("Departure", v -> {
                    })
                    : outlineButton("Departure", v -> {
                        datePickerMode = "departure";
                        render();
                    }), weightedButtonParams());
            modeRow.addView("return".equals(datePickerMode)
                    ? primaryButton("Return", v -> {
                    })
                    : outlineButton("Return", v -> {
                        datePickerMode = "return";
                        render();
                    }), weightedButtonParams());
            card.addView(modeRow);
        }

        card.addView(text("Salida: " + (criteriaDepartureDate.isEmpty() ? "-" : displayDate(criteriaDepartureDate)), 14, MUTED, Typeface.NORMAL));
        if ("roundtrip".equals(criteriaTripType)) {
            card.addView(text("Vuelta: " + (criteriaReturnDate.isEmpty() ? "-" : displayDate(criteriaReturnDate)), 14, MUTED, Typeface.NORMAL));
        }
        contentLayout.addView(card);

        LinearLayout calendar = card();
        addCalendarMonth(calendar, 2026, 6);
        addCalendarMonth(calendar, 2026, 7);
        contentLayout.addView(calendar);
    }

    private void addCalendarMonth(LinearLayout parent, int year, int month) {
        java.util.Calendar calendar = java.util.Calendar.getInstance();
        calendar.set(year, month - 1, 1);
        String title = new java.text.SimpleDateFormat("MMMM yyyy", Locale.US).format(calendar.getTime());
        parent.addView(text(title, 22, TEXT, Typeface.BOLD));
        String[] weekdays = {"MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"};
        LinearLayout week = row();
        for (String weekday : weekdays) {
            TextView day = text(weekday, 12, TEXT, Typeface.BOLD);
            day.setGravity(Gravity.CENTER);
            week.addView(day, weightedButtonParams());
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
                    row.addView(blank, weightedButtonParams());
                    continue;
                }
                String value = String.format(Locale.US, "%04d-%02d-%02d", year, month, day);
                Button dateButton = baseButton(String.valueOf(day), v -> selectDate(value));
                dateButton.setTextColor(TEXT);
                dateButton.setBackground(round(dateColor(value), dateSelected(value) ? GREEN : Color.TRANSPARENT, dp(28)));
                row.addView(dateButton, weightedButtonParams());
                day++;
            }
            parent.addView(row);
        }
    }

    private void selectDate(String value) {
        if ("return".equals(datePickerMode)) {
            criteriaReturnDate = value;
            activeView = "explorar";
        } else {
            criteriaDepartureDate = value;
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
        LinearLayout card = card();
        card.addView(sectionTitle("Who is flying?"));
        card.addView(text("Selecciona los pasajeros para esta busqueda.", 14, MUTED, Typeface.NORMAL));
        card.addView(counterRow("Adults", "15+ years", criteriaAdults, value -> criteriaAdults = Math.max(1, value)));
        card.addView(counterRow("Young", "12-14 years", criteriaYouth, value -> criteriaYouth = Math.max(0, value)));
        card.addView(counterRow("Children", "2-11 years", criteriaChildren, value -> criteriaChildren = Math.max(0, value)));
        card.addView(counterRow("Babies", "Under 2 years", criteriaBabies, value -> criteriaBabies = Math.max(0, value)));
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
        results.addView(bookingProgress(1));
        results.addView(text(routeLabel(origin, destination), 28, TEXT, Typeface.BOLD));
        results.addView(text((date == null || date.isEmpty() ? "Fecha flexible" : date) + "  •  " + passengers + " pasajero(s)", 15, MUTED, Typeface.NORMAL));
        TextView info = text("Book now. No esperamos que los precios bajen para esta fecha.", 15, GREEN, Typeface.BOLD);
        info.setPadding(dp(14), dp(14), dp(14), dp(14));
        info.setBackground(round(SOFT_GREEN, Color.rgb(190, 224, 198), dp(14)));
        results.addView(info, matchWrap());
        addResultDateCarousel(results, date);
        LinearLayout filters = row();
        filters.addView(outlineButton("Business Class +", v -> {
            lastMessage = "Filtro aplicado: Business Class.";
            render();
        }), weightedButtonParams());
        results.addView(filters);
        results.addView(sectionTitle("Vuelos disponibles"));
        results.addView(text(("roundtrip".equals(criteriaTripType) ? "Ida y vuelta" : "Solo ida") + " - " + passengers + " pasajero(s)", 13, MUTED, Typeface.NORMAL));
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

    private void addResultDateCarousel(LinearLayout parent, String selectedDate) {
        java.util.Calendar calendar = java.util.Calendar.getInstance();
        if (selectedDate != null && selectedDate.matches("\\d{4}-\\d{2}-\\d{2}")) {
            try {
                java.util.Date date = new java.text.SimpleDateFormat("yyyy-MM-dd", Locale.US).parse(selectedDate);
                if (date != null) {
                    calendar.setTime(date);
                }
            } catch (Exception ignored) {
            }
        }

        for (int i = 0; i < 6; i += 3) {
            LinearLayout row = row();
            for (int j = i; j < Math.min(i + 3, 6); j++) {
                java.util.Calendar day = (java.util.Calendar) calendar.clone();
                day.add(java.util.Calendar.DAY_OF_MONTH, j);
                String dateValue = dateInputValue(day);
                String label = new java.text.SimpleDateFormat("EEE, dd MMM", Locale.US).format(day.getTime()) + "\n" + money(fareForDate(day));
                row.addView(outlineButton(label, v -> {
                    criteriaDepartureDate = dateValue;
                    showTravelResults(criteriaOrigin, criteriaDestination, criteriaDepartureDate, passengerCountFromCriteria());
                }), weightedButtonParams());
            }
            parent.addView(row);
        }
    }

    private void renderBoard() {
        LinearLayout search = card();
        search.addView(sectionTitle("Rastreo de vuelos"));
        EditText term = input("Numero, aerolinea, origen, destino o estado", "", false);
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
        item.addView(text("From " + money(fare("economica", passengers)), 24, TEXT, Typeface.BOLD));
        if (allowBuy) {
            item.addView(label("Select your fare"));
            LinearLayout tabs = row();
            tabs.addView(navButton("Economy", true, v -> {
            }), weightedButtonParams());
            tabs.addView(navButton("Business Class", false, v -> {
            }), weightedButtonParams());
            item.addView(tabs);
            item.addView(tariffCard(flight, "economica", "Turista", "Para viajar basico", passengers, "Articulo personal incluido", "Equipaje de bodega opcional", false));
            item.addView(tariffCard(flight, "ejecutiva", "Ejecutiva", "Mas comodidad", passengers, "Articulo personal y equipaje", "Mejor balance de precio/beneficios", true));
            item.addView(tariffCard(flight, "primera", "Primera clase", "Mayor tranquilidad", passengers, "Cambios y reembolso", "Experiencia completa", false));
        }
        return item;
    }

    private View tariffCard(JSONObject flight, String className, String name, String tagline, int passengers, String benefitOne, String benefitTwo, boolean recommended) {
        LinearLayout card = compactCard();
        if (recommended) {
            card.addView(text("BEST OPTION", 12, GOLD, Typeface.BOLD));
        }
        card.addView(text(name, 28, "ejecutiva".equals(className) ? Color.rgb(192, 0, 137) : "primera".equals(className) ? Color.rgb(245, 96, 0) : Color.rgb(192, 0, 137), Typeface.BOLD));
        card.addView(text(tagline, 13, MUTED, Typeface.NORMAL));
        card.addView(text("▣ " + benefitOne, 14, TEXT, Typeface.NORMAL));
        card.addView(text("▣ " + benefitTwo, 14, TEXT, Typeface.NORMAL));
        card.addView(text("▣ Check-in en el aeropuerto", 14, TEXT, Typeface.NORMAL));
        card.addView(text("ⓘ Restricciones de tarifa", 14, TEAL, Typeface.NORMAL));
        card.addView(primaryButton(money(fare(className, passengers)), v -> chooseFare(flight, className, passengers)), matchWrap());
        TextView perPassenger = text("Precio por pasajero", 12, MUTED, Typeface.NORMAL);
        perPassenger.setGravity(Gravity.CENTER);
        card.addView(perPassenger);
        return card;
    }

    private void chooseFare(JSONObject flight, String className, int passengers) {
        if (sessionUser == null) {
            lastMessage = "Inicia sesion para agregar vuelos al carrito, igual que en la web.";
            render();
            return;
        }

        addCart(flight, className, passengers);
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
        card.addView(bookingProgress(2));
        card.addView(sectionTitle("Passenger information"));
        card.addView(text(value(checkoutFlight, "numeroVuelo", "Vuelo"), 20, TEXT, Typeface.BOLD));
        card.addView(text(value(checkoutFlight, "origen", "-") + " -> " + value(checkoutFlight, "destino", "-"), 14, MUTED, Typeface.NORMAL));
        card.addView(text("Pasos: Pasajeros -> Extras -> Pago. Todo se confirma al final como en el web.", 13, MUTED, Typeface.NORMAL));

        Spinner classSpinner = spinner(new String[]{"economica", "ejecutiva", "primera"}, value(checkoutFlight, "selectedClass", "economica"));
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
        EditText[] passengerAges = new EditText[passengerCount];
        for (int i = 0; i < passengerCount; i++) {
            LinearLayout passengerCard = compactCard();
            passengerCard.addView(text(i == 0 ? "Adult 1" : "Pasajero " + (i + 1), 20, TEXT, Typeface.NORMAL));
            passengerCard.addView(text("Ingresa el nombre y apellido exactamente como aparece en el pasaporte o documento.", 13, MUTED, Typeface.NORMAL));
            Spinner gender = spinner(new String[]{"Genero", "Masculino", "Femenino"}, "Genero");
            passengerCard.addView(gender);
            passengerNames[i] = input("Name *", i == 0 ? value(sessionUser, "nombreCompleto", "") : "", false);
            passengerDocs[i] = input("Last name / Documento *", "", false);
            passengerAges[i] = input("Date of Birth / Edad *", defaultPassengerAge(i), false);
            passengerAges[i].setInputType(InputType.TYPE_CLASS_NUMBER);
            passengerCard.addView(passengerNames[i]);
            passengerCard.addView(passengerDocs[i]);
            passengerCard.addView(passengerAges[i]);
            passengerCard.addView(input("Nationality of your travel document *", "Guatemala", false));
            passengerCard.addView(checkbox("Tengo numero de viajero frecuente (opcional)."));
            passengerCard.addView(checkbox("Necesito asistencia especial (opcional)."));
            card.addView(passengerCard);
        }

        card.addView(label("Titular de reserva"));
        EditText holderName = input("Nombre completo", value(sessionUser, "nombreCompleto", ""), false);
        EditText holderEmail = input("Email", value(sessionUser, "email", ""), false);
        EditText holderPhone = input("Telefono", "", false);
        EditText holderDocument = input("Documento", "", false);
        holderEmail.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS);
        holderPhone.setInputType(InputType.TYPE_CLASS_PHONE);
        card.addView(holderName);
        card.addView(holderEmail);
        card.addView(holderPhone);
        card.addView(holderDocument);
        CheckBox privacy = checkbox("Acepto el procesamiento de mis datos personales.");
        privacy.setChecked(true);
        card.addView(privacy);
        card.addView(checkbox("Acepto recibir promociones, ofertas y noticias."));

        card.addView(sectionTitle("Personalize your trip"));
        CheckBox seat = checkbox("Elige tu asiento - " + money(SERVICE_SEAT) + " por pasajero");
        CheckBox bag = checkbox("Equipaje adicional - " + money(SERVICE_BAG) + " por pasajero");
        CheckBox vip = checkbox("Sala VIP - " + money(SERVICE_VIP) + " por pasajero");
        CheckBox priority = checkbox("Prioridad de abordaje - " + money(SERVICE_PRIORITY) + " por pasajero");
        if (!"economica".equals(value(checkoutFlight, "selectedClass", "economica"))) {
            bag.setChecked(true);
        }
        if ("economica".equals(value(checkoutFlight, "selectedClass", "economica"))) {
            TextView warning = text("Tu tarifa Turista no incluye equipaje de bodega. Puedes agregarlo aqui o continuar sin cambios.", 13, GOLD, Typeface.BOLD);
            warning.setPadding(dp(10), dp(10), dp(10), dp(10));
            warning.setBackground(round(Color.rgb(45, 38, 18), GOLD, dp(8)));
            card.addView(warning, matchWrap());
        }
        card.addView(serviceOptionCard(seat, "Choose your seat", "Choose your favorite seat and fly at a better price than at the airport.", "From " + money(SERVICE_SEAT)));
        card.addView(serviceOptionCard(bag, "Additional baggage", "Add it now to save time at the airport.", "From " + money(SERVICE_BAG)));
        card.addView(serviceOptionCard(vip, "Travel assistance", "Medical, legal and contingency coverage at your next destination.", "From " + money(SERVICE_VIP)));
        card.addView(serviceOptionCard(priority, "Priority boarding", "Skip the lines and be among the first to board the plane.", "From " + money(SERVICE_PRIORITY)));

        card.addView(sectionTitle("Select your payment method"));
        LinearLayout paymentCard = compactCard();
        paymentCard.addView(text("● Credit and debit card", 20, TEXT, Typeface.NORMAL));
        paymentCard.addView(text("DISCOVER   VISA   Mastercard   AMEX", 13, MUTED, Typeface.BOLD));
        paymentCard.addView(checkbox("Split the payment of your flights into two different cards"));
        paymentCard.addView(text("Card information", 18, TEXT, Typeface.BOLD));
        Spinner method = spinner(new String[]{"1 - Tarjeta de credito", "2 - Tarjeta de debito", "3 - Transferencia"}, "1 - Tarjeta de credito");
        paymentCard.addView(method);
        EditText cardName = input("Nombre en tarjeta", value(sessionUser, "nombreCompleto", ""), false);
        EditText cardNumber = input("Numero de tarjeta", "", false);
        EditText cardExpiration = input("Vencimiento MM/AA", "", false);
        EditText cardCvv = input("CVV", "", false);
        cardNumber.setInputType(InputType.TYPE_CLASS_NUMBER);
        cardCvv.setInputType(InputType.TYPE_CLASS_NUMBER);
        paymentCard.addView(cardName);
        paymentCard.addView(cardNumber);
        LinearLayout cardRow = row();
        cardRow.addView(cardExpiration, weightedButtonParams());
        cardRow.addView(cardCvv, weightedButtonParams());
        paymentCard.addView(cardRow);
        card.addView(paymentCard);

        LinearLayout purchaseSummary = compactCard();
        purchaseSummary.addView(text("Resumen de compra", 16, TEXT, Typeface.BOLD));
        purchaseSummary.addView(text("Vuelo: " + money(fare(value(checkoutFlight, "selectedClass", "economica"), passengerCount)), 13, MUTED, Typeface.NORMAL));
        purchaseSummary.addView(text("Extras se calculan al confirmar segun lo seleccionado.", 13, MUTED, Typeface.NORMAL));
        purchaseSummary.addView(text("Impuestos: 12%", 13, MUTED, Typeface.NORMAL));
        card.addView(purchaseSummary);
        card.addView(primaryButton("Confirmar compra", v -> confirmPurchase(
                classSpinner,
                passengerNames,
                passengerDocs,
                passengerAges,
                holderName,
                holderEmail,
                holderPhone,
                holderDocument,
                seat,
                bag,
                vip,
                priority,
                method,
                cardName,
                cardNumber,
                cardExpiration,
                cardCvv)), matchWrap());
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
                syncCartFromServer(true);
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
        criteriaDestination = name;
        criteriaOrigin = criteriaOrigin.isEmpty() ? "La Aurora" : criteriaOrigin;
        render();
        showTravelResults(criteriaOrigin, criteriaDestination, criteriaDepartureDate, passengerCountFromCriteria());
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
            EditText[] passengerAges,
            EditText holderName,
            EditText holderEmail,
            EditText holderPhone,
            EditText holderDocument,
            CheckBox seat,
            CheckBox bag,
            CheckBox vip,
            CheckBox priority,
            Spinner method,
            EditText cardName,
            EditText cardNumber,
            EditText cardExpiration,
            EditText cardCvv) {
        if (checkoutFlight == null || sessionUser == null) {
            return;
        }
        String className = classSpinner.getSelectedItem().toString();
        int passengers = parseInt(value(checkoutFlight, "passengerCount", "1"), 1);
        String validationError = checkoutValidationError(passengerNames, passengerDocs, passengerAges, holderName, holderEmail, holderPhone, method, cardName, cardNumber, cardExpiration, cardCvv);
        if (!validationError.isEmpty()) {
            statusText.setText(validationError);
            return;
        }

        double servicesTotal = 0;
        servicesTotal += seat.isChecked() ? SERVICE_SEAT * passengers : 0;
        servicesTotal += bag.isChecked() ? SERVICE_BAG * passengers : 0;
        servicesTotal += vip.isChecked() ? SERVICE_VIP * passengers : 0;
        servicesTotal += priority.isChecked() ? SERVICE_PRIORITY * passengers : 0;
        double fare = fare(className, passengers) + servicesTotal;
        int methodId = parseInt(method.getSelectedItem().toString().substring(0, 1), 1);
        try {
            String checkoutCartId = value(checkoutFlight, "cartId", "");
            String checkoutItemId = value(checkoutFlight, "itemCarritoId", "");
            JSONObject body = new JSONObject()
                    .put("usuarioId", parseInt(value(sessionUser, "usuarioId", "0"), 0))
                    .put("pasajeroId", parseInt(value(sessionUser, "pasajeroId", "0"), 0))
                    .put("vueloId", parseInt(value(checkoutFlight, "id", "0"), 0))
                    .put("clase", className)
                    .put("equipajeFacturado", bag.isChecked() ? 1 : 0)
                    .put("pesoEquipaje", bag.isChecked() ? 23 : JSONObject.NULL)
                    .put("tarifaPagada", fare)
                    .put("metodoPagoId", methodId);
            runTask("Confirmando compra...", () -> {
                String response = apiClient.post("/api/compras/vuelos", body.toString());
                if (!checkoutItemId.isEmpty()) {
                    apiClient.delete("/api/carritos-compra/pasajero/" + passengerId() + "/items/" + encode(checkoutItemId));
                }
                return response;
            }, json -> {
                JSONObject response = new JSONObject(json);
                removeLocalCartItem(checkoutCartId);
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
                .put("precioUnitario", fare(selectedClass, passengers))
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
        view.setBackground(round(Color.WHITE, Color.rgb(152, 152, 152), dp(12)));
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
        progress.setPadding(0, 0, 0, dp(10));
        String[] steps = {"1", "2", "3"};
        for (int i = 0; i < steps.length; i++) {
            boolean done = i + 1 < activeStep;
            boolean active = i + 1 == activeStep;
            TextView step = text(done ? "✓" : steps[i], 16, done || active ? Color.WHITE : MUTED, Typeface.BOLD);
            step.setGravity(Gravity.CENTER);
            step.setPadding(dp(8), dp(8), dp(8), dp(8));
            step.setMinHeight(dp(46));
            step.setBackground(round(done ? Color.rgb(120, 120, 120) : active ? (activeStep == 3 ? RED : GREEN) : Color.rgb(238, 238, 238), active ? RED : LINE, dp(28)));
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
        editText.setBackground(round(Color.WHITE, Color.rgb(152, 152, 152), dp(12)));
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

    private CheckBox checkbox(String label) {
        CheckBox checkBox = new CheckBox(this);
        checkBox.setText(label);
        checkBox.setTextColor(TEXT);
        checkBox.setTextSize(13);
        checkBox.setButtonTintList(android.content.res.ColorStateList.valueOf(TEAL));
        checkBox.setPadding(dp(4), dp(6), dp(4), dp(6));
        return checkBox;
    }

    private View serviceOptionCard(CheckBox checkBox, String title, String description, String price) {
        LinearLayout option = compactCard();
        TextView illustration = text("▰▰▰", 34, TEAL, Typeface.BOLD);
        illustration.setGravity(Gravity.CENTER);
        illustration.setPadding(0, dp(18), 0, dp(18));
        illustration.setBackground(round(PANEL_LIGHT, PANEL_LIGHT, dp(18)));
        option.addView(illustration, matchWrap());
        option.addView(text(title, 24, TEXT, Typeface.BOLD));
        option.addView(text(description, 15, MUTED, Typeface.NORMAL));
        option.addView(text(price, 18, TEXT, Typeface.BOLD));
        option.addView(checkBox);
        return option;
    }

    private Button primaryButton(String label, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(Color.WHITE);
        button.setBackground(round(DEEP, DEEP, dp(30)));
        return button;
    }

    private Button outlineButton(String label, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(DEEP);
        button.setBackground(round(Color.TRANSPARENT, Color.rgb(165, 165, 165), dp(14)));
        return button;
    }

    private Button navButton(String label, boolean active, View.OnClickListener listener) {
        Button button = baseButton(label, listener);
        button.setTextColor(active ? Color.WHITE : TEXT);
        button.setBackground(round(active ? DEEP : Color.WHITE, active ? DEEP : LINE, dp(18)));
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

    private int passengerId() {
        return parseInt(value(sessionUser, "pasajeroId", "0"), 0);
    }

    private int passengerCountFromCriteria() {
        return Math.max(1, criteriaAdults + criteriaYouth + criteriaChildren + criteriaBabies);
    }

    private String defaultPassengerAge(int index) {
        if (index < criteriaAdults) {
            return "30";
        }
        if (index < criteriaAdults + criteriaYouth) {
            return "13";
        }
        if (index < criteriaAdults + criteriaYouth + criteriaChildren) {
            return "8";
        }
        return "1";
    }

    private String checkoutValidationError(
            EditText[] passengerNames,
            EditText[] passengerDocs,
            EditText[] passengerAges,
            EditText holderName,
            EditText holderEmail,
            EditText holderPhone,
            Spinner method,
            EditText cardName,
            EditText cardNumber,
            EditText cardExpiration,
            EditText cardCvv) {
        for (int i = 0; i < passengerNames.length; i++) {
            if (isBlank(passengerNames[i]) || isBlank(passengerDocs[i]) || parseInt(passengerAges[i].getText().toString(), -1) < 0) {
                markInvalid(passengerNames[i]);
                markInvalid(passengerDocs[i]);
                markInvalid(passengerAges[i]);
                return "Completa nombre, documento y edad de todos los pasajeros.";
            }
        }

        if (isBlank(holderName) || isBlank(holderEmail) || isBlank(holderPhone)) {
            markInvalid(holderName);
            markInvalid(holderEmail);
            markInvalid(holderPhone);
            return "Completa los datos del titular de la reserva.";
        }

        String email = holderEmail.getText().toString().trim();
        if (!email.contains("@") || !email.contains(".")) {
            markInvalid(holderEmail);
            return "Ingresa un email valido para el titular.";
        }

        String phone = holderPhone.getText().toString().replaceAll("\\D", "");
        if (phone.length() < 8) {
            markInvalid(holderPhone);
            return "Ingresa un telefono valido para el titular.";
        }

        boolean requiresCard = !method.getSelectedItem().toString().startsWith("3");
        if (requiresCard) {
            if (isBlank(cardName) || cardNumber.getText().toString().replaceAll("\\s", "").length() < 13) {
                markInvalid(cardName);
                markInvalid(cardNumber);
                return "Completa correctamente los datos de tarjeta.";
            }
            if (!cardExpiration.getText().toString().trim().matches("\\d{2}/\\d{2}") || cardCvv.getText().toString().trim().length() < 3) {
                markInvalid(cardExpiration);
                markInvalid(cardCvv);
                return "Usa vencimiento MM/AA y CVV valido.";
            }
        }

        return "";
    }

    private boolean isBlank(EditText input) {
        return input == null || input.getText().toString().trim().isEmpty();
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

    private String dateInputValue(java.util.Calendar calendar) {
        return new java.text.SimpleDateFormat("yyyy-MM-dd", Locale.US).format(calendar.getTime());
    }

    private double fareForDate(java.util.Calendar calendar) {
        int seed = calendar.get(java.util.Calendar.YEAR) + (calendar.get(java.util.Calendar.MONTH) + 1) * 19 + calendar.get(java.util.Calendar.DAY_OF_MONTH) * 37;
        return 710 + (seed % 58) * 10;
    }

    private String priceLegend(java.util.Calendar calendar) {
        int seed = calendar.get(java.util.Calendar.YEAR) + (calendar.get(java.util.Calendar.MONTH) + 1) * 19 + calendar.get(java.util.Calendar.DAY_OF_MONTH) * 37;
        int bucket = seed % 3;
        return bucket == 0 ? "$" : bucket == 1 ? "$$" : "$$$";
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

    private interface CounterSetter {
        void set(int value);
    }

    private interface RowFormatter {
        String[] lines(JSONObject item);
    }
}
