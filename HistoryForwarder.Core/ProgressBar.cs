using System;

namespace HistoryForwarder.Core
{
    public class ProgressBar: IDisposable
    {
        private readonly int ConsoleLine;
        private readonly string Label;
        private readonly int TotalItems;
        private int Count;

        public ProgressBar(string label, int totalElements)
        {
            Console.WriteLine(string.Empty);
            this.ConsoleLine = Console.CursorTop;
            this.Label = label;
            this.TotalItems = totalElements;
            this.Count = 0;
            this.Print();
        }

        public void Dispose()
        {
            Console.WriteLine(string.Empty);
        }

        public void Update(int increment)
        {
            this.Count += increment;
            this.Print();
        }

        private void Print()
        {
            Console.SetCursorPosition(0, this.ConsoleLine);
            Console.WriteLine($"{this.Label} {this.Count} / {this.TotalItems} ({this.Count * 100 / this.TotalItems} %)");
        }

    }
}
