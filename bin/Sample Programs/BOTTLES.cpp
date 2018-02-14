#include <conio.h>

int main() {
	char b[32676];
	for(int i = 0; i < sizeof(b); i++) b[i] = 0;
	char *p = b;
	p += 1;
	*p += 9;
	while(*p) {
		p -= 1;
		*p += 11;
		p += 1;
		*p -= 1;
	}
	p -= 1;
	while(*p) {
		p += 1;
		while(*p) {
			*p -= 1;
		}
		p += 1;
		while(*p) {
			*p -= 1;
		}
		p -= 2;
		while(*p) {
			p += 1;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 2;
			*p -= 1;
		}
		p += 2;
		while(*p) {
			p -= 2;
			*p += 1;
			p += 2;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			*p -= 1;
		}
		p -= 3;
		*p += 9;
		p -= 1;
		while(*p) {
			p += 3;
			*p += 1;
			p -= 2;
			while(*p) {
				p += 1;
				*p += 1;
				p += 1;
				while(*p) {
					*p -= 1;
				}
				p -= 2;
				*p -= 1;
			}
			p += 1;
			while(*p) {
				p -= 1;
				*p += 1;
				p += 1;
				*p -= 1;
			}
			p += 1;
			while(*p) {
				p -= 2;
				*p += 10;
				p += 3;
				*p += 1;
				p -= 1;
				*p -= 1;
			}
			p -= 2;
			*p -= 1;
			p -= 1;
			*p -= 1;
		}
		*p += 9;
		p += 1;
		while(*p) {
			p -= 1;
			*p -= 1;
			p += 1;
			*p -= 1;
		}
		p += 2;
		*p += 1;
		p += 1;
		while(*p) {
			p -= 1;
			while(*p) {
				*p -= 1;
			}
			p -= 2;
			*p += 1;
			p += 3;
			*p -= 1;
		}
		p += 1;
		while(*p) {
			*p -= 1;
		}
		*p += 1;
		p -= 2;
		while(*p) {
			p += 1;
			*p += 1;
			p += 1;
			*p -= 1;
			p -= 2;
			*p -= 1;
		}
		p -= 3;
		while(*p) {
			p += 2;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 3;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			p -= 3;
			*p += 1;
			p += 3;
			*p -= 1;
		}
		p += 1;
		while(*p) {
			p -= 1;
			*p += 1;
			p += 1;
			*p -= 1;
		}
		p -= 2;
		*p -= 1;
		while(*p) {
			p += 1;
			while(*p) {
				*p -= 1;
			}
			p -= 1;
			while(*p) {
				*p -= 1;
			}
		}
		p += 2;
		*p += 1;
		p -= 1;
		while(*p) {
			p += 1;
			while(*p) {
				*p -= 1;
			}
			p -= 1;
			*p -= 1;
		}
		p -= 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p += 6;
			p -= 1;
			*p += 6;
			p += 2;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			p += 1;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 2;
			*p -= 1;
		}
		p += 2;
		while(*p) {
			p -= 2;
			*p += 1;
			p += 2;
			*p -= 1;
		}
		p -= 1;
		while(*p) {
			p -= 5;
			_putch(*p);
			p += 5;
			*p -= 1;
		}
		p -= 6;
		_putch(*p);
		p += 2;
		while(*p) {
			*p -= 1;
		}
		p += 1;
		while(*p) {
			*p -= 1;
		}
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 8;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 8;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 2;
		_putch(*p);
		p += 1;
		*p += 5;
		while(*p) {
			p -= 1;
			*p += 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		p -= 1;
		*p += 5;
		_putch(*p);
		_putch(*p);
		*p -= 8;
		_putch(*p);
		*p -= 7;
		_putch(*p);
		p += 2;
		while(*p) {
			p += 2;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 3;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			p -= 3;
			*p += 1;
			p += 3;
			*p -= 1;
		}
		p -= 1;
		while(*p) {
			p -= 4;
			*p += 14;
			_putch(*p);
			p += 4;
			*p -= 1;
		}
		p -= 4;
		while(*p) {
			*p -= 1;
		}
		p += 1;
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 8;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 9;
		while(*p) {
			p -= 1;
			*p += 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		*p -= 9;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 6;
		while(*p) {
			p -= 1;
			*p += 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		*p += 3;
		_putch(*p);
		_putch(*p);
		*p += 13;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		p += 1;
		*p += 9;
		while(*p) {
			p -= 1;
			*p += 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		*p -= 1;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 2;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p += 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 4;
		_putch(*p);
		*p -= 12;
		_putch(*p);
		*p -= 3;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 1;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p += 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 1;
		_putch(*p);
		p += 1;
		*p += 2;
		while(*p) {
			p -= 1;
			*p -= 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		*p += 11;
		_putch(*p);
		_putch(*p);
		p += 1;
		*p += 9;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 5;
		_putch(*p);
		*p -= 3;
		_putch(*p);
		p += 3;
		while(*p) {
			p += 1;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 2;
			*p -= 1;
		}
		p += 2;
		while(*p) {
			p -= 2;
			*p += 1;
			p += 2;
			*p -= 1;
		}
		p -= 1;
		while(*p) {
			p -= 5;
			_putch(*p);
			p += 5;
			*p -= 1;
		}
		p -= 6;
		_putch(*p);
		p += 3;
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 6;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		p += 1;
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 8;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 2;
		_putch(*p);
		p += 1;
		*p += 5;
		while(*p) {
			p -= 1;
			*p += 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		p -= 1;
		*p += 5;
		_putch(*p);
		_putch(*p);
		*p -= 8;
		_putch(*p);
		*p -= 7;
		_putch(*p);
		p += 2;
		while(*p) {
			p += 2;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 3;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			p -= 3;
			*p += 1;
			p += 3;
			*p -= 1;
		}
		p -= 1;
		while(*p) {
			p -= 4;
			*p += 14;
			_putch(*p);
			p += 4;
			*p -= 1;
		}
		p -= 4;
		while(*p) {
			*p -= 1;
		}
		p += 1;
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 8;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 9;
		while(*p) {
			p -= 1;
			*p += 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		*p -= 9;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 6;
		while(*p) {
			p -= 1;
			*p += 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		*p += 3;
		_putch(*p);
		_putch(*p);
		*p += 13;
		_putch(*p);
		p += 1;
		*p += 10;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 1;
		_putch(*p);
		*p -= 3;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p += 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 4;
		_putch(*p);
		*p += 13;
		_putch(*p);
		*p += 10;
		_putch(*p);
		*p -= 6;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 1;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p += 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 1;
		_putch(*p);
		*p -= 1;
		_putch(*p);
		*p -= 9;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 1;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p += 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		*p += 11;
		_putch(*p);
		*p += 8;
		_putch(*p);
		*p -= 9;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 2;
		_putch(*p);
		p += 1;
		*p += 5;
		while(*p) {
			p -= 1;
			*p += 13;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		*p += 13;
		_putch(*p);
		*p -= 10;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 2;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p += 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 3;
		while(*p) {
			p -= 1;
			*p -= 5;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 3;
		while(*p) {
			p -= 1;
			*p += 6;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		_putch(*p);
		p += 1;
		*p += 9;
		while(*p) {
			p -= 1;
			*p -= 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p += 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 3;
		_putch(*p);
		*p += 11;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p -= 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 4;
		_putch(*p);
		p += 1;
		*p += 5;
		while(*p) {
			p -= 1;
			*p += 13;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 3;
		while(*p) {
			p -= 1;
			*p += 6;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 1;
		_putch(*p);
		*p -= 3;
		_putch(*p);
		*p += 6;
		_putch(*p);
		*p -= 7;
		_putch(*p);
		*p -= 10;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p -= 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 1;
		_putch(*p);
		*p -= 3;
		_putch(*p);
		while(*p) {
			*p -= 1;
		}
		p -= 3;
		*p -= 1;
		p += 1;
		while(*p) {
			*p -= 1;
		}
		p += 1;
		while(*p) {
			*p -= 1;
		}
		p -= 2;
		while(*p) {
			p += 1;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 2;
			*p -= 1;
		}
		p += 2;
		while(*p) {
			p -= 2;
			*p += 1;
			p += 2;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			*p -= 1;
		}
		p -= 3;
		*p += 9;
		p -= 1;
		while(*p) {
			p += 3;
			*p += 1;
			p -= 2;
			while(*p) {
				p += 1;
				*p += 1;
				p += 1;
				while(*p) {
					*p -= 1;
				}
				p -= 2;
				*p -= 1;
			}
			p += 1;
			while(*p) {
				p -= 1;
				*p += 1;
				p += 1;
				*p -= 1;
			}
			p += 1;
			while(*p) {
				p -= 2;
				*p += 10;
				p += 3;
				*p += 1;
				p -= 1;
				*p -= 1;
			}
			p -= 2;
			*p -= 1;
			p -= 1;
			*p -= 1;
		}
		*p += 9;
		p += 1;
		while(*p) {
			p -= 1;
			*p -= 1;
			p += 1;
			*p -= 1;
		}
		p += 2;
		*p += 1;
		p += 1;
		while(*p) {
			p -= 1;
			while(*p) {
				*p -= 1;
			}
			p -= 2;
			*p += 1;
			p += 3;
			*p -= 1;
		}
		p += 1;
		while(*p) {
			*p -= 1;
		}
		*p += 1;
		p -= 2;
		while(*p) {
			p += 1;
			*p += 1;
			p += 1;
			*p -= 1;
			p -= 2;
			*p -= 1;
		}
		p -= 3;
		while(*p) {
			p += 2;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 3;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			p -= 3;
			*p += 1;
			p += 3;
			*p -= 1;
		}
		p -= 1;
		p += 2;
		while(*p) {
			p -= 1;
			*p += 1;
			p += 1;
			*p -= 1;
		}
		p -= 2;
		*p -= 1;
		while(*p) {
			p += 1;
			while(*p) {
				*p -= 1;
			}
			p -= 1;
			while(*p) {
				*p -= 1;
			}
		}
		p += 2;
		*p += 1;
		p -= 1;
		while(*p) {
			p += 1;
			while(*p) {
				*p -= 1;
			}
			p -= 1;
			*p -= 1;
		}
		p -= 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p += 6;
			p -= 1;
			*p += 6;
			p += 2;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			p += 1;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 2;
			*p -= 1;
		}
		p += 2;
		while(*p) {
			p -= 2;
			*p += 1;
			p += 2;
			*p -= 1;
		}
		p -= 1;
		while(*p) {
			p -= 5;
			_putch(*p);
			p += 5;
			*p -= 1;
		}
		p -= 6;
		_putch(*p);
		p += 2;
		while(*p) {
			*p -= 1;
		}
		p += 1;
		while(*p) {
			*p -= 1;
		}
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 8;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 8;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 2;
		_putch(*p);
		p += 1;
		*p += 5;
		while(*p) {
			p -= 1;
			*p += 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		p -= 1;
		*p += 5;
		_putch(*p);
		_putch(*p);
		*p -= 8;
		_putch(*p);
		*p -= 7;
		_putch(*p);
		p += 2;
		while(*p) {
			p += 2;
			*p += 1;
			p += 1;
			*p += 1;
			p -= 3;
			*p -= 1;
		}
		p += 3;
		while(*p) {
			p -= 3;
			*p += 1;
			p += 3;
			*p -= 1;
		}
		p -= 1;
		while(*p) {
			p -= 4;
			*p += 14;
			_putch(*p);
			p += 4;
			*p -= 1;
		}
		p -= 4;
		while(*p) {
			*p -= 1;
		}
		p += 1;
		*p += 4;
		while(*p) {
			p -= 1;
			*p += 8;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 9;
		while(*p) {
			p -= 1;
			*p += 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		*p -= 9;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		p += 1;
		*p += 6;
		while(*p) {
			p -= 1;
			*p += 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		*p += 3;
		_putch(*p);
		_putch(*p);
		*p += 13;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		p += 1;
		*p += 9;
		while(*p) {
			p -= 1;
			*p += 9;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 2;
		_putch(*p);
		*p -= 1;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 2;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p += 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 4;
		_putch(*p);
		*p -= 12;
		_putch(*p);
		*p -= 3;
		_putch(*p);
		p += 1;
		*p += 7;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p += 1;
		_putch(*p);
		p += 1;
		*p += 8;
		while(*p) {
			p -= 1;
			*p += 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 1;
		_putch(*p);
		p += 1;
		*p += 2;
		while(*p) {
			p -= 1;
			*p -= 11;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		_putch(*p);
		*p += 11;
		_putch(*p);
		_putch(*p);
		p += 1;
		*p += 9;
		while(*p) {
			p -= 1;
			*p -= 10;
			p += 1;
			*p -= 1;
		}
		p -= 1;
		*p -= 5;
		_putch(*p);
		*p -= 3;
		_putch(*p);
		*p += 3;
		_putch(*p);
		*p -= 3;
		_putch(*p);
		while(*p) {
			*p -= 1;
		}
		p -= 3;
	}
	return 0;
}