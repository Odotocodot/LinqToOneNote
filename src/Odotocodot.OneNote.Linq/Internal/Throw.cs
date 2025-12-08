using System;
using System.Diagnostics.CodeAnalysis;
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

        internal static void IfInvalidSearch(string? search, [CallerArgumentExpression(nameof(search))] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(search, paramName);
            ArgumentException.ThrowIfNullOrWhiteSpace(search, paramName);
            if (!char.IsLetterOrDigit(search[0]))
            {
                throw new ArgumentException("Search string must start with a letter or digit", paramName);
            }
        }

        internal static void IfNullSection(Section? section)
        {
            throw new ArgumentNullException(nameof(section), $"Parameter 'section' cannot be null. Use {nameof(OneNote)}.{nameof(OneNote.CreateQuickNote)} instead.");
        }

        internal static void IfInvalidName<T>(string? name) where T : INameInvalidCharacters
        {
            if (!OneNote.IsValidName<T>(name))
            {
                throw new ArgumentException($"Invalid {nameof(T).ToLower()} name provided: \"{name}\". {nameof(T)} names cannot empty, only whitespace or contain the symbols: \t {string.Join(" ", T.InvalidCharacters)}");
            }
        }

#nullable restore
        internal static IOneNoteItem InvalidXmlElement(string elementName)
        {
            throw new InvalidOperationException($"The XML element '{elementName}' is not valid in the current context.");
        }

        internal static IOneNoteItem InvalidXmlNodeType(string nodeType)
        {
            throw new InvalidOperationException($"The XML node type '{nodeType}' is not valid in the current context.");
        }

        internal static void InvalidIOneNoteItem(IOneNoteItem item)
        {
            throw new InvalidOperationException($"'{item.GetType().Name}' is not a valid OneNote item type.");
        }
    }
}
