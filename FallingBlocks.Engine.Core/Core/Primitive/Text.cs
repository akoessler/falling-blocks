namespace FallingBlocks.Engine.Core.Core.Primitive
{
    public class Text2D : ARenderObject
    {
        public TextFont Font { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// ctor.
        /// </summary>
        public Text2D()
        {
        }

        /// <inheritdoc/>
        protected override void RenderInternal(IRenderContext context)
        {
            context.Render(this);
        }


        /// <summary>
        /// Must use a generic font definition for GDI and OGL.
        /// 
        /// These are the fonts exposed by OGL with constants.
        /// (Maybe there are more...)
        /// </summary>
        public enum TextFont
        {
            FixedSize_8,
            FixedSize_9,
            TimesRoman_10,
            TimesRoman_24,
            Helvetica_10,
            Helvetica_12,
            Helvetica_18
        }
    }
}