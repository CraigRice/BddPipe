namespace BddPipe.UnitTests.Model
{
    internal sealed class Model2(string name)
    {
        public string Name { get; } = name;

        public Model2() : this(nameof(Model2)) { }
    }
}
