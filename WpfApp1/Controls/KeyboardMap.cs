using System.Windows.Input;

namespace WpfApp1.Controls;

public static class KeyboardMap
{
    private const char FirstLetter = 'a';
    private const char FirstDigit = '0';
    private const int MinimumPrintableCode = 0x20;
    private const int MaximumPrintableCode = 0x7E;

    private static readonly char[] ShiftedDigits = [')', '!', '@', '#', '$', '%', '^', '&', '*', '('];

    public static bool TryGetCharacter(Key key, Key systemKey, ModifierKeys modifiers, out char character)
    {
        if (key == Key.Space)
        {
            character = ' ';
            return true;
        }

        if (TryGetLetter(key, out character))
        {
            return true;
        }

        if (TryGetDigit(key, modifiers, out character))
        {
            return true;
        }

        if (IsPunctuationKey(key) && TryGetSystemCharacter(systemKey, out character))
        {
            return true;
        }

        character = '\0';
        return false;
    }

    private static bool TryGetLetter(Key key, out char character)
    {
        if (key is >= Key.A and <= Key.Z)
        {
            character = (char)(FirstLetter + (key - Key.A));
            return true;
        }

        character = '\0';
        return false;
    }

    private static bool TryGetDigit(Key key, ModifierKeys modifiers, out char character)
    {
        if (key is >= Key.D0 and <= Key.D9)
        {
            var digit = key - Key.D0;
            var isShifted = (modifiers & ModifierKeys.Shift) != 0;
            character = isShifted ? ShiftedDigits[digit] : (char)(FirstDigit + digit);
            return true;
        }

        if (key is >= Key.NumPad0 and <= Key.NumPad9)
        {
            character = (char)(FirstDigit + (key - Key.NumPad0));
            return true;
        }

        character = '\0';
        return false;
    }

    private static bool IsPunctuationKey(Key key) => key is
        Key.OemSemicolon or
        Key.OemPlus or
        Key.OemComma or
        Key.OemMinus or
        Key.OemPeriod or
        Key.OemQuestion or
        Key.OemTilde or
        Key.DeadCharProcessed or
        Key.Oem102 or
        Key.OemOpenBrackets or
        Key.OemPipe or
        Key.OemCloseBrackets or
        Key.OemQuotes;

    private static bool TryGetSystemCharacter(Key systemKey, out char character)
    {
        if ((int)systemKey is >= MinimumPrintableCode and <= MaximumPrintableCode)
        {
            character = (char)systemKey;
            return true;
        }

        character = '\0';
        return false;
    }
}
