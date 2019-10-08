namespace BddPipe.UnitTests.Model
{
    public sealed class Model2
    {
        public string Name { get; }

        public Model2() : this(nameof(Model2)) { }
        public Model2(string name)
        {
            Name = name;
        }
    }
}
