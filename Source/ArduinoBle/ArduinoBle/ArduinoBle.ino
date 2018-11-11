/* Copyright 2018 Ellisnet - Jeremy Ellis (jeremy@ellisnet.com)
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

#include <Adafruit_NeoPixel.h>
#include <SPI.h>
#include <Wire.h>
#include <Adafruit_BLE.h>
#include <Adafruit_BluefruitLE_UART.h>
#include <Adafruit_BluefruitLE_SPI.h>
//#include <Adafruit_BLEMIDI.h>
#include <Adafruit_BLEGatt.h>
//#include <Adafruit_BLEEddystone.h>
//#include <Adafruit_BLEBattery.h>
#include <Adafruit_ATParser.h>

//BEGIN Adafruit BLE settings

#define FACTORYRESET_ENABLE         1
#define MINIMUM_FIRMWARE_VERSION    "0.6.6"
#define MODE_LED_BEHAVIOUR          "MODE"

// ----------------------------------------------------------------------------------------------
// These settings are used in both SW UART, HW UART and SPI mode
// ----------------------------------------------------------------------------------------------
#define BUFSIZE                        128   // Size of the read buffer for incoming data
#define VERBOSE_MODE                   true  // If set to 'true' enables debug output

// SHARED SPI SETTINGS
#define BLUEFRUIT_SPI_CS               8
#define BLUEFRUIT_SPI_IRQ              7
#define BLUEFRUIT_SPI_RST              4    // Optional but recommended, set to -1 if unused

// SOFTWARE SPI SETTINGS
#define BLUEFRUIT_SPI_SCK              13
#define BLUEFRUIT_SPI_MISO             12
#define BLUEFRUIT_SPI_MOSI             11

//END Adafruit BLE settings

#define JOYSTICK 0x20
#define KEYPAD 0x06
#define GPIO_BOARD 0x27

#define NEOPIXEL_PIN 6
#define NEOPIXEL_LED_COUNT 2

const int ONBOARD_LED = 13;

const int GREEN_BUTTON = 2;
const int BLUE_BUTTON = 3;
const int RED_BUTTON = 5;
const int YELLOW_BUTTON = 6;

const int POWER_BUTTON = 10;

const byte KEY_STAR = 0x2A;
const byte KEY_HASH = 0x23;
const byte KEY_0 = 0x30;
const byte KEY_1 = 0x31;
const byte KEY_2 = 0x32;
const byte KEY_3 = 0x33;
const byte KEY_4 = 0x34;
const byte KEY_5 = 0x35;
const byte KEY_6 = 0x36;
const byte KEY_7 = 0x37;
const byte KEY_8 = 0x38;
const byte KEY_9 = 0x39;

const byte JOY_CENTER = 0x01;
const byte JOY_N = 0x02;
const byte JOY_NE = 0x03;
const byte JOY_E = 0x04;
const byte JOY_SE = 0x05;
const byte JOY_S = 0x06;
const byte JOY_SW = 0x07;
const byte JOY_W = 0x08;
const byte JOY_NW = 0x09;

const byte JOY_CENTER_H = 0x01;
const byte JOY_LEFT = 0x02;
const byte JOY_RIGHT = 0x03;

const byte JOY_CENTER_V = 0x01;
const byte JOY_UP = 0x02;
const byte JOY_DOWN = 0x03;

const byte JOY_BUTTON_DOWN = 0x00;

const int CYCLE_MILLISECONDS = 40;

int green_button_state = LOW;
int blue_button_state = LOW;
int red_button_state = LOW;
int yellow_button_state = LOW;

int prev_green_button_state = LOW;
int prev_blue_button_state = LOW;
int prev_red_button_state = LOW;
int prev_yellow_button_state = LOW;

int power_button_state = LOW;
int menu_button_state = LOW;
int up_button_state = LOW;
int left_button_state = LOW;
int enter_button_state = LOW;
int right_button_state = LOW;
int down_button_state = LOW;
int shift_button_state = LOW;
int backspace_button_state = LOW;

int prev_power_button_state = LOW;
int prev_menu_button_state = LOW;
int prev_up_button_state = LOW;
int prev_left_button_state = LOW;
int prev_enter_button_state = LOW;
int prev_right_button_state = LOW;
int prev_down_button_state = LOW;
int prev_shift_button_state = LOW;
int prev_backspace_button_state = LOW;

byte prev_joystick_direction = JOY_CENTER;
boolean prev_joystick_button_state = false;

boolean scan_complete = false;

boolean keypad_found = false;
boolean joystick_found = false;
boolean gpio_board_found = false;

//Setup for NeoPixels
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NEOPIXEL_LED_COUNT, NEOPIXEL_PIN, NEO_GRB + NEO_KHZ800);

// A small helper
void error(const __FlashStringHelper*err) {
	Serial.println(err);
	while (1);
}

//BEGIN Joystick functions

int reading_joystick = 0;

// getJoystickHorizontal() returns an int between 0 and 1023
// representing the Joystiic's horizontal position
// (axis indicated by silkscreen on the board)
// centered roughly on 512
int getJoystickHorizontal() {
	Wire.beginTransmission(JOYSTICK);
	Wire.write(0x00);
	Wire.endTransmission(false);
	Wire.requestFrom(JOYSTICK, 2);

	while (Wire.available()) {
		uint8_t msb = Wire.read();
		uint8_t lsb = Wire.read();
		reading_joystick = (uint16_t)msb << 8 | lsb;
	}

	Wire.endTransmission();
	return reading_joystick;
}

// getJoystickVertical() returns an int between 0 and 1023
// representing the Joystiic's vertical position
// (axis indicated by silkscreen on the board)
// centered roughly on 512
int getJoystickVertical() {
	Wire.beginTransmission(JOYSTICK);
	Wire.write(0x02);
	Wire.endTransmission(false);
	Wire.requestFrom(JOYSTICK, 2);

	while (Wire.available()) {
		uint8_t msb = Wire.read();
		uint8_t lsb = Wire.read();
		reading_joystick = (uint16_t)msb << 8 | lsb;
	}

	Wire.endTransmission();
	return reading_joystick;
}

// getJoystickButton() returns a byte indicating the
// position of the button where a 0b1 is not
// pressed and 0b0 is pressed
byte getJoystickButton() {
	byte btnbyte = 0b00000000;

	Wire.beginTransmission(JOYSTICK);
	Wire.write(0x04);
	Wire.endTransmission(false);
	Wire.requestFrom(JOYSTICK, 1);    // request 1 byte

	while (Wire.available()) {
		btnbyte = Wire.read();
	}

	Wire.endTransmission();
	return btnbyte;
}

//END Joystick functions

//BEGIN Qwiic gpio board functions

#define REGISTER_INPUT_PORT           	0x00    //register 0 
#define REGISTER_OUTPUT_PORT          	0X01    //register 1
#define REGISTER_CONFIGURATION			0X03	//register 3

/*
	readRegister(byte address, byte registerToRead) will read the register
	values of a given register at a given address.
	Inputs: address, register values
*/
byte readRegister(byte address, byte registerToRead) {
	Wire.beginTransmission(address);
	Wire.write(registerToRead);
	Wire.endTransmission(false);
	Wire.requestFrom((int)address, 1);

	byte currentRegisterValue = 0;

	byte count = 0;
	while (Wire.available() > 0) {
		if (count == 0) {
			currentRegisterValue = Wire.read();
		}
		Wire.read(); // don't collect
		count++;
	}
	return currentRegisterValue;
}

/*
	writeRegister(byte address, byte registerToWrite, byte valueToWrite) writes the
	value passed to a specific register at the Qwiic GPIO address.

	Note: when writing to a register a full 8 bits is written- overwriting any
	previous state.

	Inputs: Qwiic Gpio Address, Register to write to, and Byte to write
*/
void writeRegister(byte address, byte registerToWrite, byte valueToWrite) {
	Wire.beginTransmission(address);
	Wire.write(registerToWrite);
	Wire.write(valueToWrite);
	Wire.endTransmission();
}

/*
	setPinMode(byte address, byte pin, byte direction) sets a single pin
	as an input or an output and does not affect the other pins. The direction
	input parameter is either INPUT or OUTPUT.

		similar to pinMode()
*/
void setPinMode(byte address, byte pin, byte direction) {
	byte currentPinDirection = readRegister(address, REGISTER_CONFIGURATION);
	pin = 1 << pin;

	if (direction == INPUT || direction == INPUT_PULLUP) {
		currentPinDirection |= pin; // pin will come in correctly masked because of a define. 
	}
	else if (direction == OUTPUT) {
		currentPinDirection &= ~(pin);
	}
	writeRegister(address, REGISTER_CONFIGURATION, currentPinDirection);
}

/*
	readPin(byte address, byte pin) reads the input state of the specified pin
	and returns the corresponding pin's state.

	similar to digitalRead()
*/
byte readPin(byte address, byte pin) {
	byte currentRegisterValue = readRegister(address, REGISTER_INPUT_PORT);
	if (currentRegisterValue & (1 << pin)) {
		return HIGH;
	}
	return LOW;
}

/*
	setPinOutput(byte address, byte pin, byte state) will set a pin as high or low

			similar to digitalWrite(pin, state)
*/
void setPinOutput(byte address, byte pin, byte state) {
	pin = 1 << pin;

	byte currentRegisterValue = readRegister(address, REGISTER_OUTPUT_PORT);

	if (state == LOW) { //INVERSE OF ARDUINO, but this will result in the same effect a user would expect. 
		currentRegisterValue |= pin;
	}
	else if (state == HIGH) {
		currentRegisterValue &= ~pin;
	}
	writeRegister(address, REGISTER_OUTPUT_PORT, currentRegisterValue);
}

//END Qwiic gpio board functions

//BEGIN Adafruit BLE functions

//Initialize the Arduino BLE library
// Option 1 - hardware SPI
/* ...hardware SPI, using SCK/MOSI/MISO hardware SPI pins and then user selected CS/IRQ/RST */
//Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_CS, BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);

//Option 2 - software SPI
/* ...software SPI, using SCK/MOSI/MISO user-defined SPI pins and then user selected CS/IRQ/RST */
Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_SCK, BLUEFRUIT_SPI_MISO,
	BLUEFRUIT_SPI_MOSI, BLUEFRUIT_SPI_CS,
	BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);

/**************************************************************************/
/*!
@brief  Checks for user input (via the Serial Monitor)
*/
/**************************************************************************/
bool getUserInput(char buffer[], uint8_t maxSize)
{
	// timeout in 100 milliseconds
	TimeoutTimer timeout(100);

	memset(buffer, 0, maxSize);
	while ((!Serial.available()) && !timeout.expired()) { delay(1); }

	if (timeout.expired()) return false;

	delay(2);
	uint8_t count = 0;
	do
	{
		count += Serial.readBytes(buffer + count, maxSize);
		delay(2);
	} while ((count < maxSize) && (Serial.available()));

	return true;
}

void sendBleMsg(char inputs[]) {
	ble.print("AT+BLEUARTTX=");
	ble.println(inputs);

	// check response stastus
	if (!ble.waitForOK()) {
		Serial.println(F("Failed to send?"));
	}
}

//END Adafruit BLE functions

void setup() {
    //Initialize Wire library
	Wire.begin();

	Serial.begin(115200);
	Serial.println("Starting application...");

	//BEGIN Setup code for Adafruit BLE
	
	/* Initialise the module */
	Serial.print(F("Initialising the Bluefruit LE module: "));

	if (!ble.begin(VERBOSE_MODE))
	{
		error(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
	}
	Serial.println(F("OK!"));

	if (FACTORYRESET_ENABLE)
	{
		/* Perform a factory reset to make sure everything is in a known state */
		Serial.println(F("Performing a factory reset: "));
		if (!ble.factoryReset()) {
			error(F("Couldn't factory reset"));
		}
	}

	/* Disable command echo from Bluefruit */
	ble.echo(false);

	Serial.println("Requesting Bluefruit info:");
	/* Print Bluefruit information */
	ble.info();

	//Serial.println(F("Please use Adafruit Bluefruit LE app to connect in UART mode"));
	//Serial.println(F("Then Enter characters to send to Bluefruit"));
	//Serial.println();

	ble.verbose(false);  // debug info is a little annoying after this point!

	/* Wait for connection */
	while (!ble.isConnected()) {
		delay(500);
	}

	// LED Activity command is only supported from 0.6.6
	if (ble.isVersionAtLeast(MINIMUM_FIRMWARE_VERSION))
	{
		// Change Mode LED Activity
		Serial.println(F("******************************"));
		Serial.println(F("Change LED activity to " MODE_LED_BEHAVIOUR));
		ble.sendCommandCheckOK("AT+HWModeLED=" MODE_LED_BEHAVIOUR);
		Serial.println(F("******************************"));
	}

	//END Setup code for Adafruit BLE

	// initialize digital pin 13 as an output.
	pinMode(ONBOARD_LED, OUTPUT);

	// initialize digital pins 4-7 as inputs
	//pinMode(GREEN_BUTTON, INPUT);
	//pinMode(BLUE_BUTTON, INPUT);
	//pinMode(RED_BUTTON, INPUT);
	//Now being used for power button
	//pinMode(YELLOW_BUTTON, INPUT);
	pinMode(POWER_BUTTON, INPUT);

	pixels.begin();

	//pixels.setPixelColor(0, pixels.Color(0, 0, 0));
	//pixels.setPixelColor(1, pixels.Color(0, 0, 0));
	//pixels.show();
}

// the loop function runs over and over again forever
void loop() {
	if (!scan_complete) {
		byte error, address;
		int nDevices;

		Serial.println("Scanning for I2C devices...");

		nDevices = 0;
		for (address = 1; address < 127; address++)
		{
			// The i2c_scanner uses the return value of
			// the Write.endTransmission to see if
			// a device did acknowledge to the address.
			Wire.beginTransmission(address);
			error = Wire.endTransmission();

			if (error == 0)
			{
				Serial.print("I2C device found at address 0x");
				if (address < 16) {
					Serial.print("0");
				}
				Serial.print(address, HEX);
				Serial.println("  !");

				if (address == KEYPAD) {
					keypad_found = true;
					Serial.println("QWIIC keypad found.");
				}

				if (address == JOYSTICK) {
					joystick_found = true;
					Serial.println("QWIIC joystick found.");
				}

				if (address == GPIO_BOARD) {
					gpio_board_found = true;
					Serial.println("QWIIC GPIO board found.");

					setPinMode(GPIO_BOARD, 0, INPUT);
					setPinMode(GPIO_BOARD, 1, INPUT);
					setPinMode(GPIO_BOARD, 2, INPUT);
					setPinMode(GPIO_BOARD, 3, INPUT);
					setPinMode(GPIO_BOARD, 4, INPUT);
					setPinMode(GPIO_BOARD, 5, INPUT);
					setPinMode(GPIO_BOARD, 6, INPUT);
					setPinMode(GPIO_BOARD, 7, INPUT);
				}

				nDevices++;
			}
			else if (error == 4)
			{
				Serial.print("Unknown error at address 0x");
				if (address < 16) {
					Serial.print("0");
				}
				Serial.println(address, HEX);
			}
		}
		if (nDevices == 0) {
			Serial.println("No I2C devices found\n");
		}
		else {
			Serial.println("done\n");
		}

		scan_complete = true;
	}

	if (keypad_found) {
		Wire.requestFrom(KEYPAD, 1);
		byte keyboard_byte = Wire.read();
		switch (keyboard_byte)
		{
		case KEY_STAR:
			Serial.println("KEY * pressed.");
			sendBleMsg("KS");
			break;
		case KEY_HASH:
			Serial.println("KEY # pressed.");
			sendBleMsg("KH");
			break;
		case KEY_0:
			Serial.println("KEY 0 pressed.");
			sendBleMsg("K0");
			break;
		case KEY_1:
			Serial.println("KEY 1 pressed.");
			sendBleMsg("K1");
			break;
		case KEY_2:
			Serial.println("KEY 2 pressed.");
			sendBleMsg("K2");
			break;
		case KEY_3:
			Serial.println("KEY 3 pressed.");
			sendBleMsg("K3");
			break;
		case KEY_4:
			Serial.println("KEY 4 pressed.");
			sendBleMsg("K4");
			break;
		case KEY_5:
			Serial.println("KEY 5 pressed.");
			sendBleMsg("K5");
			break;
		case KEY_6:
			Serial.println("KEY 6 pressed.");
			sendBleMsg("K6");
			break;
		case KEY_7:
			Serial.println("KEY 7 pressed.");
			sendBleMsg("K7");
			break;
		case KEY_8:
			Serial.println("KEY 8 pressed.");
			sendBleMsg("K8");
			break;
		case KEY_9:
			Serial.println("KEY 9 pressed.");
			sendBleMsg("K9");
			break;
		default:
			break;
		}
	}

	/*
	if (joystick_found) {
		if (getJoystickButton() == JOY_BUTTON_DOWN) {
			if (!prev_joystick_button_state) {
				Serial.println("JOYSTICK button pressed.");
				sendBleMsg("JB");
			}
			prev_joystick_button_state = true;
		}
		else {
			prev_joystick_button_state = false;
		}

		int joystickH = getJoystickHorizontal();
		int joystickV = getJoystickVertical();
		byte joystickButton = getJoystickButton();

		byte joystick_direction = JOY_CENTER;

		byte joystick_horiz = JOY_CENTER_H;
		if (joystickH < (512 - 40)) {
			joystick_horiz = JOY_LEFT;
		}
		else if (joystickH >(512 + 40)) {
			joystick_horiz = JOY_RIGHT;
		}

		byte joystick_vert = JOY_CENTER_H;
		if (joystickV < (512 - 40)) {
			joystick_vert = JOY_UP;
		}
		else if (joystickV >(512 + 40)) {
			joystick_vert = JOY_DOWN;
		}

		if (joystick_horiz == JOY_CENTER_H & joystick_vert == JOY_UP) {
			joystick_direction = JOY_N;
		}
		else if (joystick_horiz == JOY_RIGHT & joystick_vert == JOY_UP) {
			joystick_direction = JOY_NE;
		}
		else if (joystick_horiz == JOY_RIGHT & joystick_vert == JOY_CENTER_V) {
			joystick_direction = JOY_E;
		}
		else if (joystick_horiz == JOY_RIGHT & joystick_vert == JOY_DOWN) {
			joystick_direction = JOY_SE;
		}
		else if (joystick_horiz == JOY_CENTER_H & joystick_vert == JOY_DOWN) {
			joystick_direction = JOY_S;
		}
		else if (joystick_horiz == JOY_LEFT & joystick_vert == JOY_DOWN) {
			joystick_direction = JOY_SW;
		}
		else if (joystick_horiz == JOY_LEFT & joystick_vert == JOY_CENTER_V) {
			joystick_direction = JOY_W;
		}
		else if (joystick_horiz == JOY_LEFT & joystick_vert == JOY_UP) {
			joystick_direction = JOY_NW;
		}

		if (joystick_direction != prev_joystick_direction) {
			if (joystick_direction == JOY_CENTER) {
				Serial.println("JOYSTICK is CENTERED.");
				sendBleMsg("JC");
			}
			else if (joystick_direction == JOY_N) {
				Serial.println("JOYSTICK is NORTH.");
				sendBleMsg("JN");
			}
			else if (joystick_direction == JOY_NE) {
				Serial.println("JOYSTICK is NORTHEAST.");
				sendBleMsg("JO");
			}
			else if (joystick_direction == JOY_E) {
				Serial.println("JOYSTICK is EAST.");
				sendBleMsg("JE");
			}
			else if (joystick_direction == JOY_SE) {
				Serial.println("JOYSTICK is SOUTHEAST.");
				sendBleMsg("JF");
			}
			else if (joystick_direction == JOY_S) {
				Serial.println("JOYSTICK is SOUTH.");
				sendBleMsg("JS");
			}
			else if (joystick_direction == JOY_SW) {
				Serial.println("JOYSTICK is SOUTHWEST.");
				sendBleMsg("JT");
			}
			else if (joystick_direction == JOY_W) {
				Serial.println("JOYSTICK is WEST.");
				sendBleMsg("JW");
			}
			else if (joystick_direction == JOY_NW) {
				Serial.println("JOYSTICK is NORTHWEST.");
				sendBleMsg("JX");
			}

			prev_joystick_direction = joystick_direction;
		}
	}
	*/

	if (gpio_board_found) {

		shift_button_state = readPin(GPIO_BOARD, 0);
		if (shift_button_state != prev_shift_button_state) {
			if (shift_button_state == HIGH) {
				Serial.println("KEY Shift pressed.");
				sendBleMsg("BH");
			}
			prev_shift_button_state = shift_button_state;
		}

		menu_button_state = readPin(GPIO_BOARD, 1);
		if (menu_button_state != prev_menu_button_state) {
			if (menu_button_state == HIGH) {
				Serial.println("KEY Menu pressed.");
				sendBleMsg("BM");
			}
			prev_menu_button_state = menu_button_state;
		}

		up_button_state = readPin(GPIO_BOARD, 2);
		if (up_button_state != prev_up_button_state) {
			if (up_button_state == HIGH) {
				Serial.println("KEY Up pressed.");
				sendBleMsg("BU");
			}
			prev_up_button_state = up_button_state;
		}

		left_button_state = readPin(GPIO_BOARD, 3);
		if (left_button_state != prev_left_button_state) {
			if (left_button_state == HIGH) {
				Serial.println("KEY Left pressed.");
				sendBleMsg("BL");
			}
			prev_left_button_state = left_button_state;
		}

		enter_button_state = readPin(GPIO_BOARD, 4);
		if (enter_button_state != prev_enter_button_state) {
			if (enter_button_state == HIGH) {
				Serial.println("KEY Enter pressed.");
				sendBleMsg("BE");
			}
			prev_enter_button_state = enter_button_state;
		}

		right_button_state = readPin(GPIO_BOARD, 5);
		if (right_button_state != prev_right_button_state) {
			if (right_button_state == HIGH) {
				Serial.println("KEY Right pressed.");
				sendBleMsg("BR");
			}
			prev_right_button_state = right_button_state;
		}

		down_button_state = readPin(GPIO_BOARD, 6);
		if (down_button_state != prev_down_button_state) {
			if (down_button_state == HIGH) {
				Serial.println("KEY Down pressed.");
				sendBleMsg("BD");
			}
			prev_down_button_state = down_button_state;
		}

		backspace_button_state = readPin(GPIO_BOARD, 7);
		if (backspace_button_state != prev_backspace_button_state) {
			if (backspace_button_state == HIGH) {
				Serial.println("KEY Backspace pressed.");
				sendBleMsg("BS");
			}
			prev_backspace_button_state = backspace_button_state;
		}
	}

	power_button_state = digitalRead(POWER_BUTTON);
	if (power_button_state != prev_power_button_state) {
		if (power_button_state == HIGH) {
			Serial.println("KEY Power pressed.");
			sendBleMsg("BP");
		}
		prev_power_button_state = power_button_state;
	}

	//For some reason, getting phantom presses on colored buttons
	//green_button_state = digitalRead(GREEN_BUTTON);
	//blue_button_state = digitalRead(BLUE_BUTTON);
	//red_button_state = digitalRead(RED_BUTTON);
	//Now being used for power button, instead of yellow
	//yellow_button_state = digitalRead(YELLOW_BUTTON);
	
	/*

	if (green_button_state != prev_green_button_state) {
		if (green_button_state == HIGH) {
			Serial.println("GREEN button pressed.");
			//sendBleMsg("BG");
		}
		prev_green_button_state = green_button_state;
	}

	if (blue_button_state != prev_blue_button_state) {
		if (blue_button_state == HIGH) {
			Serial.println("BLUE button pressed.");
			//sendBleMsg("BB");
		}
		prev_blue_button_state = blue_button_state;
	}

	if (red_button_state != prev_red_button_state) {
		if (red_button_state == HIGH) {
			Serial.println("RED button pressed.");
			//sendBleMsg("BR");
		}
		prev_red_button_state = red_button_state;
	}

	if (yellow_button_state != prev_yellow_button_state) {
		if (yellow_button_state == HIGH) {
			Serial.println("YELLOW button pressed.");
			//sendBleMsg("BY");
		}
		prev_yellow_button_state = yellow_button_state;
	}

	*/

	// Check for incoming characters from Bluefruit
	ble.println("AT+BLEUARTRX");
	ble.readline();
	if (!(strcmp(ble.buffer, "OK") == 0))
	{
		if (ble.buffer[0] == 'L' & ble.buffer[1] == 'R') {
			Serial.println("Left LED -> Red");
			pixels.setPixelColor(0, pixels.Color(150, 0, 0));
			pixels.show();
		}
		else if (ble.buffer[0] == 'L' & ble.buffer[1] == 'G') {
			Serial.println("Left LED -> Green");
			pixels.setPixelColor(0, pixels.Color(0, 150, 0));
			pixels.show();
		}
		else if (ble.buffer[0] == 'L' & ble.buffer[1] == 'B') {
			Serial.println("Left LED -> Blue");
			pixels.setPixelColor(0, pixels.Color(0, 0, 150));
			pixels.show();
		}
		else if (ble.buffer[0] == 'L' & ble.buffer[1] == 'O') {
			Serial.println("Left LED -> Off");
			pixels.setPixelColor(0, pixels.Color(0, 0, 0));
			pixels.show();
		}
		else if (ble.buffer[0] == 'R' & ble.buffer[1] == 'R') {
			Serial.println("Right LED -> Red");
			pixels.setPixelColor(1, pixels.Color(150, 0, 0));
			pixels.show();
		}
		else if (ble.buffer[0] == 'R' & ble.buffer[1] == 'G') {
			Serial.println("Right LED -> Green");
			pixels.setPixelColor(1, pixels.Color(0, 150, 0));
			pixels.show();
		}
		else if (ble.buffer[0] == 'R' & ble.buffer[1] == 'B') {
			Serial.println("Right LED -> Blue");
			pixels.setPixelColor(1, pixels.Color(0, 0, 150));
			pixels.show();
		}
		else if (ble.buffer[0] == 'R' & ble.buffer[1] == 'O') {
			Serial.println("Right LED -> Off");
			pixels.setPixelColor(1, pixels.Color(0, 0, 0));
			pixels.show();
		}
		else {
			// Some data was found, its in the buffer
			Serial.print(F("[Recv] "));
			Serial.println(ble.buffer);
		}
		ble.waitForOK();
	}

	delay(CYCLE_MILLISECONDS); //loop delay
}
