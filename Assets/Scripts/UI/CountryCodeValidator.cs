using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "CountryCodeValidator", menuName = "CountryCodeValidator")]
public class CountryCodeValidator : TMP_InputValidator
{
    private bool IsEnglishLetter(char ch)
    {
        if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
        {
            return true;
        }
        return false;
    }

    public override char Validate(ref string text, ref int pos, char ch)
    {
        int length = text.Length;
        if (!IsEnglishLetter(ch) || length >= 3) { return '\0'; }

        if (ch >= 'a' && ch <= 'z') { ch = (char)((int)ch - 'a' + 'A'); }
        text += ch;
        pos++;
        return ch;
    }
}