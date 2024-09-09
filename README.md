# Compiler 
Welcome to my repo! <br>
I am working on a compiler that converts BASIC to C <br>

BASIC code:<br>
<br>
LET a = 0<br>
WHILE a < 1 REPEAT<br>
    &emsp;PRINT "Enter number of scores: "<br>
    &emsp;INPUT a<br>
ENDWHILE<br>

LET b = 0<br>
LET s = 0<br>
PRINT "Enter one value at a time: "<br>
WHILE b < a REPEAT<br>
    &emsp;INPUT c<br>
    &emsp;LET s = s + c<br>
    &emsp;LET b = b + 1<br>
ENDWHILE<br>

PRINT "Average: "<br>
PRINT s / a<br>

Compiler-generated code:<br>

#include<stdio.h><br>
int main(void){<br>
float a;<br>
float b;<br>
float s;<br>
float c;<br>
a=0;<br>
while(a<1){<br>
printf("Enter number of scores: \n");<br>
if(0 == scanf("%f",&a)) {<br>
a = 0;<br>
scanf("%*s");<br>
}<br>
}<br>
b=0;<br>
s=0;<br>
printf("Enter one value at a time: \n");<br>
while(b<a){<br>
if(0 == scanf("%f",&c)) {<br>
c = 0;<br>
scanf("%*s");<br>
}<br>
s=s+c;<br>
b=b+1;<br>
}<br>
printf("Average: \n");<br>
printf("%.2f\n", (float)(s/a));<br>
printf("Press Enter to exit...\n");<br>
getchar();<br>
getchar();<br>
return 0;<br>
}<br>


Setup : <br>
--install gcc compiler
<br>
Optional:<br>
-- Try to keep this on c drive so you won't face any access issues.
<br>
<br>
Peace
