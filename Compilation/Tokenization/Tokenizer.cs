using System.Collections.Immutable;
using TSharp.Compilation.Utils;

namespace TSharp.Compilation.Tokenization
{
	public sealed class Tokenizer
	{
		private delegate bool Recognizer(SourceCode source, int start, out int exclusiveStop);

		private static readonly IImmutableDictionary<TokenKind, Recognizer> KindRecognizers = new ImmutableDictionaryBuilder<TokenKind, Recognizer>()
		{
			{ TokenKind.Word, WordRecognizer },
			{ TokenKind.Period, NewExactRecognizer(SourceCode.FromText(".")) },
			{ TokenKind.Semicolon, NewExactRecognizer(SourceCode.FromText(";")) },
			{ TokenKind.QuestionMark, NewExactRecognizer(SourceCode.FromText("?")) },
			{ TokenKind.BracketOpen, NewExactRecognizer(SourceCode.FromText("[")) },
			{ TokenKind.BracketClose, NewExactRecognizer(SourceCode.FromText("]")) },
			{ TokenKind.BraceOpen, NewExactRecognizer(SourceCode.FromText("{")) },
			{ TokenKind.BraceClose, NewExactRecognizer(SourceCode.FromText("}")) },
			{ TokenKind.ParenthesisOpen, NewExactRecognizer(SourceCode.FromText("(")) },
			{ TokenKind.ParenthesisClose, NewExactRecognizer(SourceCode.FromText(")")) }
		}.Build();

		private static readonly IImmutableList<TokenKind> KindOrder = new ImmutableListBuilder<TokenKind>
		{
			TokenKind.Word,
			TokenKind.Period,
			TokenKind.Semicolon,
			TokenKind.QuestionMark,
			TokenKind.BracketOpen,
			TokenKind.BracketClose,
			TokenKind.BraceOpen,
			TokenKind.BraceClose,
			TokenKind.ParenthesisOpen,
			TokenKind.ParenthesisClose
		}.Build();

		public IImmutableList<Token> Tokenize(SourceCode source)
		{
			ImmutableList<Token>.Builder builder = ImmutableList.CreateBuilder<Token>();

			int offset = 0;
			while (offset < source.Length)
			{
				foreach(var kind in KindOrder)
				{
					int exclusiveStop;
					if (KindRecognizers[kind](source, offset, out exclusiveStop))
					{
						int span = exclusiveStop - offset;
						builder.Add(new Token(kind, span));
						offset += span;
						break;
					}
				}
			}

			return builder.ToImmutable();
		}

		private static bool WordRecognizer(SourceCode source, int start, out int exclusiveStop)
		{
			exclusiveStop = start;

			var first = source[start];
			if (first.Length > 1 || !(first[0] >= 'a' && first [0] <= 'z') || !(first[0] >= 'A' && first[0] <= 'Z'))
			{
				return false;
			}
			exclusiveStop = start + 1;

			int offset = 1;
			while (start + offset < source.Length)
			{
				var current = source[start + offset];
				if (current.Length > 1 || !(current[0] >= 'a' && current[0] <= 'z') || !(current[0] >= 'A' && current[0] <= 'Z'))
				{
					return false;
				}
				exclusiveStop = start + offset + 1;
				offset++;
			}

			return true;
		}

		private static Recognizer NewExactRecognizer(SourceCode lexeme)
		{
			return (SourceCode source, int start, out int exclusiveStop) =>
			{
				exclusiveStop = start;
				for (int i = 0; i < lexeme.Length; i++)
				{
					if (!string.Equals(source[start + i], lexeme[i]))
					{
						return false;
					}
				}
				exclusiveStop = start + lexeme.Length;
				return true;
			};
		}
	}
}