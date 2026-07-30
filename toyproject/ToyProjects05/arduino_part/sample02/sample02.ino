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
  char color;
  delay(100);
  TCS.getRawData(&red, &green, &blue, &clear);

  int r = map(red, 0, 21504, 0, 2000);
  int g = map(green, 0, 21504, 0, 2000);
  int b = map(blue, 0, 21504, 0, 2000);

  // Serial.print("    R: ");
  // Serial.print(r);
  // Serial.print("    G: ");
  // Serial.print(g);
  // Serial.print("    B: ");
  // Serial.println(b);

  color = getColor(r, g, b);

   Serial.println(color);
}


char getColor(int r, int g, int b) {
  if (r < 5 && g < 5 && b < 5) {
    return 'N';
  } else if (r > g && r > b) {
    return 'R';
  } else if (g > r && g > b) {
    return 'G';
  } else if (b > r && b > g) { 
    return 'B';
  }
}