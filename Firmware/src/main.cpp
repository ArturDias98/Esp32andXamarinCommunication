#include <Arduino.h>
#include <BluetoothSerial.h>

#define SERIAL_TIME_INTERVAL 1000 //miliseconds
#define BT_TIME_INTERVAL 50 //miliseconds

BluetoothSerial serialBT;
uint8_t dataReceived;
unsigned long serialSendInterval;
unsigned long btSendInterval;
const int analogPin = 34;
int analogData = 0;

void SendData_Bluetooth();
void SendData_Serial();
void setup()
{
  // put your setup code here, to run once:
  Serial.begin(9600);
  serialBT.begin("ESP32");
  Serial.println("Started");
  dataReceived = 0;
  serialSendInterval = millis();
  btSendInterval = millis();
}

void loop()
{
  // put your main code here, to run repeatedly:

  analogData = analogRead(analogPin);

  SendData_Bluetooth();
  SendData_Serial();
}

void SendData_Bluetooth()
{
  // Sending 16 bits of data over bluetooth.
  uint8_t data1 = analogData & 0xFF;        // lsb
  uint8_t data2 = (analogData >> 8) & 0xFF; // msb

  if (millis() - btSendInterval >= BT_TIME_INTERVAL)
  {
    serialBT.write(0xAB);  // Protocol header
    serialBT.write(0xCD);  // Protocol header
    serialBT.write(data1); // Data lsb
    serialBT.write(data2); // Data msb
    serialBT.write(0xAF);  // Protocol tail
    serialBT.write(0xCF);  // Protocol tail

    btSendInterval = millis();
  }
}

void SendData_Serial()
{
  // Send data over serial with one second interval.
  // Just for debug.
  if (millis() - serialSendInterval >= SERIAL_TIME_INTERVAL)
  {
    serialSendInterval = millis();
    Serial.println(analogData);
  }
}
