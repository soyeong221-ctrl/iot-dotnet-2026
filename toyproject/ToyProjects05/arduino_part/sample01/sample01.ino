// Color Sensor
#include <Wire.h>
#include <Adafruit_TCS34725.h>

Adafruit_TCS34725 TCS = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_50MS, TCS34725_GAIN_4X);

void setup() {
  Serial.begin(19200);
  TCS.begin();  
}

void loop() {
  uint16_t clear, red, green, blue;
  delay(100);
  TCS.getRawData(&red, &green, &blue, &clear);

  int r = map(red, 0, 21504, 0, 2000);
  int g = map(green, 0, 21504, 0, 2000);
  int b = map(blue, 0, 21504, 0, 2000);

  Serial.print("    R: ");
  Serial.print(r);
  Serial.print("    G: ");
  Serial.print(g);
  Serial.print("    B: ");
  Serial.println(b);
}