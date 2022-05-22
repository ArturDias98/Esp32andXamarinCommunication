#include <Arduino.h>
#include <BluetoothSerial.h>

BluetoothSerial serialBT;
uint8_t dataReceived;
unsigned long plotTime;
const int analogPin = 34;
int analogData = 0;

void SendData_Bluetooth();
void SendData_Serial();
void setup()
{
  // put your setup code here, to run once:
  Serial.begin(115200);
  serialBT.begin("ESP32");
  Serial.println("Started");
  dataReceived = 0;
  plotTime = millis();
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
  uint8_t data1 = analogData & 0xFF;//lsb
  uint8_t data2 = (analogData >> 8) & 0xFF;//msb

  serialBT.write(0xAB);//Protocol header
  serialBT.write(0xCD);// Protocol header
  serialBT.write(data1);//Data lsb
  serialBT.write(data2);//Data msb
  serialBT.write(0xAF);//Protocol tail
  serialBT.write(0xCF);//Protocol tail
}

void SendData_Serial()
{
  //Send data over serial with one second interval.
  //Just for debug.
  if (millis() - plotTime >= 1000)
  {
    plotTime = millis();
    Serial.println(analogData);
  }
}