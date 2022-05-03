/*
 * PROJEKT: Poprawa kontrastu
 * OPIS ALGORYTMU: Wykonanie operacji jednostkowej na ka¿dym pikselu obrazu
 * DATA WYKONANIA PROJEKTU: 21.01.2022
 * AUTOR: Œlusarczyk Micha³
 * WERSJA: 1.0
*/

#include "pch.h"
#include <math.h>

extern "C"
{
	/*
	* Nazwa: ContrastCPP
	* 
	* Krótki opis:
	* Pocz¹tkowo obliczany jest mno¿nik na podstawie wartoœci min i max. PóŸniej nastêpuje iteracja po wszystkich elementach tablicy w której ka¿dy bajt modyfikowany jest na podstawie realizowanej operacji arytmetycznej.
	* 
	* Parametry wejœciowe: 
	*	- bytes - wskaŸnik na tablice danych
	*	- length - d³ugoœæ tablicy danych
	*	- min - wartoœc minimalna ze zbioru danych
	*	- max -wartoœæ maksymalna ze zbioru danych
	*/
	__declspec(dllexport) void ContrastCPP(unsigned char* bytes, int length, int min, int max)
	{
		int mnoznik = 255 / (max - min);

		for (int i = 0; i < length; i++)
		{
			bytes[i] = (char)(mnoznik * (bytes[i] - min));
		}
	}
}