using System;
using System.Runtime.CompilerServices;
using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq.Internal
{
    internal static class Throw
    {
#nullable enable
        internal static void IfNull<T>(T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
        }

        public static void IfNullOrWhiteSpace(string? name, [CallerArgumentExpression(nameof(name))] string? paramName = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, paramName);
        }

        internal static void IfInvalidSearch(string? search, [CallerArgumentExpression(nameof(search))] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(search, paramName);
            ArgumentException.ThrowIfNullOrWhiteSpace(search, paramName);
            if (!char.IsLetterOrDigit(search[0]))
            {
                throw new ArgumentException("Search string must start with a letter or digit", paramName);
            }
        }

        internal static void IfInvalidParent<T>(T? parent, string? messageExtra = null, [CallerArgumentExpression(nameof(parent))] string? paramName = null) where T : IOneNoteItem
        {
            if (parent == null)
            {
                throw new ArgumentNullException(paramName, $"Parameter '{paramName}' cannot be null. {messageExtra}");
            }
            IfInRecycleBin(parent, "Cannot create OneNote items if their parent is in the Recycle Bin.");
        }

        internal static void IfInRecycleBin<T>(T item, string message, [CallerArgumentExpression(nameof(item))] string? paramName = null) where T : IOneNoteItem
        {
            if (item.IsInRecycleBin())
            {
                throw new ArgumentException(message + $" Consider checking with the {nameof(Extensions.IsInRecycleBin)}() extension method.", paramName);
            }
        }

        internal static void IfInvalidName<T>(string? name) where T : INameInvalidCharacters
        {
            if (!OneNote.IsValidName<T>(name))
            {
                throw new ArgumentException($"Invalid {nameof(T).ToLower()} name provided: \"{name}\". {nameof(T)} names cannot empty, only whitespace or contain the symbols: \t {string.Join(" ", T.InvalidCharacters)}");
            }
        }
    }
}
