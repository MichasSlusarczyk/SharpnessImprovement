/*
 * PROJEKT: Poprawa kontrastu
 * OPIS ALGORYTMU: Wykonanie operacji jednostkowej na ka�dym pikselu obrazu
 * DATA WYKONANIA PROJEKTU: 21.01.2022
 * AUTOR: �lusarczyk Micha�
 * WERSJA: 1.0
*/

#include "pch.h"
#include <math.h>

extern "C"
{
	/*
	* Nazwa: ContrastCPP
	* 
	* Kr�tki opis:
	* Pocz�tkowo obliczany jest mno�nik na podstawie warto�ci min i max. P�niej nast�puje iteracja po wszystkich elementach tablicy w kt�rej ka�dy bajt modyfikowany jest na podstawie realizowanej operacji arytmetycznej.
	* 
	* Parametry wej�ciowe: 
	*	- bytes - wska�nik na tablice danych
	*	- length - d�ugo�� tablicy danych
	*	- min - warto�c minimalna ze zbioru danych
	*	- max -warto�� maksymalna ze zbioru danych
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