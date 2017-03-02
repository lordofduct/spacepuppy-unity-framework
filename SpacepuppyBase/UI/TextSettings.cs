using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace com.spacepuppy.UI
{

    [System.Serializable]
    public struct TextSettings
    {

        [Header("Character")]
        public Color Color;
        public Font Font;
        public FontStyle FontStyle;
        public int FontSize;
        public float LineSpacing;
        public bool RichText;
        [Header("Paragraph")]
        public TextAnchor Alignment;
        public bool AlignByGeometry;
        public HorizontalWrapMode HorizontalOverflow;
        public VerticalWrapMode VerticalOverflow;
        public bool BestFit;

        public void Apply(Text txt)
        {
            if (txt == null) throw new System.ArgumentNullException("txt");

            txt.color = this.Color;
            txt.font = this.Font;
            txt.fontStyle = this.FontStyle;
            txt.fontSize = this.FontSize;
            txt.lineSpacing = this.LineSpacing;
            txt.supportRichText = this.RichText;
            txt.alignment = this.Alignment;
            txt.alignByGeometry = this.AlignByGeometry;
            txt.horizontalOverflow = this.HorizontalOverflow;
            txt.verticalOverflow = this.VerticalOverflow;
            txt.resizeTextForBestFit = this.BestFit;
        }

        public static TextSettings FromText(Text txt)
        {
            if (txt == null) throw new System.ArgumentNullException("txt");

            var settings = new TextSettings();
            settings.Color = txt.color;
            settings.Font = txt.font;
            settings.FontStyle = txt.fontStyle;
            settings.FontSize = txt.fontSize;
            settings.LineSpacing = txt.lineSpacing;
            settings.RichText = txt.supportRichText;
            settings.Alignment = txt.alignment;
            settings.AlignByGeometry = txt.alignByGeometry;
            settings.HorizontalOverflow = txt.horizontalOverflow;
            settings.VerticalOverflow = txt.verticalOverflow;
            settings.BestFit = txt.resizeTextForBestFit;
            return settings;
        }

    }
}
