# CASSEL
## Colourful Assembly Language

Colourful Assembly Language, CASSEL, is a simplistic transpiled programming language making use of only HTML colour codes to write functional, Turing-Complete* code.

### How does it work?
The compiler reads commands from colour codes in the pixels of a .PNG image and transpiles it into C++ code that you can compile and run yourself. The pixels are read from left to right, top to bottom. Any pixels that do not have an Alpha value of **FF** (= 255) will be ignored and treated like comments.

### Features of the language
1. Input and output
2. Addition and subtraction
3. Loops
4. Data storage in memory

### Hexadecimal register system
CASSEL features a three-way input system for calling registers to perform a certain function. The values in the Green and Blue channels are always automatically converted to int before use.
| Channel | Argument |
|--------|--------------|
| Red | Chosen register |
| Green | Variable address |
| Blue | Argument value |

### Registers
| **Register** | **Method**  |
|--------------|-------------|
| **FF**       | Assignment  |
| **FA**       | Addition    |
| **F0**       | Subtraction |
| **CA**       | Input       |
| **C0**       | Output      |
| **FE**       | Loop Begin      |
| **CE**       | Loop End      |

### Fixed Values
Due to CASSEL's limitation by the max number, **FF** (= 255), that can be presented in a single channel, CASSEL features a fixed value as seen below.

| **Hex Code** | **Behaviour** |
|--------|-------------|
|**CC**|Get value from next colour|

When the hex code **CC** is used in the Green or Blue channel, this tells CASSEL to look at the following colour code to get its information for that specific argument.
This means the limit of **FF** (= 255) is removed and instead can hold up to **FFFFFF** (= 16,777,215).

Therefore, if you have reached the soft limit of 255, you can replace the channel value with **CC** and specify a greater-length number/address in the following colour code. If you use **CC** in both the Green channel and the Blue channel, you must specify two following colour codes containing values to be read into the Green and Blue channel respectively.

Example:
```CASSEL
#FFCCCC
#000539
#0001A4
```

In the above example, the **FF** register is used in the Red channel to declare a variable. The **CC** in the Green channel specifies that the entire following colour code (0x539) will be used as a variable address. Because **CC** has been used in the Green channel already, **CC** in the Blue channel will now tell the compiler to use the second colour code specified (0x1A4) as a value to be set to the variable with the address specified in the Green channel.

## Documentation
This section will cover the usage and arguments of all 7 registers in the language.

### Assignment Register - **FF**
Declares a variable with a given address and an assigned integer value within the current scope.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**FF**|Variable Address|Value|

Variable Address - The name of the variable declared.
Value - The value assigned to the variable.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FF010C
```
C++ (Generated)
```c++
int v_1 = 12;
```

### Addition Register - **FA**
Performs an addition operation on a variable with a given address using an assigned integer value.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**FA**|Variable Address|Value Added|

Variable Address - The name of the variable being added to.
Valued Added - The amount being added to the variable.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FA0103
```
C++ (Generated)
```cpp
v_1 += 3;
```

### Subtraction Register - **F0**
Performs a subtraction operation on a variable with a given address using an assigned integer value.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**F0**|Variable Address|Value Subtracted|

Variable Address - The name of the variable being subtracted from.
Valued Added - The amount being subtracted from the variable.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#F00F0A
```
C++ (Generated)
```cpp
v_15 -= 10;
```

### Input Register - **CA**
Gets single-character input from the user with stdio, converts the character to an integer from UTF-8 and stores the value in a variable.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**CA**|Variable Address|-|

Variable Address - The name of the variable in which the user's input is stored.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FF0100
#CA0100
```
C++ (Generated)
```cpp
int v_1 = 0;
char v_xxxx{};
std::cin.get(v_xxxx);
v_1 = int(v_xxxx);
```
_xxxx is a placeholder for a compiler-generated UUID_

### Output Register - **C0**
Prints out a single character from an integer stored in a variable with a given address. The integer is converted into a UTF-8 character before being outputted to the console.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**C0**|Variable Address|-|

Variable Address - The name of the variable in which the value that will be printed out is stored.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FF0161
#C00100
```
C++ (Generated)
```cpp
int v_1 = 97;
std::cout << char(v_1);
```

### Loop Registers - **FE & CE**
Begins a loop over the code between this register and the closing register. Loops the number of times specified by the value in the variable with the given variable address.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**FE**|Variable Address|-|
|**CE**|-|-|

Variable Address - The variable containing the amount of times to run the loop.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FF0061
#FF0105
#FE0100
#C00200
#FA0001
#CE0000
```
C++ (Generated)
```cpp
int v_0 = 97;
int v_1 = 5;
for (int v_xxxx = 0; v_xxxx < v_1; v_xxxx++)
{
	std::cout << char(v_0);
	v_0 += 1;
}
```
_xxxx is a placeholder for a compiler-generated UUID_

## Example
The following is a simple Hello World program written (drawn) in CASSEL. This program does not feature string optimisation and is intended for demonstration purposes.

### Drawn image
The following is a zoomed-in version of the image to be able to see it. The original is a 32x32 pixel image. The actual code is made up of the oddly coloured pixels in the ears. The rest of the pixels that are considered unimportant, have an Alpha value of FE (= 254). Enough to let the compiler know to ignore those pixels, and also to make the difference in opacity unnoticable to the viewer. The size of the image is unimportant when writing a normal program in CASSEL, the colours of individual pixels and their order is all that matters.

![Zoomed-in Image of written program](https://github.com/toasted226/colourful-assembly-language/blob/master/example/zoomedincode.png)

Original image:

![Original 32x32 image](https://github.com/toasted226/colourful-assembly-language/blob/master/example/code.png)

### Written code
The following is the CASSEL code represented as individual colour codes in regular text.

```CASSEL
#FF0048
#C00000
#FF0165
#C00100
#FA0107
#FF0A02
#FE0A00
#C00100
#CE0000
#FF026F
#C00200
#FF032C
#C00300
#FF0420
#C00400
#FF0577
#C00500
#C00200
#FF0672
#C00600
#C00100
#FF0764
#C00700
#FF0821
#C00800
```

The following is the generated C++ code after the code in the image is transpiled.

```cpp
#include<iostream>
int main()
{
	int v_0 = 72;
	std::cout << char(v_0);
	int v_1 = 101;
	std::cout << char(v_1);
	v_1 += 7;
	int v_10 = 2;
	for (int v_xxxx = 0; v_xxxx < v_10; v_xxxx++)
	{
		std::cout << char(v_1);
	}
	int v_2 = 111;
	std::cout << char(v_2);
	int v_3 = 44;
	std::cout << char(v_3);
	int v_4 = 32;
	std::cout << char(v_4);
	int v_5 = 119;
	std::cout << char(v_5);
	std::cout << char(v_2);
	int v_6 = 114;
	std::cout << char(v_6);
	std::cout << char(v_1);
	int v_7 = 100;
	std::cout << char(v_7);
	int v_8 = 33;
	std::cout << char(v_8);

	return 0;
}
```
_xxxx is a placeholder for a compiler-generated UUID_

### Output
Upon compiling and running the generated C++ code, the following output is received.

```output
Hello, world!
```

## Optimisation
This section will cover the preprocessing step in the compilation process. This information will help you write CASSEL code that generates cleaner and more efficient C++ code.

### String Optimisation
CASSEL has a string optimisation feature in which assignment registers followed by output registers that have consecutive matching variable addresses instead generate a string variable in C++. This is to avoid repeatedly creating int variables and printing out their char conversions.

For this to work, there must be at least 2 assignment registers followed by corresponding output registers. You can still have unrelated assignment or output registers before or after these string-optimised sections, they will be safely ignored as regular registers.

Example:
In the following example, you can see that there are 3 assignments followed by 3 outputs, each variable address in the assignment registers corresponding to addresses in the output registers. Refer to the [documentation](##Documentation) for more information on how the assignment and output registers work.

CASSEL (HTML Colour Code Translation)
```CASSEL
#FF0048
#FF0169
#FF0221
#C00000
#C00100
#C00200
```
C++ (Generated)
```cpp
std::string v_xxxx = "Hi!";
std::cout << v_xxxx;
```
_xxxx is a placeholder for a compiler-generated UUID_

---

<span style="font-size:0.7em;">*As the amount of variable addresses is finite, CASSEL is technically not Turing-Complete. However, there are over 16 million variable addresses available to be used. While not every problem could theoretically be solved with such a limitation, you could solve a good number. Also, this language is not meant to be taken seriously.</span>

