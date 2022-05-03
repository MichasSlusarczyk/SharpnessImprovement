;PROJEKT: Poprawa kontrastu
;OPIS ALGORYTMU: Wykonanie operacji jednostkowej na ka�dym pikselu obrazu
;DATA WYKONANIA PROJEKTU: 21.01.2022
;AUTOR: �lusarczyk Micha�
;WERSJA: 1.0

.data
MASK1 db           00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 0Ch, 0Dh, 0Eh, 0Fh

MASK2 db           00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 08h, 09h, 0Ah, 0Bh

MASK3 db           00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 04h, 05h, 06h, 07h

MASK4 db           00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 01h, 02h, 03h

MASKW1 db          00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 04h, 08h, 0Ch

MASKMM db          00h, 00h, 00h, 00h, 0Fh, 0Fh, 0Fh, 0Fh, 0Fh, 0Fh, 0Fh, 0Fh, 0Fh, 0Fh, 0Fh, 0Fh

C255 db           0FFh, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h, 00h


; Nazwa: ContrastASM
;
; Kr�tki opis:
; Realizacja algorytmu poprawiajacego kontrast z wykorzystaniem asemblera oraz instrukcji wektorowych
; 
; Parametry wej�ciowe: 
;	- RCX - wska�nik na tablice danych
;	- RDX - d�ugo�� tablicy danych
;	- R8 - warto�� minimalna ze zbioru danych
;	- R9 -warto�� maksymalna ze zbioru danych
;
; Rejestry ulegaj�ce zmianie po wykonaniu procedury:
;	- RBX, RSI, R10, RIP, XMM0, XMM1, XMM1, XMM2, XMM3, XMM4, XMM7, XMM13, XMM15
; Flagi ulegaj�ce zmianie po wykonaniu procedury:
;	- brak


.code
ContrastASM proc

;Przygotowanie danych do algorytmu

;wczytanie min max
MOVD XMM14, R8
MOVD XMM15, R9

;maska do pomno�enia wartosci
LEA RSI, MASKMM  
MOVUPS XMM10, [RSI]
PSHUFB XMM14, XMM10 
PSHUFB XMM15, XMM10

;przejcie na zmiennoprzecinkowe
PMOVZXBD XMM14, XMM14
CVTDQ2PS XMM14, XMM14

PMOVZXBD XMM15, XMM15
CVTDQ2PS XMM15, XMM15

MOVUPS XMM13, XMM15

;odjecie max od min
SUBPS XMM13, XMM14

;wczytanie sta�ej 255
LEA RSI, C255 
MOVUPS XMM7, [RSI]

;maska do pomnozenia wartosci
LEA RSI, MASKMM 
MOVUPS XMM10, [RSI]
PSHUFB XMM7, XMM10 

;zmiana na zmiwnnoprzecinkowe
PMOVZXBD XMM7, XMM7
CVTDQ2PS XMM7, XMM7

;Podzielenie 255 przez r�nice min max
DIVPS XMM7, XMM13

;przepisanie adresu
MOV RBX, RCX

;przygotowanie licznika
XOR R10, R10

;Wykonanie w p�tli algorytmu dla danych

;wczytanie danych do rejestru
RPT:
MOVUPS XMM0, [RBX]

;podzielenie na 4 rejestry

;wczytanie do 4 rejsetr�w
MOVUPS XMM1, XMM0
MOVUPS XMM2, XMM0
MOVUPS XMM3, XMM0
MOVUPS XMM4, XMM0

;wydobycie odpowiednich bajt�w na pocz�tek ka�dego z rejestr�w
LEA RSI, MASK1 
MOVUPS XMM10, [RSI]
PSHUFB XMM1, XMM10

LEA RSI, MASK2 
MOVUPS XMM10, [RSI]
PSHUFB XMM2, XMM10

LEA RSI, MASK3 
MOVUPS XMM10, [RSI]
PSHUFB XMM3, XMM10

LEA RSI, MASK4 
MOVUPS XMM10, [RSI]
PSHUFB XMM4, XMM10

;przesuni�cie do prawej i pozbycie sie zb�dnych danych
PSRLDQ XMM1, 12
PSRLDQ XMM2, 12
PSRLDQ XMM3, 12
PSRLDQ XMM4, 12

;przejscie na zmiennoprzecinkowe
PMOVZXBD XMM1, XMM1
CVTDQ2PS XMM1, XMM1

PMOVZXBD XMM2, XMM2
CVTDQ2PS XMM2, XMM2

PMOVZXBD XMM3, XMM3
CVTDQ2PS XMM3, XMM3

PMOVZXBD XMM4, XMM4
CVTDQ2PS XMM4, XMM4

;od ka�dego z rejsetr�w odj�cie min (zmiennoprzecinkowe)
SUBPS XMM1, XMM14
SUBPS XMM2, XMM14
SUBPS XMM3, XMM14
SUBPS XMM4, XMM14

;pomno�enie ka�dego z rejstrow razy 255/(max - min)
MULPS XMM1, XMM7
MULPS XMM2, XMM7
MULPS XMM3, XMM7
MULPS XMM4, XMM7

;przejscie z powrotem
CVTPS2DQ XMM1, XMM1
CVTPS2DQ XMM2, XMM2
CVTPS2DQ XMM3, XMM3
CVTPS2DQ XMM4, XMM4

;po��cznie w jeden rejstr
LEA RSI, MASKW1 
MOVUPS XMM10, [RSI]

;wydobycie odpowiednich bajt�w na poczatek ka�dego z rejestr�w
PSHUFB XMM1, XMM10
PSHUFB XMM2, XMM10
PSHUFB XMM3, XMM10
PSHUFB XMM4, XMM10

;przesuni�cie do prawej tak aby wyeliminowa� zb�dne dane
PSRLDQ XMM1, 12
PSRLDQ XMM2, 12
PSRLDQ XMM3, 12
PSRLDQ XMM4, 12

;przesuni�cie na odpowiednie miejsca tak aby mo�na je po��czy�
PSLLDQ XMM1, 12
PSLLDQ XMM2, 8
PSLLDQ XMM3, 4

;po��czneie 4 rejestr�w w jeden
ORPS XMM5, XMM1
ORPS XMM5, XMM2
ORPS XMM5, XMM3
ORPS XMM5, XMM4

;zapisanie do pamieci
MOVUPS [RBX], XMM5
XORPS XMM5, XMM5

;przesuniecie wskaznika w tablicy na kolejne dane
ADD RBX, 16
;inkremanetacja licznika
ADD R10, 16
;sprawdzenie czy przejrzeli�my wszystkie bajty
CMP R10, RDX
;je�li licznik jest mniejszy od liczby bajtow do przejrzenia powt�rz cykl
JL RPT

;je�li nie powr��
RET

ContrastASM endp
END
