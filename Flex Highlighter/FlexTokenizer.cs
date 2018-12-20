using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using System.Diagnostics;

namespace Flex_Highlighter
{
    internal sealed class FlexTokenizer
    {

        internal static class Classes
        {
            internal readonly static short WhiteSpace = 0;
            internal readonly static short Keyword = 1;
            internal readonly static short MultiLineComment = 2;
            internal readonly static short Comment = 3;
            internal readonly static short NumberLiteral = 4;
            internal readonly static short StringLiteral = 5;
            internal readonly static short ExcludedCode = 6;
            internal readonly static short FlexDefinition = 7;
            internal readonly static short RegexQuantifier = 8;
            internal readonly static short EscapedCharacter = 9;
            internal readonly static short RegexGroup = 10;
            internal readonly static short RegexCharacterSet = 11;
            internal readonly static short RegexLetters = 12;
            internal readonly static short RegexSpecialCharacters = 13;
            internal readonly static short Other = -1;
            internal readonly static short C = -2;
            internal readonly static short FlexDefinitions = -3;
            internal readonly static short CIndent = -4;
            internal readonly static short FlexRules = -5;
            internal readonly static short CEnding = -6;
        }
        internal FlexTokenizer(IStandardClassificationService classifications) => Classifications = classifications;
        internal IStandardClassificationService Classifications { get; }
        internal static List<string> FlexDefinitions = new List<string>();
        internal static List<string> CDefinitions = new List<string>();

        internal Token Scan(string text, int startIndex, int length, ref Languages language, ref Cases ecase, List<int[]> innerSections, int startTokenId = -1, int startState = 0)
        {
            int index = startIndex;
            Token token = new Token();
            token.StartIndex = index;
            token.TokenId = startTokenId;
            token.State = startState;
            token.Length = length - index;
            if (index >= text.Length)
                return token;

            if (language == Languages.Regex || language == Languages.Flex)
            {
                var matches = Regex.Matches(text, @"{[0-9]+,*[0-9]*}");
                if (matches.Count != 0 && index > 0)
                {
                    foreach (Match match in matches)
                    {
                        if(match.Index == index)
                        {
                            token.TokenId = Classes.RegexQuantifier;
                            token.Length = match.Length;
                            return token;
                        }
                    }
                }

                if (text[index] == '(')
                {
                    int numberOfBrackets = 1;
                    index++;
                    while (index < length && text[index] != '\n')
                    {
                        if (text[index] == '(' && index - 1 >= 0 && text[index - 1] != '\\')
                        {
                            numberOfBrackets++;
                        }
                        else if (text[index] == ')' && index - 1 >= 0 && text[index - 1] != '\\')
                        {
                            numberOfBrackets--;
                            if (numberOfBrackets >= 0)
                            {
                                token.Length = ++index - token.StartIndex;
                                token.TokenId = Classes.RegexGroup;
                                return token;
                            }
                        }
                        index++;
                    }
                    index = startIndex;
                }

                var regex = new Regex(@"\[[^\n\]]*(?<!\\)\]");
                matches = regex.Matches(text, index);
                if (matches.Count != 0)
                {
                    foreach (Match match in matches)
                    {
                        if (match.Index == index)
                        {
                            token.TokenId = Classes.RegexCharacterSet;
                            token.Length = match.Length;
                            return token;
                        }
                    }
                }

                foreach (var escapeCharacter in FlexKeywords.AllEscapedCharacters)
                {
                    matches = Regex.Matches(text, escapeCharacter);
                    if (matches.Count != 0)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Index == index)
                            {
                                token.TokenId = Classes.EscapedCharacter;
                                token.Length = match.Length;
                                return token;
                            }
                        }
                    }
                }
            }

            if (index + 1 < length && text[index] == '/' && text[index + 1] == '/')
            {
                token.TokenId = Classes.Comment;
                return token;
            }

            if ((index + 1 < length && text[index] == '/' && text[index + 1] == '*') || token.State == (int)Cases.MultiLineComment)
            {
                //if (index + 1 < length && text[index] == '/' && text[index + 1] == '*')
                {
                    index += 2;
                    token.State = (int)Cases.MultiLineComment;
                    token.TokenId = Classes.MultiLineComment;
                }

                while (index < length)
                {
                    index = AdvanceWhile(text, ++index, chr => chr != '*');
                    if (index + 1 < length && text[index + 1] == '/')
                    {
                        token.State = (int)Cases.NoCase;
                        token.Length = index + 2 - startIndex;
                        token.TokenId = Classes.MultiLineComment;
                        return token;
                    }
                }
                return token;
            }


            int start = index;
            if (((index + 1 < length && text[index] == '%' && text[index + 1] == '{' && index == 0) || token.State == (int)Cases.C) && language == Languages.FlexDefinitions)
            {
                if ((index + 1 < length && text[index] == '%' && text[index + 1] == '{'))
                {
                    index += 2;
                    token.State = (int)Cases.C;
                    token.TokenId = Classes.C;
                }

                while (index < length)
                {
                    index = AdvanceWhile(text, index, chr => chr != '%');
                    if (index + 1 < length && text[index + 1] == '}' && (index - 1 > 0 && text[index - 1] == '\n') && !IsBetween(index, innerSections))
                    {
                        index += 2;
                        token.StartIndex = start;
                        token.State = (int)Cases.NoCase;
                        token.TokenId = Classes.C;
                        token.Length = index - start;
                        return token;
                    }
                    index++;
                    if (index >= length)
                    {
                        index += 2;
                        token.StartIndex = start;
                        token.TokenId = Classes.C;
                        return token;
                    }
                }
            }

            if (language == Languages.NoLanguage)
            {
                token.State = (int)Cases.FlexDefinitions;
                while (index <= length)
                {
                    index = AdvanceWhile(text, index, chr => chr != '%');
                    if (index + 1 < length && text[index + 1] == '%' && !IsBetween(index, innerSections) && index - 1 > 0 && text[index - 1] == '\n')
                    {
                        index += 2;
                        token.StartIndex = start;
                        token.State = (int)Cases.NoCase;
                        token.TokenId = Classes.FlexDefinitions;
                        token.Length = index - start;
                        return token;
                    }
                    if (index >= length)
                    {
                        token.StartIndex = start;
                        token.State = (int)Cases.NoCase;
                        token.TokenId = Classes.FlexDefinitions;
                        token.Length = index - start;
                        return token;
                    }
                    index++;
                }
            }

            index = start;
            if ((index < length && text[index] == '\t' || token.State == (int)Cases.CIndent) && language == Languages.Flex)
            {
                if (text[index] == '\t')
                {
                    index++;
                    token.TokenId = Classes.CIndent;
                    language = Languages.C;
                }
                while (index < length)
                {
                    index = AdvanceWhile(text, index, chr => chr == '\t');
                    token.StartIndex = start;
                    token.State = (int)Cases.NoCase;
                    token.TokenId = Classes.Other;
                    token.Length = index - start;
                    return token;
                }
            }

            if ((index < length && text[index] == '\t' || token.State == (int)Cases.CIndent) && language == Languages.FlexDefinitions)
            {
                if (text[index] == '\t')
                {
                    index++;
                    token.TokenId = Classes.CIndent;
                    language = Languages.Regex;
                }
                while (index < length)
                {
                    index = AdvanceWhile(text, index, chr => chr == '\t');
                    token.StartIndex = start;
                    token.State = (int)Cases.NoCase;
                    token.TokenId = Classes.Other;
                    token.Length = index - start;
                    return token;
                }
            }

            index = start;
            if (((index + 1 < length && text[index] == '%' && text[index + 1] == '%' && index == 0) || token.State == (int)Cases.FlexRules) && language == Languages.FlexDefinitions)
            {
                index += 2;
                token.State = (int)Cases.FlexRules;
                token.TokenId = Classes.FlexRules;

                while (index < length)
                {
                    index = AdvanceWhile(text, index, chr => chr != '%');
                    if (index + 1 < length && text[index + 1] == '%' && text[index - 1] == '\n' && !IsBetween(index, innerSections))
                    {
                        //index += 2;
                        token.StartIndex = start;
                        token.TokenId = Classes.FlexRules;
                        token.State = (int)Cases.NoCase;
                        token.Length = index - start;
                        return token;
                    }
                    index++;
                    if (index >= length)
                    {
                        token.StartIndex = start;
                        return token;
                    }
                }
            }

            if (((index + 1 < length && text[index] == '%' && text[index + 1] == '%' && index == 0) || token.State == (int)Cases.CEnding) && language == Languages.Flex)
            {
                index += 2;
                token.State = (int)Cases.CEnding;
                token.TokenId = Classes.CEnding;
                token.StartIndex = index;
                return token;
            }

            index = start;
            if(language == Languages.Flex)
                index = AdvanceWhile(text, index, chr => chr == ' ');
            else
                index = AdvanceWhile(text, index, chr => Char.IsWhiteSpace(chr));

            if (index > start)
            {
                token.TokenId = Classes.WhiteSpace;
                token.Length = index - start;
                return token;
            }

            if (language == Languages.FlexDefinitions)
            {
                if (start == 0 || (index - 1 > 0 && text[index - 1] == '\n'))
                {
                    index = start;
                    if (text[index] == '_' || Char.IsLetter(text[index]))
                    {
                        index++;
                        index = AdvanceWhileDefinition(text, index);
                        if ((index < text.Length && Char.IsWhiteSpace(text[index])) || index == length)
                        {
                            var s = new string(text.ToCharArray(), start, index - start);
                            if (!FlexDefinitions.Contains(s))
                                FlexDefinitions.Add(s);
                            token.Length = index - start;
                            token.TokenId = Classes.FlexDefinition;
                            return token;
                        }
                    }

                }
                index = start;
            }

            if (language == Languages.C || language == Languages.CEnding)
            {
                if (text[index] == '\"')
                {
                    index = AdvanceWhile(text, ++index, chr => chr != '\"');
                    token.TokenId = Classes.StringLiteral;
                    token.Length = index - start + (text.IndexOf('\"', index) != -1 ? 1 : 0);
                    return token;
                }

                if (ecase == Cases.Include)
                {
                    if (text[index] == '<')
                    {
                        index = AdvanceWhile(text, index, chr => chr != '>');
                        token.TokenId = Classes.StringLiteral;
                        token.Length = index - token.StartIndex + (text.IndexOf('>', start) != -1 ? 1 : 0);
                        ecase = Cases.NoCase;
                        return token;
                    }
                }
                if (ecase == Cases.CMacro)
                {
                    index = start;
                    if (text[index] == '_' || Char.IsLetter(text[index]))
                    {
                        index++;
                        index = AdvanceWhileDefinition(text, index);
                        if ((index <= text.Length /*&& Char.IsWhiteSpace(text[index])) || index == length*/))
                        {
                            var s = new string(text.ToCharArray(), start, index - start);
                            if (!CDefinitions.Contains(s))
                                CDefinitions.Add(s);
                            token.Length = index - start;
                            token.TokenId = Classes.FlexDefinition;
                            ecase = Cases.NoCase;
                            return token;
                        }
                    }
                    //index = AdvanceWhile(text, index, chr => !Char.IsWhiteSpace(chr));
                    //index = 
                    //token.TokenId = Classes.FlexDefinition;
                    //token.Length = index - start;
                    //ecase = Cases.NoCase;
                    //return token;
                }

                string[] test = { @"#include\s", @"#pragma\s", @"#define\s", @"#if\s", @"#endif\s", @"#undef\s", @"#ifdef\s", @"#ifndef\s", @"#else\s", @"#elif\s", @"#error\s" };
                foreach (var s in test)
                {
                    int i = -1;
                    foreach (Match match in new Regex(s).Matches(text))
                    {
                        if (match.Index == index)
                        {
                            i = index;
                        }
                    }
                    if (i == index)
                    {
                        token.TokenId = Classes.ExcludedCode;
                        token.Length = s.Length - 1;
                        switch (s)
                        {
                            case @"#include\s":
                                ecase = Cases.Include;
                                return token;
                            case @"#define\s":
                                ecase = Cases.CMacro;
                                return token;
                            case @"#if\s":
                                ecase = Cases.CMacro;
                                return token;
                            default:
                                return token;
                        }
                    }
                }

                foreach (var definition in CDefinitions)
                {
                    foreach (Match match in new Regex($"{definition}[^A-Za-z0-9]").Matches(text))
                    {
                        if (match.Index == index)
                        {
                            token.TokenId = Classes.FlexDefinition;
                            token.Length = definition.Length;
                            ecase = Cases.NoCase;
                            return token;
                        }
                    }
                }
            }

            if (language == Languages.Flex)
            {
                if (ecase == Cases.DefinitionUsed)
                {
                    foreach (var definition in FlexDefinitions)
                    {
                        foreach (Match match in new Regex($"{{{definition}}}").Matches(text))
                        {
                            if (match.Index == index - 1)
                            {
                                token.TokenId = Classes.FlexDefinition;
                                token.Length = definition.Length;
                                ecase = Cases.NoCase;
                                return token;
                            }
                        }
                    }
                }
                if (text[index] == '{')
                {
                    ecase = Cases.DefinitionUsed;
                }
            }

            if (language == Languages.FlexDefinitions)
            {
                if (ecase == Cases.Option)
                {
                    token.TokenId = Classes.Other;
                    token.Length = length - index;
                    return token;
                }
                
                string[] test = { @"%option\s" };
                foreach (var s in test)
                {
                    int i = -1;
                    foreach (Match match in new Regex(s).Matches(text))
                    {
                        if (match.Index == 0)
                        {
                            i = index;
                        }
                    }
                    if (i == index)
                    {
                        token.Length = s.Length - 1;
                        token.TokenId = Classes.ExcludedCode;
                        ecase = Cases.Option;
                        switch (s)
                        {
                            case @"%option\s":
                                return token;
                            default:
                                return token;
                        }
                    }
                }
            }

            start = index;
            if (Char.IsDigit(text[index]))
            {
                index = AdvanceWhile(text, index, chr => Char.IsDigit(chr));
            }
            else if (Char.IsLetter(text[index]))
            {
                index = AdvanceWhile(text, index, chr => Char.IsLetter(chr));
            }
            else
            {
                index++;
            }
            string word = text.Substring(start, index - start);
            if (IsDecimalInteger(word))
            {
                token.TokenId = Classes.NumberLiteral;
                token.Length = index - start;
                return token;
            }
            else
            {
                if (language == Languages.C || language == Languages.CEnding)
                {
                    token.TokenId = FlexKeywords.CContains(word) ? Classes.Keyword : Classes.Other;
                }
                else if (language == Languages.Flex || language == Languages.Regex)
                {
                    if (Regex.IsMatch(word, "^[A-Za-z]+$"))
                    {
                        token.TokenId = Classes.RegexLetters;
                    }
                    else
                    {
                        token.TokenId = FlexKeywords.SpecialCharactersContains(word) ? Classes.RegexSpecialCharacters : Classes.Other;
                    }
                }
                else
                {
                    token.TokenId = FlexKeywords.FlexContains(word) ? Classes.Keyword : Classes.Other;
                }
                token.Length = index - start;
            }
            return token;
        }

        internal static bool IsDecimalInteger(string word)
        {
            foreach (var chr in word)
            {
                if (chr < '0' || chr > '9')
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsBetween(int value, int x1, int x2)
        {
            if (value >= x1 && value <= x2)
            {
                return true;
            }
            return false;
        }
        private bool IsBetween(int value, Tuple<int, int> range)
        {
            if (value >= range.Item1 && value <= range.Item2)
            {
                return true;
            }
            return false;
        }

        private bool IsBetween(int value, List<int[]> innerSections)
        {
            foreach (var range in innerSections)
                if (value >= range[0] && value <= range[1])
                {
                    return true;
                }

            return false;
        }

        internal static int AdvanceWhile(string text, int index, Func<char, bool> predicate)
        {
            for (int length = text.Length; index < length && predicate(text[index]); index++) ;
            return index;
        }

        private int AdvanceWhileDefinition(string text, int index)
        {
            for (int length = text.Length; index < length; index++)
            {
                if (!(Char.IsLetterOrDigit(text[index]) || text[index] == '_'))
                {
                    break;
                }
            }
            return index;
        }
    }
}
