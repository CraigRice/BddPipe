﻿namespace BddPipe
{
    internal sealed class Title
    {
        public Step Step { get; }
        public Option<string> Text { get; }

        public Title(Step step, Option<string> text)
        {
            Step = step;
            Text = text;
        }
    }
}
