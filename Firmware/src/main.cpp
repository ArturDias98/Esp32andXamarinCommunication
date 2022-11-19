#include <Arduino.h>
#include <BluetoothSerial.h>

#define SERIAL_TIME_INTERVAL 1000 // miliseconds
#define BT_TIME_INTERVAL 100        // miliseconds
#define BT_NUM_PACKAGES 200

BluetoothSerial serialBT;
uint8_t dataReceived;
unsigned long serialSendInterval;
unsigned long btSendInterval;
const int analogPin = 34;
int analogData = 0;
uint32_t package = 0;
bool btCongested = false;

void SendData_Bluetooth();
void SendData_Serial();
void callback(esp_spp_cb_event_t event, esp_spp_cb_param_t *param);

void setup()
{
  // put your setup code here, to run once:
  Serial.begin(9600);
  serialBT.begin("ESP32");
  serialBT.register_callback(callback);
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
  if (!serialBT.hasClient())
  {
    Serial.println("Not connected");
    serialBT.flush();
    delay(1000);
    return;
  }

  while (btCongested)
  {
    serialBT.flush();
  }
  

  if (millis() - btSendInterval >= BT_TIME_INTERVAL)
  {
    uint8_t data[6];

    data[0] = 0xAB;
    data[1] = 0xCD;
    data[2] = analogData & 0xFF;;
    data[3] = (analogData >> 8) & 0xFF;
    data[4] = (analogData >> 16) & 0xFF;
    data[5] = (analogData >> 24) & 0xFF;
    data[6] = 0xAF;
    data[7] = 0xCF;

    serialBT.write(data, sizeof(data));
    //package++;

    /*if (package >= BT_NUM_PACKAGES)
    {
      // Prevent congested.
      serialBT.flush();
      package = 0;
    }*/

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
void callback(esp_spp_cb_event_t event, esp_spp_cb_param_t *param)
{
  if (event == ESP_SPP_CONG_EVT)
  {
    if (param->cong.cong)
    {
      btCongested = true;
      
    }
    else
    {
      btCongested = false;
    }
  }
}