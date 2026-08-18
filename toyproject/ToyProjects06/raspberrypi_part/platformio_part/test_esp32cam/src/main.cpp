#include <Arduino.h>
#include <WiFi.h>
#include "esp_camera.h"
#include "esp_http_server.h"

// ============================================================
// WiFi 설정
// ============================================================
const char* ssid = "pknu2G";
const char* password = "iotiot1234";

// ============================================================
// AI Thinker ESP32-CAM PIN 설정
// ============================================================
#define PWDN_GPIO_NUM     32
#define RESET_GPIO_NUM    -1
#define XCLK_GPIO_NUM      0
#define SIOD_GPIO_NUM     26
#define SIOC_GPIO_NUM     27

#define Y9_GPIO_NUM       35
#define Y8_GPIO_NUM       34
#define Y7_GPIO_NUM       39
#define Y6_GPIO_NUM       36
#define Y5_GPIO_NUM       21
#define Y4_GPIO_NUM       19
#define Y3_GPIO_NUM       18
#define Y2_GPIO_NUM        5

#define VSYNC_GPIO_NUM    25
#define HREF_GPIO_NUM     23
#define PCLK_GPIO_NUM     22


// ============================================================
// HTTP 서버
// ============================================================
httpd_handle_t camera_httpd = NULL;


// ============================================================
// 메인 페이지 // 브라우저에서 실행되는 html 화면
// ============================================================
static esp_err_t index_handler(httpd_req_t *req)
{
    const char html[] = R"rawliteral(
<!DOCTYPE html>
<html>

<head>
    <meta charset="UTF-8">
    <title>ESP32-CAM</title>
    <style>
        body {
            background-color: #202020;
            color: white;
            text-align: center;
            font-family: Arial;
        }

        h1 {
            margin-top: 30px;
        }

        img {
            width: 90%;
            max-width: 800px;

            border: 3px solid white;
            border-radius: 10px;
        }
    </style>
</head>

<body>
    <h1>ESP32-CAM Streaming</h1>
    <img src="/stream">
</body>
</html>
)rawliteral";

    httpd_resp_set_type(req, "text/html");

    return httpd_resp_send(req, html, HTTPD_RESP_USE_STRLEN);
}


// ============================================================
// MJPEG Streaming
// ============================================================
static esp_err_t stream_handler(httpd_req_t *req)
{
    camera_fb_t *fb = NULL;
    esp_err_t res = ESP_OK;

    char part_buf[64];

    static const char* STREAM_CONTENT_TYPE =
        "multipart/x-mixed-replace;boundary=frame";

    static const char* STREAM_BOUNDARY =
        "\r\n--frame\r\n";

    static const char* STREAM_PART =
        "Content-Type: image/jpeg\r\n"
        "Content-Length: %u\r\n\r\n";

    res = httpd_resp_set_type(
        req,
        STREAM_CONTENT_TYPE
    );

    if (res != ESP_OK)
        return res;

    while (true)
    {
        // 카메라 프레임 획득
        fb = esp_camera_fb_get();

        if (!fb)
        {
            Serial.println("Camera capture failed");
            res = ESP_FAIL;
            break;
        }


        // boundary 전송
        res = httpd_resp_send_chunk(
            req,
            STREAM_BOUNDARY,
            strlen(STREAM_BOUNDARY)
        );


        if (res == ESP_OK)
        {
            size_t header_len =
                snprintf(
                    part_buf,
                    sizeof(part_buf),
                    STREAM_PART,
                    fb->len
                );

            res = httpd_resp_send_chunk(
                req,
                part_buf,
                header_len
            );
        }


        // JPEG 이미지 전송
        if (res == ESP_OK)
        {
            res = httpd_resp_send_chunk(
                req,
                (const char*)fb->buf,
                fb->len
            );
        }


        // 프레임 버퍼 반환
        esp_camera_fb_return(fb);
        fb = NULL;

        if (res != ESP_OK)
        {
            break;
        }
    }

    return res;
}


// ============================================================
// Web Server 시작
// ============================================================
void startCameraServer()
{
    httpd_config_t config =
        HTTPD_DEFAULT_CONFIG();

    config.server_port = 80;

    // --------------------------------------------------------
    // 메인 페이지
    // --------------------------------------------------------
    httpd_uri_t index_uri = {
        .uri       = "/",
        .method    = HTTP_GET,
        .handler   = index_handler,
        .user_ctx  = NULL
    };

    // --------------------------------------------------------
    // Stream
    // --------------------------------------------------------
    httpd_uri_t stream_uri = {
        .uri       = "/stream",
        .method    = HTTP_GET,
        .handler   = stream_handler,
        .user_ctx  = NULL
    };

    Serial.println("Starting Web Server...");

    if (httpd_start(
            &camera_httpd,
            &config) == ESP_OK)
    {
        httpd_register_uri_handler(
            camera_httpd,
            &index_uri
        );

        httpd_register_uri_handler(
            camera_httpd,
            &stream_uri
        );
    }
}

// ============================================================
// Camera 초기화
// ============================================================
bool initCamera()
{
    camera_config_t config;

    config.ledc_channel = LEDC_CHANNEL_0;
    config.ledc_timer   = LEDC_TIMER_0;

    config.pin_d0 = Y2_GPIO_NUM;
    config.pin_d1 = Y3_GPIO_NUM;
    config.pin_d2 = Y4_GPIO_NUM;
    config.pin_d3 = Y5_GPIO_NUM;
    config.pin_d4 = Y6_GPIO_NUM;
    config.pin_d5 = Y7_GPIO_NUM;
    config.pin_d6 = Y8_GPIO_NUM;
    config.pin_d7 = Y9_GPIO_NUM;

    config.pin_xclk  = XCLK_GPIO_NUM;
    config.pin_pclk  = PCLK_GPIO_NUM;

    config.pin_vsync = VSYNC_GPIO_NUM;
    config.pin_href  = HREF_GPIO_NUM;

    config.pin_sccb_sda = SIOD_GPIO_NUM;
    config.pin_sccb_scl = SIOC_GPIO_NUM;

    config.pin_pwdn  = PWDN_GPIO_NUM;
    config.pin_reset = RESET_GPIO_NUM;

    config.xclk_freq_hz = 20000000;

    config.pixel_format = PIXFORMAT_JPEG;

    // --------------------------------------------------------
    // PSRAM 확인
    // --------------------------------------------------------
    if (psramFound())
    {
        Serial.println("PSRAM Found");
        config.frame_size = FRAMESIZE_VGA;
        config.jpeg_quality = 10;
        config.fb_count = 2;
    }
    else
    {
        Serial.println("PSRAM Not Found");
        config.frame_size = FRAMESIZE_QVGA;
        config.jpeg_quality = 12;
        config.fb_count = 1;
    }

    // --------------------------------------------------------
    // Camera 초기화
    // --------------------------------------------------------
    esp_err_t err =
        esp_camera_init(&config);

    if (err != ESP_OK)
    {
        Serial.printf(
            "Camera init failed: 0x%x\n",
            err
        );

        return false;
    }

    return true;
}


// ============================================================
// setup
// ============================================================
void setup()
{
    Serial.begin(115200);
    delay(1000);

    Serial.println();
    Serial.println("==========================");
    Serial.println("ESP32-CAM Starting...");
    Serial.println("==========================");

    // --------------------------------------------------------
    // Camera
    // --------------------------------------------------------
    if (!initCamera())
    {
        Serial.println("Camera initialization failed");
        return;
    }

    Serial.println("Camera OK");

    // --------------------------------------------------------
    // WiFi
    // --------------------------------------------------------
    WiFi.mode(WIFI_STA);

    WiFi.begin(
        ssid,
        password
    );

    Serial.print("Connecting WiFi");


    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);

        Serial.print(".");
    }

    Serial.println();
    Serial.println("WiFi connected");

    // --------------------------------------------------------
    // 서버 시작
    // --------------------------------------------------------
    startCameraServer();

    Serial.println();
    Serial.println("==========================");

    Serial.print("Camera URL : http://");

    Serial.println(
        WiFi.localIP()
    );

    Serial.println("==========================");
}

// ============================================================
// loop
// ============================================================
void loop()
{
    delay(1000);
}