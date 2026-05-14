package com.aeropuertoaurora.android;

import android.content.Context;
import android.content.SharedPreferences;

final class SettingsStore {
    private static final String FILE_NAME = "aeropuerto_aurora_settings";
    private static final String KEY_BASE_URL = "base_url";
    private static final String KEY_API_KEY = "api_key";
    private static final String KEY_SESSION = "session_json";
    private static final String KEY_CART = "cart_json";
    private static final String DEFAULT_BASE_URL = "http://192.168.0.3:5185";

    private final SharedPreferences preferences;

    SettingsStore(Context context) {
        preferences = context.getSharedPreferences(FILE_NAME, Context.MODE_PRIVATE);
    }

    String getBaseUrl() {
        return preferences.getString(KEY_BASE_URL, DEFAULT_BASE_URL);
    }

    String getApiKey() {
        return preferences.getString(KEY_API_KEY, "");
    }

    void save(String baseUrl, String apiKey) {
        preferences.edit()
                .putString(KEY_BASE_URL, baseUrl == null ? DEFAULT_BASE_URL : baseUrl.trim())
                .putString(KEY_API_KEY, apiKey == null ? "" : apiKey.trim())
                .apply();
    }

    String getSessionJson() {
        return preferences.getString(KEY_SESSION, "");
    }

    void saveSessionJson(String sessionJson) {
        preferences.edit().putString(KEY_SESSION, sessionJson == null ? "" : sessionJson).apply();
    }

    void clearSession() {
        preferences.edit().remove(KEY_SESSION).apply();
    }

    String getCartJson() {
        return preferences.getString(KEY_CART, "[]");
    }

    void saveCartJson(String cartJson) {
        preferences.edit().putString(KEY_CART, cartJson == null ? "[]" : cartJson).apply();
    }
}
