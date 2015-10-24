using System.Diagnostics.Contracts;

namespace TSharp.Compilation.Tokenization
{
	public sealed class Token
	{
		public TokenKind Kind { get; private set; }
		public SourceSpan Span { get; private set; }

		public Token(TokenKind kind, SourceSpan span)
		{
			Kind = kind;
			Span = span;
		}
	}
}