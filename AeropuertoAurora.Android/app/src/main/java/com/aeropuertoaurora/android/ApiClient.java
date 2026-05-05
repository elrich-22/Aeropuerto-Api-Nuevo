package com.aeropuertoaurora.android;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.StandardCharsets;

final class ApiClient {
    private final String baseUrl;
    private final String apiKey;

    ApiClient(String baseUrl, String apiKey) {
        String cleanBaseUrl = baseUrl == null ? "" : baseUrl.trim();
        while (cleanBaseUrl.endsWith("/")) {
            cleanBaseUrl = cleanBaseUrl.substring(0, cleanBaseUrl.length() - 1);
        }

        this.baseUrl = cleanBaseUrl;
        this.apiKey = apiKey == null ? "" : apiKey.trim();
    }

    String get(String path) throws IOException {
        return request("GET", path, null);
    }

    String post(String path, String body) throws IOException {
        return request("POST", path, body);
    }

    String put(String path, String body) throws IOException {
        return request("PUT", path, body);
    }

    String delete(String path) throws IOException {
        return request("DELETE", path, null);
    }

    private String request(String method, String path, String body) throws IOException {
        HttpURLConnection connection = null;
        try {
            URL url = new URL(baseUrl + path);
            connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod(method);
            connection.setConnectTimeout(10000);
            connection.setReadTimeout(15000);
            connection.setRequestProperty("Accept", "application/json");

            if (!apiKey.isEmpty()) {
                connection.setRequestProperty("X-Api-Key", apiKey);
            }

            if (body != null) {
                byte[] bytes = body.getBytes(StandardCharsets.UTF_8);
                connection.setDoOutput(true);
                connection.setRequestProperty("Content-Type", "application/json; charset=utf-8");
                connection.setFixedLengthStreamingMode(bytes.length);
                try (OutputStream output = connection.getOutputStream()) {
                    output.write(bytes);
                }
            }

            int code = connection.getResponseCode();
            InputStream stream = code >= 200 && code < 300
                    ? connection.getInputStream()
                    : connection.getErrorStream();
            String response = readAll(stream);

            if (code < 200 || code >= 300) {
                throw new IOException("Error " + code + ": " + response);
            }

            return response;
        } finally {
            if (connection != null) {
                connection.disconnect();
            }
        }
    }

    private static String readAll(InputStream stream) throws IOException {
        if (stream == null) {
            return "";
        }

        StringBuilder builder = new StringBuilder();
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(stream, StandardCharsets.UTF_8))) {
            String line;
            while ((line = reader.readLine()) != null) {
                builder.append(line);
            }
        }

        return builder.toString();
    }
}
