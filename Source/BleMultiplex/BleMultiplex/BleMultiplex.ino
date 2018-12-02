/*
 Name:		BleMultiplex.ino
 Created:	12/1/2018 9:41:33 AM
 Author:	jerem
*/

//Wire library needed for I2C/QWIIC
#include <Wire.h>

//SoftwareSerial library needed for board-to-board communications
#include <SoftwareSerial.h>

//SparkFun Micro OLED library
#include <SFE_MicroOLED.h>

//SparkFun ToF Sensor library
#include <SparkFun_RFD77402_Arduino_Library.h>

const int LOOP_DELAY_MILLISECONDS = 1000;
boolean i2c_scan_complete = false;

//BEGIN SoftSerial on pins 10 & 11

SoftwareSerial outSerial(10, 11); // RX, TX
String messageDelimiter = "*";
String serialMessage;

//END SoftSerial on pins 10 & 11

//BEGIN SparkFun Micro OLED settings

#define QWIIC_OLED 0x3D
boolean oled_found = false;

#define OLED_PIN_RESET 9  
#define OLED_DC_JUMPER 1 
MicroOLED oled(OLED_PIN_RESET, OLED_DC_JUMPER);

//END SparkFun Micro OLED settings

//BEGIN SparkFun ToF Sensor settings

#define QWIIC_TOF_SENSOR 0x4C
boolean tof_sensor_found = false;

RFD77402 distance_sensor;

//END SparkFun ToF Sensor settings

//BEGIN Sample functions for displaying info on SparkFun Micro OLED

void lineExample()
{
	int middleX = oled.getLCDWidth() / 2;
	int middleY = oled.getLCDHeight() / 2;
	int xEnd, yEnd;
	int lineWidth = min(middleX, middleY);

	printTitle("Lines!", 1);

	//for (int i = 0; i < 3; i++)
	//{
	for (int deg = 0; deg < 360; deg += 15)
	{
		xEnd = lineWidth * cos(deg * PI / 180.0);
		yEnd = lineWidth * sin(deg * PI / 180.0);

		oled.line(middleX, middleY, middleX + xEnd, middleY + yEnd);
		oled.display();
		delay(10);
	}
	for (int deg = 0; deg < 360; deg += 15)
	{
		xEnd = lineWidth * cos(deg * PI / 180.0);
		yEnd = lineWidth * sin(deg * PI / 180.0);

		oled.line(middleX, middleY, middleX + xEnd, middleY + yEnd, BLACK, NORM);
		oled.display();
		delay(10);
	}
	//}
}

void shapeExample()
{
	printTitle("Shapes!", 0);

	// Silly pong demo. It takes a lot of work to fake pong...
	int paddleW = 3;  // Paddle width
	int paddleH = 15;  // Paddle height
	// Paddle 0 (left) position coordinates
	int paddle0_Y = (oled.getLCDHeight() / 2) - (paddleH / 2);
	int paddle0_X = 2;
	// Paddle 1 (right) position coordinates
	int paddle1_Y = (oled.getLCDHeight() / 2) - (paddleH / 2);
	int paddle1_X = oled.getLCDWidth() - 3 - paddleW;
	int ball_rad = 2;  // Ball radius
	// Ball position coordinates
	int ball_X = paddle0_X + paddleW + ball_rad;
	int ball_Y = random(1 + ball_rad, oled.getLCDHeight() - ball_rad);//paddle0_Y + ball_rad;
	int ballVelocityX = 1;  // Ball left/right velocity
	int ballVelocityY = 1;  // Ball up/down velocity
	int paddle0Velocity = -1;  // Paddle 0 velocity
	int paddle1Velocity = 1;  // Paddle 1 velocity

	//while(ball_X >= paddle0_X + paddleW - 1)
	while ((ball_X - ball_rad > 1) &&
		(ball_X + ball_rad < oled.getLCDWidth() - 2))
	{
		// Increment ball's position
		ball_X += ballVelocityX;
		ball_Y += ballVelocityY;
		// Check if the ball is colliding with the left paddle
		if (ball_X - ball_rad < paddle0_X + paddleW)
		{
			// Check if ball is within paddle's height
			if ((ball_Y > paddle0_Y) && (ball_Y < paddle0_Y + paddleH))
			{
				ball_X++;  // Move ball over one to the right
				ballVelocityX = -ballVelocityX; // Change velocity
			}
		}
		// Check if the ball hit the right paddle
		if (ball_X + ball_rad > paddle1_X)
		{
			// Check if ball is within paddle's height
			if ((ball_Y > paddle1_Y) && (ball_Y < paddle1_Y + paddleH))
			{
				ball_X--;  // Move ball over one to the left
				ballVelocityX = -ballVelocityX; // change velocity
			}
		}
		// Check if the ball hit the top or bottom
		if ((ball_Y <= ball_rad) || (ball_Y >= (oled.getLCDHeight() - ball_rad - 1)))
		{
			// Change up/down velocity direction
			ballVelocityY = -ballVelocityY;
		}
		// Move the paddles up and down
		paddle0_Y += paddle0Velocity;
		paddle1_Y += paddle1Velocity;
		// Change paddle 0's direction if it hit top/bottom
		if ((paddle0_Y <= 1) || (paddle0_Y > oled.getLCDHeight() - 2 - paddleH))
		{
			paddle0Velocity = -paddle0Velocity;
		}
		// Change paddle 1's direction if it hit top/bottom
		if ((paddle1_Y <= 1) || (paddle1_Y > oled.getLCDHeight() - 2 - paddleH))
		{
			paddle1Velocity = -paddle1Velocity;
		}

		// Draw the Pong Field
		oled.clear(PAGE);  // Clear the page
		// Draw an outline of the screen:
		oled.rect(0, 0, oled.getLCDWidth() - 1, oled.getLCDHeight());
		// Draw the center line
		oled.rectFill(oled.getLCDWidth() / 2 - 1, 0, 2, oled.getLCDHeight());
		// Draw the Paddles:
		oled.rectFill(paddle0_X, paddle0_Y, paddleW, paddleH);
		oled.rectFill(paddle1_X, paddle1_Y, paddleW, paddleH);
		// Draw the ball:
		oled.circle(ball_X, ball_Y, ball_rad);
		// Actually draw everything on the screen:
		oled.display();
		delay(25);  // Delay for visibility
	}
	delay(1000);
}

void textExamples()
{
	printTitle("Text!", 1);

	// Demonstrate font 0. 5x8 font
	oled.clear(PAGE);     // Clear the screen
	oled.setFontType(0);  // Set font to type 0
	oled.setCursor(0, 0); // Set cursor to top-left
	// There are 255 possible characters in the font 0 type.
	// Lets run through all of them and print them out!
	for (int i = 0; i <= 255; i++)
	{
		// You can write byte values and they'll be mapped to
		// their ASCII equivalent character.
		oled.write(i);  // Write a byte out as a character
		oled.display(); // Draw on the screen
		delay(10);      // Wait 10ms
		// We can only display 60 font 0 characters at a time.
		// Every 60 characters, pause for a moment. Then clear
		// the page and start over.
		if ((i % 60 == 0) && (i != 0))
		{
			delay(500);           // Delay 500 ms
			oled.clear(PAGE);     // Clear the page
			oled.setCursor(0, 0); // Set cursor to top-left
		}
	}
	delay(500);  // Wait 500ms before next example

	// Demonstrate font 1. 8x16. Let's use the print function
	// to display every character defined in this font.
	oled.setFontType(1);  // Set font to type 1
	oled.clear(PAGE);     // Clear the page
	oled.setCursor(0, 0); // Set cursor to top-left
	// Print can be used to print a string to the screen:
	oled.print(" !\"#$%&'()*+,-./01234");
	oled.display();       // Refresh the display
	delay(1000);          // Delay a second and repeat
	oled.clear(PAGE);
	oled.setCursor(0, 0);
	oled.print("56789:;<=>?@ABCDEFGHI");
	oled.display();
	delay(1000);
	oled.clear(PAGE);
	oled.setCursor(0, 0);
	oled.print("JKLMNOPQRSTUVWXYZ[\\]^");
	oled.display();
	delay(1000);
	oled.clear(PAGE);
	oled.setCursor(0, 0);
	oled.print("_`abcdefghijklmnopqrs");
	oled.display();
	delay(1000);
	oled.clear(PAGE);
	oled.setCursor(0, 0);
	oled.print("tuvwxyz{|}~");
	oled.display();
	delay(1000);

	// Demonstrate font 2. 10x16. Only numbers and '.' are defined. 
	// This font looks like 7-segment displays.
	// Lets use this big-ish font to display readings from the
	// analog pins.
	for (int i = 0; i < 25; i++)
	{
		oled.clear(PAGE);            // Clear the display
		oled.setCursor(0, 0);        // Set cursor to top-left
		oled.setFontType(0);         // Smallest font
		oled.print("A0: ");          // Print "A0"
		oled.setFontType(2);         // 7-segment font
		oled.print(analogRead(A0));  // Print a0 reading
		oled.setCursor(0, 16);       // Set cursor to top-middle-left
		oled.setFontType(0);         // Repeat
		oled.print("A1: ");
		oled.setFontType(2);
		oled.print(analogRead(A1));
		oled.setCursor(0, 32);
		oled.setFontType(0);
		oled.print("A2: ");
		oled.setFontType(2);
		oled.print(analogRead(A2));
		oled.display();
		delay(100);
	}

	// Demonstrate font 3. 12x48. Stopwatch demo.
	oled.setFontType(3);  // Use the biggest font
	int ms = 0;
	int s = 0;
	while (s <= 5)
	{
		oled.clear(PAGE);     // Clear the display
		oled.setCursor(0, 0); // Set cursor to top-left
		if (s < 10)
			oled.print("00");   // Print "00" if s is 1 digit
		else if (s < 100)
			oled.print("0");    // Print "0" if s is 2 digits
		oled.print(s);        // Print s's value
		oled.print(":");      // Print ":"
		oled.print(ms);       // Print ms value
		oled.display();       // Draw on the screen
		ms++;         // Increment ms
		if (ms >= 10) // If ms is >= 10
		{
			ms = 0;     // Set ms back to 0
			s++;        // and increment s
		}
	}
}

// Center and print a small title
// This function is quick and dirty. Only works for titles one
// line long.
void printTitle(String title, int font)
{
	int middleX = oled.getLCDWidth() / 2;
	int middleY = oled.getLCDHeight() / 2;

	oled.clear(PAGE);
	oled.setFontType(font);
	// Try to set the cursor in the middle of the screen
	oled.setCursor(middleX - (oled.getFontWidth() * (title.length() / 2)),
		middleY - (oled.getFontWidth() / 2));
	// Print the title:
	oled.print(title);
	oled.display();
	delay(1500);
	oled.clear(PAGE);
}

void printDistance(unsigned int distance) {
	oled.clear(PAGE);            // Clear the display
	oled.setCursor(0, 0);        // Set cursor to top-left
	oled.setFontType(0);         // Smallest font
	oled.print("Distance");      // Print "Distance"
	oled.setCursor(0, 16);       // Set cursor to top-middle-left
	oled.setFontType(2);         // 7-segment font
	oled.print(distance);		 // Print distance reading
	oled.setFontType(0);         // Smallest font
	oled.print("mm");            // Print "mm"
	oled.display();
}

//END Sample functions for displaying info on SparkFun Micro OLED

// the setup function runs once when you press reset or power the board
void setup() {
	//Initialize Wire library
	Wire.begin();

	//Serial output for debugging
	Serial.begin(115200);
	Serial.println("Starting application...");

	//SoftwareSerial out port- set the data rate for the port
	outSerial.begin(9600);
}

// the loop function runs over and over again until power down or reset
void loop() {
	if (!i2c_scan_complete) {
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

				if (address == QWIIC_OLED) {
					oled_found = true;
					Serial.println("QWIIC OLED display found.");
				}

				if (address == QWIIC_TOF_SENSOR) {
					tof_sensor_found = true;
					Serial.println("QWIIC ToF sensor found.");
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

		if (oled_found) {
			//BEGIN Setup code for SparkFun Micro OLED

			oled.begin();    // Initialize the OLED
			oled.clear(ALL); // Clear the display's internal memory
			//oled.display();  // Display what's in the buffer (splashscreen)
			//delay(1000);     // Delay 1000 ms
			oled.clear(PAGE); // Clear the buffer.

			//randomSeed(analogRead(A0) + analogRead(A1));

			//END Setup code for SparkFun Micro OLED
		}

		if (tof_sensor_found) {
			//BEGIN Setup code for SparkFun ToF distance sensor

			if (distance_sensor.begin() == false) //Initializes the sensor. Tells the user if initialization has failed.
			{
				Serial.println("ToF distance sensor failed to initialize. Check wiring.");
				while (1); //Freeze!
			}
			Serial.println("ToF distance sensor online!");

			//END Setup code for SparkFun ToF distance sensor
		}

		i2c_scan_complete = true;
	}

	//BEGIN Application work
	
	//if (oled_found) {
	//	//Show OLED screen samples
	//	lineExample();   // First the line example function
	//	shapeExample();  // Then the shape example
	//	textExamples();  // Finally the text example
	//}

	if (tof_sensor_found) {
		distance_sensor.takeMeasurement(); //Tell sensor to take measurement and populate distance variable with measurement value

		unsigned int distance = distance_sensor.getDistance(); //Retrieve the distance value

		Serial.print("distance: "); //Print the distance
		Serial.print(distance);
		Serial.print("mm");
		Serial.println();
		serialMessage = messageDelimiter + distance;
		outSerial.println(serialMessage + messageDelimiter);
		//outSerial.println()
		if (oled_found) {
			printDistance(distance);
		}
	}

	//END Application work
	
	delay(LOOP_DELAY_MILLISECONDS); //loop delay
}
