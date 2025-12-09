using System;
using System.Runtime.CompilerServices;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Extensions;

namespace Odotocodot.OneNote.Linq.Internal
{
    internal static class Throw
    {
#nullable enable
        internal static void IfNull<T>(T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
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
            if (parent.IsInRecycleBin())
            {
                throw new ArgumentException("Cannot create OneNote items if their parent is in the Recycle Bin.", paramName);
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
