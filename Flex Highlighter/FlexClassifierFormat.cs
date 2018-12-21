using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Flex_Highlighter
{
    #region Standard Regex Classifiers
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Flex Definition")]
    [Name("Flex Definition")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.Default, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class FlexClassifierFormat : ClassificationFormatDefinition
    {
        public FlexClassifierFormat()
        {
            this.DisplayName = "Flex Definition"; // Human readable version of the name
            this.ForegroundColor = Color.FromRgb(189, 99, 197);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Special Character")]
    [Name("Regex Special Character")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.Default, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexSpecialCharacter : ClassificationFormatDefinition
    {
        public RegexSpecialCharacter()
        {
            this.DisplayName = "Regex Special Character"; // Human readable version of the name
            this.ForegroundColor = DefaultColors.SpecialCharacterForeground;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Letters")]
    [Name("Regex Letters")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.Default, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexLetters : ClassificationFormatDefinition
    {
        public RegexLetters()
        {
            this.DisplayName = "Regex Letters"; // Human readable version of the name
            this.ForegroundColor = Colors.LightSeaGreen;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Escaped Character")]
    [Name("Regex Escaped Character")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.Default, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexEscapedCharacter : ClassificationFormatDefinition
    {
        public RegexEscapedCharacter()
        {
            this.DisplayName = "Regex Escaped Character"; // Human readable version of the name
            this.ForegroundColor = DefaultColors.EscapedCharacterForeground;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Quantifier")]
    [Name("Regex Quantifier")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.High, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexQuantifier : ClassificationFormatDefinition
    {
        public RegexQuantifier()
        {
            this.DisplayName = "Regex Quantifier"; // Human readable version of the name
            this.ForegroundColor = Color.FromRgb(0, 195, 195);
            this.BackgroundColor = Color.FromRgb((byte)(Colors.LightSkyBlue.R / 4), (byte)(Colors.LightSkyBlue.G / 4), (byte)(Colors.LightSkyBlue.B / 4));
        }
    }

    #endregion

    #region Regex Group Classifiers
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Group")]
    [Name("Regex Group")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.High, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexGroup : ClassificationFormatDefinition
    {
        public RegexGroup()
        {
            this.DisplayName = "Regex Group"; // Human readable version of the name
            this.ForegroundColor = Color.FromRgb(83, 184, 2);
            this.BackgroundColor = DefaultColors.RegexGroupBackground;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Special Character in Group")]
    [Name("Regex Special Character in Group")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.Default, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexSpecialCharacterInGroup : ClassificationFormatDefinition
    {
        public RegexSpecialCharacterInGroup()
        {
            this.DisplayName = "Regex Special Character in Group"; // Human readable version of the name
            this.ForegroundColor = DefaultColors.SpecialCharacterForeground;
            this.BackgroundColor = DefaultColors.RegexGroupBackground;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Flex Definition in Group")]
    [Name("Flex Definition in Group")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.Default, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class FlexDefinitionInGroup : ClassificationFormatDefinition
    {
        public FlexDefinitionInGroup()
        {
            this.DisplayName = "Flex Definition in Group"; // Human readable version of the name
            this.ForegroundColor = Color.FromRgb(189, 99, 197);
            this.BackgroundColor = DefaultColors.RegexGroupBackground;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Letters in Group")]
    [Name("Regex Letters in Group")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.Default, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexLettersInGroup : ClassificationFormatDefinition
    {
        public RegexLettersInGroup()
        {
            this.DisplayName = "Regex Letters in Group"; // Human readable version of the name
            this.ForegroundColor = Color.FromRgb(189, 99, 197);
            this.BackgroundColor = DefaultColors.RegexGroupBackground;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Escaped Character in Regex Group")]
    [Name("Regex Escaped Character in Regex Group")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.High, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class EscapedCharacterInRegexGroup : ClassificationFormatDefinition
    {
        public EscapedCharacterInRegexGroup()
        {
            this.DisplayName = "Regex Escaped Character in Regex Group"; // Human readable version of the name
            this.ForegroundColor = DefaultColors.EscapedCharacterForeground;
            this.BackgroundColor = DefaultColors.RegexGroupBackground;
        }
    }
    #endregion

    #region Regex Character Set Classifiers
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Character Set")]
    [Name("Regex Character Set")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.High, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexCharacterSet : ClassificationFormatDefinition
    {
        public RegexCharacterSet()
        {
            this.DisplayName = "Regex Character Set"; // Human readable version of the name
            this.ForegroundColor = Color.FromRgb(255, 170, 0);
            this.BackgroundColor = DefaultColors.RegexCharacterSetBackground;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Digits in Character Set")]
    [Name("Regex Digits in Character Set")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.Default, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class RegexDigitsInSet : ClassificationFormatDefinition
    {
        public RegexDigitsInSet()
        {
            this.DisplayName = "Regex Digits in Character Set"; // Human readable version of the name
            this.ForegroundColor = Colors.Aqua;
            this.BackgroundColor = DefaultColors.RegexCharacterSetBackground;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Regex Escaped Character in Character Set")]
    [Name("Escaped Character in Character Set")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(After = Priority.High, Before = Priority.High)] // Set the priority to be after the default classifiers
    internal sealed class EscapedCharacterInCharacterSet : ClassificationFormatDefinition
    {
        public EscapedCharacterInCharacterSet()
        {
            this.DisplayName = "Regex Escaped Character in Character Set"; // Human readable version of the name
            this.ForegroundColor = DefaultColors.EscapedCharacterForeground;
            this.BackgroundColor = DefaultColors.RegexCharacterSetBackground;
        }
    }
    #endregion
}
