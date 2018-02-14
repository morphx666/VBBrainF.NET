#include <conio.h>

int main() {
	char b[32676];
	for(int i = 0; i < sizeof(b); i++) b[i] = 0;
	char *p = b;
	p += 1;
	*p += 9;
	while(*p) {
		p -= 1;
		*p += 8;
		p += 1;
		*p -= 1;
	}
	p -= 1;
	_putch(*p);
	p += 1;
	*p += 7;
	while(*p) {
		p -= 1;
		*p += 4;
		p += 1;
		*p -= 1;
	}
	p -= 1;
	*p += 1;
	_putch(*p);
	*p += 7;
	_putch(*p);
	_putch(*p);
	*p += 3;
	_putch(*p);
	while(*p) {
		*p -= 1;
	}
	p += 1;
	*p += 8;
	while(*p) {
		p -= 1;
		*p += 4;
		p += 1;
		*p -= 1;
	}
	p -= 1;
	_putch(*p);
	p += 1;
	*p += 11;
	while(*p) {
		p -= 1;
		*p += 5;
		p += 1;
		*p -= 1;
	}
	p -= 1;
	_putch(*p);
	p += 1;
	*p += 8;
	while(*p) {
		p -= 1;
		*p += 3;
		p += 1;
		*p -= 1;
	}
	p -= 1;
	_putch(*p);
	*p += 3;
	_putch(*p);
	*p -= 6;
	_putch(*p);
	*p -= 8;
	_putch(*p);
	while(*p) {
		*p -= 1;
	}
	p += 1;
	*p += 8;
	while(*p) {
		p -= 1;
		*p += 4;
		p += 1;
		*p -= 1;
	}
	p -= 1;
	*p += 1;
	_putch(*p);
	while(*p) {
		*p -= 1;
	}
	*p += 10;
	_putch(*p);
	return 0;
}