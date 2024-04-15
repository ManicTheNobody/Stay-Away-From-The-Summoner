using Godot;
using System;
using System.Globalization;

public static class HelperFunctions{
    private static string[] ones = new string[]{"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"};
    private static string[] tens = new string[]{"zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"};
    public static string IntToEnglish(int input){
        if(input == 0){
            return ones[0];
        }
        if(input < 0){
            return "negative " + IntToEnglish(Math.Abs(input));
        }
        string ret = "";
        if(input/100 > 0){
            ret += IntToEnglish(input / 100) + " hundred ";
            input %= 100;
        }
        if(input > 0){
            if(input < 20){
                ret += ones[input];
            }
            else{
                ret += tens[input/10];
                if(input%10>0){
                    ret+=" " + ones[input%10];
                }
            }
        }
        return ret;
    }
}