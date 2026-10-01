using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1.Controls;

public static class CaretScroll
{
    private static readonly ConditionalWeakTable<LetterViewModel, FrameworkElement> Elements = new();

    public static readonly DependencyProperty CaretProperty =
        DependencyProperty.RegisterAttached(
            "Caret",
            typeof(LetterViewModel),
            typeof(CaretScroll),
            new PropertyMetadata(OnCaretChanged));

    public static void SetCaret(DependencyObject element, LetterViewModel? value) =>
        element.SetValue(CaretProperty, value);

    public static LetterViewModel? GetCaret(DependencyObject element) => (LetterViewModel?)element.GetValue(CaretProperty);

    private static void OnCaretChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is LetterViewModel previous)
        {
            previous.PropertyChanged -= OnLetterPropertyChanged;
            Elements.Remove(previous);
        }

        if (e.NewValue is not LetterViewModel current)
        {
            return;
        }

        current.PropertyChanged += OnLetterPropertyChanged;

        if (sender is FrameworkElement element)
        {
            Elements.Add(current, element);
        }

        ScrollIntoView(current);
    }

    private static void OnLetterPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LetterViewModel.IsCurrent) && sender is LetterViewModel letter)
        {
            ScrollIntoView(letter);
        }
    }

    private static void ScrollIntoView(LetterViewModel letter)
    {
        if (letter.IsCurrent && Elements.TryGetValue(letter, out var element))
        {
            element.BringIntoView();
        }
    }
}
