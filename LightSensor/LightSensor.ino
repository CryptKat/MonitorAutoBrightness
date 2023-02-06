#include <DigiUSB.h>
#define NUMSAMPLES 16

void setup() {  
  pinMode(2, INPUT_PULLUP);
  DigiUSB.begin();
}

void loop() {
  DigiUSB.refresh();
  DigiUSB.println(readSampledPinValue(1), DEC); // analog 1 is P2
  DigiUSB.delay(200);
}

uint32_t readSampledPinValue(uint8_t pin)
{
    uint8_t i;
    uint32_t v = 0;
    uint32_t samples[NUMSAMPLES];
    for (i = 0; i < NUMSAMPLES; i++)
    {
        samples[i] = analogRead(pin);
        delay(1);
    }
    for (i = 0; i < NUMSAMPLES; i++)
        v += samples[i];
    v /= NUMSAMPLES;
    return v;
}
