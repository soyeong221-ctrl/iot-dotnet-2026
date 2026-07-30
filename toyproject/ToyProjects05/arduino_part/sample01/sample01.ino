int motorSpeedPin = 10;
int motorDirectionPin = 12;
int value;

void setup() {  
  Serial.begin(9600);
  noTone(4);
  pinMode(motorDirectionPin, OUTPUT);
  digitalWrite(motorDirectionPin, HIGH);
  value = 80;
  analogWrite(motorSpeedPin, value);
}

void loop() {
  if (Serial.available()) {
    value = Serial.parseInt();
    if (value >= 255) {
      value = 255;
    } else if (value <= 0) {
      value = 0;
    }

    Serial.println(value);
    analogWrite(motorSpeedPin, value);
  }  
}
