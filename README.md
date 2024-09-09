# Compiler 
Welcome to my repo!<br>
I am working on a compiler that converts BASIC to C why you ask cause I am psycho...................

BASIC code:
PRINT "How many fibonacci numbers do you want?"
INPUT nums
PRINT ""

LET a = 0
LET b = 1
WHILE nums > 0 REPEAT
    PRINT a
    LET c = a + b
    LET a = b
    LET b = c
    LET nums = nums - 1
ENDWHILE

Compiler-generated code:

#include<stdio.h>

int main(void){

float nums;

float a;

float b;

float c;

float nums;

printf("How many fibonacci numbers do you want?\n");

if(0 == scanf("%f",&nums(( {

nums = 0;

scanfprintf("\n");

a=;

b=;

while(){

printf("%.2f\n", (float)());

c=;

a=;

b=;

nums=;

}

return 0;

}

I know not perfect but working on it.

